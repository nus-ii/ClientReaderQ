using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserReaderLibrary;

namespace ClientReader
{
	class Program
	{
		static void Main(string[] args)
		{
            //Привет
			string filePathMap = @"BaseMap.csv";
			var baseMap = MapReader.Read(StreamFile(filePathMap));

			string filePathMapFl = @"MapFL.csv";
			var flMap = MapReader.Read(StreamFile(filePathMapFl));

			string filePathMapUl = @"MapUL.csv";
			var ulMap = MapReader.Read(StreamFile(filePathMapUl));

			string filePathData = @"Data.csv";

			string selectorPath = "ClientType";
			string[] selectorValues=new string[2];
			selectorValues[0] = "FL";
			selectorValues[1] = "UL";

			List<MapLine>[] maps=new List<MapLine>[2];
			maps[0] = flMap;
			maps[1] = ulMap;

			var bigResult = UserReaderLibrary.DataProcessor.Process(StreamFile(filePathData), baseMap, maps, selectorPath,
				selectorValues);

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
