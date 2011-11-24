using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests
{
    ///<summary>
    ///  This is a test class for HashtableMessageTest and is intended
    ///  to contain all HashtableMessageTest Unit Tests
    ///</summary>
    [TestClass]
    public class HashtableMessageTest
    {
        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestFromMessageString()
        {
            var hash = new HashtableMessage();
            const string msgString = "15Unit1227This is the data for unit 1210TagNumber215tag 2";
            hash.FromMessageString(msgString);

            Assert.AreEqual("This is the data for unit 1", hash["Unit1"]);
            Assert.AreEqual("tag 2", hash["TagNumber2"]);

            Assert.AreEqual(2, hash.Keys.Count);
            Assert.AreEqual(2, hash.Count);
        }

        ///<summary>
        ///  A test for ToString
        ///</summary>
        [TestMethod]
        public void TestToMessageString()
        {
            var hash = new HashtableMessage();
            hash["Unit1"] = "This is the data for unit 1";
            hash["TagNumber2"] = "tag 2";
            const string expected = "15Unit1227This is the data for unit 1210TagNumber215tag 2";
            Assert.AreEqual(expected, hash.ToMessageString());
        }
    }
}