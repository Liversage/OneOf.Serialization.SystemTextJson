# OneOf.Serialization.SystemTextJson [![NuGet](https://img.shields.io/nuget/v/OneOf.Serialization.SystemTextJson?logo=nuget)](https://www.nuget.org/packages/OneOf.Serialization.SystemTextJson/) [![GitHub](https://img.shields.io/github/license/Liversage/OneOf.Serialization.SystemTextJson)](LICENSE)

As of this time of writing [`OneOf`](https://github.com/mcintyre321/OneOf) has limited support for JSON serialization. There exists a converter for Json.NET but this converter only supports writing. This library provides support for `System.Text.Json` both reading and writing.

## Usage

Add a reference to `OneOf.Serialization.SystemTextJson` in your `.csproj`:

```xml
<ItemGroup>
    <PackageReference Include="OneOf.Serialization.SystemTextJson" Version="1.1.1"/>
</ItemGroup>
```

This provides access to two `JsonConverter` classes. Create `JsonSerializerOptions` with both converters:

```csharp
var serializerOptions = new JsonSerializerOptions
{
    Converters =
    {
        new OneOfJsonConverter(),
        new OneOfBaseJsonConverter(),
    },
};
```

(You can omit `OneOfBaseJsonConverter` if you are not serializing types derived from `OneOfBase` but not the other way around.)

Then serialize and deserialize using the `serializerOptions`:

```csharp
var json = JsonSerializer.Serialize(oneOfToSerialize, serializerOptions);
var deserializeOneOf = JsonSerializer.Deserialize<OneOf<Foo, Bar, Baz>>(json, serializerOptions);
```

## JSON format

In the case of an `OneOf` with types `Foo`, `Bar` and `Baz` that all serialize to JSON objects the generated JSON will be one of the following:

When `Foo` is the value:

```jsonc
{
    "Foo": {
        // ...
    }
}
```

When `Bar` is the value:

```jsonc
{
    "Bar": {
        // ...
    }
}
```

And when `Baz` is the value:

```jsonc
{
    "Baz": {
        // ...
    }
}
```

## Analysis and design of JSON format

Being able to support reading of a discriminated type requires some consideration. The JSON should somehow be tagged in a way that makes it possible to determine which union value the `OneOf` contains. Json.NET provides a mechanism where a `$type` property at the beginning of a JSON object is used to determine its .NET type. This can be useful when serializing type hierarchies, and a union type like `OneOf` could be built using an abstract base type and derived classes for each union type. However, the `$type` tag has several drawbacks:

1. It's non-standard so not easily consumable by applications not written using .NET.
2. By default, the full .NET type name including the assembly name is used which not only is unwieldy (especially for generic types) but can also in some cases pose a security risk.
3. The actual type to deserialize is only known after parsing of the JSON object has begun and this makes it more tricky to deserialize the object.

A reasonable assumption is that each type in the `OneOf` is uniquely named without having to use namespace names as discriminators. In that case the type names can be used as tags and this fixes point 1. and 2. above. However, point 3. is still an issue. To avoid this the serialization format used by this library is as follows:

> Each possible value of an `OneOf` is represented as a JSON property having the name of the type of the value. The nature of the `OneOf` ensures that exactly one property is present in the serialized JSON object. 
 
See previous section for the resulting JSON.

Drawbacks:

- Using `OneOf<Foo, None>` as an option type results in JSON that's not as succinct as the `OneOf<Foo, None>` either being present as a serialized `Foo` or being absent. It's possible to create a specialized `JsonConverter` for this case if that's desired. 
- Using primitive types like `string`, `int` and `DateTime` as union types results in JSON with non-descriptive property names.
- Using arrays or generic types as union types results in exotic property names.

## Customizing property names

When property names like `Int32`, `String[]` or `Dictionary\u00602` are unsatisfactory it's possible to customize them. As mentioned the default is to use the name of the type but it's possible to provide a mapping from types to names in the constructor of either serializer:

```csharp
var jsonConverter = new OneOfJsonConverter((typeof(string[]), "Results"), (typeof(int), "ErrorCode"));
```

An `OneOf<string[], int>` will serialize to something like

```json
{
    "Results": [
        "Foo",
        "Bar"
    ]
}
```

or

```json
{
    "ErrorCode": 123
}
```

Notice that setting `PropertyNamingPolicy` of `JsonSerializerOptions` to `JsonNamingPolicy.CamelCase` will affect the serialized property names even when they have been customized. 

## Supported `OneOf` types

The serializers only support `OneOf` and `OneOfBase` with arity 1‒9. However, the converters in this library are code generated and it's possible to reuse this code generator to create `JsonConverter` classes with the desired arity.

## Ignoring JSON values

`OneOf<string?, int>` can have the value `null` when it's a `string` and `0` when it's an `int`. It's possible to configure the JSON serializer to ignore these values when serializing by specifying `JsonIgnoreCondition.WhenWritingNull` or `JsonIgnoreCondition.WhenWritingDefault` as the value for `JsonSerializerOptions.DefaultIgnoreCondition`.

Arguably, one of these `DefaultIgnoreCondition` values and an `OneOf` value of either `null` or `0` should result in the `OneOf` being serialized as an empty JSON object. However, that makes it impossible to determine which specific union type the `OneOf` contains from the JSON so `DefaultIgnoreCondition` is ignored by this serializer.

When `OneOf<string?, int>` has the value `null` the serializer creates the following JSON even when the value of `DefaultIgnoreCondition` is either `WhenWritingNull` or `WhenWritingDefault`:

```json
{
    "String": null
}
```

Similarly, when `OneOf<string?, int>` has the value `0` the serializer creates the following JSON even when the value of `DefaultIgnoreCondition` is `WhenWritingDefault`:

```
{
    "Int32": 0
}
```