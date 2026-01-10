using System.Text.Encodings.Web;

namespace Business_Layer.Sanitizations;

public static class Sanitization
{
    public static string SanitizeInput(string input)
    {
        return HtmlEncoder.Default.Encode(input);
    }
}
