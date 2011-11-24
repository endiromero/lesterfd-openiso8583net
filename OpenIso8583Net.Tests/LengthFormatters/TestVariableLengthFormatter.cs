using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.LengthFormatters;

namespace OpenIso8583Net.Tests.LengthFormatters
{
    [TestClass]
    public class TestVariableLengthFormatter
    {
        private readonly VariableLengthFormatter _formatter = new VariableLengthFormatter(2, 12);

        [TestMethod]
        public void TestLengthOfLengthIndicator()
        {
            Assert.AreEqual(2, _formatter.LengthOfLengthIndicator);
        }

        [TestMethod]
        public void TestPackLength()
        {
            var data = new byte[14];
            var offset = _formatter.Pack(data, 8, 2);
            Assert.AreEqual(4, offset);
            Assert.AreEqual((byte)'0', data[2]);
            Assert.AreEqual((byte)'8', data[3]);

        }

        [TestMethod]
        public void TestLengthOfField()
        {
            var data = new byte[14];
            data[7] = (byte)'1';
            data[8] = (byte)'0';
            var length = _formatter.GetLengthOfField(data, 7);
            Assert.AreEqual(10, length);
        }

        [TestMethod]
        public void TestValidity()
        {
            Assert.IsTrue(_formatter.IsValidLength(0));
            Assert.IsTrue(_formatter.IsValidLength(8));
            Assert.IsTrue(_formatter.IsValidLength(12));
            Assert.IsFalse(_formatter.IsValidLength(13));
        }
    }
}