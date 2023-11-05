using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace OneOf.Serialization.SystemTextJson.Tests;

public class SerializeAndDeserializeWithCamelCaseNamingTests
{
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void Test(object oneOf, string expectedJson)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                Converters.OneOfJsonConverter,
                Converters.OneOfBaseJsonConverter,
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
            yield return new object[] { (OneOfBased1) new A("A"), """{"a":{"value":"A"}}""" };
            
            yield return new object[] { (OneOfBased2) new A("A"), """{"a":{"value":"A"}}""" };
            yield return new object[] { (OneOfBased2) new B("B"), """{"b":{"value":"B"}}""" };
            
            yield return new object[] { (OneOfBased3) new A("A"), """{"a":{"value":"A"}}""" };
            yield return new object[] { (OneOfBased3) new B("B"), """{"b":{"value":"B"}}""" };
            yield return new object[] { (OneOfBased3) new C("C"), """{"c":{"value":"C"}}""" };
            
            yield return new object[] { (OneOfBased4) new A("A"), """{"a":{"value":"A"}}""" };
            yield return new object[] { (OneOfBased4) new B("B"), """{"b":{"value":"B"}}""" };
            yield return new object[] { (OneOfBased4) new C("C"), """{"c":{"value":"C"}}""" };
            yield return new object[] { (OneOfBased4) new D("D"), """{"d":{"value":"D"}}""" };
            
            yield return new object[] { (OneOfBased5) new A("A"), """{"a":{"value":"A"}}""" };
            yield return new object[] { (OneOfBased5) new B("B"), """{"b":{"value":"B"}}""" };
            yield return new object[] { (OneOfBased5) new C("C"), """{"c":{"value":"C"}}""" };
            yield return new object[] { (OneOfBased5) new D("D"), """{"d":{"value":"D"}}""" };
            yield return new object[] { (OneOfBased5) new E("E"), """{"e":{"value":"E"}}""" };
            
            yield return new object[] { (OneOfBased6) new A("A"), """{"a":{"value":"A"}}""" };
            yield return new object[] { (OneOfBased6) new B("B"), """{"b":{"value":"B"}}""" };
            yield return new object[] { (OneOfBased6) new C("C"), """{"c":{"value":"C"}}""" };
            yield return new object[] { (OneOfBased6) new D("D"), """{"d":{"value":"D"}}""" };
            yield return new object[] { (OneOfBased6) new E("E"), """{"e":{"value":"E"}}""" };
            yield return new object[] { (OneOfBased6) new F("F"), """{"f":{"value":"F"}}""" };
            
            yield return new object[] { (OneOfBased7) new A("A"), """{"a":{"value":"A"}}""" };
            yield return new object[] { (OneOfBased7) new B("B"), """{"b":{"value":"B"}}""" };
            yield return new object[] { (OneOfBased7) new C("C"), """{"c":{"value":"C"}}""" };
            yield return new object[] { (OneOfBased7) new D("D"), """{"d":{"value":"D"}}""" };
            yield return new object[] { (OneOfBased7) new E("E"), """{"e":{"value":"E"}}""" };
            yield return new object[] { (OneOfBased7) new F("F"), """{"f":{"value":"F"}}""" };
            yield return new object[] { (OneOfBased7) new G("G"), """{"g":{"value":"G"}}""" };
            
            yield return new object[] { (OneOfBased8) new A("A"), """{"a":{"value":"A"}}""" };
            yield return new object[] { (OneOfBased8) new B("B"), """{"b":{"value":"B"}}""" };
            yield return new object[] { (OneOfBased8) new C("C"), """{"c":{"value":"C"}}""" };
            yield return new object[] { (OneOfBased8) new D("D"), """{"d":{"value":"D"}}""" };
            yield return new object[] { (OneOfBased8) new E("E"), """{"e":{"value":"E"}}""" };
            yield return new object[] { (OneOfBased8) new F("F"), """{"f":{"value":"F"}}""" };
            yield return new object[] { (OneOfBased8) new G("G"), """{"g":{"value":"G"}}""" };
            yield return new object[] { (OneOfBased8) new H("H"), """{"h":{"value":"H"}}""" };
            
            yield return new object[] { (OneOfBased9) new A("A"), """{"a":{"value":"A"}}""" };
            yield return new object[] { (OneOfBased9) new B("B"), """{"b":{"value":"B"}}""" };
            yield return new object[] { (OneOfBased9) new C("C"), """{"c":{"value":"C"}}""" };
            yield return new object[] { (OneOfBased9) new D("D"), """{"d":{"value":"D"}}""" };
            yield return new object[] { (OneOfBased9) new E("E"), """{"e":{"value":"E"}}""" };
            yield return new object[] { (OneOfBased9) new F("F"), """{"f":{"value":"F"}}""" };
            yield return new object[] { (OneOfBased9) new G("G"), """{"g":{"value":"G"}}""" };
            yield return new object[] { (OneOfBased9) new H("H"), """{"h":{"value":"H"}}""" };
            yield return new object[] { (OneOfBased9) new I("I"), """{"i":{"value":"I"}}""" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}