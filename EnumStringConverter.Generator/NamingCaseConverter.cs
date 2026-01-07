using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnumStringConverter.Generator
{
    /// <summary>
    /// Provides conversion between different naming cases
    /// </summary>
    internal static class NamingCaseConverter
    {
        /// <summary>
        /// Converts a PascalCase string to the specified naming case
        /// </summary>
        public static string Convert(string pascalCaseInput, NamingCase fromCase, NamingCase toCase)
        {
            if (fromCase == toCase)
                return pascalCaseInput;

            // First, split the input into words based on the 'from' case
            var words = SplitIntoWords(pascalCaseInput, fromCase);

            // Then, join the words according to the 'to' case
            return JoinWords(words, toCase);
        }

        private static List<string> SplitIntoWords(string input, NamingCase namingCase)
        {
            // NamingCase enum values:
            // 0: None, 1: PascalCase, 2: CamelCase, 3: SnakeCase, 4: UpperSnakeCase,
            // 5: PascalSnakeCase, 6: KebabCase, 7: CobolCase, 8: TrainCase,
            // 9: DotCase, 10: PathCase, 11: LowerCase, 12: UpperCase,
            // 13: TitleCase, 14: SentenceCase

            switch (namingCase)
            {
                case NamingCase.None:
                case NamingCase.PascalCase:
                case NamingCase.CamelCase:
                    return SplitPascalCase(input);

                case NamingCase.SnakeCase:
                case NamingCase.UpperSnakeCase:
                    return input.Split('_').Where(w => !string.IsNullOrEmpty(w)).ToList();

                case NamingCase.PascalSnakeCase:
                    return input.Split('_').Where(w => !string.IsNullOrEmpty(w)).ToList();

                case NamingCase.KebabCase:
                case NamingCase.CobolCase:
                case NamingCase.TrainCase:
                    return input.Split('-').Where(w => !string.IsNullOrEmpty(w)).ToList();

                case NamingCase.DotCase:
                    return input.Split('.').Where(w => !string.IsNullOrEmpty(w)).ToList();

                case NamingCase.PathCase:
                    return input.Split('/').Where(w => !string.IsNullOrEmpty(w)).ToList();

                case NamingCase.LowerCase:
                case NamingCase.UpperCase:
                case NamingCase.TitleCase:
                case NamingCase.SentenceCase:
                    return input.Split(' ').Where(w => !string.IsNullOrEmpty(w)).ToList();

                default:
                    return SplitPascalCase(input);
            }
        }

        private static List<string> SplitPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new List<string>();

            var words = new List<string>();
            var currentWord = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                // If we hit an uppercase letter and we have content in current word
                if (char.IsUpper(c) && currentWord.Length > 0)
                {
                    // Check if the previous character is also uppercase (we're in an acronym)
                    bool previousIsUpper = i > 0 && char.IsUpper(input[i - 1]);
                    // Check if the next character is uppercase
                    bool nextIsUpper = i + 1 < input.Length && char.IsUpper(input[i + 1]);
                    // Check if the next character is lowercase
                    bool nextIsLower = i + 1 < input.Length && char.IsLower(input[i + 1]);
                    // Check if this is the last character
                    bool isLastChar = i + 1 >= input.Length;

                    // If previous is uppercase and next is lowercase, this is the start of a new word
                    // Example: "XMLHttp" -> at 'H', previous='L' (upper), next='t' (lower)
                    if (previousIsUpper && nextIsLower)
                    {
                        words.Add(currentWord.ToString());
                        currentWord.Clear();
                        currentWord.Append(c);
                    }
                    // If previous is uppercase and this is the last character, continue the current word
                    // Example: "API" -> at 'I', previous='P' (upper), last char
                    else if (previousIsUpper && isLastChar)
                    {
                        currentWord.Append(c);
                    }
                    // If previous is uppercase and next is uppercase, continue the acronym
                    // Example: "XML" -> at 'M', previous='X' (upper), next='L' (upper)
                    else if (previousIsUpper && nextIsUpper)
                    {
                        currentWord.Append(c);
                    }
                    // Otherwise, start a new word
                    else
                    {
                        words.Add(currentWord.ToString());
                        currentWord.Clear();
                        currentWord.Append(c);
                    }
                }
                else
                {
                    currentWord.Append(c);
                }
            }

            if (currentWord.Length > 0)
            {
                words.Add(currentWord.ToString());
            }

            return words;
        }

        private static string JoinWords(List<string> words, NamingCase namingCase)
        {
            if (words.Count == 0)
                return string.Empty;

            switch (namingCase)
            {
                case NamingCase.None:
                case NamingCase.PascalCase:
                    return string.Join("", words.Select(ToPascalWord));

                case NamingCase.CamelCase:
                    return ToLowerWord(words[0]) + string.Join("", words.Skip(1).Select(ToPascalWord));

                case NamingCase.SnakeCase:
                    return string.Join("_", words.Select(ToLowerWord));

                case NamingCase.UpperSnakeCase:
                    return string.Join("_", words.Select(ToUpperWord));

                case NamingCase.PascalSnakeCase:
                    return string.Join("_", words.Select(ToPascalWord));

                case NamingCase.KebabCase:
                    return string.Join("-", words.Select(ToLowerWord));

                case NamingCase.CobolCase:
                    return string.Join("-", words.Select(ToUpperWord));

                case NamingCase.TrainCase:
                    return string.Join("-", words.Select(ToPascalWord));

                case NamingCase.DotCase:
                    return string.Join(".", words.Select(ToLowerWord));

                case NamingCase.PathCase:
                    return string.Join("/", words.Select(ToLowerWord));

                case NamingCase.LowerCase:
                    return string.Join(" ", words.Select(ToLowerWord));

                case NamingCase.UpperCase:
                    return string.Join(" ", words.Select(ToUpperWord));

                case NamingCase.TitleCase:
                    return string.Join(" ", words.Select(ToPascalWord));

                case NamingCase.SentenceCase:
                {
                    var result = string.Join(" ", words.Select(ToLowerWord));
                    if (result.Length > 0)
                    {
                        result = char.ToUpperInvariant(result[0]) + result.Substring(1);
                    }
                    return result;
                }

                default:
                    return string.Join("", words.Select(ToPascalWord));
            }
        }

        private static string ToPascalWord(string word)
        {
            if (string.IsNullOrEmpty(word))
                return word;

            return char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant();
        }

        private static string ToLowerWord(string word)
        {
            return word.ToLowerInvariant();
        }

        private static string ToUpperWord(string word)
        {
            return word.ToUpperInvariant();
        }
    }
}
