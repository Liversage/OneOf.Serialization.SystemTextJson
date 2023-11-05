using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace OneOf.Serialization.SystemTextJson.Tests;

public class SerializeAndDeserializeAmbiguousTypeNameTests
{
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void Test(OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName> oneOf, string expectedJson)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new OneOfJsonConverter((typeof(X.AmbiguousTypeName), "X"), (typeof(Y.AmbiguousTypeName), "Y")),
            },
        };
        var json = JsonSerializer.Serialize(oneOf, serializerOptions);
        var deserializedOneOf = JsonSerializer.Deserialize<OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName>>(json, serializerOptions);
        Assert.Equal(expectedJson, json);
        Assert.Equal(oneOf, deserializedOneOf);
    }

    class TestDataGenerator : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { (OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName>) new X.AmbiguousTypeName("X"), """{"X":{"Value":"X"}}""" };
            yield return new object[] { (OneOf<X.AmbiguousTypeName, Y.AmbiguousTypeName>) new Y.AmbiguousTypeName("Y"), """{"Y":{"Value":"Y"}}""" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}