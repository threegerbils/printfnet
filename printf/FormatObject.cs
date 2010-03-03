
using System;
using System.Collections.Generic;
using System.Text;

namespace printf {
	/// <summary>
	/// Description of FormatObject.
	/// </summary>
	public class FormatObject {
		public class FormatStringPart {
			public Flags flags;
			public int? width;
			public int precision = 1;
			public char length;
			public char specifier;
		}

		List<object> args = new List<object>();
		int argCount = 0;
		FormatStringPart[] parts;
		string[] staticParts;

		string finalString;

		[Flags]
		public enum Flags {
			LeftJustify, ForcePlus, BlankIfPlus, PrecedingSpecial, LeftPadZero
		}

		enum ParseMode {
			Static, Format
		}
		enum FormatStep {
			Flags, Width, PrecisionStart, Precision, Length, Specifier
		}

		public FormatObject(string format): this(format, true) {}

		public FormatObject(string format, bool useDefaultFormatters) {
			//Parse format string
			List<FormatStringPart> parts = new List<FormatStringPart>();
			List<string> staticParts = new List<string>();
			StringBuilder staticPart = new StringBuilder();


			ParseMode mode = ParseMode.Static;
			FormatStringPart part = null;
			FormatStep step = FormatStep.Flags;
			StringBuilder tempBuffer = null;

			for (int i = 0; i < format.Length; ++i) {
				char c = format[i];

				if (mode == ParseMode.Static) {
					//If
					if (c == '%') {
						mode = ParseMode.Format;
						step = FormatStep.Flags;
					}
					else {
						staticPart.Append(c);
					}
				}
				else {
					switch (step) {
					case FormatStep.Flags:
						if (c == '%') {
							staticPart.Append('%');
							mode = ParseMode.Static;
							continue;
						}
						part = new FormatStringPart();
						if (c == '-') {
							part.flags &= Flags.LeftJustify;
						}
						else if (c == '+') {
							part.flags &= Flags.ForcePlus;
						}
						else if (c == ' ') {
							part.flags &= Flags.BlankIfPlus;
						}
						else if (c == '#') {
							part.flags &= Flags.PrecedingSpecial;
						}
						else if (c == '0') {
							part.flags &= Flags.LeftPadZero;
						}
						else {
							step = FormatStep.Width;
						goto case FormatStep.Width;
						}
						break;
					case FormatStep.Width:
						if (tempBuffer != null) {
							if (c >= '0' && c <= '9') {
								tempBuffer.Append(c);
							}
							else {
								part.width = int.Parse(tempBuffer.ToString());
								tempBuffer = null;
								step = FormatStep.PrecisionStart;
							goto case FormatStep.PrecisionStart;
							}
						}
						else if (c == '*') {
							part.width = -1;
							argCount++;
						}
						else if (c >= '0' && c <= '9') {
							tempBuffer = new StringBuilder();
							tempBuffer.Append(c);
						}
						else {
							step = FormatStep.PrecisionStart;
						goto case FormatStep.PrecisionStart;
						}
						break;
					case FormatStep.PrecisionStart:
						if (c != '.') {
							step = FormatStep.Length;
						goto case FormatStep.Length;
						}
						else {
							step = FormatStep.Precision;
						}
						break;
					case FormatStep.Precision:
						if (tempBuffer != null) {
							if (c >= '0' && c <= '9') {
								tempBuffer.Append(c);
							}
							else {
								part.precision = int.Parse(tempBuffer.ToString());
								tempBuffer = null;
								step = FormatStep.Length;
							goto case FormatStep.Length;
							}
						}
						else if (c == '*') {
							part.precision = -1;
							argCount++;
						}
						else if (c >= '0' && c <= '9') {
							tempBuffer = new StringBuilder();
							tempBuffer.Append(c);
						}
						else {
							part.precision = 0;
							step = FormatStep.Length;
						goto case FormatStep.Length;
						}
						break;
					case FormatStep.Length:
						if ("hlL".IndexOf(c) != -1) {
							part.length = c;
							step = FormatStep.Specifier;
						}
						else {
							step = FormatStep.Specifier;
						goto case FormatStep.Specifier;
						}
						break;
					case FormatStep.Specifier:
						argCount++;
						part.specifier = c;

						staticParts.Add(staticPart.ToString());
						staticPart = new StringBuilder();
						parts.Add(part);
						part = new FormatStringPart();

						mode = ParseMode.Static;
						break;
					}
				}
			}
			if (mode == ParseMode.Format) {
				throw new ArgumentException("Invalid format string", "format");
			}
			staticParts.Add(staticPart.ToString());

			this.parts = parts.ToArray();
			this.staticParts = staticParts.ToArray();

			if (useDefaultFormatters) LoadDefaultFormatters();
		}


