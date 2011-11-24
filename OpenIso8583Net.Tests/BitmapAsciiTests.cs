using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.Formatter;

namespace OpenIso8583Net.Tests
{
    /// <summary>
    ///   Summary description for BitmapTests
    /// </summary>
    [TestClass]
    public class BitmapAsciiTests
    {
        private Bitmap _bitmap;

        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///   Initialize the bitmap for testing
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _bitmap = new Bitmap(new AsciiFormatter());
        }

        /// <summary>
        ///   Create a new bitmap and set a field under 64 to true.  Check that the bitmap is not extended
        /// </summary>
        [TestMethod]
        public void TestNewBitmapNotExtended()
        {
            _bitmap[2] = true;
            _bitmap[64] = true;
            Assert.IsFalse(_bitmap.IsExtendedBitmap);
        }

        /// <summary>
        ///   Create a new bitmap, set a field under 64 to true and one over 64 to true.  Check that the bitmap
        ///   is extended
        /// </summary>
        [TestMethod]
        public void TestNewBitmapThenExtended()
        {
            _bitmap[2] = true;
            _bitmap[64] = true;
            _bitmap[65] = true;
            Assert.IsTrue(_bitmap.IsExtendedBitmap);
        }

        /// <summary>
        ///   Create a new bitmap, set a field under 64 to true and one over 64 to true.  Then set that guy
        ///   to false. Check that the bitmap is NOT extended
        /// </summary>
        [TestMethod]
        public void TestNewBitmapThatIsExtendedAndThenUnextended()
        {
            _bitmap[2] = true;
            _bitmap[64] = true;
            _bitmap[65] = true;
            _bitmap[65] = false;
            Assert.IsFalse(_bitmap.IsExtendedBitmap);
        }

        /// <summary>
        ///   Test the bitmap pack method
        /// </summary>
        [TestMethod]
        public void TestBitmapToMsg()
        {
            _bitmap[2] = true;
            _bitmap[64] = true;
            var data = _bitmap.ToMsg();
            var expected = new byte[data.Length];
            for (var i = 0; i < expected.Length; i++)
                expected[i] = 0x30;
            expected[0] = 0x34;
            expected[15] = 0x31;
            for (var i = 0; i < data.Length; i++)
                Assert.AreEqual(expected[i], data[i], "Error in index " + i);
        }

        /// <summary>
        ///   Test the bitmap pack method with an extended bitmap
        /// </summary>
        [TestMethod]
        public void TestBitmapExtendedToMsg()
        {
            _bitmap[2] = true;
            _bitmap[64] = true;
            _bitmap[65] = true;
            _bitmap[128] = true;
            var data = _bitmap.ToMsg();
            var expected = new byte[data.Length];
            for (var i = 0; i < expected.Length; i++)
                expected[i] = 0x30; // Make sure it's all padded with zero's
            expected[0] = 67;
            expected[15] = 49;
            expected[16] = 56;
            expected[31] = 49;
            for (var i = 0; i < data.Length; i++)
                Assert.AreEqual(expected[i], data[i], "Error in field " + (i + 1));
        }

        [TestMethod]
        public void TestUnpackBitmapNotExtended()
        {
            var input = new byte[36];
            for (var i = 0; i < input.Length; i++)
                input[i] = 0x30;
            input[4] = 0x34;
            input[4 + 15] = 0x31;
            var offset = _bitmap.Unpack(input, 4);
            Assert.AreEqual(20, offset, "Offset expected to be 20");
            Assert.IsTrue(_bitmap[2], "Field 2 expected to be set");
            Assert.IsTrue(_bitmap[64], "Field 64 expected to be set");
            Assert.IsFalse(_bitmap.IsExtendedBitmap, "This is not an extended bitmap");
            Assert.IsFalse(_bitmap[63], "Field 63 expected to be off");
        }

        [TestMethod]
        public void TestUnpackBitmaExtended()
        {
            var input = new byte[36];
            for (var i = 0; i < input.Length; i++)
                input[i] = 0x30;
            input[4] = 0x43;
            input[4 + 15] = 0x31;//49
            input[4 + 16] = 0x38;//56
            input[4 + 31] = 0x31;
            var offset = _bitmap.Unpack(input, 4);
            Assert.AreEqual(36, offset, "Offset expected to be 20");
            Assert.AreEqual(true, _bitmap[2], "Field 2 expected to be set");
            Assert.AreEqual(true, _bitmap[64], "Field 64 expected to be set");
            Assert.AreEqual(true, _bitmap[65], "Field 65 expected to be set");
            Assert.AreEqual(true, _bitmap[128], "Field 128 expected to be set");
            Assert.AreEqual(true, _bitmap.IsExtendedBitmap, "This is an extended bitmap");
            Assert.AreEqual(false, _bitmap[63], "Field 63 expected to be off");
        }
    }
}