using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace OneOf.Serialization.JsonConverterGenerator;

[Generator(LanguageNames.CSharp)]
public class OneOfJsonConverterGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDescriptors = context.SyntaxProvider.CreateSyntaxProvider(
                (syntaxNode, _) => syntaxNode.Kind() is SyntaxKind.ClassDeclaration,
                (generatorSyntaxContext, _) => CreateClassDescriptor((ClassDeclarationSyntax) generatorSyntaxContext.Node))
            .Where(descriptor => descriptor.ShouldGenerateCode);
        context.RegisterSourceOutput(
            classDescriptors,
            (sourceProductionContext, classDescriptor) =>
            {
                sourceProductionContext.AddSource(
                    $"{classDescriptor.Name}{classDescriptor.Arity}.g.cs",
                    SourceText.From(GenerateSourceCode(classDescriptor.Name, classDescriptor.Arity), Encoding.UTF8));
            });
    }

    static ClassDescriptor CreateClassDescriptor(TypeDeclarationSyntax classDeclaration)
    {
        var isPartial = classDeclaration.Modifiers.Any(syntaxToken => syntaxToken.Text is "partial");
        var arity = classDeclaration.TypeParameterList?.Parameters.Count ?? 0;
        var jsonConverterBase = (GenericNameSyntax?) classDeclaration.BaseList?.Types
            .FirstOrDefault(baseTypeSyntax => baseTypeSyntax.Type is GenericNameSyntax { Arity: 1, Identifier.Text: "JsonConverter" })
            ?.Type;
        var typeConverted = jsonConverterBase?.TypeArgumentList.Arguments[0];
        var isOneOfConverter = typeConverted is GenericNameSyntax { Identifier.Text: "OneOf" } genericNameSyntax && arity == genericNameSyntax.Arity;
        return new ClassDescriptor(isPartial, isOneOfConverter, arity, classDeclaration.Identifier.Text);
    }

    static string GenerateSourceCode(string className, int arity)
    {
        var genericArgumentList = GetGenericArgumentList(arity);
        return
            $$"""
              #nullable enable
              using OneOf;
              using System;
              using System.Collections.Concurrent;
              using System.Collections.Generic;
              using System.Linq;
              using System.Reflection;
              using System.Text.Json;
              using System.Text.Json.Serialization;

              namespace OneOf.Serialization.SystemTextJson;

              partial class {{className}}<{{genericArgumentList}}>
              {
                  static readonly ConcurrentDictionary<CacheKey, IReadOnlyDictionary<string, OneOfDescriptor>> cache = new();
                  
                  readonly IReadOnlyDictionary<Type, string> propertyNames;
                  
                  public {{className}}(IReadOnlyDictionary<Type, string> propertyNames) =>
                      this.propertyNames = propertyNames;
              
                  public override OneOf<{{genericArgumentList}}> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                  {
                      if (reader.TokenType is not JsonTokenType.StartObject)
                          throw new JsonException();
                      reader.Read();
                      if (reader.TokenType is not JsonTokenType.PropertyName)
                          throw new JsonException();
                      var propertyName = reader.GetString() ?? throw new JsonException();
                      if (!GetDescriptors(options).TryGetValue(propertyName, out var descriptor))
                          throw new JsonException();
                      var oneOfValue = JsonSerializer.Deserialize(ref reader, descriptor.Type, options);
                      reader.Read();
                      if (reader.TokenType is not JsonTokenType.EndObject)
                          throw new JsonException();
                      return (OneOf<{{genericArgumentList}}>) descriptor.FactoryMethod.Invoke(null, new object?[] { oneOfValue })!;
                  }
              
                  public override void Write(Utf8JsonWriter writer, OneOf<{{genericArgumentList}}> value, JsonSerializerOptions options)
                  {
                      value.Switch(
                          {{GetSwitchArguments(arity, 12)}});
                  
                      void WriteOneOf<TOneOf>(TOneOf value)
                      {
                          writer.WriteStartObject();
                          writer.WritePropertyName(GetPropertyName(typeof(TOneOf), options.PropertyNamingPolicy));
                          JsonSerializer.Serialize(writer, value, options);
                          writer.WriteEndObject();
                      }
                  }
              
                  IReadOnlyDictionary<string, OneOfDescriptor> GetDescriptors(JsonSerializerOptions options)
                  {
                      return cache.GetOrAdd(new CacheKey(options.PropertyNamingPolicy, options.PropertyNameCaseInsensitive), ValueFactory);
                  
                      IReadOnlyDictionary<string, OneOfDescriptor> ValueFactory(CacheKey key)
                      {
                          try
                          {
                              return GetProperties(key.NamingPolicy).ToDictionary(
                                  kvp => kvp.Key,
                                  kvp => kvp.Value,
                                  key.IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
                          }
                          catch (ArgumentException exception)
                          {
                              throw new JsonException("Multiple union values share the same JSON property name.", exception);
                          }
                      }
                  
                      IEnumerable<KeyValuePair<string, OneOfDescriptor>> GetProperties(JsonNamingPolicy? namingPolicy)
                      {
                          {{GetDescriptorYieldStatements(arity, 12)}}
                      }
                  }
              
                  string GetPropertyName(Type type, JsonNamingPolicy? namingPolicy)
                  {
                      var propertyName = propertyNames.TryGetValue(type, out var name) ? name : GetPropertyNameHandlingNullable(type);
                      return namingPolicy?.ConvertName(propertyName) ?? propertyName;
                  }
                  
                  static string GetPropertyNameHandlingNullable(Type type) => 
                      !type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>) ? type.Name : type.GetGenericArguments()[0].Name;
              
                  static MethodInfo GetFactoryMethod(int index) =>
                      typeof(OneOf<{{genericArgumentList}}>).GetMethod($"FromT{index}", BindingFlags.Public | BindingFlags.Static)!;
              
                  record CacheKey(JsonNamingPolicy? NamingPolicy, bool IgnoreCase);
              
                  record OneOfDescriptor(Type Type, MethodInfo FactoryMethod);
              }
              """;

        static string GetGenericArgumentList(int arity) =>
            string.Join(", ", Enumerable.Range(0, arity).Select(i => $"T{i}"));

        static string GetSwitchArguments(int arity, int indentation)
        {
            var separator = $",\n{new string(' ', indentation)}";
            return string.Join(separator, Enumerable.Range(0, arity).Select(_ => "WriteOneOf"));
        }

        static string GetDescriptorYieldStatements(int arity, int indentation)
        {
            var separator = $"\n{new string(' ', indentation)}";
            return string.Join(
                separator,
                Enumerable.Range(0, arity)
                    .Select(
                        i => "yield return new KeyValuePair<string, OneOfDescriptor>(" +
                             $"GetPropertyName(typeof(T{i}), namingPolicy), new OneOfDescriptor(typeof(T{i}), GetFactoryMethod({i})));"));
        }
    }

    record ClassDescriptor(bool IsPartial, bool IsOneOfConverter, int Arity, string Name)
    {
        public bool ShouldGenerateCode => IsPartial && IsOneOfConverter;
    }
}