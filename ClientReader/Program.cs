using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UserReaderLibrary;
using ClientReaderSettings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClientReader
{
	class Program
	{
		static void Main(string[] args)
		{
			//Привет парень, это правки с рабочего от 05.07.16
			//Создание настроек
			//var s=new Settings();
			//var js = JsonConvert.SerializeObject(s);
			//File.WriteAllText("settings.json",js);

			var ss = File.ReadAllText("settings.json");
			var settings = JsonConvert.DeserializeObject<Settings>(ss);
            
			string filePathMap = @"BaseMap.csv";
			var baseMap = MapReader.Read(StreamFile(settings.BaseMapPath));//StreamFile(filePathMap));

			//string filePathMapFl = @"MapFL.csv";
			//var flMap = MapReader.Read(StreamFile(filePathMapFl));

			//string filePathMapUl = @"MapUL.csv";
			//var ulMap = MapReader.Read(StreamFile(filePathMapUl));

			string filePathData =settings.DataPath;//@"Data.csv";

			string selectorPath = settings.SpecialMap.SelectorPath;//"ClientType";
			//Dictionary<string,List<MapLine>> selectorDictionary=new Dictionary<string, List<MapLine>>();
			//selectorDictionary.Add("FL",flMap);
			//selectorDictionary.Add("UL", ulMap);

			Dictionary<string, List<MapLine>> dictionaryFromSettings = ReadDict(settings);

			var bigResult = DataProcessor.Process(StreamFile(filePathData), baseMap, selectorPath, dictionaryFromSettings);

			foreach (var jo in bigResult)
			{
				Console.WriteLine(jo.ToString());
			}
			Console.ReadLine();
		}

		private static Dictionary<string, List<MapLine>> ReadDict(Settings set)
		{
			Dictionary<string, List<MapLine>> result=new Dictionary<string, List<MapLine>>();
			foreach (var sm in set.SpecialMap.Maps)
			{
				result.Add(sm.SelectorValue, MapReader.Read(StreamFile(sm.Path)));
			}
			return result;
		}

		private static StreamReader StreamFile(string filePath)
		{
			var text = File.ReadAllText(filePath);
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(text);
			writer.Flush();
			stream.Position = 0;
			var result = new StreamReader(stream);
			return result;
		}
	}
}
