using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClientReaderSettings
{
	public class SpecMap
	{
		public string SelectorPath;
		public List<Map> Maps;

		public SpecMap(string selectorPath, List<Map> maps)
		{
			SelectorPath = selectorPath;
			Maps = maps;
		}

		public SpecMap()
		{
			SelectorPath = "Client";
			Maps=new List<Map>();
			Maps.Add(new Map());
		}
	}
}