		public void Add(params object[] objs) {
			for (int i = 0; i < objs.Length; ++i) {
				args.Add(objs[i]);
			}
			this.finalString = null;
		}
		public void Clear() {
			this.args.Clear();
			this.finalString = null;
		}

		public delegate string Formatter(FormatStringPart part, object arg);
		Dictionary<char, Formatter> formatters = new Dictionary<char, Formatter>();

		public void AddFormatter(char specifier, Formatter formatter) {
			formatters.Add(specifier, formatter);
		}

		private void DoFormat() {
			try {
				if (argCount > args.Count) {
					throw new InvalidOperationException(string.Format(
					                                        "Expected argument count: {0}, provided: {1}", argCount, args.Count));
				}
				StringBuilder final = new StringBuilder();
				for (int i = 0, j = 0; i < parts.Length; ++i, ++j) {
					final.Append(staticParts[i]);
					var f = parts[j];
					if (f.width == -1) {
						f.width = (int)args[j];
						j++;
					}
					if (f.precision == -1) {
						f.width = (int)args[j];
						j++;
					}
					final.Append(formatters[f.specifier].Invoke(f, args[j]));
				}
				final.Append(staticParts[staticParts.Length-1]);
				finalString = final.ToString();
			}
			catch (KeyNotFoundException ex) {
				throw new InvalidOperationException("Invalid specifier in format string", ex);
			}
		}

		public override string ToString() {
			if (this.finalString == null) DoFormat();
			return finalString;
		}

		private void LoadDefaultFormatters() {
			Formatter charFormatter = (part, arg) => {
				char c = (char)arg;
				return c.ToString();
			};
			AddFormatter('c', charFormatter);
			Formatter intFormatter = (part, arg) => {
				long i = Convert.ToInt64(arg);
				return i.ToString();
			};
			AddFormatter('i', intFormatter);
			AddFormatter('d', intFormatter);
			Formatter uintFormatter = (part, arg) => {
				ulong u = Convert.ToUInt64(arg);
				return u.ToString();
			};
			AddFormatter('u', uintFormatter);
			Formatter sciFormatter = (part, arg) => {
				double d = Convert.ToDouble(arg);
				return d.ToString();
			};
			AddFormatter('e', sciFormatter);
			AddFormatter('E', sciFormatter);
			Formatter floatFormatter = (part, arg) => {
				double d = Convert.ToDouble(arg);
				return d.ToString();
			};
			AddFormatter('f', floatFormatter);
			Formatter octFormatter = (part, arg) => {
				long l = Convert.ToInt64(arg);
				return Convert.ToString(l, 8);
			};
			AddFormatter('o', octFormatter);
			Formatter hexFormatter = (part, arg) => {
				long l = Convert.ToInt64(arg);
				return string.Format(
				           part.specifier == 'x' ? "{0:x}" : "{0:X}",
				           l);
			};
			AddFormatter('x', hexFormatter);
			AddFormatter('X', hexFormatter);
			Formatter strFormatter = (part, arg) => {
				return arg.ToString();
			};
			AddFormatter('s', strFormatter);
			Formatter ptrFormatter = (part, arg) => {
				return arg.GetHashCode().ToString();
			};
			AddFormatter('p', ptrFormatter);
			AddFormatter('n', (part, arg) => {
			    throw new NotSupportedException("Getting the number of cahracters is not supported");
			});
		}
	}
}
