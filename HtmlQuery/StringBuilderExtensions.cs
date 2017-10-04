namespace PeinearyDevelopment.Framework.HtmlQuery
{
    using System;
    using System.Globalization;
    using System.Text;

    internal static class StringBuilderExtensions
    {
        internal static bool EndsWith(this StringBuilder builder, string text)
        {
            if (builder.Length < text.Length)
            {
                return false;
            }

            var builderLength = builder.Length;
            var textLength = text.Length;
            for (var i = 1; i <= textLength; i++)
            {
                if (!string.Equals(text[textLength - i].ToString(CultureInfo.InvariantCulture), builder[builderLength - i].ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool StartsWith(this StringBuilder builder, string text)
        {
            if (builder.Length < text.Length)
            {
                return false;
            }

            var builderLength = builder.Length;
            var textLength = text.Length;
            for (var i = 0; i < textLength; i++)
            {
                if (!string.Equals(text[i].ToString(CultureInfo.InvariantCulture), builder[i].ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
