using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace OneOf.Serialization.SystemTextJson.Tests;

public class OneOfSerializeAndDeserializeTests
{
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void Test(object oneOf, string expectedJson)
    {
        var serializerOptions = new JsonSerializerOptions
        {
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
            yield return new object[] { (OneOf<A>) new A("A"), """{"A":{"Value":"A"}}""" };

            yield return new object[] { (OneOf<A, B>) new A("A"), """{"A":{"Value":"A"}}""" };
            yield return new object[] { (OneOf<A, B>) new B("B"), """{"B":{"Value":"B"}}""" };

            yield return new object[] { (OneOf<A, B, C>) new A("A"), """{"A":{"Value":"A"}}""" };
            yield return new object[] { (OneOf<A, B, C>) new B("B"), """{"B":{"Value":"B"}}""" };
            yield return new object[] { (OneOf<A, B, C>) new C("C"), """{"C":{"Value":"C"}}""" };

            yield return new object[] { (OneOf<A, B, C, D>) new A("A"), """{"A":{"Value":"A"}}""" };
            yield return new object[] { (OneOf<A, B, C, D>) new B("B"), """{"B":{"Value":"B"}}""" };
            yield return new object[] { (OneOf<A, B, C, D>) new C("C"), """{"C":{"Value":"C"}}""" };
            yield return new object[] { (OneOf<A, B, C, D>) new D("D"), """{"D":{"Value":"D"}}""" };

            yield return new object[] { (OneOf<A, B, C, D, E>) new A("A"), """{"A":{"Value":"A"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E>) new B("B"), """{"B":{"Value":"B"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E>) new C("C"), """{"C":{"Value":"C"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E>) new D("D"), """{"D":{"Value":"D"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E>) new E("E"), """{"E":{"Value":"E"}}""" };

            yield return new object[] { (OneOf<A, B, C, D, E, F>) new A("A"), """{"A":{"Value":"A"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F>) new B("B"), """{"B":{"Value":"B"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F>) new C("C"), """{"C":{"Value":"C"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F>) new D("D"), """{"D":{"Value":"D"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F>) new E("E"), """{"E":{"Value":"E"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F>) new F("F"), """{"F":{"Value":"F"}}""" };

            yield return new object[] { (OneOf<A, B, C, D, E, F, G>) new A("A"), """{"A":{"Value":"A"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G>) new B("B"), """{"B":{"Value":"B"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G>) new C("C"), """{"C":{"Value":"C"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G>) new D("D"), """{"D":{"Value":"D"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G>) new E("E"), """{"E":{"Value":"E"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G>) new F("F"), """{"F":{"Value":"F"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G>) new G("G"), """{"G":{"Value":"G"}}""" };

            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H>) new A("A"), """{"A":{"Value":"A"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H>) new B("B"), """{"B":{"Value":"B"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H>) new C("C"), """{"C":{"Value":"C"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H>) new D("D"), """{"D":{"Value":"D"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H>) new E("E"), """{"E":{"Value":"E"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H>) new F("F"), """{"F":{"Value":"F"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H>) new G("G"), """{"G":{"Value":"G"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H>) new H("H"), """{"H":{"Value":"H"}}""" };

            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H, I>) new A("A"), """{"A":{"Value":"A"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H, I>) new B("B"), """{"B":{"Value":"B"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H, I>) new C("C"), """{"C":{"Value":"C"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H, I>) new D("D"), """{"D":{"Value":"D"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H, I>) new E("E"), """{"E":{"Value":"E"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H, I>) new F("F"), """{"F":{"Value":"F"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H, I>) new G("G"), """{"G":{"Value":"G"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H, I>) new H("H"), """{"H":{"Value":"H"}}""" };
            yield return new object[] { (OneOf<A, B, C, D, E, F, G, H, I>) new I("I"), """{"I":{"Value":"I"}}""" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}