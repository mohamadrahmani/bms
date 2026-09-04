using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using BMS.Application.Common.Settings;

internal static partial class SystemErrorLogSanitizer
{
    private static readonly HashSet<string> SensitiveKeys =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "password",
            "pass",
            "token",
            "accessToken",
            "access_token",
            "refreshToken",
            "refresh_token",
            "authorization",
            "clientSecret",
            "client_secret",
            "apiKey",
            "api_key",
            "secret",
            "cardNumber",
            "card_number"
        };

    public static async Task<string?> ReadRequestBodyAsync(
        HttpRequest request,
        SystemErrorLogOptions options,
        CancellationToken cancellationToken)
    {
        if (request.ContentLength is 0)
            return null;

        if (IsSensitiveEndpoint(request.Path))
            return "[Request body omitted because the endpoint may contain sensitive data]";

        if (request.ContentLength is > 1024 * 1024)
            return "[Request body omitted because it exceeds the safe inspection limit]";

        if (!request.Body.CanSeek)
            return "[Request body unavailable]";

        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync(cancellationToken);
        request.Body.Position = 0;

        if (string.IsNullOrWhiteSpace(body))
            return null;

        var sanitized = IsJsonRequest(request)
            ? SanitizeJson(body)
            : SanitizeText(body);

        return Truncate(sanitized, options.MaxRequestBodyLength);
    }

    public static string? SanitizeQueryString(
        string? queryString,
        SystemErrorLogOptions options)
    {
        if (string.IsNullOrWhiteSpace(queryString))
            return queryString;

        return Truncate(SanitizeText(queryString), options.MaxRequestBodyLength);
    }

    public static string Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return value ?? string.Empty;

        var limit = Math.Max(256, maxLength);
        return value.Length <= limit
            ? value
            : value[..limit] + "...[truncated]";
    }

    private static string SanitizeJson(string body)
    {
        try
        {
            var node = JsonNode.Parse(body);
            if (node is null)
                return "[Request body was empty]";

            RedactJsonNode(node);
            return node.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }
        catch (JsonException)
        {
            return "[Request body was not valid JSON]";
        }
    }

    private static void RedactJsonNode(JsonNode node)
    {
        if (node is JsonObject obj)
        {
            foreach (var property in obj.ToList())
            {
                if (SensitiveKeys.Contains(property.Key))
                {
                    obj[property.Key] = "***REDACTED***";
                }
                else if (property.Value is not null)
                {
                    RedactJsonNode(property.Value);
                }
            }
        }
        else if (node is JsonArray array)
        {
            foreach (var item in array)
            {
                if (item is not null)
                    RedactJsonNode(item);
            }
        }
    }

    private static string SanitizeText(string value)
    {
        return SensitiveValueRegex().Replace(
            value,
            match => $"{match.Groups["key"].Value}{match.Groups["separator"].Value}***REDACTED***");
    }

    private static bool IsJsonRequest(HttpRequest request) =>
        request.ContentType?.Contains("json", StringComparison.OrdinalIgnoreCase) == true;

    private static bool IsSensitiveEndpoint(PathString path)
    {
        var value = path.Value ?? string.Empty;
        return value.Contains("/login", StringComparison.OrdinalIgnoreCase) ||
               value.Contains("/password", StringComparison.OrdinalIgnoreCase) ||
               value.Contains("/token", StringComparison.OrdinalIgnoreCase);
    }

    [GeneratedRegex(
        "(?<key>password|pass|token|accessToken|access_token|refreshToken|refresh_token|authorization|clientSecret|client_secret|apiKey|api_key|secret|cardNumber|card_number)(?<separator>\\s*[:=]\\s*)(\"[^\"]*\"|'[^']*'|[^&,\\s}]+)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex SensitiveValueRegex();
}
