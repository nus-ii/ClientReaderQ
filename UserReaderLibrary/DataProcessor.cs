using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UserReaderLibrary
{
	public class DataProcessor
	{
		/// <summary>
		/// Создание объекта из одной строчки файла
		/// </summary>
		/// <param name="line">Исходная строка</param>
		/// <param name="map">Карта</param>
		/// <param name="success">Признак успешности обработки строки</param>
		/// <param name="target">Целевой объект</param>
		/// <param name="message">Сообщение об ошибке</param>
		/// <returns></returns>
		public static JObject ProcessLine(string line, List<MapLine> map,ref string message,ref bool success,JObject target=null)
		{
			if (target == null)
			{
				target=new JObject();
			}
			try
			{

				var cols = line.Split(';');

				foreach (var mapLine in map)
				{
					if (!AddValue(ref target, cols, mapLine, ref message))
					{
						success = false;
						break;
					}
				}
			}
			catch (Exception e)
			{
				message=e.Message;
				success = false;
			}

			return target;
		}

		/// <summary>
		/// Добавляет значение в переданный объект
		/// </summary>
		/// <param name="target">Целевой объект</param>
		/// <param name="data">Массив соответствующий входной строке данных</param>
		/// <param name="mapLine">Строка карты</param>
		/// <param name="s"></param>
		public static bool AddValue(ref JObject target, string[] data, MapLine mapLine, ref string message)
		{

			string fullline = "";
			for (int q = 0; q < data.Length; q++)
			{
				fullline = fullline + data[q] + ";";
			}

			if (mapLine.RqValue && (string.IsNullOrEmpty(data[mapLine.PositionInt]) || string.IsNullOrWhiteSpace(data[mapLine.PositionInt])))
			{
				message=string.Format("Отсутствуют данные в столбце {0} для заполнения поля {1}, строка {2}",mapLine.Position,mapLine.Path,fullline);
				return false;
			}

			try
			{
				switch (mapLine.TypeValue)
				{
					//TODO: Точка врезки новых типов
					case "int":
						CreateToken<int>(mapLine.Path.Split('.'), ref target, Convert.ToInt32(data[mapLine.PositionInt]));
						break;
					case "decimal":
						CreateToken<decimal>(mapLine.Path.Split('.'), ref target, Convert.ToDecimal(data[mapLine.PositionInt]));
						break;
					case "date":
						CreateToken<DateTime>(mapLine.Path.Split('.'), ref target, DateTime.Parse(data[mapLine.PositionInt]));
						break;
					default:
						CreateToken<string>(mapLine.Path.Split('.'), ref target, data[mapLine.PositionInt]);
						break;
				}
			}
			catch (Exception e)
			{
				message = string.Format("Проблема с данными в столбце {0} для заполнения поля {1} требуемый формат {3}, строка {2}. Дополнительные данные: {4}", mapLine.Position, mapLine.Path, fullline, mapLine.TypeValue,e.Message);
				return false;
			}
			return true;
		}

		private static JObject CreateToken<TData>(string[] tree, ref JObject target, TData value)
		{
			bool haveLeaf = false;

			try
			{
				var t = target.SelectToken(tree[0]);
				if (t != null)
					haveLeaf = true;
			}
			catch (Exception)
			{
				haveLeaf = false;
			}

			if (tree.Length == 1)
			{
				target[tree[0]] = new JValue(value);
				return target;
			}
			else
			{
				JObject temp = new JObject();
				if (haveLeaf)
				{
					temp = (JObject)target.SelectToken(tree[0]);
				}
				else
				{
					target[tree[0]] = new JObject();
					temp = (JObject)target[tree[0]];
				}

				string[] tempTree = new string[tree.Length - 1];

				for (int q = 0; q < tempTree.Length; q++)
				{
					tempTree[q] = tree[q + 1] + "";
				}
				return CreateToken<TData>(tempTree, ref temp, value);
			}
		}
		//public static JObject MegaProcessLine(string line, List<MapLine>[] maps, ref JObject target,string selectorPath, string[] selectorValues)
		//{
		//	for (int i = 0; i < selectorValues.Length; i++)
		//	{
		//		if (selectorValues[i].Equals(target[selectorPath].Value<string>(), StringComparison.Ordinal))
		//		{
		//			return ProcessLine(line, maps[i], target);
		//		}
		//	}
		//	throw new ArgumentException("Нет нужной таблицы");
		//}

		/// <summary>
		/// Обработка строки пакетом карт 
		/// </summary>
		/// <param name="line">Строка для разбора</param>
		/// <param name="target">Целевой объект</param>
		/// <param name="selectorPath">Путь к признаку</param>
		/// <param name="selectorDictionary">Пакет карт</param>
		/// <param name="success">Признак успешности</param>
		/// <param name="message">Сообщение об ошибке</param>
		/// <returns></returns>
		private static JObject MegaProcessLine(string line, ref JObject target, string selectorPath, Dictionary<string, List<MapLine>> selectorDictionary,ref bool success,ref string message)
		{

			foreach (var selectorP in selectorDictionary)
			{
				if (selectorP.Key.Equals(target[selectorPath].Value<string>(), StringComparison.Ordinal))
				{
					return ProcessLine(line, selectorP.Value,ref message,ref success,target);
				}
			}
			throw new ArgumentException("Нет нужной таблицы");
		}

		//public static List<JObject> Process(StreamReader rdr, List<MapLine> mainMap, List<MapLine>[] maps,
		//	string selectorPath, string[] selectorValues)
		//{
		//	List<JObject> resultList=new List<JObject>();

		//	var line = rdr.ReadLine();
		//	while (line != null)
		//	{
		//		var temp = ProcessLine(line, mainMap);
		//		var tempB = MegaProcessLine(line, maps, ref temp, selectorPath, selectorValues);
		//		resultList.Add(tempB);
		//		line = rdr.ReadLine();
		//	}

		//	return resultList;
		//}

		/// <summary>
		/// Главный метод разбора
		/// </summary>
		/// <param name="rdr">Файл с данными</param>
		/// <param name="mainMap">Базовая схема разбора</param>
		/// <param name="selectorPath">Путь к признаку типа объекта</param>
		/// <param name="selectorDictionary">Словарь Значение признака-соответсвующая карта</param>
		/// <param name="errorLog"></param>
		/// <param name="badLines"></param>
		/// <returns></returns>
		public static List<JObject> Process(StreamReader rdr, List<MapLine> mainMap, string selectorPath,
			Dictionary<string, List<MapLine>> selectorDictionary,ref List<string> errorLog, ref List<string> badLines)
		{
			List<JObject> resultList = new List<JObject>();
			var line = rdr.ReadLine();
			while (line != null)
			{
				string message = "";
				bool success = true;
				//Разбор по базовой карте
				var temp = ProcessLine(line, mainMap,ref message,ref success);
				if (!success)
				{
					errorLog.Add(message);
					badLines.Add(line);
				}

				if (success&&selectorDictionary.Count > 0)
				{
					//Разбор по дополнительным картам
					var tempB = MegaProcessLine(line, ref temp, selectorPath, selectorDictionary,ref success,ref message);

					if (!success)
					{
						errorLog.Add(message);
						badLines.Add(line);
					}

					if(success)
					resultList.Add(tempB);
				}
				else
				{
					if(success)
					resultList.Add(temp);
				}

				line = rdr.ReadLine();
			}
			return resultList;
		}

		/// <summary>
		/// Создаёт в переданном объекте значение типа int
		/// </summary>
		/// <param name="tree">Путь к значению в целевом объекте</param>
		/// <param name="target">Целевой объект</param>
		/// <param name="toInt32">Целевое значение</param>
		/// <returns></returns>
		private static JObject CreateTokenI(string[] tree, ref JObject target, int toInt32)
		{
			bool haveLeaf = false;

			try
			{
				var t=target.SelectToken(tree[0]);
				if (t != null)
					haveLeaf = true;
			}
			catch (Exception)
			{
				haveLeaf = false;
			}

			if (tree.Length == 1)
			{
				target[tree[0]] = new JValue(toInt32);
				return target;
			}
			else
			{
				JObject temp=new JObject();
				if (haveLeaf)
				{
					temp = (JObject)target.SelectToken(tree[0]);
				}
				else
				{
					target[tree[0]] = new JObject(); 
					temp = (JObject)target[tree[0]];
				}
				
				

				string[] tempTree = new string[tree.Length - 1];

				for (int q = 0; q < tempTree.Length; q++)
				{
					tempTree[q] = tree[q + 1] + "";
				}
				return CreateTokenI(tempTree, ref temp, toInt32);
			}
		}

		/// <summary>
		/// Создание в переданном объекте значения типа строка 
		/// </summary>
		/// <param name="tree">Путь к значению в целевом объекте</param>
		/// <param name="target">Целевой объект</param>
		/// <param name="value">Целевое значение</param>
		/// <returns></returns>
		public static JObject CreateTokenS(string[] tree,ref JObject target, string value)
		{
			bool haveLeaf = false;

			try
			{
				var t = target.SelectToken(tree[0]);
				if (t != null)
					haveLeaf = true;
			}
			catch (Exception)
			{
				haveLeaf = false;
			}


			if (tree.Length==1)
			{
				target[tree[0]] = new JValue(value);
				return target;
			}
			else
			{
				JObject temp = new JObject();
				if (haveLeaf)
				{
					temp = (JObject)target.SelectToken(tree[0]);
				}
				else
				{
					target[tree[0]] = new JObject();
					temp = (JObject)target[tree[0]];
				}

				string[] tempTree = new string[tree.Length-1];

				for (int q = 0; q < tempTree.Length; q++)
				{
					tempTree[q] = tree[q + 1] + "";
				}
				return CreateTokenS(tempTree, ref temp, value);
			}
		}


		
	}

}
