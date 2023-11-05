using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace OneOf.Serialization.SystemTextJson.Tests;

public class DeserializeJsonThatIsNotAnObjectTests
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
            yield return new object[] { 123, typeof(OneOf<A>) };
            yield return new object[] { 123, typeof(OneOf<A, B>) };
            yield return new object[] { 123, typeof(OneOf<A, B, C>) };
            yield return new object[] { 123, typeof(OneOf<A, B, C, D>) };
            yield return new object[] { 123, typeof(OneOf<A, B, C, D, E>) };
            yield return new object[] { 123, typeof(OneOf<A, B, C, D, E, F>) };
            yield return new object[] { 123, typeof(OneOf<A, B, C, D, E, F, G>) };
            yield return new object[] { 123, typeof(OneOf<A, B, C, D, E, F, G, H>) };
            yield return new object[] { 123, typeof(OneOf<A, B, C, D, E, F, G, H, I>) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}