using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests
{
    ///<summary>
    ///  This is a test class for ProcessingCodeTest and is intended
    ///  to contain all ProcessingCodeTest Unit Tests
    ///</summary>
    [TestClass]
    public class ProcessingCodeTest
    {
        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestDataTooShort()
        {
            const string data = "12345";
            try
            {
                new ProcessingCode(data);
                Assert.Fail("Failed length processing");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestDataTooLong()
        {
            const string data = "1234567";
            try
            {
                new ProcessingCode(data);
                Assert.Fail("Failed length processing");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestValidConstructor()
        {
            const string data = "112233";
            var proc = new ProcessingCode(data);
            Assert.AreEqual("11", proc.TranType);
            Assert.AreEqual("22", proc.FromAccountType);
            Assert.AreEqual("33", proc.ToAccountType);
        }
    }
}