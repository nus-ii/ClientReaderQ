using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClientReaderSettings
{
	public class Save
	{
		public bool OneFile;
		public int MaxItemInFile;
		public string FileName;
		public string FileFolder;
		public string FileExt;

		public Save(string fileFolder, string fileName, int maxItemInFile, bool oneFile, string fileExt)
		{
			FileFolder = fileFolder;
			FileName = fileName;
			MaxItemInFile = maxItemInFile;
			OneFile = oneFile;
			FileExt = fileExt;
		}

		public Save()
		{
			FileFolder = "";
			FileName = "Out";
			MaxItemInFile = 10;
			FileExt = ".json";
			OneFile = true;
		}

		public string GetFileFullName()
		{
			return FileFolder + FileName + FileExt;
		}
	}
}
