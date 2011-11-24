using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.LengthValidators;

namespace OpenIso8583Net.Tests
{
    /// <summary>
    ///   Summary description for VariableLengthValidatorTests
    /// </summary>
    [TestClass]
    public class VariableLengthValidatorTests
    {
        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestVariableLengthValidatorTooShort()
        {
            var val = new VariableLengthValidator(5, 7);
            var actual = val.IsValid("1234");
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void TestVariableLengthValidatorTooLong()
        {
            var val = new VariableLengthValidator(5, 7);
            var actual = val.IsValid("12345678");
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void TestVariableLengthValidatorCorrect()
        {
            var val = new VariableLengthValidator(5, 7);
            var actual = val.IsValid("12345");
            Assert.AreEqual(true, actual);
            actual = val.IsValid("1234567");
            Assert.AreEqual(true, actual);
        }
    }
}