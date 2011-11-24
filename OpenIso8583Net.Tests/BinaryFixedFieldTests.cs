using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.Exceptions;
using OpenIso8583Net.FieldValidator;
using OpenIso8583Net.Formatter;
using OpenIso8583Net.LengthFormatters;

namespace OpenIso8583Net.Tests
{
    /// <summary>
    ///   Summary description for FixedFieldTests
    /// </summary>
    [TestClass]
    public class BinaryFixedFieldTests
    {
        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestBinaryFixedFieldUnpack()
        {
            var field = new Field(2,
                                  new FieldDescriptor(new FixedLengthFormatter(2), FieldValidators.Hex,
                                                      Formatters.Binary));
            var msg = new byte[8];
            msg[0] = 65;
            msg[1] = 65;
            msg[2] = 65;
            msg[3] = 65;
            msg[4] = 65;
            msg[5] = 65;
            msg[6] = 65;
            msg[7] = 65;

            var offset = field.Unpack(msg, 2);
            Assert.AreEqual(4, offset, "Offset expected to be 4");
            Assert.AreEqual("4141", field.Value);
        }

        [TestMethod]
        public void TestBinaryFixedFieldPack()
        {
            var field = new Field(2,
                                  new FieldDescriptor(new FixedLengthFormatter(4), FieldValidators.Hex,
                                                      Formatters.Binary));
            field.Value = "31323334";
            var expected = Encoding.ASCII.GetBytes("1234");

            var msg = field.ToMsg();
            CollectionAssert.AreEqual(expected, msg);
        }

        [TestMethod]
        public void TestBinaryFixedFieldTooShortPack()
        {
            var f = new Field(2,
                                     new FieldDescriptor(new FixedLengthFormatter(4), FieldValidators.Hex,
                                                         Formatters.Binary));
            f.Value = "12345";
            try
            {
                f.ToMsg();
                Assert.Fail("Expected FieldLengthException");
            }
            catch (FieldLengthException)
            {
            }
        }

        [TestMethod]
        public void TestBinaryFixedFieldTooLongPack()
        {
            var f = new Field(2,
                                   new FieldDescriptor(new FixedLengthFormatter(3), FieldValidators.Hex,
                                                       Formatters.Binary));
            f.Value = "12345678";
            try
            {
                f.ToMsg();
                Assert.Fail("Expected FieldLengthException");
            }
            catch (FieldLengthException)
            {
            }
        }

        [TestMethod]
        public void TestBinaryFixedFieldCorrectLengthPack()
        {
            var f = new Field(2,
                                    new FieldDescriptor(new FixedLengthFormatter(3), FieldValidators.Hex,
                                                        Formatters.Binary));
            f.Value = "123456";
            try
            {
                f.ToMsg();
            }
            catch (FieldLengthException)
            {
                Assert.Fail("Did not expect FieldLengthException");
            }
        }

        [TestMethod]
        public void TestBinaryFixedFieldImplementsValidatorPack()
        {
            var f = new Field(2,
                                   new FieldDescriptor(new FixedLengthFormatter(3), FieldValidators.Hex,
                                                       Formatters.Binary));
            f.Value = "abcdr5";
            try
            {
                f.ToMsg();
                Assert.Fail("Expected FieldFormatException");
            }
            catch (FieldFormatException)
            {
            }

        }

        [TestMethod]
        public void TestBinaryFixedFieldPackedLength()
        {
            var f = new Field(2,
                              new FieldDescriptor(new FixedLengthFormatter(4), FieldValidators.Hex,
                                                  Formatters.Binary));
            f.Value = "12345678";
            Assert.AreEqual(4, f.PackedLength);
        }
    }
}