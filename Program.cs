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
