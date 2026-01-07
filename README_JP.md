# EnumStringConverter

[![NuGet](https://img.shields.io/nuget/v/EnumStringConverter.svg)](https://www.nuget.org/packages/EnumStringConverter/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Enum型の高速な文字列変換と列挙を提供するSource Generatorです。

[English version](README.md)

## 特徴

- **高速な変換**: リフレクションを使わず、switch式による静的ディスパッチで高速に動作
- **命名規則の変換**: PascalCase、camelCase、snake_case など14種類の命名規則に対応
- **アロケーションフリー**: `GetValues()`/`GetNames()`は`ReadOnlySpan<T>`を返却
- **柔軟な使用方法**: Enumに直接属性を付けるか、部分クラスで定義

## インストール

```bash
dotnet add package EnumStringConverter
```

## 基本的な使い方

### 1. Enumに直接属性を付ける

```csharp
using EnumStringConverter;

[GenerateEnumStringConverter]
public enum MyEnum
{
    None,
    First,
    Second
}

// 使用例
var myEnum = MyEnum.First;
string name = MyEnumConverter.GetName(myEnum); // "First"

var parsed = MyEnumConverter.Parse("Second"); // MyEnum.Second

if (MyEnumConverter.TryParse("Third", out MyEnum result))
{
    Console.WriteLine(result);
}

// 値の列挙
foreach (var value in MyEnumConverter.GetValues())
{
    Console.WriteLine(value);
}
```

### 2. 部分クラスに属性を付ける

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

// 使い方は同じ
string name = MyEnumConverter.GetName(MyEnum.First);
```

## 命名規則の変換

`From`と`To`プロパティを使用して、Enum名の命名規則を変換できます。

### snake_caseに変換

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

### kebab-caseに変換

```csharp
[GenerateEnumStringConverter(To = NamingCase.KebabCase)]
public enum StatusCode
{
    NotFound,           // "not-found"
    InternalServerError // "internal-server-error"
}
```

### camelCaseに変換

```csharp
[GenerateEnumStringConverter(To = NamingCase.CamelCase)]
public enum MyEnum
{
    FirstValue,   // "firstValue"
    SecondValue   // "secondValue"
}
```

## サポートされている命名規則

| 命名規則 | 例 |
|---------|-----|
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

## Fromパラメータのカスタマイズ

デフォルトでは、Enum名は`PascalCase`として扱われます。別の命名規則から変換する場合は`From`を指定します。

```csharp
// snake_caseからkebab-caseへ変換
[GenerateEnumStringConverter(From = NamingCase.SnakeCase, To = NamingCase.KebabCase)]
public enum MyEnum
{
    first_value,   // "first-value"
    second_value   // "second-value"
}
```

## 生成されるメソッド

各Enumに対して以下のメソッドが生成されます：

- `GetName(T value)`: Enum値を文字列に変換
- `Parse(string value)`: 文字列をEnum値に変換（失敗時は例外）
- `TryParse(string value, out T result)`: 文字列をEnum値に安全に変換
- `IsDefined(T value)`: Enum値が定義されているかチェック
- `GetValues()`: すべてのEnum値を`ReadOnlySpan<T>`として取得
- `GetNames()`: すべてのEnum名を`ReadOnlySpan<string>`として取得

## パフォーマンス

- **GetName()**: switch式による静的ディスパッチで高速
- **Parse()/TryParse()**: switch文による静的ディスパッチで高速
- **GetValues()/GetNames()**: 配列の直接返却でアロケーションなし
- **命名規則の変換**: コンパイル時に実行されるため、実行時のオーバーヘッドなし

## ライセンス

MIT License

## リポジトリ

https://github.com/gameshalico/EnumStringConverter
