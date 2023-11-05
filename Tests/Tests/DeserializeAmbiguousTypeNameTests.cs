using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace OneOf.Serialization.SystemTextJson.Tests;

public class DeserializeAmbiguousTypeNameTests
{
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void Test(string json, Type oneOfType)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            Converters =
            {
                Converters.OneOfJsonConverter,
            },
        };
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize(json, oneOfType, serializerOptions));
    }

    class TestDataGenerator : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { """{"AmbiguousTypeName":{"Value":"?"}"}""", typeof(OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName>) };
            yield return new object[] { """{"AmbiguousTypeName":{"Value":"?"}"}""", typeof(OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName, C>) };
            yield return new object[] { """{"AmbiguousTypeName":{"Value":"?"}"}""", typeof(OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName, C, D>) };
            yield return new object[] { """{"AmbiguousTypeName":{"Value":"?"}"}""", typeof(OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName, C, D, E>) };
            yield return new object[] { """{"AmbiguousTypeName":{"Value":"?"}"}""", typeof(OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName, C, D, E, F>) };
            yield return new object[] { """{"AmbiguousTypeName":{"Value":"?"}"}""", typeof(OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName, C, D, E, F, G>) };
            yield return new object[] { """{"AmbiguousTypeName":{"Value":"?"}"}""", typeof(OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName, C, D, E, F, G, H>) };
            yield return new object[] { """{"AmbiguousTypeName":{"Value":"?"}"}""", typeof(OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName, C, D, E, F, G, H, I>) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}