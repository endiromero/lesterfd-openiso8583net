using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.Formatter;

namespace OpenIso8583Net.Tests
{
    [TestClass]
    public class BinaryFieldFormatterTest
    {
        [TestMethod]
        public void GetBytesTest()
        {
            IFormatter target = new BinaryFormatter();
            const string value = "31323334";
            var expected = new byte[] { 0x31, 0x32, 0x33, 0x34 };
            var actual = target.GetBytes(value);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetString()
        {
            IFormatter target = new BinaryFormatter();
            const string expected = "31323334";
            var input = new byte[] { 0x31, 0x32, 0x33, 0x34 };
            var actual = target.GetString(input);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPackedLength()
        {
            IFormatter formatter = new BinaryFormatter();
            var actual = formatter.GetPackedLength(8);
            Assert.AreEqual(4, actual);
        }
    }
}