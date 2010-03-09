
#if TEST

using System;
using NUnit.Framework;

namespace printf {
	[TestFixture]
	public class FormatTest {
		string decSep = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;

		[Test]
		public void TestTypes() {
			//Test the basic data types
			Assert.AreEqual("b",
			                Printf.sprintf("%c", 'b'));
			Assert.AreEqual("121",
			                Printf.sprintf("%d", 121));
			Assert.AreEqual("-34",
			                Printf.sprintf("%i", -34));
			Assert.AreEqual("3.123400e+002",
			                Printf.sprintf("%e", 3.1234e02));
			Assert.AreEqual("3.123400E+002",
			                Printf.sprintf("%E", 3.1234e02));
			Assert.AreEqual("3.123000",
			                Printf.sprintf("%f", 3.123));
			Assert.AreEqual("610",
			                Printf.sprintf("%o", Convert.ToInt32("0610", 8)));
			Assert.AreEqual("asdfjkl",
			                Printf.sprintf("%s", "asdfjkl"));
			Assert.AreEqual("4567",
			                Printf.sprintf("%u", 4567));
			Assert.AreEqual("deadf00d",
			                Printf.sprintf("%x", unchecked((int)0xdeadf00d)));
			Assert.AreEqual("DEADF00D",
			                Printf.sprintf("%X", unchecked((int)0xdeadf00d)));
			Assert.IsNotEmpty(Printf.sprintf("%p", new object()));

			Assert.Throws<NotSupportedException>(delegate() {
			                                         Printf.sprintf("%n", new IntPtr());
			                                     });

			//Invalid format string
			Assert.Throws<InvalidOperationException>(delegate() {
			            Printf.sprintf("%z", new IntPtr());
			        });

			Assert.AreEqual("%",
			                Printf.sprintf("%%"));
		}

		[Test]
		public void TestStaticText() {
			//Static text
			Assert.AreEqual("aaaa",
			                Printf.sprintf("aaaa"));
			//Param at the end
			Assert.AreEqual("Hello World",
			                Printf.sprintf("Hello %s", "World"));
			//Param at the beginning
			Assert.AreEqual("Hello World!",
			                Printf.sprintf("%s World!", "Hello"));
			//Param in the middle
			Assert.AreEqual("a 1 b",
			                Printf.sprintf("a %d b", 1));
			//Multiple params
			Assert.AreEqual("Hello World!",
			                Printf.sprintf("%s %s!", "Hello", "World"));
		}

		[Test]
		public void TestArgumentCount() {
			//More arguments should not cause errors
			Assert.DoesNotThrow(delegate() {
			                        Printf.sprintf("%d %d", 1, 2, 3);
			                    });
			//Less arguments should raise error
			Assert.Throws<InvalidOperationException>(delegate() {
			            Printf.sprintf("%d %d", 1);
			        });
			//Also check for the extra arguments required for *
			Assert.Throws<InvalidOperationException>(delegate() {
			            Printf.sprintf("%*d", 1);
			        });
			Assert.Throws<InvalidOperationException>(delegate() {
			            Printf.sprintf("%.*d", 1);
			        });
			Assert.Throws<InvalidOperationException>(delegate() {
			            Printf.sprintf("%*.*d", 1, 2);
			        });
			Assert.DoesNotThrow(delegate() {
			                        Printf.sprintf("%*.*d", 1, 2, 3);
			                    });
		}

		[Test]
		public void TestPadding() {
			//Simple padding
			Assert.AreEqual("   33",
			                Printf.sprintf("%5d", 33));
			//More padding than required
			Assert.AreEqual("123456",
			                Printf.sprintf("%4d", 123456));
			//Left-align
			Assert.AreEqual("888  ",
			                Printf.sprintf("%-5d", 888));
			//Pad with zeros
			Assert.AreEqual("0043, 00zz",
			                Printf.sprintf("%04d, %04s", 43, "zz"));
			//Left align always pads with spaces
			Assert.AreEqual("123  ",
			                Printf.sprintf("%-05d", 123));
			//Width specified on the argument list
			Assert.AreEqual("    11",
			                Printf.sprintf("%*d", 6, 11));
			//Sign with padding
			Assert.AreEqual("   -1 -0001  0001",
			                Printf.sprintf("%5d %05d % 05d", -1, -1, 1));
		}

		[Test]
		public void TestSign() {
			//Sign of number
			Assert.AreEqual("-5",
			                Printf.sprintf("%d", -5));
			//Positive signs
			Assert.AreEqual("3 +4",
			                Printf.sprintf("%d %+d", 3, 4));
			//Sign is not printed for o x and X
			Assert.AreEqual("610 ffffffff FFFFFFFF",
			                Printf.sprintf("%+o %+x %+X", Convert.ToInt32("0610", 8), -1, -1));
			//Insert space if there is no sign
			Assert.AreEqual("-11  22",
			                Printf.sprintf("% d % d", -11, 22));
		}
		
		[Test]
		public void TestPrecision() {
			//INTEGERS: minimum number of digits
			Assert.AreEqual("123",
			                Printf.sprintf("%.2d", 123));
			Assert.AreEqual("ff",
			                Printf.sprintf("%.1x", 0xff));
			//Padding with 0
			Assert.AreEqual("0044",
			                Printf.sprintf("%.4d", 44));
			Assert.AreEqual("000FF",
			                Printf.sprintf("%.5X", 0xff));
			Assert.AreEqual("003",
			                Printf.sprintf("%.3u", 3));
			Assert.AreEqual("0010",
			                Printf.sprintf("%.4o", 8));
			//Sign is not included:
			Assert.AreEqual("-01",
			                Printf.sprintf("%.2d", -1));
			//Mixing:
			Assert.AreEqual("  001",
			                Printf.sprintf("%5.3d", 1));
			
			//Zero:
			Assert.AreEqual("00",
			                Printf.sprintf("%.2d", 0));
			Assert.AreEqual("",
			                Printf.sprintf("%.0d", 0));
			Assert.AreEqual("",
			                Printf.sprintf("%.d", 0));
			
			//Floating point:
			Assert.AreEqual("0.1235",
			                Printf.sprintf("%.4f", 0.123456));
			Assert.AreEqual("0",
			                Printf.sprintf("%.0f", 0.123456));
			//Mixing:
			Assert.AreEqual(" 0.123",
			                Printf.sprintf("%6.3f", 0.123));
			Assert.AreEqual("0.123",
			                Printf.sprintf("%4.3f", 0.123));
			
			//Scientific:
			Assert.AreEqual("1.2346e-001",
			                Printf.sprintf("%.4e", 0.123456));
			Assert.AreEqual("1e-001",
			                Printf.sprintf("%.0e", 0.123456));
			//Mixing:
			Assert.AreEqual("1.235e-001",
			                Printf.sprintf("%4.3e", 0.123456));
			Assert.AreEqual("     1.235e-001",
			                Printf.sprintf("%15.3e", 0.123456));
			
			
			//String:
			Assert.AreEqual("asd",
			                Printf.sprintf("%.3s", "asdfjkl"));
			Assert.AreEqual("",
			                Printf.sprintf("%.s", "asdfjkl"));
			//Mixing:
			Assert.AreEqual("   asd",
			                Printf.sprintf("%6.3s", "asdfjkl"));
		}
	}
}
#endif
