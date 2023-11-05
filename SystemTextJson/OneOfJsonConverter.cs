using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace OneOf.Serialization.SystemTextJson;

/// <summary>
///     Converts an <c>OneOf</c> to or from JSON.
/// </summary>
public class OneOfJsonConverter : JsonConverterFactory
{
    static readonly IReadOnlyDictionary<Type, string> empty = new Dictionary<Type, string>();
    static readonly Regex oneOfGenericTypeNameRegex = new("^OneOf.OneOf`[1-9]+$");

    readonly IReadOnlyDictionary<Type, string> propertyNames;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OneOfJsonConverter" /> class.
    /// </summary>
    public OneOfJsonConverter() : this(empty)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OneOfJsonConverter" /> class providing customized JSON property names.
    /// </summary>
    /// <param name="propertyNames">
    ///     An <see cref="IReadOnlyDictionary{Type, string}" /> mapping union types to JSON property names. When no
    ///     mapping is provided the name of the union type is used at the JSON property name.
    /// </param>
    public OneOfJsonConverter(IReadOnlyDictionary<Type, string> propertyNames)
    {
        this.propertyNames = propertyNames;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OneOfJsonConverter" /> class providing customized JSON property names.
    /// </summary>
    /// <param name="propertyNames">
    ///     An array of tuples mapping union types to JSON property names. When no mapping is provided the name of the union
    ///     type is used at the JSON property name.
    /// </param>
    public OneOfJsonConverter(params (Type Type, string PropertyName)[] propertyNames)
        : this(propertyNames.ToDictionary(tuple => tuple.Type, tuple => tuple.PropertyName))
    {
    }

    /// <inheritdoc cref="JsonConverterFactory"/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;
        var genericTypeDefinition = typeToConvert.GetGenericTypeDefinition();
        return genericTypeDefinition.FullName is not null && oneOfGenericTypeNameRegex.IsMatch(genericTypeDefinition.FullName);
    }

    /// <inheritdoc cref="JsonConverterFactory"/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var genericArguments = typeToConvert.GetGenericArguments();
        // This factory as well as each converter should use the same type name.
        var genericTypeName = $"{GetType().FullName}`{genericArguments.Length}";
        var genericTypeDefinition = GetType().Assembly.GetType(genericTypeName)!;
        var converterType = genericTypeDefinition.MakeGenericType(genericArguments);
        
        return (JsonConverter) Activator.CreateInstance(converterType, propertyNames)!;
    }
}

// Converters to generate.
partial class OneOfJsonConverter<T0> : JsonConverter<OneOf<T0>> { }
partial class OneOfJsonConverter<T0, T1> : JsonConverter<OneOf<T0, T1>> { }
partial class OneOfJsonConverter<T0, T1, T2> : JsonConverter<OneOf<T0, T1, T2>> { }
partial class OneOfJsonConverter<T0, T1, T2, T3> : JsonConverter<OneOf<T0, T1, T2, T3>> { }
partial class OneOfJsonConverter<T0, T1, T2, T3, T4> : JsonConverter<OneOf<T0, T1, T2, T3, T4>> { }
partial class OneOfJsonConverter<T0, T1, T2, T3, T4, T5> : JsonConverter<OneOf<T0, T1, T2, T3, T4, T5>> { }
partial class OneOfJsonConverter<T0, T1, T2, T3, T4, T5, T6> : JsonConverter<OneOf<T0, T1, T2, T3, T4, T5, T6>> { }
partial class OneOfJsonConverter<T0, T1, T2, T3, T4, T5, T6, T7> : JsonConverter<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> { }
partial class OneOfJsonConverter<T0, T1, T2, T3, T4, T5, T6, T7, T8> : JsonConverter<OneOf<T0, T1, T2, T3, T4, T5, T6, T7, T8>> { }