using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClientReaderSettings
{
	public class Settings
	{
		public string BaseMapPath;
		public string DataPath;
		public SpecMap SpecialMap;
		public Save SaveSettings;

		public Settings()
		{
			BaseMapPath = "BaseMap.csv";
			DataPath = "Data.csv";

			SpecialMap=new SpecMap();
			SaveSettings=new Save("","efefef",10,true,".json");
		}
	}


}
