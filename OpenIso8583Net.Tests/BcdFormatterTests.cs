using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.Exceptions;
using OpenIso8583Net.Formatter;

namespace OpenIso8583Net.Tests
{
    [TestClass]
    public class BcdFormatterTests
    {
        [TestMethod]
        public void GetBytesTest()
        {
            IFormatter target = new BcdFormatter();
            const string value = "0245";
            var expected = new byte[2];
            expected[0] = 0x02;
            expected[1] = 0x45;
            var actual = target.GetBytes(value);
            CollectionAssert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void TestUnpack()
        {
            IFormatter formatter = new BcdFormatter();
            var data = new byte[2];
            data[0] = 0x02;
            data[1] = 0x45;
            var actual = formatter.GetString(data);
            Assert.AreEqual("0245", actual);
        }

        [TestMethod]
        public void TestPackedLength()
        {
            IFormatter formatter = new BcdFormatter();
            var actual = formatter.GetPackedLength(8);
            Assert.AreEqual(4, actual);
        }
    }
}
