using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClientReaderSettings
{
	public class Map
	{
		public string SelectorValue;
		public string Path;

		public Map(string selectorValue, string path)
		{
			SelectorValue = selectorValue;
			Path = path;
		}

		public Map() : this("gh","w.csv")
		{
			
		}
	}
}
