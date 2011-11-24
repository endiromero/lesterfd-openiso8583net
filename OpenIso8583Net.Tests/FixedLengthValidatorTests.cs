using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.LengthValidators;

namespace OpenIso8583Net.Tests
{
    /// <summary>
    ///   Summary description for FixedLengthValidatorTests
    /// </summary>
    [TestClass]
    public class FixedLengthValidatorTests
    {
        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestTooShort()
        {
            var val = new FixedLengthValidator(6);
            var actual = val.IsValid("13245");
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void TestTooLong()
        {
            var val = new FixedLengthValidator(6);
            var actual = val.IsValid("1324567");
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void TestCorrectLength()
        {
            var val = new FixedLengthValidator(6);
            var actual = val.IsValid("132456");
            Assert.AreEqual(true, actual);
        }
    }
}