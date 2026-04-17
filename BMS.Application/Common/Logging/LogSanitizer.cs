using System.Text.Json.Nodes;

namespace BMS.Application.Common.Logging;

public static class LogSanitizer
{
    //private static readonly string[] SensitiveFields =
    private static readonly HashSet<string> SensitiveFields =
    new(StringComparer.OrdinalIgnoreCase)
    {
        "Password",
        "ConfirmPassword",
        "NewPassword",
        "CurrentPassword",
        "Token",
        "RefreshToken",
        "Secret",
        "ApiKey"
    };

    public static string Sanitize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;

        var node = JsonNode.Parse(json);

        Mask(node);

        return node?.ToJsonString() ?? json;
    }

    private static void Mask(JsonNode? node)
    {
        if (node is JsonObject obj)
        {
            foreach (var prop in obj.ToList())
            {
                if (SensitiveFields.Contains(prop.Key))
                {
                    obj[prop.Key] = "***";
                }
                else
                {
                    Mask(prop.Value);
                }
            }
        }
        else if (node is JsonArray arr)
        {
            foreach (var item in arr)
                Mask(item);
        }
    }
}
