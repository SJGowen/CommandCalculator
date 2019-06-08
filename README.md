# CommandCalculator

A command line calculator that will evaluate an equation or string containing the following operators:
  1. Parenthesis ( and )
  2. Power ^
  3. Division / Remainder % and Multiplcation *
  4. Addition + and Subtraction -
  
If the equation contains multiple operators they are acted on in the order from top to bottom of the list. If they are shown in the same bullet point they are acted on in a left to right sequence of the order that they appear in the equation.

This is written as an extension method of the string class, therefore as long as you have the StringExtensionsCalculate.cs in scope and are using the System namespace all should be well.

    using System;
    using CommandCalculator;
    
    namespace CommandPrompt
    {
        class Program
        {
            static void Main(string[] args)
            {
                while (true)
                {   
                    Console.Write("Calculate > ");
                    var equation = Console.ReadLine();
                    if (equation?.ToLower() == "exit") break;
                    var result = equation.Calculate();
                    Console.WriteLine(result);
                }
            }
        }
    }
