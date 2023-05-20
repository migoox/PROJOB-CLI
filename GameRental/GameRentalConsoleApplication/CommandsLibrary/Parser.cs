using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameRentalClient
{
    public static class Parser
    {
        public static IComparable ComparableTypeParser(string input)
        {
            // is string
            if (Regex.IsMatch(input, "^\".*\"$"))
            {
                input = input[1..input.Length];
                input = input[0..(input.Length - 1)];
                return input;
            }
            // is numeric value
            int intResult = 0;
            double doubleResult = 0.0;
            if (int.TryParse(input, out intResult))
            {
                return intResult;
            }
            else if (double.TryParse(input, out doubleResult))
            {
                return doubleResult;
            }
            else
            {
                throw new ArgumentException("Input should be a string or a numeric value");
            }
        }

        public static int[] ParseIntArray(string input)
        {
            // Check if the input starts and ends with square brackets
            if (!input.StartsWith("[") || !input.EndsWith("]"))
            {
                throw new ArgumentException("Input must start and end with square brackets.");
            }

            input = input.Substring(1, input.Length - 2);
            string[] parts = input.Split(',');
            
            int[] result = new int[parts.Length];

            // Parse each part of the input string and add it to the result array
            for (int i = 0; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i], out int value))
                {
                    throw new ArgumentException($"Invalid input at index {i}: '{parts[i]}' is not a valid integer.");
                }

                result[i] = value;
            }

            return result;
        }

        public static string FormatObject(object obj)
        {
            if (obj is string)
            {
                return "\"" + (string)obj + "\"";
            } 
            else if (obj is int)
            {
                return ((int)obj).ToString();
            }
            else if (obj is double)
            {
                return ((double)obj).ToString();
            }
            else if (obj is int[])
            {
                return "[" + string.Join(",", (int[])obj) + "]";
            }

            throw new Exception("Cannot convert to string");
        }
    }
}
