using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
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
		//Привет парень, это правки с рабочего от 05.08.16
		static void Main(string[] args)
		{
		
			Settings settings;
			if (args!=null&&args.Length>=1&&!string.IsNullOrEmpty(args[0]))
			{
				try
				{
					string filedata = File.ReadAllText(args[0]);
					settings = JsonConvert.DeserializeObject<Settings>(filedata);
				}
				catch
				{
					settings = SetSettings(string.Format("Не удалось получить настройки из файла {0}. Введите путь к файлу настроек.",args[0]));
				}
			}
			else
			{
				settings=SetSettings("Введите путь к файлу настроек.");
			}
			

			var baseMap = MapReader.Read(StreamFile(settings.BaseMapPath));
			string filePathData =settings.DataPath;
			string selectorPath = settings.SpecialMap.SelectorPath;
			Dictionary<string, List<MapLine>> dictionaryFromSettings = new Dictionary<string, List<MapLine>>();
			dictionaryFromSettings=ReadDict(settings);

			List<string> errorList=new List<string>();
			List<string> badLineList=new List<string>();

			var Stream = new StreamReader(filePathData, Encoding.GetEncoding(settings.Encoding));
			var bigResult = DataProcessor.Process(Stream, baseMap, selectorPath, dictionaryFromSettings,settings,ref errorList,ref badLineList);

			bool success = ResultSave(bigResult, settings.SaveSettings);
			foreach (var l in bigResult)
			{
				Console.WriteLine(l.ToString());
			}

			SaveList(badLineList,".csv","badLines",settings,true);
			foreach (var l in badLineList)
			{
				Console.WriteLine(l);
			}

			SaveList(errorList, ".txt", "errorList",settings);
			foreach (var l in errorList)
			{
				Console.WriteLine(l);
			}
			
			Console.ReadLine();
		}

		private static void SaveList(List<string> targetList, string ext, string dopname, Settings settings, bool withHeader=false)
		{
			Save saveSettings = settings.SaveSettings;
			string result = "";
			if (withHeader&&settings.FirstValueLineExcelLike>1)
			{
				for (int q = 0; q < settings.FirstValueLineExcelLike-1; q++)
				{
					result = result +";;;;;;" +"\r\n";
				}
			}
			
			foreach (var l in targetList)
			{
				result = result + l + "\r\n";
			}

			File.WriteAllText(string.Format("{0}-{1}{2}",saveSettings.GetFileNameWithoutExt(),dopname,ext),result);
		}

		private static Settings SetSettings(string message)
		{
			Settings result=new Settings();
			string path = "";

			try
			{
				Console.WriteLine(message);
				path = Console.ReadLine();
				string filedata = File.ReadAllText(path);
				result = JsonConvert.DeserializeObject<Settings>(filedata);
				return result;
			}
			catch (Exception)
			{
				return SetSettings(string.Format("Не удалось получить настройки из файла {0}. Введите путь к файлу настроек.", path));
			}
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

				JObject gigaResult=new JObject();
				gigaResult["result"] = result;


				File.WriteAllText(saveSettings.GetFileFullName(),gigaResult.ToString());
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

			if (set.SpecialMap.Maps.Count>0)
			{
				foreach (var sm in set.SpecialMap.Maps)
				{
					result.Add(sm.SelectorValue, MapReader.Read(StreamFile(sm.Path)));
				} 
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
