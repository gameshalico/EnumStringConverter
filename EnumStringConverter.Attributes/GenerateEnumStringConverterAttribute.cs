using System;

namespace EnumStringConverter
{
    public enum NamingCase
    {
        /// <summary>None - No conversion (uses original enum name)</summary>
        None = 0,
        /// <summary>PascalCase (e.g., MyVariable)</summary>
        PascalCase = 1,
        /// <summary>camelCase (e.g., myVariable)</summary>
        CamelCase = 2,
        /// <summary>snake_case (e.g., my_variable)</summary>
        SnakeCase = 3,
        /// <summary>UPPER_SNAKE (e.g., MY_VARIABLE)</summary>
        UpperSnakeCase = 4,
        /// <summary>Pascal_Snake (e.g., My_Variable)</summary>
        PascalSnakeCase = 5,
        /// <summary>kebab-case (e.g., my-variable)</summary>
        KebabCase = 6,
        /// <summary>COBOL-CASE (e.g., MY-VARIABLE)</summary>
        CobolCase = 7,
        /// <summary>Train-Case (e.g., My-Variable)</summary>
        TrainCase = 8,
        /// <summary>dot.case (e.g., my.variable)</summary>
        DotCase = 9,
        /// <summary>path/case (e.g., my/variable)</summary>
        PathCase = 10,
        /// <summary>lower case (e.g., my variable)</summary>
        LowerCase = 11,
        /// <summary>UPPER CASE (e.g., MY VARIABLE)</summary>
        UpperCase = 12,
        /// <summary>Title Case (e.g., My Variable)</summary>
        TitleCase = 13,
        /// <summary>Sentence case (e.g., My variable)</summary>
        SentenceCase = 14
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = true, Inherited = false)]
    public sealed class GenerateEnumStringConverterAttribute : Attribute
    {
        public Type? EnumType { get; }
        public string? ClassName { get; set; }
        public NamingCase From { get; set; } = NamingCase.PascalCase;
        public NamingCase To { get; set; } = NamingCase.None;

        public GenerateEnumStringConverterAttribute()
        {
        }

        public GenerateEnumStringConverterAttribute(string className)
        {
            ClassName = className;
        }

        public GenerateEnumStringConverterAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}
