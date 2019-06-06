using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CommandCalculator
{
    public class Calculator
    {
        private readonly string _invalidExpression = "Invalid expression.";
        private static readonly char _decimalSeparator = '.';
        private bool _floatingPointExpression;

        public string Calculate(string equation)
        {
            equation = RemoveSpaces(equation);
            _floatingPointExpression = equation.Contains(_decimalSeparator);
            if (!ParenthesisIsBalanced(equation)) return _invalidExpression;
            equation = EvaluateParenthesisedPiecesOfEquation(equation);
            return WeightedCalculate(equation);
        }

        private static string RemoveSpaces(string equation)
        {
            return equation.Replace(" ", string.Empty);
        }

        private static bool ParenthesisIsBalanced(string equation)
        {
            return equation.Count(x => x == '(') == equation.Count(x => x == ')');
        }

        private string EvaluateParenthesisedPiecesOfEquation(string equation)
        {
            while (equation.Contains("("))
            {
                var length = 0;
                var startIndex = 0;
                var equationIndex = 0;
                foreach (var character in equation)
                {
                    if (character == '(')
                    {
                        startIndex = equationIndex + 1;
                        length = 0;
                    }
                    else if (character == ')' && length == 0)
                    {
                        length = equationIndex - startIndex;
                    }

                    equationIndex++;
                }

                if (length <= 0) continue;
                var subEquation = equation.Substring(startIndex, length);
                var result = WeightedCalculate(subEquation);
                equation = equation.Replace("(" + subEquation + ")", result);
            }

            return equation;
        }

        private string WeightedCalculate(string equation)
        {
            if (equation.Length == 0) return string.Empty;
            if (equation.Contains(_invalidExpression)) return _invalidExpression;

            var list = BreakUpEquation(equation);
            ApplyNegatives(list);
            if (list.Count % 2 == 0) return _invalidExpression; // Valid expression have an odd number of list
            if (list.Count <= 1) return list.Count == 1 ? list[0] : _invalidExpression;
            CondenseListByCalculating(list, "^");
            CondenseListByCalculating(list, "/%*");
            CondenseListByCalculating(list, "+-");
            return list.Count == 1 ? list[0] : _invalidExpression;
        }

        private static List<string> BreakUpEquation(string equation)
        {
            var item = string.Empty;
            var list = new List<string>();
            foreach (var character in equation)
            {
                if (int.TryParse(character.ToString(), out var _) || character == _decimalSeparator)
                {
                    item += character;
                }
                else
                {
                    if (item != string.Empty)
                    {
                        list.Add(item);
                        item = string.Empty;
                    }

                    list.Add(character.ToString());
                }
            }

            if (item != string.Empty) list.Add(item);

            return list;
        }

        private void ApplyNegatives(List<string> list)
        {
            if (list.IndexOf("-") == -1) return;
            var itemIndex = 0;
            while (itemIndex < list.Count)
            {
                if (list[itemIndex] == "-" && ItemPrecededByNonNumeric(list, itemIndex))
                {
                    if (_floatingPointExpression)
                    {
                        if (float.TryParse(list[itemIndex + 1], out var float1))
                        {
                            list[itemIndex + 1] = (float1 * -1).ToString("F");
                            list.RemoveAt(itemIndex);
                        }
                    }
                    else
                    {
                        if (int.TryParse(list[itemIndex + 1], out var integer1))
                        {
                            list[itemIndex + 1] = (integer1 * -1).ToString();
                            list.RemoveAt(itemIndex);
                        }
                    }
                }

                itemIndex++;
            }
        }

        private static bool ItemPrecededByNonNumeric(IReadOnlyList<string> list, int itemIndex)
        {
            return itemIndex == 0 || !float.TryParse(list[itemIndex - 1], out var _);
        }

        private void CondenseListByCalculating(IList<string> list, string mathsOperator)
        {
            if (list.Count == 1) return;

            var itemIndex = 1; // Looking at operators only, hence start at 1 and increment by 2
            while (itemIndex < list.Count)
            {
                foreach (var mathsOp in mathsOperator)
                {
                    WriteDebugMessageAndArray($"Looking at = '{list[itemIndex]}'.\tLooking for = '{mathsOp.ToString()}'.\t", list);
                    if (list[itemIndex] == mathsOp.ToString())
                    {
                        list[itemIndex] = _floatingPointExpression 
                            ? CalculateAsFloat(list[itemIndex - 1], list[itemIndex], list[itemIndex + 1]) 
                            : CalculateAsInteger(list[itemIndex - 1], list[itemIndex], list[itemIndex + 1]);

                        list.RemoveAt(itemIndex + 1);
                        list.RemoveAt(itemIndex - 1);
                        itemIndex -= 2;
                        break;
                    }
                }

                itemIndex += 2;
            }

            WriteDebugMessageAndArray($"After ConsolidateListByDoing ({mathsOperator}).\t", list);
        }

        private static void WriteDebugMessageAndArray(string message, IEnumerable<string> list)
        {
            Debug.WriteLine($"{message}list = '{string.Join(' ', list)}'.");
        }

        private string CalculateAsFloat(string number1, string operation, string number2)
        {
            if (!float.TryParse(number1, out var float1) ||
                (!float.TryParse(number2, out var float2))) return _invalidExpression;
            switch (operation)
            {
                case "^": return Math.Pow(float1, float2).ToString("F");
                case "*": return (float1 * float2).ToString("F");
                case "/": return (float1 / float2).ToString("F");
                case "%": return (float1 % float2).ToString("F");
                case "+": return (float1 + float2).ToString("F");
                case "-": return (float1 - float2).ToString("F");
                default: return _invalidExpression;
            }
        }

        private string CalculateAsInteger(string number1, string operation, string number2)
        {
            if (!int.TryParse(number1, out var integer1) ||
                (!int.TryParse(number2, out var integer2))) return _invalidExpression;
            switch (operation)
            {
                case "^": return Math.Pow(integer1, integer2).ToString("F0");
                case "*": return (integer1 * integer2).ToString();
                case "/": return (integer1 / integer2).ToString();
                case "%": return (integer1 % integer2).ToString();
                case "+": return (integer1 + integer2).ToString();
                case "-": return (integer1 - integer2).ToString();
                default: return _invalidExpression;
            }
        }
    }
}