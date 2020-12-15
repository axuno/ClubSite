using System;
using System.Text.RegularExpressions;

namespace ClubSite.Library
{
    public class HtmlTool
    {
        /// <summary>
        /// Checks, whether the input string leaves neither a text literal nor an image tag, after removing all HTML tags and non-breaking spaces except the image tag.
        /// </summary>
        /// <param name="input">The input HTML to check.</param>
        /// <returns>Returns <see langword="true"/>, if the input string neither leaves a text literal nor an image tag, after removing all HTML tags and non-breaking spaces except the image tag, else <see langword="false"/>.</returns>
        public static bool IsEmptyHtml(string input)
        {
            return string.IsNullOrWhiteSpace(Regex.Replace(input, "(<([^>]+)>|&nbsp;)", string.Empty, RegexOptions.Multiline)) 
                   && !input.Contains("<img", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
