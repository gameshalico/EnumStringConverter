using System;
using Xunit;

namespace EnumStringConverter.Tests;

public sealed class MyEnumConverterTests
{
    [Fact]
    public void GetName_ValidValue_ReturnsCorrectName()
    {
        Assert.Equal("None", MyEnumConverter.GetName(MyEnum.None));
        Assert.Equal("First", MyEnumConverter.GetName(MyEnum.First));
        Assert.Equal("Second", MyEnumConverter.GetName(MyEnum.Second));
        Assert.Equal("Third", MyEnumConverter.GetName(MyEnum.Third));
    }

    [Fact]
    public void GetName_InvalidValue_ReturnsToStringResult()
    {
        var invalidValue = (MyEnum)999;
        Assert.Equal("999", MyEnumConverter.GetName(invalidValue));
    }

    [Fact]
    public void IsDefined_ValidValue_ReturnsTrue()
    {
        Assert.True(MyEnumConverter.IsDefined(MyEnum.None));
        Assert.True(MyEnumConverter.IsDefined(MyEnum.First));
        Assert.True(MyEnumConverter.IsDefined(MyEnum.Second));
        Assert.True(MyEnumConverter.IsDefined(MyEnum.Third));
    }

    [Fact]
    public void IsDefined_InvalidValue_ReturnsFalse()
    {
        var invalidValue = (MyEnum)999;
        Assert.False(MyEnumConverter.IsDefined(invalidValue));
    }

    [Fact]
    public void Parse_ValidString_ReturnsCorrectEnum()
    {
        Assert.Equal(MyEnum.None, MyEnumConverter.Parse("None"));
        Assert.Equal(MyEnum.First, MyEnumConverter.Parse("First"));
        Assert.Equal(MyEnum.Second, MyEnumConverter.Parse("Second"));
        Assert.Equal(MyEnum.Third, MyEnumConverter.Parse("Third"));
    }

    [Fact]
    public void Parse_InvalidString_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => MyEnumConverter.Parse("Invalid"));
        Assert.Contains("Invalid value 'Invalid'", exception.Message);
        Assert.Contains("MyEnum", exception.Message);
    }

    [Fact]
    public void TryParse_ValidString_ReturnsTrueAndCorrectEnum()
    {
        Assert.True(MyEnumConverter.TryParse("None", out MyEnum none));
        Assert.Equal(MyEnum.None, none);

        Assert.True(MyEnumConverter.TryParse("First", out MyEnum first));
        Assert.Equal(MyEnum.First, first);

        Assert.True(MyEnumConverter.TryParse("Second", out MyEnum second));
        Assert.Equal(MyEnum.Second, second);

        Assert.True(MyEnumConverter.TryParse("Third", out MyEnum third));
        Assert.Equal(MyEnum.Third, third);
    }

    [Fact]
    public void TryParse_InvalidString_ReturnsFalseAndDefaultValue()
    {
        Assert.False(MyEnumConverter.TryParse("Invalid", out MyEnum result));
        Assert.Equal(default, result);
    }

    [Fact]
    public void GetValues_ReturnsAllEnumValues()
    {
        var values = MyEnumConverter.GetValues();

        Assert.Equal(4, values.Length);
        Assert.Contains(MyEnum.None, values.ToArray());
        Assert.Contains(MyEnum.First, values.ToArray());
        Assert.Contains(MyEnum.Second, values.ToArray());
        Assert.Contains(MyEnum.Third, values.ToArray());
    }

    [Fact]
    public void GetNames_ReturnsAllEnumNames()
    {
        var names = MyEnumConverter.GetNames();

        Assert.Equal(4, names.Length);
        Assert.Contains("None", names.ToArray());
        Assert.Contains("First", names.ToArray());
        Assert.Contains("Second", names.ToArray());
        Assert.Contains("Third", names.ToArray());
    }
}
