using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.FieldValidator;
using OpenIso8583Net.Formatter;
using OpenIso8583Net.LengthFormatters;

namespace OpenIso8583Net.Tests
{
    /// <summary>
    ///   Summary description for BcdVariableField
    /// </summary>
    [TestClass]
    public class BcdVariableFieldTests
    {
        [TestMethod]
        public void TestPack()
        {
            var f = new Field(2,
                              new FieldDescriptor(new VariableLengthFormatter(2, 15, Formatters.Bcd), FieldValidators.N,
                                                  Formatters.Bcd));
            f.Value = "77";
            var actual = f.ToMsg();
            var expected = new byte[2];
            expected[0] = 0x02;
            expected[1] = 0x77;
            CollectionAssert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void Unpack()
        {
            var f = new Field(2,
                                  new FieldDescriptor(new VariableLengthFormatter(2, 15, Formatters.Bcd), FieldValidators.N,
                                                      Formatters.Bcd));
            var msg = new byte[2];
            msg[0] = 0x02;
            msg[1] = 0x77;
            f.Unpack(msg, 0);
            var actual = f.Value;
            const string expected = "77";
            Assert.AreEqual(expected, actual);
        }
    }
}