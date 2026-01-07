namespace EnumStringConverter.Tests;

[GenerateEnumStringConverter]
public enum MyEnum
{
    None,
    First,
    Second,
    Third
}

[GenerateEnumStringConverter]
public enum StatusCode
{
    OK = 200,
    NotFound = 404,
    InternalServerError = 500
}
