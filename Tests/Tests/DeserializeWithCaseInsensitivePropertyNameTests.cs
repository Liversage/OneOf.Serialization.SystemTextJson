using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace OneOf.Serialization.SystemTextJson.Tests;

public class DeserializeWithCaseInsensitivePropertyNameTests
{
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void Test(string json, object expectedOneOf)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                Converters.OneOfJsonConverter,
                Converters.OneOfBaseJsonConverter,
            },
        };
        var deserializedOneOf = JsonSerializer.Deserialize(json, expectedOneOf.GetType(), serializerOptions);
        Assert.Equal(expectedOneOf, deserializedOneOf);
    }

    class TestDataGenerator : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { """{"a":{"value":"A"}}""", (OneOfBased1) new A("A") };
            
            yield return new object[] { """{"a":{"value":"A"}}""", (OneOfBased2) new A("A") };
            yield return new object[] { """{"b":{"value":"B"}}""", (OneOfBased2) new B("B") };
            
            yield return new object[] { """{"a":{"value":"A"}}""", (OneOfBased3) new A("A") };
            yield return new object[] { """{"b":{"value":"B"}}""", (OneOfBased3) new B("B") };
            yield return new object[] { """{"c":{"value":"C"}}""", (OneOfBased3) new C("C") };
            
            yield return new object[] { """{"a":{"value":"A"}}""", (OneOfBased4) new A("A") };
            yield return new object[] { """{"b":{"value":"B"}}""", (OneOfBased4) new B("B") };
            yield return new object[] { """{"c":{"value":"C"}}""", (OneOfBased4) new C("C") };
            yield return new object[] { """{"d":{"value":"D"}}""", (OneOfBased4) new D("D") };
            
            yield return new object[] { """{"a":{"value":"A"}}""", (OneOfBased5) new A("A") };
            yield return new object[] { """{"b":{"value":"B"}}""", (OneOfBased5) new B("B") };
            yield return new object[] { """{"c":{"value":"C"}}""", (OneOfBased5) new C("C") };
            yield return new object[] { """{"d":{"value":"D"}}""", (OneOfBased5) new D("D") };
            yield return new object[] { """{"e":{"value":"E"}}""", (OneOfBased5) new E("E") };
            
            yield return new object[] { """{"a":{"value":"A"}}""", (OneOfBased6) new A("A") };
            yield return new object[] { """{"b":{"value":"B"}}""", (OneOfBased6) new B("B") };
            yield return new object[] { """{"c":{"value":"C"}}""", (OneOfBased6) new C("C") };
            yield return new object[] { """{"d":{"value":"D"}}""", (OneOfBased6) new D("D") };
            yield return new object[] { """{"e":{"value":"E"}}""", (OneOfBased6) new E("E") };
            yield return new object[] { """{"f":{"value":"F"}}""", (OneOfBased6) new F("F") };
            
            yield return new object[] { """{"a":{"value":"A"}}""", (OneOfBased7) new A("A") };
            yield return new object[] { """{"b":{"value":"B"}}""", (OneOfBased7) new B("B") };
            yield return new object[] { """{"c":{"value":"C"}}""", (OneOfBased7) new C("C") };
            yield return new object[] { """{"d":{"value":"D"}}""", (OneOfBased7) new D("D") };
            yield return new object[] { """{"e":{"value":"E"}}""", (OneOfBased7) new E("E") };
            yield return new object[] { """{"f":{"value":"F"}}""", (OneOfBased7) new F("F") };
            yield return new object[] { """{"g":{"value":"G"}}""", (OneOfBased7) new G("G") };
            
            yield return new object[] { """{"a":{"value":"A"}}""", (OneOfBased8) new A("A") };
            yield return new object[] { """{"b":{"value":"B"}}""", (OneOfBased8) new B("B") };
            yield return new object[] { """{"c":{"value":"C"}}""", (OneOfBased8) new C("C") };
            yield return new object[] { """{"d":{"value":"D"}}""", (OneOfBased8) new D("D") };
            yield return new object[] { """{"e":{"value":"E"}}""", (OneOfBased8) new E("E") };
            yield return new object[] { """{"f":{"value":"F"}}""", (OneOfBased8) new F("F") };
            yield return new object[] { """{"g":{"value":"G"}}""", (OneOfBased8) new G("G") };
            yield return new object[] { """{"h":{"value":"H"}}""", (OneOfBased8) new H("H") };
            
            yield return new object[] { """{"a":{"value":"A"}}""", (OneOfBased9) new A("A") };
            yield return new object[] { """{"b":{"value":"B"}}""", (OneOfBased9) new B("B") };
            yield return new object[] { """{"c":{"value":"C"}}""", (OneOfBased9) new C("C") };
            yield return new object[] { """{"d":{"value":"D"}}""", (OneOfBased9) new D("D") };
            yield return new object[] { """{"e":{"value":"E"}}""", (OneOfBased9) new E("E") };
            yield return new object[] { """{"f":{"value":"F"}}""", (OneOfBased9) new F("F") };
            yield return new object[] { """{"g":{"value":"G"}}""", (OneOfBased9) new G("G") };
            yield return new object[] { """{"h":{"value":"H"}}""", (OneOfBased9) new H("H") };
            yield return new object[] { """{"i":{"value":"I"}}""", (OneOfBased9) new I("I") };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}