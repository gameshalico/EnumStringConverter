using System;
using Xunit;

namespace EnumStringConverter.Tests;

[GenerateEnumStringConverter(To = NamingCase.SnakeCase)]
public enum SnakeCaseEnum
{
    FirstValue,
    SecondValue,
    ThirdValue
}

[GenerateEnumStringConverter(To = NamingCase.KebabCase)]
public enum KebabCaseEnum
{
    FirstValue,
    SecondValue,
    ThirdValue
}

[GenerateEnumStringConverter(To = NamingCase.CamelCase)]
public enum CamelCaseEnum
{
    FirstValue,
    SecondValue,
    ThirdValue
}

[GenerateEnumStringConverter(To = NamingCase.SnakeCase)]
public enum AcronymEnum
{
    JSONParser,
    XMLHttpRequest,
    HTTPSConnection,
    IOError,
    HTMLElement
}

// Additional test enum for edge cases in acronym handling
[GenerateEnumStringConverter(To = NamingCase.SnakeCase)]
public enum ComplexAcronymEnum
{
    // Single acronym
    API,
    // Acronym + Word
    APIKey,
    // Word + Acronym
    MyAPI,
    // Multiple acronyms
    HTTPAPI,
    // Acronym + Word + Acronym
    APIKeyURL,
    // Complex case
    HTMLToXML,
    // Two letter acronym
    ID,
    IOStream
}

public class CaseConversionTests
{
    [Theory]
    [InlineData(SnakeCaseEnum.FirstValue, "first_value")]
    [InlineData(SnakeCaseEnum.SecondValue, "second_value")]
    [InlineData(SnakeCaseEnum.ThirdValue, "third_value")]
    public void GetName_SnakeCase_ReturnsSnakeCaseName(SnakeCaseEnum value, string expected)
    {
        Assert.Equal(expected, SnakeCaseEnumSnakeCaseConverter.GetName(value));
    }

    [Theory]
    [InlineData("first_value", SnakeCaseEnum.FirstValue)]
    [InlineData("second_value", SnakeCaseEnum.SecondValue)]
    [InlineData("third_value", SnakeCaseEnum.ThirdValue)]
    public void Parse_SnakeCase_ParsesCorrectly(string input, SnakeCaseEnum expected)
    {
        Assert.Equal(expected, SnakeCaseEnumSnakeCaseConverter.Parse(input));
    }

    [Fact]
    public void GetNames_SnakeCase_ReturnsAllSnakeCaseNames()
    {
        var names = SnakeCaseEnumSnakeCaseConverter.GetNames();
        Assert.Equal(3, names.Length);
        Assert.Contains("first_value", names.ToArray());
        Assert.Contains("second_value", names.ToArray());
        Assert.Contains("third_value", names.ToArray());
    }

    [Theory]
    [InlineData(KebabCaseEnum.FirstValue, "first-value")]
    [InlineData(KebabCaseEnum.SecondValue, "second-value")]
    [InlineData(KebabCaseEnum.ThirdValue, "third-value")]
    public void GetName_KebabCase_ReturnsKebabCaseName(KebabCaseEnum value, string expected)
    {
        Assert.Equal(expected, KebabCaseEnumKebabCaseConverter.GetName(value));
    }

    [Theory]
    [InlineData("first-value", KebabCaseEnum.FirstValue)]
    [InlineData("second-value", KebabCaseEnum.SecondValue)]
    [InlineData("third-value", KebabCaseEnum.ThirdValue)]
    public void Parse_KebabCase_ParsesCorrectly(string input, KebabCaseEnum expected)
    {
        Assert.Equal(expected, KebabCaseEnumKebabCaseConverter.Parse(input));
    }

    [Theory]
    [InlineData(CamelCaseEnum.FirstValue, "firstValue")]
    [InlineData(CamelCaseEnum.SecondValue, "secondValue")]
    [InlineData(CamelCaseEnum.ThirdValue, "thirdValue")]
    public void GetName_CamelCase_ReturnsCamelCaseName(CamelCaseEnum value, string expected)
    {
        Assert.Equal(expected, CamelCaseEnumCamelCaseConverter.GetName(value));
    }

    [Theory]
    [InlineData("firstValue", CamelCaseEnum.FirstValue)]
    [InlineData("secondValue", CamelCaseEnum.SecondValue)]
    [InlineData("thirdValue", CamelCaseEnum.ThirdValue)]
    public void Parse_CamelCase_ParsesCorrectly(string input, CamelCaseEnum expected)
    {
        Assert.Equal(expected, CamelCaseEnumCamelCaseConverter.Parse(input));
    }

    [Fact]
    public void TryParse_SnakeCase_ValidValue_ReturnsTrue()
    {
        var result = SnakeCaseEnumSnakeCaseConverter.TryParse("first_value", out var value);
        Assert.True(result);
        Assert.Equal(SnakeCaseEnum.FirstValue, value);
    }

    [Fact]
    public void TryParse_SnakeCase_InvalidValue_ReturnsFalse()
    {
        var result = SnakeCaseEnumSnakeCaseConverter.TryParse("invalid_value", out var value);
        Assert.False(result);
        Assert.Equal(default(SnakeCaseEnum), value);
    }

