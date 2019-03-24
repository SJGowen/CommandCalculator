# CommandCalculator

A command line calculator that will evaluate an equation or string containing the following operators:
  1. Parenthesis ( and )
  2. Power ^
  3. Division / Remainder % and Multiplcation *
  4. Addition + and Subtraction -
  
If the equation contains multiple operators they are acted on in the order from the top. If they are shown in the same bullet point they are acted on in a left to right sequence.

This is an example of how you would use CommandCalculator or Calculator.cs

using System;
using CommandCalculator;

namespace CommandPrompt
{
    class Program
    {
        static void Main(string[] args)
        {
            string equation;
            var calculator = new Calculator();
            do
            {   
                Console.Write("calculator> ");
                equation = Console.ReadLine();
                var result = calculator.Calculate(equation);
                Console.WriteLine(result);
            } while (equation?.ToLower() != "exit");
        }
    }
}
