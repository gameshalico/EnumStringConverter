using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EnumStringConverter.Generator
{
    internal enum NamingCase
    {
        None = 0,
        PascalCase = 1,
        CamelCase = 2,
        SnakeCase = 3,
        UpperSnakeCase = 4,
        PascalSnakeCase = 5,
        KebabCase = 6,
        CobolCase = 7,
        TrainCase = 8,
        DotCase = 9,
        PathCase = 10,
        LowerCase = 11,
        UpperCase = 12,
        TitleCase = 13,
        SentenceCase = 14
    }

    internal readonly struct GenerationConfig
    {
        public INamedTypeSymbol EnumSymbol { get; }
        public string? ClassName { get; }
        public NamingCase FromCase { get; }
        public NamingCase ToCase { get; }
        public INamedTypeSymbol? ContainingClass { get; }

        public GenerationConfig(
            INamedTypeSymbol enumSymbol,
            string? className,
            NamingCase fromCase,
            NamingCase toCase,
            INamedTypeSymbol? containingClass = null)
        {
            EnumSymbol = enumSymbol;
            ClassName = className;
            FromCase = fromCase;
            ToCase = toCase;
            ContainingClass = containingClass;
        }

        public static string GetNamingCaseSuffix(NamingCase namingCase)
        {
            return namingCase switch
            {
                NamingCase.None => "",
                NamingCase.PascalCase => "PascalCase",
                NamingCase.CamelCase => "CamelCase",
                NamingCase.SnakeCase => "SnakeCase",
                NamingCase.UpperSnakeCase => "UpperSnakeCase",
                NamingCase.PascalSnakeCase => "PascalSnakeCase",
                NamingCase.KebabCase => "KebabCase",
                NamingCase.CobolCase => "CobolCase",
                NamingCase.TrainCase => "TrainCase",
                NamingCase.DotCase => "DotCase",
                NamingCase.PathCase => "PathCase",
                NamingCase.LowerCase => "LowerCase",
                NamingCase.UpperCase => "UpperCase",
                NamingCase.TitleCase => "TitleCase",
                NamingCase.SentenceCase => "SentenceCase",
                _ => ""
            };
        }
    }

    [Generator(LanguageNames.CSharp)]
    public sealed class EnumStringConverterGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classProvider = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "EnumStringConverter.GenerateEnumStringConverterAttribute",
                    predicate: static (node, _) => node is ClassDeclarationSyntax { Modifiers: var modifiers }
                        && modifiers.Any(m => m.ValueText == "partial"),
                    transform: static (ctx, _) => GetEnumTypesToGenerateFromClass(ctx))
                .Where(static x => x is not null);

            var enumProvider = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "EnumStringConverter.GenerateEnumStringConverterAttribute",
                    predicate: static (node, _) => node is EnumDeclarationSyntax,
                    transform: static (ctx, _) => GetEnumTypesToGenerateFromEnum(ctx))
                .SelectMany(static (configs, _) => configs);

            context.RegisterSourceOutput(classProvider, static (ctx, source) =>
            {
                if (!source.HasValue) return;
                var config = source.Value;
                var code = GeneratePartialExtensions(config);
                ctx.AddSource($"{config.ContainingClass!.Name}.{config.EnumSymbol.Name}.g.cs", code);
            });

            context.RegisterSourceOutput(enumProvider, static (ctx, config) =>
            {
                var code = GenerateStandaloneExtensions(config);
                ctx.AddSource($"{config.ClassName}.g.cs", code);
            });
        }

        private static GenerationConfig? GetEnumTypesToGenerateFromClass(
            GeneratorAttributeSyntaxContext context)
        {
            if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
                return null;

            if (context.Attributes.Length == 0)
                return null;

            var attribute = context.Attributes[0];
            if (attribute.ConstructorArguments.Length != 1)
                return null;

            var arg = attribute.ConstructorArguments[0];
            if (arg.Kind != TypedConstantKind.Type || arg.Value is not INamedTypeSymbol enumSymbol)
                return null;

            if (enumSymbol.TypeKind != TypeKind.Enum)
                return null;

            // Read From and To properties
            var fromCase = NamingCase.PascalCase;
            var toCase = NamingCase.None;

            foreach (var namedArgument in attribute.NamedArguments)
            {
                if (namedArgument.Key == "From" && namedArgument.Value.Value is int fromValue)
                {
                    fromCase = (NamingCase)fromValue;
                }
                else if (namedArgument.Key == "To" && namedArgument.Value.Value is int toValue)
                {
                    toCase = (NamingCase)toValue;
                }
            }

            return new GenerationConfig(
                enumSymbol: enumSymbol,
                className: null,
                fromCase: fromCase,
                toCase: toCase,
                containingClass: classSymbol);
        }

        private static ImmutableArray<GenerationConfig> GetEnumTypesToGenerateFromEnum(
            GeneratorAttributeSyntaxContext context)
        {
            if (context.TargetSymbol is not INamedTypeSymbol enumSymbol)
                return ImmutableArray<GenerationConfig>.Empty;

            if (enumSymbol.TypeKind != TypeKind.Enum)
                return ImmutableArray<GenerationConfig>.Empty;

            var configs = ImmutableArray.CreateBuilder<GenerationConfig>();

            foreach (var attribute in context.Attributes)
            {
                string? className = null;
                var fromCase = NamingCase.PascalCase;
                var toCase = NamingCase.None;

                if (attribute.ConstructorArguments.Length == 1
                    && attribute.ConstructorArguments[0].Value is string ctorClassName)
                {
                    className = ctorClassName;
                }

                foreach (var namedArgument in attribute.NamedArguments)
                {
                    if (namedArgument.Key == "ClassName" && namedArgument.Value.Value is string value)
                    {
                        className = value;
                    }
                    else if (namedArgument.Key == "From" && namedArgument.Value.Value is int fromValue)
                    {
                        fromCase = (NamingCase)fromValue;
                    }
                    else if (namedArgument.Key == "To" && namedArgument.Value.Value is int toValue)
                    {
                        toCase = (NamingCase)toValue;
                    }
                }

                className ??= $"{enumSymbol.Name}{GenerationConfig.GetNamingCaseSuffix(toCase)}Converter";

                configs.Add(new GenerationConfig(
                    enumSymbol: enumSymbol,
                    className: className,
                    fromCase: fromCase,
                    toCase: toCase,
                    containingClass: null));
            }

            return configs.ToImmutable();
        }

        private static string GeneratePartialExtensions(GenerationConfig config)
        {
            var namespaceName = config.ContainingClass!.ContainingNamespace.IsGlobalNamespace
                ? null
                : config.ContainingClass.ContainingNamespace.ToDisplayString();

            return GenerateExtensionsCore(namespaceName, config.ContainingClass.Name, config);
        }

        private static void GenerateGetNameMethod(
            StringBuilder sb,
            string enumName,
            ImmutableArray<IFieldSymbol> members,
            GenerationConfig config)
        {
            sb.AppendLine($"        public static string GetName({enumName} value)");
            sb.AppendLine("        {");
            sb.AppendLine("            return value switch");
            sb.AppendLine("            {");

            foreach (var member in members)
            {
                string nameValue;
                if (config.ToCase != NamingCase.None)
                {
                    var converted = NamingCaseConverter.Convert(member.Name, config.FromCase, config.ToCase);
                    nameValue = $"\"{converted}\"";
                }
                else
                {
                    nameValue = $"nameof({enumName}.{member.Name})";
                }
                sb.AppendLine($"                {enumName}.{member.Name} => {nameValue},");
            }

            sb.AppendLine($"                _ => value.ToString()");
            sb.AppendLine("            };");
            sb.AppendLine("        }");
        }

        private static void GenerateIsDefinedMethod(
            StringBuilder sb,
            string enumName,
            ImmutableArray<IFieldSymbol> members)
        {
            sb.AppendLine($"        public static bool IsDefined({enumName} value)");
            sb.AppendLine("        {");
            sb.AppendLine("            return value switch");
            sb.AppendLine("            {");

            foreach (var member in members)
            {
                sb.AppendLine($"                {enumName}.{member.Name} => true,");
            }

            sb.AppendLine("                _ => false");
            sb.AppendLine("            };");
            sb.AppendLine("        }");
        }

        private static void GenerateParseMethod(
            StringBuilder sb,
            string enumName,
            ImmutableArray<IFieldSymbol> members,
            GenerationConfig config)
        {
            var enumShortName = enumName.Split('.').Last();
            sb.AppendLine($"        public static {enumName} Parse(string value)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (string.IsNullOrEmpty(value))");
            sb.AppendLine($"                throw new System.ArgumentException(\"Value cannot be null or empty.\", nameof(value));");
            sb.AppendLine();
            sb.AppendLine("            return value switch");
            sb.AppendLine("            {");

            foreach (var member in members)
            {
                string matchValue;
                if (config.ToCase != NamingCase.None)
                {
                    var converted = NamingCaseConverter.Convert(member.Name, config.FromCase, config.ToCase);
                    matchValue = $"\"{converted}\"";
                }
                else
                {
                    matchValue = $"nameof({enumName}.{member.Name})";
                }
                sb.AppendLine($"                {matchValue} => {enumName}.{member.Name},");
            }

            sb.AppendLine($"                _ => throw new System.ArgumentException($\"Invalid value '{{value}}' for enum type '{enumShortName}'.\", nameof(value))");
            sb.AppendLine("            };");
            sb.AppendLine("        }");
        }

        private static void GenerateTryParseMethod(
            StringBuilder sb,
            string enumName,
            ImmutableArray<IFieldSymbol> members,
            GenerationConfig config)
        {
            sb.AppendLine($"        public static bool TryParse(string value, out {enumName} result)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (string.IsNullOrEmpty(value))");
            sb.AppendLine("            {");
            sb.AppendLine("                result = default;");
            sb.AppendLine("                return false;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            switch (value)");
            sb.AppendLine("            {");

            foreach (var member in members)
            {
                string matchValue;
                if (config.ToCase != 0) // 0 = NamingCase.None
                {
                    var converted = NamingCaseConverter.Convert(member.Name, config.FromCase, config.ToCase);
                    matchValue = $"\"{converted}\"";
                }
                else
                {
                    matchValue = $"nameof({enumName}.{member.Name})";
                }
                sb.AppendLine($"                case {matchValue}:");
                sb.AppendLine($"                    result = {enumName}.{member.Name};");
                sb.AppendLine("                    return true;");
            }

            sb.AppendLine("                default:");
            sb.AppendLine("                    result = default;");
            sb.AppendLine("                    return false;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
        }

        private static void GenerateCachedFields(
            StringBuilder sb,
            string enumName,
            ImmutableArray<IFieldSymbol> members,
            GenerationConfig config)
        {
            var values = string.Join(", ", members.Select(m => $"{enumName}.{m.Name}"));
            sb.AppendLine($"        private static readonly {enumName}[] _values = new[] {{ {values} }};");

            string names;
            if (config.ToCase != NamingCase.None)
            {
                var convertedNames = members.Select(m =>
                {
                    var converted = NamingCaseConverter.Convert(m.Name, config.FromCase, config.ToCase);
                    return $"\"{converted}\"";
                });
                names = string.Join(", ", convertedNames);
            }
            else
            {
                names = string.Join(", ", members.Select(m => $"nameof({enumName}.{m.Name})"));
            }
            sb.AppendLine($"        private static readonly string[] _names = new[] {{ {names} }};");
        }

        private static void GenerateGetValuesMethod(
            StringBuilder sb,
            string enumName)
        {
            sb.AppendLine($"        public static System.ReadOnlySpan<{enumName}> GetValues()");
            sb.AppendLine("        {");
            sb.AppendLine("            return _values;");
            sb.AppendLine("        }");
        }

        private static void GenerateGetNamesMethod(StringBuilder sb)
        {
            sb.AppendLine("        public static System.ReadOnlySpan<string> GetNames()");
            sb.AppendLine("        {");
            sb.AppendLine("            return _names;");
            sb.AppendLine("        }");
        }

        private static string GenerateStandaloneExtensions(GenerationConfig config)
        {
            var namespaceName = config.EnumSymbol.ContainingNamespace.IsGlobalNamespace
                ? null
                : config.EnumSymbol.ContainingNamespace.ToDisplayString();

            var className = config.ClassName ?? $"{config.EnumSymbol.Name}Converter";
            return GenerateExtensionsCore(namespaceName, className, config);
        }

        private static string GenerateExtensionsCore(string? namespaceName, string className, GenerationConfig config)
        {
            var enumSymbol = config.EnumSymbol;
            var enumName = enumSymbol.ToDisplayString();

            var members = enumSymbol.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(f => f.IsConst)
                .ToImmutableArray();

            var sb = new StringBuilder();

            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine("#nullable enable");
            sb.AppendLine();

            if (namespaceName is not null)
            {
                sb.AppendLine($"namespace {namespaceName}");
                sb.AppendLine("{");
            }

            sb.AppendLine($"    public static partial class {className}");
            sb.AppendLine("    {");

            GenerateCachedFields(sb, enumName, members, config);
            sb.AppendLine();
            GenerateGetNameMethod(sb, enumName, members, config);
            sb.AppendLine();
            GenerateIsDefinedMethod(sb, enumName, members);
            sb.AppendLine();
            GenerateParseMethod(sb, enumName, members, config);
            sb.AppendLine();
            GenerateTryParseMethod(sb, enumName, members, config);
            sb.AppendLine();
            GenerateGetValuesMethod(sb, enumName);
            sb.AppendLine();
            GenerateGetNamesMethod(sb);

            sb.AppendLine("    }");

            if (namespaceName is not null)
            {
                sb.AppendLine("}");
            }

            return sb.ToString();
        }
    }
}
