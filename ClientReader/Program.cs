using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientReader
{
	class Program
	{
		static void Main(string[] args)
		{
			string filePath = @"target.csv";
			var result = StreamFile(filePath);

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
