namespace OneOf.Serialization.SystemTextJson.Tests;

static class Converters
{
    // Reuse converters across tests to verify that internal caching works.
    public static readonly OneOfJsonConverter OneOfJsonConverter = new();
    public static readonly OneOfBaseJsonConverter OneOfBaseJsonConverter = new();
}