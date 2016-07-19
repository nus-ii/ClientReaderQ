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
			var ss = File.ReadAllText("Settings.json");
			var settings = JsonConvert.DeserializeObject<Settings>(ss);

			var baseMap = MapReader.Read(StreamFile(settings.BaseMapPath));
			string filePathData =settings.DataPath;
			string selectorPath = settings.SpecialMap.SelectorPath;
			Dictionary<string, List<MapLine>> dictionaryFromSettings = ReadDict(settings);

			var bigResult = DataProcessor.Process(StreamFile(filePathData), baseMap, selectorPath, dictionaryFromSettings);

			bool success = ResultSave(bigResult, settings.SaveSettings);

			foreach (var jo in bigResult)
			{
				Console.WriteLine(jo.ToString());
			}
			
			Console.ReadLine();
		}

		private static bool ResultSave(List<JObject> bigResult, Save saveSettings)
		{
			if (saveSettings.OneFile)
			{
				JArray result=new JArray();

				foreach (var jo in bigResult)
				{
					result.Add(jo);
				}

				File.WriteAllText(saveSettings.GetFileFullName(),result.ToString());
			}
			else
			{
				throw new NotImplementedException();
			}
			return false;
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
