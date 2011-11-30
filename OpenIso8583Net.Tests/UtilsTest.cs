using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests
{
    /// <summary>
    ///This is a test class for UtilsTest and is intended
    ///to contain all UtilsTest Unit Tests
    ///</summary>
    [TestClass]
    public class UtilsTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///A test for MaskPan
        ///</summary>
        [TestMethod]
        public void MaskPanTest()
        {
            const string pan = "1234567890123456";
            const string expected = "123456xxxxxx3456";
            var actual = Utils.MaskPan(pan);
            Assert.AreEqual(expected, actual);

            const string shortPan = "1234567890";
            var actualShort = Utils.MaskPan(shortPan);
            Assert.AreEqual(shortPan, actualShort);
        }
    }
}
