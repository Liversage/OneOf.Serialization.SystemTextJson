using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace OneOf.Serialization.SystemTextJson.Tests;

public class DefaultIgnoreConditionTests
{
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void WhenWritingDefaultTest(object oneOf, string expectedJson)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters =
            {
                Converters.OneOfJsonConverter,
            },
        };
        var json = JsonSerializer.Serialize(oneOf, serializerOptions);
        var deserializedOneOf = JsonSerializer.Deserialize(json, oneOf.GetType(), serializerOptions);
        Assert.Equal(expectedJson, json);
        Assert.Equal(oneOf, deserializedOneOf);
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void WhenWritingNullTest(object oneOf, string expectedJson)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                Converters.OneOfJsonConverter,
            },
        };
        var json = JsonSerializer.Serialize(oneOf, serializerOptions);
        var deserializedOneOf = JsonSerializer.Deserialize(json, oneOf.GetType(), serializerOptions);
        Assert.Equal(expectedJson, json);
        Assert.Equal(oneOf, deserializedOneOf);
    }

    class TestDataGenerator : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { (OneOf<int, string?, DateTime?, A?>) 0, """{"Int32":0}""" };
            yield return new object[] { (OneOf<int, string?, DateTime?, A?>) (string?) null, """{"String":null}""" };
            yield return new object[] { (OneOf<int, string?, DateTime?, A?>) (DateTime?) null, """{"DateTime":null}""" };
            yield return new object[] { (OneOf<int, string?, DateTime?, A?>) (A?) null, """{"A":null}""" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}