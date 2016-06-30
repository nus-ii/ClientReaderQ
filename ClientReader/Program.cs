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
			//Создание настроек
			var s=new Settings();
			var js = JsonConvert.SerializeObject(s);
			File.WriteAllText("s.set",js);

            //Привет парень, это правки с рабочего от 30.06.16
			string filePathMap = @"BaseMap.csv";
			var baseMap = MapReader.Read(StreamFile(filePathMap));

			string filePathMapFl = @"MapFL.csv";
			var flMap = MapReader.Read(StreamFile(filePathMapFl));

			string filePathMapUl = @"MapUL.csv";
			var ulMap = MapReader.Read(StreamFile(filePathMapUl));

			string filePathData = @"Data.csv";

			string selectorPath = "ClientType";
			Dictionary<string,List<MapLine>> selectorDictionary=new Dictionary<string, List<MapLine>>();
			selectorDictionary.Add("FL",flMap);
			selectorDictionary.Add("UL", ulMap);

			var bigResult = DataProcessor.Process(StreamFile(filePathData), baseMap, selectorPath, selectorDictionary);

			Console.ReadLine();
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
