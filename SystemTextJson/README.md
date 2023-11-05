# JSON serialization

This library provides support for JSON serialization and deserialization of [`OneOf`](https://github.com/mcintyre321/OneOf/) types using `System.Text.Json`.

# Usage

Add a reference to `OneOf.Serialization.SystemTextJson` in your `.csproj`:

```xml
<ItemGroup>
    <PackageReference Include="OneOf.Serialization.SystemTextJson" Version="1.1.1"/>
</ItemGroup>
```

This provides access to two `JsonConverter` classes. Create a `JsonSerializerOptions` with both converters:

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

# JSON format

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

See project repository for more information.