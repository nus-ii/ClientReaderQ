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


		public static void AddValue(ref JObject target, string[] data, MapLine mapLine)
		{
			switch (mapLine.TypeValue)
			{
					//target.SelectToken(mapLine.Path)
				case "int":
					//target[mapLine.Path]=new JValue(Convert.ToInt32(data[mapLine.PositionInt]));
					CreateTokenI(mapLine.Path.Split('.'), ref target, Convert.ToInt32(data[mapLine.PositionInt]));
					break;
				default:
					//target[mapLine.Path] = new JValue(data[mapLine.PositionInt]);
					CreateTokenS(mapLine.Path.Split('.'), ref target, data[mapLine.PositionInt]);
					break;
			}
		}

		

		public static JObject MegaProcessLine(string line, List<MapLine>[] maps, ref JObject target,string selectorPath, string[] selectorValues)
		{
			for (int i = 0; i < selectorValues.Length; i++)
			{
				if (selectorValues[i].Equals(target[selectorPath].Value<string>(), StringComparison.Ordinal))
				{
					return ProcessLine(line, maps[i], target);
				}
			}
			throw new ArgumentException("Нет нужной таблицы");
		}


		private static JObject MegaProcessLine(string line, ref JObject target, string selectorPath, Dictionary<string, List<MapLine>> selectorDictionary)
		{
			//for (int i = 0; i < selectorValues.Length; i++)
			//{
			//	if (selectorValues[i].Equals(target[selectorPath].Value<string>(), StringComparison.Ordinal))
			//	{
			//		return ProcessLine(line, maps[i], target);
			//	}
			//}

			foreach (var selectorP in selectorDictionary)
			{
				if (selectorP.Key.Equals(target[selectorPath].Value<string>(), StringComparison.Ordinal))
				{
					return ProcessLine(line, selectorP.Value, target);
				}
			}
			throw new ArgumentException("Нет нужной таблицы");
		}


		public static List<JObject> Process(StreamReader rdr, List<MapLine> mainMap, List<MapLine>[] maps,
			string selectorPath, string[] selectorValues)
		{
			List<JObject> resultList=new List<JObject>();

			var line = rdr.ReadLine();
			while (line != null)
			{
				//var cols = line.Split(';');
				var temp = ProcessLine(line, mainMap);
				var tempB = MegaProcessLine(line, maps, ref temp, selectorPath, selectorValues);
				resultList.Add(tempB);
				line = rdr.ReadLine();
			}

			return resultList;
		}


		public static List<JObject> Process(StreamReader rdr, List<MapLine> mainMap, string selectorPath,
			Dictionary<string, List<MapLine>> selectorDictionary)
		{
			List<JObject> resultList = new List<JObject>();

			var line = rdr.ReadLine();
			while (line != null)
			{
				//var cols = line.Split(';');
				var temp = ProcessLine(line, mainMap);
				var tempB = MegaProcessLine(line, ref temp, selectorPath, selectorDictionary);
				resultList.Add(tempB);
				line = rdr.ReadLine();
			}

			return resultList;
		}

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
