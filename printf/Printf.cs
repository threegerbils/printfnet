
using System;

namespace printf {
	/// <summary>
	/// Contains the printf family of functions.
	/// It supports the following formats: 'diufeExXoscp';
	/// The following flags: '-+ #0';
	/// Width and precision;
	/// Length (h, l) for hexadecimal output (for other formats, integers are treated as Int64 or UInt64)
	/// 
	/// Decimal separator is not localized, it is always a '.' character.
	/// %g and %G does not remove trailing zeroes.
	/// NaN and infinities are not guaranteed to have a fixed representation accross platforms.
	/// 
	/// At the moment, it does NOT support %n (number of chars printed, through a prointer),
	///  and the 1$..n$ position specifier.
	/// </summary>
	public static class Printf {
		/// <summary>
		/// Formats the arguments according to the format string.
		/// Returns the result as a string.
		/// </summary>
		/// <param name="format">The format string</param>
		/// <param name="args">The objects to format</param>
		/// <returns>The formatted output</returns>
		/// <exception cref="ArgumentException">The format string is invalid or too few arguments provided.</exception>
		public static string sprintf(string format, params object[] args) {
			try {
				FormatObject f = new FormatObject(format);
				f.SetArgs(args);
				return f.ToString();
			}
			catch(InvalidOperationException ex) {
				throw new ArgumentException("Error in format string, see inner exception for details", ex);
			}
		}
	}
}
