using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests
{
    ///<summary>
    ///  This is a test class for AdditionalAmountTest and is intended
    ///  to contain all AdditionalAmountTest Unit Tests
    ///</summary>
    [TestClass]
    public class AdditionalAmountTest
    {
        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestValidConstructor()
        {
            const string input = "1001840C000000022000";
            var amount = new AdditionalAmount(input);
            Assert.AreEqual("10", amount.AccountType, "AccountType");
            Assert.AreEqual("01", amount.AmountType, "AmountType");
            Assert.AreEqual("840", amount.CurrencyCode, "CurrencyCode");
            Assert.AreEqual("C", amount.Sign, "Sign");
            Assert.AreEqual("000000022000", amount.Amount, "Amount");
            Assert.AreEqual(22000, amount.Value, "Value");
        }

        [TestMethod]
        public void TestConstructorTooShort()
        {
            const string input = "1001840C00000002200";
            try
            {
                new AdditionalAmount(input);
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestConstructorTooLong()
        {
            const string input = "1001840C0000000220000";
            try
            {
                new AdditionalAmount(input);
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestValuePositive()
        {
            var amount = new AdditionalAmount();
            amount.Sign = "C";
            amount.Amount = "000002000000";
            const long expected = 2000000;
            var actual = amount.Value;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestValueNegative()
        {
            var amount = new AdditionalAmount();
            amount.Sign = "D";
            amount.Amount = "000002000000";
            const long expected = -2000000;
            var actual = amount.Value;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestValuePropagatesPositive()
        {
            var amount = new AdditionalAmount {Value = 2245};
            Assert.AreEqual("C", amount.Sign);
            Assert.AreEqual("000000002245", amount.Amount);
        }

        [TestMethod]
        public void TestValuePropagatesNegative()
        {
            var amount = new AdditionalAmount {Value = -2245};
            Assert.AreEqual("D", amount.Sign);
            Assert.AreEqual("000000002245", amount.Amount);
        }

        [TestMethod]
        public void TestAmountPads()
        {
            var amount = new AdditionalAmount {Amount = "200"};
            Assert.AreEqual("000000000200", amount.Amount);
        }

        [TestMethod]
        public void TestToString()
        {
            var amount = new AdditionalAmount
                             {AccountType = "10", Amount = "200", AmountType = "01", CurrencyCode = "840", Sign = "C"};
            var actual = amount.ToString();
            const string expected = "1001840C000000000200";
            Assert.AreEqual(expected, actual);
        }
    }
}