# EnumStringConverter

[![NuGet](https://img.shields.io/nuget/v/EnumStringConverter.svg)](https://www.nuget.org/packages/EnumStringConverter/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A C# source generator that provides fast enum-to-string conversion and enumeration with support for naming convention transformations.

[日本語版](README_JP.md)

## Features

- **High Performance**: Uses static dispatch with switch expressions instead of reflection
- **Naming Convention Support**: Convert between 14 different naming conventions (PascalCase, camelCase, snake_case, etc.)
- **Zero Allocation**: `GetValues()` and `GetNames()` return `ReadOnlySpan<T>`
- **Flexible Usage**: Apply attributes directly to enums or use partial classes

## Installation

```bash
dotnet add package EnumStringConverter
```

## Quick Start

### Apply Attribute Directly to Enum

```csharp
using EnumStringConverter;

[GenerateEnumStringConverter]
public enum MyEnum
{
    None,
    First,
    Second
}

// Usage
var myEnum = MyEnum.First;
string name = MyEnumConverter.GetName(myEnum); // "First"

var parsed = MyEnumConverter.Parse("Second"); // MyEnum.Second

if (MyEnumConverter.TryParse("Third", out MyEnum result))
{
    Console.WriteLine(result);
}

// Enumerate values
foreach (var value in MyEnumConverter.GetValues())
{
    Console.WriteLine(value);
}
```

### Using Partial Class

```csharp
public enum MyEnum
{
    None,
    First,
    Second
}

[GenerateEnumStringConverter(typeof(MyEnum))]
public static partial class MyEnumConverter
{
}

// Same usage
string name = MyEnumConverter.GetName(MyEnum.First);
```

## Naming Convention Conversion

Use `From` and `To` properties to convert between different naming conventions.

### Convert to snake_case

```csharp
[GenerateEnumStringConverter(To = NamingCase.SnakeCase)]
public enum MyEnum
{
    FirstValue,   // "first_value"
    SecondValue,  // "second_value"
    ThirdValue    // "third_value"
}

var value = MyEnum.FirstValue;
string name = MyEnumConverter.GetName(value); // "first_value"
var parsed = MyEnumConverter.Parse("second_value"); // MyEnum.SecondValue
```

### Convert to kebab-case

```csharp
[GenerateEnumStringConverter(To = NamingCase.KebabCase)]
public enum StatusCode
{
    NotFound,           // "not-found"
    InternalServerError // "internal-server-error"
}
```

### Convert to camelCase

```csharp
[GenerateEnumStringConverter(To = NamingCase.CamelCase)]
public enum MyEnum
{
    FirstValue,   // "firstValue"
    SecondValue   // "secondValue"
}
```

## Supported Naming Conventions

| Convention | Example |
|-----------|---------|
| `PascalCase` | MyVariable |
| `CamelCase` | myVariable |
| `SnakeCase` | my_variable |
| `UpperSnakeCase` | MY_VARIABLE |
| `PascalSnakeCase` | My_Variable |
| `KebabCase` | my-variable |
| `CobolCase` | MY-VARIABLE |
| `TrainCase` | My-Variable |
| `DotCase` | my.variable |
| `PathCase` | my/variable |
| `LowerCase` | my variable |
| `UpperCase` | MY VARIABLE |
| `TitleCase` | My Variable |
| `SentenceCase` | My variable |

## Custom From Parameter

By default, enum names are treated as `PascalCase`. To convert from a different convention, specify the `From` parameter.

```csharp
// Convert from snake_case to kebab-case
[GenerateEnumStringConverter(From = NamingCase.SnakeCase, To = NamingCase.KebabCase)]
public enum MyEnum
{
    first_value,   // "first-value"
    second_value   // "second-value"
}
```

## Generated Methods

The following methods are generated for each enum:

- `GetName(T value)`: Convert enum value to string
- `Parse(string value)`: Convert string to enum value (throws on failure)
- `TryParse(string value, out T result)`: Safely convert string to enum value
- `IsDefined(T value)`: Check if enum value is defined
- `GetValues()`: Get all enum values as `ReadOnlySpan<T>`
- `GetNames()`: Get all enum names as `ReadOnlySpan<string>`

## Performance

- **GetName()**: Fast static dispatch using switch expressions
- **Parse()/TryParse()**: Fast static dispatch using switch statements
- **GetValues()/GetNames()**: Zero allocation by returning spans directly
- **Naming Convention Conversion**: Performed at compile time with zero runtime overhead

## License

MIT License

## Repository

https://github.com/gameshalico/EnumStringConverter