    [Theory]
    [InlineData(AcronymEnum.JSONParser, "json_parser")]
    [InlineData(AcronymEnum.XMLHttpRequest, "xml_http_request")]
    [InlineData(AcronymEnum.HTTPSConnection, "https_connection")]
    [InlineData(AcronymEnum.IOError, "io_error")]
    [InlineData(AcronymEnum.HTMLElement, "html_element")]
    public void GetName_Acronym_ReturnsCorrectSnakeCase(AcronymEnum value, string expected)
    {
        Assert.Equal(expected, AcronymEnumSnakeCaseConverter.GetName(value));
    }

    [Theory]
    [InlineData("json_parser", AcronymEnum.JSONParser)]
    [InlineData("xml_http_request", AcronymEnum.XMLHttpRequest)]
    [InlineData("https_connection", AcronymEnum.HTTPSConnection)]
    [InlineData("io_error", AcronymEnum.IOError)]
    [InlineData("html_element", AcronymEnum.HTMLElement)]
    public void Parse_Acronym_ParsesCorrectly(string input, AcronymEnum expected)
    {
        Assert.Equal(expected, AcronymEnumSnakeCaseConverter.Parse(input));
    }

    [Fact]
    public void GetNames_Acronym_ReturnsAllConvertedNames()
    {
        var names = AcronymEnumSnakeCaseConverter.GetNames();
        Assert.Equal(5, names.Length);
        Assert.Contains("json_parser", names.ToArray());
        Assert.Contains("xml_http_request", names.ToArray());
        Assert.Contains("https_connection", names.ToArray());
        Assert.Contains("io_error", names.ToArray());
        Assert.Contains("html_element", names.ToArray());
    }

    // Additional test to verify the exact behavior with complex acronyms
    [Fact]
    public void GetName_ComplexAcronym_ShouldSplitCorrectly()
    {
        // These tests verify the expected behavior for acronym splitting
        // Current implementation may fail some of these - they serve as regression tests
        var xmlResult = AcronymEnumSnakeCaseConverter.GetName(AcronymEnum.XMLHttpRequest);
        // Expected: "xml_http_request" (XML|Http|Request)
        // This test will verify if the implementation correctly handles acronyms followed by PascalCase
        Assert.Equal("xml_http_request", xmlResult);
    }

    [Fact]
    public void GetName_HTTPSConnection_ShouldSplitCorrectly()
    {
        var result = AcronymEnumSnakeCaseConverter.GetName(AcronymEnum.HTTPSConnection);
        // Expected: "https_connection" (HTTPS|Connection)
        Assert.Equal("https_connection", result);
    }

    [Fact]
    public void GetName_IOError_ShouldSplitCorrectly()
    {
        var result = AcronymEnumSnakeCaseConverter.GetName(AcronymEnum.IOError);
        // Expected: "io_error" (IO|Error)
        Assert.Equal("io_error", result);
    }

    // Complex acronym edge case tests
    [Theory]
    [InlineData(ComplexAcronymEnum.API, "api")]
    [InlineData(ComplexAcronymEnum.APIKey, "api_key")]
    [InlineData(ComplexAcronymEnum.MyAPI, "my_api")]
    [InlineData(ComplexAcronymEnum.HTTPAPI, "httpapi")] // or "http_api"?
    [InlineData(ComplexAcronymEnum.APIKeyURL, "api_key_url")]
    [InlineData(ComplexAcronymEnum.HTMLToXML, "html_to_xml")]
    [InlineData(ComplexAcronymEnum.ID, "id")]
    [InlineData(ComplexAcronymEnum.IOStream, "io_stream")]
    public void GetName_ComplexAcronymEdgeCases_CorrectConversion(ComplexAcronymEnum value, string expected)
    {
        // This test documents the expected behavior for various acronym edge cases
        Assert.Equal(expected, ComplexAcronymEnumSnakeCaseConverter.GetName(value));
    }
}

// Tests for null and empty string handling
public class ParseEdgeCaseTests
{
    [Fact]
    public void Parse_NullString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SnakeCaseEnumSnakeCaseConverter.Parse(null!));
    }

    [Fact]
    public void Parse_EmptyString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SnakeCaseEnumSnakeCaseConverter.Parse(string.Empty));
    }

    [Fact]
    public void TryParse_NullString_ReturnsFalse()
    {
        var result = SnakeCaseEnumSnakeCaseConverter.TryParse(null!, out var value);
        Assert.False(result);
        Assert.Equal(default(SnakeCaseEnum), value);
    }

    [Fact]
    public void TryParse_EmptyString_ReturnsFalse()
    {
        var result = SnakeCaseEnumSnakeCaseConverter.TryParse(string.Empty, out var value);
        Assert.False(result);
        Assert.Equal(default(SnakeCaseEnum), value);
    }

    [Fact]
    public void TryParse_WhitespaceString_ReturnsFalse()
    {
        var result = SnakeCaseEnumSnakeCaseConverter.TryParse("   ", out var value);
        Assert.False(result);
        Assert.Equal(default(SnakeCaseEnum), value);
    }
}
