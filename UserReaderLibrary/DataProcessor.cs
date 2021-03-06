﻿using System;
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
		/// <param name="target">Целевой объект</param>
		/// <returns></returns>
		public static JObject ProcessLine(string line, List<MapLine> map,JObject target=null)
		{
			if (target == null)
			{
				target=new JObject();
			}

			var cols = line.Split(';');

			foreach (var mapLine in map)
			{
				AddValue(ref target,cols,mapLine);
			}

			return target;
		}

		/// <summary>
		/// Добавляет значение в переданный объект
		/// </summary>
		/// <param name="target">Целевой объект</param>
		/// <param name="data">Массив соответствующий входной строке данных</param>
		/// <param name="mapLine">Строка карты</param>
		public static void AddValue(ref JObject target, string[] data, MapLine mapLine)
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
		/// <returns></returns>
		private static JObject MegaProcessLine(string line, ref JObject target, string selectorPath, Dictionary<string, List<MapLine>> selectorDictionary)
		{

			foreach (var selectorP in selectorDictionary)
			{
				if (selectorP.Key.Equals(target[selectorPath].Value<string>(), StringComparison.Ordinal))
				{
					return ProcessLine(line, selectorP.Value, target);
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
		/// <returns></returns>
		public static List<JObject> Process(StreamReader rdr, List<MapLine> mainMap, string selectorPath,
			Dictionary<string, List<MapLine>> selectorDictionary)
		{
			List<JObject> resultList = new List<JObject>();
			var line = rdr.ReadLine();
			while (line != null)
			{
				//Разбор по базовой карте
				var temp = ProcessLine(line, mainMap);

				//Разбор по дополнительным картам
				var tempB = MegaProcessLine(line, ref temp, selectorPath, selectorDictionary);
				resultList.Add(tempB);
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
