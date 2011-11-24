using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.LengthFormatters;

namespace OpenIso8583Net.Tests.LengthFormatters
{
    [TestClass]
    public class TestFixedLengthFormatter
    {
        private readonly FixedLengthFormatter _formatter = new FixedLengthFormatter(8);

        [TestMethod]
        public void TestLengthOfLengthIndicator()
        {
            Assert.AreEqual(0, _formatter.LengthOfLengthIndicator);
        }

        [TestMethod]
        public void TestPackLength()
        {
            var data = new byte[4];
            var offset = _formatter.Pack(data, 8, 2);
            Assert.AreEqual(2, offset);
            CollectionAssert.AreEqual(new byte[4], data);
        }

        [TestMethod]
        public void TestLengthOfField()
        {
            var data = new byte[14];
            var length = _formatter.GetLengthOfField(data, 7);
            Assert.AreEqual(8, length);
        }

        [TestMethod]
        public void TestValidity()
        {
            Assert.IsFalse(_formatter.IsValidLength(0));
            Assert.IsFalse(_formatter.IsValidLength(7));
            Assert.IsFalse(_formatter.IsValidLength(9));
            Assert.IsTrue(_formatter.IsValidLength(8));
        }
    }
}