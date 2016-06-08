using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserReaderLibrary
{
	public class MapLine
	{
		public string Name;

		public string Position;

		public string TypeValue;

		public string Value;

		public string RequiredValue;

		public string Path;

		public bool RqValue
		{
			get
			{
				if (!string.IsNullOrEmpty(RequiredValue) && RequiredValue.Equals("true", StringComparison.Ordinal))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public int PositionInt
		{
			get { return Convert.ToInt32(Position); }
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3}",Name,TypeValue,RequiredValue,Path);
		}
	}
}
