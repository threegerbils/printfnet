
using System;

namespace printf {
	/// <summary>
	/// Description of Printf.
	/// </summary>
	public static class Printf {
		public static string sprintf(string format, params object[] args) {
			FormatObject f = new FormatObject(format);
			f.Add(args);
			return f.ToString();
		}
	}
}
