using System;
using Xunit;

namespace EnumStringConverter.Tests;

public sealed class StatusCodeConverterTests
{
    [Fact]
    public void GetName_ValidValue_ReturnsCorrectName()
    {
        Assert.Equal("OK", StatusCodeConverter.GetName(StatusCode.OK));
        Assert.Equal("NotFound", StatusCodeConverter.GetName(StatusCode.NotFound));
        Assert.Equal("InternalServerError", StatusCodeConverter.GetName(StatusCode.InternalServerError));
    }

    [Fact]
    public void GetName_InvalidValue_ReturnsToStringResult()
    {
        var invalidValue = (StatusCode)999;
        Assert.Equal("999", StatusCodeConverter.GetName(invalidValue));
    }

    [Fact]
    public void IsDefined_ValidValue_ReturnsTrue()
    {
        Assert.True(StatusCodeConverter.IsDefined(StatusCode.OK));
        Assert.True(StatusCodeConverter.IsDefined(StatusCode.NotFound));
        Assert.True(StatusCodeConverter.IsDefined(StatusCode.InternalServerError));
    }

    [Fact]
    public void IsDefined_InvalidValue_ReturnsFalse()
    {
        var invalidValue = (StatusCode)999;
        Assert.False(StatusCodeConverter.IsDefined(invalidValue));
    }

    [Fact]
    public void Parse_ValidString_ReturnsCorrectEnum()
    {
        Assert.Equal(StatusCode.OK, StatusCodeConverter.Parse("OK"));
        Assert.Equal(StatusCode.NotFound, StatusCodeConverter.Parse("NotFound"));
        Assert.Equal(StatusCode.InternalServerError, StatusCodeConverter.Parse("InternalServerError"));
    }

    [Fact]
    public void Parse_InvalidString_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => StatusCodeConverter.Parse("BadRequest"));
        Assert.Contains("Invalid value 'BadRequest'", exception.Message);
        Assert.Contains("StatusCode", exception.Message);
    }

    [Fact]
    public void TryParse_ValidString_ReturnsTrueAndCorrectEnum()
    {
        Assert.True(StatusCodeConverter.TryParse("OK", out StatusCode ok));
        Assert.Equal(StatusCode.OK, ok);

        Assert.True(StatusCodeConverter.TryParse("NotFound", out StatusCode notFound));
        Assert.Equal(StatusCode.NotFound, notFound);

        Assert.True(StatusCodeConverter.TryParse("InternalServerError", out StatusCode error));
        Assert.Equal(StatusCode.InternalServerError, error);
    }

    [Fact]
    public void TryParse_InvalidString_ReturnsFalseAndDefaultValue()
    {
        Assert.False(StatusCodeConverter.TryParse("BadRequest", out StatusCode result));
        Assert.Equal(default, result);
    }

    [Fact]
    public void GetValues_ReturnsAllEnumValues()
    {
        var values = StatusCodeConverter.GetValues();

        Assert.Equal(3, values.Length);
        Assert.Contains(StatusCode.OK, values.ToArray());
        Assert.Contains(StatusCode.NotFound, values.ToArray());
        Assert.Contains(StatusCode.InternalServerError, values.ToArray());
    }

    [Fact]
    public void GetNames_ReturnsAllEnumNames()
    {
        var names = StatusCodeConverter.GetNames();

        Assert.Equal(3, names.Length);
        Assert.Contains("OK", names.ToArray());
        Assert.Contains("NotFound", names.ToArray());
        Assert.Contains("InternalServerError", names.ToArray());
    }

    [Fact]
    public void EnumValues_HaveCorrectUnderlyingValues()
    {
        Assert.Equal(200, (int)StatusCode.OK);
        Assert.Equal(404, (int)StatusCode.NotFound);
        Assert.Equal(500, (int)StatusCode.InternalServerError);
    }
}
