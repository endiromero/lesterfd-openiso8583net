using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace OpenIso8583Net.Tests
{
    [TestClass]
    public class Iso8583Rev93Tests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestMessagePackLength()
        {
            var msg = new Iso8583Rev93();
            msg[2] = "58889212354567816";
            msg[3] = "270010";
            msg[102] = "9012273811";
            msg.MessageType = Iso8583Rev93.MsgType._1200_TRAN_REQ;

            var actual = msg.PackedLength;

            Assert.AreEqual(57, actual);
        }
    }
}