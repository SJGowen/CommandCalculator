using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandCalculator
{
    [TestClass]
    public class CalculatorTest
    {
        [TestMethod]
        [DataRow("", "")]
        [DataRow("", " ")]
        [DataRow("1", "1")]
        [DataRow("123", "123")]
        [DataRow("2", "1+1")]
        [DataRow("444", "123+321")]
        [DataRow("15", "1+2 + 3 +4+5")]
        [DataRow("5", "1+2 + 3 +4-5")]
        [DataRow("26", "1+2 + 3 +4*5")]
        [DataRow("8", "1+2 + 3 +10/5")]
        [DataRow("10", "1+2+2 + 3 +10/5")]
        [DataRow("13", "1+2+3 + 3 +4%5")]
        [DataRow("7", "1+2+5 % 3 +12%5")]
        [DataRow("21", "1+4^2+5 % 3 +12%5")]
        public void CalculationsTests(string expectedResult, string equation)
        {
            var result = equation.Calculate();
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow("3", "1+5-(6/2)")]
        [DataRow("14", "10+5-((8/2)-(6/2))")]
        [DataRow("9", "10+5-(6/2)-(6/2)")]
        public void CalculationsWithBracketsTests(string expectedResult, string equation)
        {
            var result = equation.Calculate();
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow("-46", "-23+-23")]
        [DataRow("0", "-23+23")]
        [DataRow("4", "-24/-6")]
        [DataRow("-4", "24/-6")]
        public void CalculationsWithNegativeNumbersTests(string expectedResult, string equation)
        {
            var result = equation.Calculate();
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow("Invalid expression.", "-23+&23")]
        [DataRow("Invalid expression.", "-23+$23.5")]
        [DataRow("Invalid expression.", "-24//6")]
        [DataRow("Invalid expression.", "1+(24/-6))")]
        [DataRow("Invalid expression.", "1+(24/-6O3)")]
        [DataRow("Invalid expression.", "1+(2O4/-63)")]
        [DataRow("Invalid expression.", ")3+5(")]
        public void CalculationsWithErrorsTests(string expectedResult, string equation)
        {
            var result = equation.Calculate();
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow("3.00", "24.3/8.1")]
        [DataRow("3.00", "24.0/8.0")]
        [DataRow("-31.50", "-4.5*7")]
        [DataRow("31.50", "8%5-3+4.5*7")]
        [DataRow("27.00", "3.0^3")]
        public void CalculationsForFloatingPointTests(string expectedResult, string equation)
        {
            var result = equation.Calculate();
            Assert.AreEqual(expectedResult, result);
        }
    }
}
