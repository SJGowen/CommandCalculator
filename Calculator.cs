using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CommandCalculator
{
    public class Calculator
    {
        private readonly string _invalidExpression = "Invalid expression.";
        private bool _floatingPointExpression;

        public string Calculate(string equation)
        {
            equation = RemoveSpaces(equation);
            _floatingPointExpression = equation.Contains('.');
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

                    equationIndex += 1;
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

            var bits = BreakUpEquation(equation);
            ApplyNegatives(bits);
            if (bits.Count % 2 == 0) return _invalidExpression; // Valid expression have an odd number of bits
            if (bits.Count <= 1) return bits.Count == 1 ? bits[0] : _invalidExpression;
            CondenseListByDoing(bits, "^");
            CondenseListByDoing(bits, "/%*");
            CondenseListByDoing(bits, "+-");
            return bits.Count == 1 ? bits[0] : _invalidExpression;
        }

        private static List<string> BreakUpEquation(string equation)
        {
            var bit = string.Empty;
            var bits = new List<string>();
            foreach (var character in equation)
            {
                if (int.TryParse(character.ToString(), out var _) || character == '.')
                {
                    bit += character;
                }
                else
                {
                    if (bit != string.Empty)
                    {
                        bits.Add(bit);
                        bit = string.Empty;
                    }

                    bits.Add(character.ToString());
                }
            }

            if (bit != string.Empty) bits.Add(bit);

            return bits;
        }

        private void ApplyNegatives(IList<string> bits)
        {
            if (bits.IndexOf("-") == -1) return;
            var bitIndex = 0;
            while (bitIndex < bits.Count)
            {
                if (bits[bitIndex] == "-" && NoNumberPrecedesThisCharacter(bits, bitIndex))
                {
                    if (_floatingPointExpression)
                    {
                        if (float.TryParse(bits[bitIndex + 1], out var float1))
                        {
                            bits[bitIndex + 1] = (float1 * -1).ToString("F");
                            bits.RemoveAt(bitIndex);
                        }
                    }
                    else
                    {
                        if (int.TryParse(bits[bitIndex + 1], out var integer1))
                        {
                            bits[bitIndex + 1] = (integer1 * -1).ToString();
                            bits.RemoveAt(bitIndex);
                        }
                    }
                }

                bitIndex++;
            }
        }

        private static bool NoNumberPrecedesThisCharacter(IList<string> bits, int bitIndex)
        {
            return bitIndex == 0 || !float.TryParse(bits[bitIndex - 1], out var _);
        }

        private void CondenseListByDoing(IList<string> bits, string mathsOperator)
        {
            if (bits.Count == 1) return;

            var bitIndex = 1; // Looking at operators only, hence start at 1 and increment by 2
            while (bitIndex < bits.Count)
            {
                foreach (var mathsOp in mathsOperator)
                {
                    WriteDebugMessageAndArray($"Looking at = '{bits[bitIndex]}'.\tLooking for = '{mathsOp.ToString()}'.\t", bits);
                    if (bits[bitIndex] == mathsOp.ToString())
                    {
                        if (_floatingPointExpression)
                        {
                            CondenseAsFloat(bits, bitIndex);
                        }
                        else
                        {
                            CondenseAsInt(bits, bitIndex);
                        }

                        bitIndex -= 2;
                        break;
                    }
                }

                bitIndex += 2;
            }

            WriteDebugMessageAndArray($"After ConsolidateListByDoing ({mathsOperator}).\t", bits);
        }

        private static void WriteDebugMessageAndArray(string message, IEnumerable<string> bits)
        {
            Debug.WriteLine($"{message}Bits = '{string.Join(' ', bits)}'.");
        }

        private void CondenseAsFloat(IList<string> bits, int bitIndex)
        {
            if (!float.TryParse(bits[bitIndex - 1], out var float1) ||
                (!float.TryParse(bits[bitIndex + 1], out var float2))) return;
            switch (bits[bitIndex])
            {
                case "^":
                    bits[bitIndex] = Math.Pow(float1, float2).ToString("F");
                    break;
                case "*":
                    bits[bitIndex] = (float1 * float2).ToString("F");
                    break;
                case "/":
                    bits[bitIndex] = (float1 / float2).ToString("F");
                    break;
                case "%":
                    bits[bitIndex] = (float1 % float2).ToString("F");
                    break;
                case "+":
                    bits[bitIndex] = (float1 + float2).ToString("F");
                    break;
                case "-":
                    bits[bitIndex] = (float1 - float2).ToString("F");
                    break;
                default:
                    bits[bitIndex] = _invalidExpression;
                    break;
            }

            bits.RemoveAt(bitIndex + 1);
            bits.RemoveAt(bitIndex - 1);
        }

        private void CondenseAsInt(IList<string> bits, int bitIndex)
        {
            if (!int.TryParse(bits[bitIndex - 1], out var integer1) ||
                (!int.TryParse(bits[bitIndex + 1], out var integer2))) return;
            switch (bits[bitIndex])
            {
                case "^":
                    bits[bitIndex] = Math.Pow(integer1, integer2).ToString("F0");
                    break;
                case "*":
                    bits[bitIndex] = (integer1 * integer2).ToString();
                    break;
                case "/":
                    bits[bitIndex] = (integer1 / integer2).ToString();
                    break;
                case "%":
                    bits[bitIndex] = (integer1 % integer2).ToString();
                    break;
                case "+":
                    bits[bitIndex] = (integer1 + integer2).ToString();
                    break;
                case "-":
                    bits[bitIndex] = (integer1 - integer2).ToString();
                    break;
                default:
                    bits[bitIndex] = _invalidExpression;
                    break;
            }

            bits.RemoveAt(bitIndex + 1);
            bits.RemoveAt(bitIndex - 1);
        }
    }
}