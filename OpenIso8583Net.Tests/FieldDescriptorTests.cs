using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.Exceptions;
using OpenIso8583Net.FieldValidator;
using OpenIso8583Net.Formatter;
using OpenIso8583Net.LengthFormatters;

namespace OpenIso8583Net.Tests
{
    [TestClass]
 public  class FieldDescriptorTests
    {
        [TestMethod]
        public void TestBinaryFieldMustHaveHexValidator()
        {
            try
            {
                new FieldDescriptor(new FixedLengthFormatter(8), FieldValidators.None, Formatters.Binary);
                Assert.Fail("Binary formatter must have hex validator");
            }
            catch (FieldDescriptorException)
            {
            }
        }
    }
}
