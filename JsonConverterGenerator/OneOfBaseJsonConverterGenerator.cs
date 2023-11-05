using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace OneOf.Serialization.JsonConverterGenerator;

[Generator(LanguageNames.CSharp)]
public class OneOfBaseJsonConverterGenerator : IIncrementalGenerator
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
        var arity = (classDeclaration.TypeParameterList?.Parameters.Count ?? 0) - 1;
        var jsonConverterBase = (GenericNameSyntax?) classDeclaration.BaseList?.Types
            .FirstOrDefault(baseTypeSyntax => baseTypeSyntax.Type is GenericNameSyntax { Arity: 1, Identifier.Text: "JsonConverter" })
            ?.Type;
        var isJsonConverter = jsonConverterBase is not null;
        var isConstrainedToOneOfBase = classDeclaration.ConstraintClauses is
                                       [
                                           {
                                               Constraints:
                                               [
                                                   TypeConstraintSyntax { Type: GenericNameSyntax { Identifier.Text: "OneOfBase" } genericNameSyntax },
                                               ],
                                           },
                                       ] &&
                                       genericNameSyntax.Arity == arity;
        return new ClassDescriptor(isPartial, isJsonConverter, isConstrainedToOneOfBase, arity, classDeclaration.Identifier.Text);
    }

    static string GenerateSourceCode(string className, int arity)
    {
        var genericArgumentList = GetGenericArgumentList(arity);
        return
            $$"""
              #nullable enable
              using OneOf;
              using System;
              using System.Collections.Generic;
              using System.Reflection;
              using System.Text.Json;
              using System.Text.Json.Serialization;

              namespace OneOf.Serialization.SystemTextJson;

              partial class {{className}}<T, {{genericArgumentList}}>
              {
                  readonly ConstructorInfo constructor;
                  readonly IReadOnlyDictionary<Type, string> propertyNames;
                  
                  public {{className}}(IReadOnlyDictionary<Type, string> propertyNames)
                  {
                      this.propertyNames = propertyNames;
                      constructor = typeof(T)
                          .GetConstructor(
                              BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                              null,
                              new[] { typeof(OneOf<{{genericArgumentList}}>) },
                              null)
                          ?? throw new JsonException();
                  }
              
                  public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                  {
                      var oneOf = JsonSerializer.Deserialize(ref reader, typeof(OneOf<{{genericArgumentList}}>), options)!;
                      return (T) constructor.Invoke(new[] { oneOf });
                  }
              
                  public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
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
              
                  string GetPropertyName(Type type, JsonNamingPolicy? namingPolicy)
                  {
                      var propertyName = propertyNames.TryGetValue(type, out var name) ? name : type.Name;
                      return namingPolicy?.ConvertName(propertyName) ?? propertyName;
                  }
              }
              """;

        static string GetGenericArgumentList(int arity) =>
            string.Join(", ", Enumerable.Range(0, arity).Select(i => $"T{i}"));

        static string GetSwitchArguments(int arity, int indentation)
        {
            var separator = $",\n{new string(' ', indentation)}";
            return string.Join(separator, Enumerable.Range(0, arity).Select(_ => "WriteOneOf"));
        }
    }

    record ClassDescriptor(bool IsPartial, bool IsJsonConverter, bool IsConstrainedToOneOfBase, int Arity, string Name)
    {
        public bool ShouldGenerateCode => IsPartial && IsJsonConverter && IsConstrainedToOneOfBase;
    }
}