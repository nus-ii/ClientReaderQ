using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserReaderLibrary
{
	public class MapLine
	{
		/// <summary>
		/// Название значения
		/// </summary>
		public string Name;

		/// <summary>
		/// Номер столбца во входном файле - string
		/// </summary>
		public string Position;

		/// <summary>
		/// Тип значения
		/// </summary>
		public string TypeValue;

		/// <summary>
		/// Признак обязательности значения - string
		/// </summary>
		public string RequiredValue;

		/// <summary>
		/// Путь в выходном файле
		/// </summary>
		public string Path;

		/// <summary>
		/// Признак обязательности значения - bool
		/// </summary>
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

		/// <summary>
		///  Номер столбца во входном файле - int
		/// </summary>
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
