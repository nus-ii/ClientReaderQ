using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserReaderLibrary
{
	public class MapReader
	{
		public static List<MapLine> Read(StreamReader rdr)
		{
			List<MapLine> result=new List<MapLine>();

			var line = rdr.ReadLine();
			while (line!=null)
			{
				var cols = line.Split(';');

				MapLine temp = new MapLine()
				{
					Name = cols[0],
					Position = cols[1],
					TypeValue = cols[2],
					RequiredValue = cols[3],
					Path = cols[4],
				};

				result.Add(temp);
				line = rdr.ReadLine();
			}
			return result;
		}
	}
}
