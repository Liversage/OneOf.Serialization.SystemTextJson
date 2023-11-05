using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace OneOf.Serialization.SystemTextJson;

/// <summary>
///     Converts a class derived from <c>OneOfBase</c> to or from JSON.
/// </summary>
public class OneOfBaseJsonConverter : JsonConverterFactory
{
    static readonly IReadOnlyDictionary<Type, string> empty = new Dictionary<Type, string>();
    static readonly Regex oneOfBaseGenericTypeNameRegex = new("^OneOf.OneOfBase`[1-9]+$");
    
    readonly IReadOnlyDictionary<Type, string> propertyNames;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OneOfBaseJsonConverter" /> class.
    /// </summary>
    public OneOfBaseJsonConverter() : this(empty)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OneOfBaseJsonConverter" /> class providing customized JSON property
    ///     names.
    /// </summary>
    /// <param name="propertyNames">
    ///     An <see cref="IReadOnlyDictionary{Type, string}" /> mapping union types to JSON property names. When no
    ///     mapping is provided the name of the union type is used at the JSON property name.
    /// </param>
    public OneOfBaseJsonConverter(IReadOnlyDictionary<Type, string> propertyNames) => this.propertyNames = propertyNames;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OneOfBaseJsonConverter" /> class providing customized JSON property
    ///     names.
    /// </summary>
    /// <param name="propertyNames">
    ///     An array of tuples mapping union types to JSON property names. When no mapping is provided the name of the union
    ///     type is used at the JSON property name.
    /// </param>
    public OneOfBaseJsonConverter(params (Type Type, string PropertyName)[] propertyNames)
        : this(propertyNames.ToDictionary(tuple => tuple.Type, tuple => tuple.PropertyName))
    {
    }

    /// <inheritdoc cref="JsonConverterFactory"/>
    public override bool CanConvert(Type typeToConvert) =>
        typeof(IOneOf).IsAssignableFrom(typeToConvert) && FindOneOfBaseType(typeToConvert) is not null;

    /// <inheritdoc cref="JsonConverterFactory"/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var oneOfType = FindOneOfBaseType(typeToConvert)!;
        var genericArguments = oneOfType.GetGenericArguments();
        // This factory as well as each converter should use the same type name.
        var genericTypeName = $"{GetType().FullName}`{genericArguments.Length + 1}";
        var genericTypeDefinition = GetType().Assembly.GetType(genericTypeName)!;
        var converterType = genericTypeDefinition.MakeGenericType(genericArguments.Prepend(typeToConvert).ToArray());
        return (JsonConverter) Activator.CreateInstance(converterType, propertyNames)!;
    }
    
    static Type? FindOneOfBaseType(Type typeToConvert)
    {
        var type = typeToConvert;
        while (type is not null)
        {
            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition.FullName is not null && oneOfBaseGenericTypeNameRegex.IsMatch(genericTypeDefinition.FullName))
                    return type;
            }
            type = type.BaseType;
        }
        return null;
    }
}

// Converters to generate.
partial class OneOfBaseJsonConverter<T, T0> : JsonConverter<T> where T : OneOfBase<T0> { }
partial class OneOfBaseJsonConverter<T, T0, T1> : JsonConverter<T> where T : OneOfBase<T0, T1> { }
partial class OneOfBaseJsonConverter<T, T0, T1, T2> : JsonConverter<T> where T : OneOfBase<T0, T1, T2> { }
partial class OneOfBaseJsonConverter<T, T0, T1, T2, T3> : JsonConverter<T> where T : OneOfBase<T0, T1, T2, T3> { }
partial class OneOfBaseJsonConverter<T, T0, T1, T2, T3, T4> : JsonConverter<T> where T : OneOfBase<T0, T1, T2, T3, T4> { }
partial class OneOfBaseJsonConverter<T, T0, T1, T2, T3, T4, T5> : JsonConverter<T> where T : OneOfBase<T0, T1, T2, T3, T4, T5> { }
partial class OneOfBaseJsonConverter<T, T0, T1, T2, T3, T4, T5, T6> : JsonConverter<T> where T: OneOfBase<T0, T1, T2, T3, T4, T5, T6> { }
partial class OneOfBaseJsonConverter<T, T0, T1, T2, T3, T4, T5, T6, T7> : JsonConverter<T> where T : OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7> { }
partial class OneOfBaseJsonConverter<T, T0, T1, T2, T3, T4, T5, T6, T7, T8> : JsonConverter<T> where T : OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8> { }