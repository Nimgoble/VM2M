using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelToModelConverterApp
{
	public static class GbrxUtils
	{
		public static bool FromIndicator(this string s)
		{
			return (s == "Y");
		}
		public static string ToIndicator(this bool b)
		{
			return (b) ? "Y" : "N";
		}
	}
}
