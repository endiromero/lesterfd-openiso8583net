using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests
{
    /// <summary>
    ///   Summary description for ConvertTests
    /// </summary>
    [TestClass]
    public class IsoConvertTests
    {
        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void FromIntToMsgTypeTest()
        {
            var res = IsoConvert.FromIntToMsgType(0x200);
            Assert.AreEqual("0200", res);
        }

        [TestMethod]
        public void FromMsgTypeToInt()
        {
            var res = IsoConvert.FromMsgTypeToInt("0200");
            Assert.AreEqual(0x200, res);
        }
    }
}