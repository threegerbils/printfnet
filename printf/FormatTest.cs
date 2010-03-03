
#if TEST

using System;
using NUnit.Framework;

namespace printf {
	[TestFixture]
	public class FormatTest {
		string decSep = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;

		[Test]
		public void TestTypes() {
			Assert.AreEqual("b",
			                Printf.sprintf("%c", 'b'));
			Assert.AreEqual("121",
			                Printf.sprintf("%d", 121));
			Assert.AreEqual("-34",
			                Printf.sprintf("%i", -34));
			//TODO: double format string
			//Assert.AreEqual("3.1234e+2",
			//                Printf.sprintf("%e", 3.1234e2));
			Assert.AreEqual("3" + decSep + "123",
			                Printf.sprintf("%f", 3.123));
			Assert.AreEqual("610",
			                Printf.sprintf("%o", Convert.ToInt32("0610", 8)));
			Assert.AreEqual("asdfjkl",
			                Printf.sprintf("%s", "asdfjkl"));
			Assert.AreEqual("4567",
			                Printf.sprintf("%u", 4567));
			Assert.AreEqual("deadf00d",
			                Printf.sprintf("%x", 0xdeadf00d));
			Assert.AreEqual("DEADF00D",
			                Printf.sprintf("%X", 0xdeadf00d));
			Assert.IsNotEmpty(Printf.sprintf("%p", new object()));

			Assert.Throws<NotSupportedException>(delegate() {
			                                         Printf.sprintf("%n", new IntPtr());
			                                     });

			Assert.Throws<InvalidOperationException>(delegate() {
			            Printf.sprintf("%z", new IntPtr());
			        });
			
			Assert.AreEqual("%",
			                Printf.sprintf("%%"));
		}
	}
}
#endif
