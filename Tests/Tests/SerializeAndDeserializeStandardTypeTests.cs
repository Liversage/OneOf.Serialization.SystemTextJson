using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace OneOf.Serialization.SystemTextJson.Tests;

public class SerializeAndDeserializeStandardTypeTests
{
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void Test(OneOf<int, string[], List<int>, TimeSpan?> oneOf, string expectedJson)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            Converters =
            {
                Converters.OneOfJsonConverter,
            },
        };
        var json = JsonSerializer.Serialize(oneOf, serializerOptions);
        var deserializedOneOf = JsonSerializer.Deserialize<OneOf<int, string[], List<int>, TimeSpan?>>(json, serializerOptions);
        Assert.Equal(expectedJson, json);
        deserializedOneOf.Switch(
            _ => Assert.Equal(oneOf.AsT0, deserializedOneOf.AsT0),
            _ => Assert.Equivalent(oneOf.AsT1, deserializedOneOf.AsT1),
            _ => Assert.Equivalent(oneOf.AsT2, deserializedOneOf.AsT2),
            _ => Assert.Equal(oneOf.AsT3, deserializedOneOf.AsT3));
    }

    class TestDataGenerator : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { (OneOf<int, string[], List<int>, TimeSpan?>) 1, """{"Int32":1}""" };
            yield return new object[] { (OneOf<int, string[], List<int>, TimeSpan?>) new[] { "A", "B" }, """{"String[]":["A","B"]}""" };
            yield return new object[] { (OneOf<int, string[], List<int>, TimeSpan?>) new List<int> { 1, 2 }, """{"List\u00601":[1,2]}""" };
            yield return new object[] { (OneOf<int, string[], List<int>, TimeSpan?>) TimeSpan.FromSeconds(62), """{"TimeSpan":"00:01:02"}""" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}