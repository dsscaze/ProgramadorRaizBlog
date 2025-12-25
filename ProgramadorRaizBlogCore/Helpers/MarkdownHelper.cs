using System.Text.RegularExpressions;

namespace ProgramadorRaizBlogCore.Helpers;

public static class MarkdownHelper
{
    public static string FormatarCorpoPost(string? corpo)
    {
        if (string.IsNullOrEmpty(corpo))
        {
            return corpo ?? string.Empty;
        }

        string corpoOriginal = corpo;

        // Formatar blocos de código C#
        corpoOriginal = corpoOriginal.Replace("\n```c#", "<pre class=\"prettyprint\">");
        corpoOriginal = corpoOriginal.Replace("\n```", "</pre>");

        // Remover linhas de divisão
        corpoOriginal = corpoOriginal.Replace("-------------------------------------------------------------------------------------------", "<hr />");

        // Formatar URLs
        Regex urlRx = new Regex(@"(?<url>(http[s]?:[/][/]|www.)([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])*)", RegexOptions.IgnoreCase);
        MatchCollection matches = urlRx.Matches(corpoOriginal);
        foreach (Match match in matches)
        {
            var url = match.Groups["url"].Value;
            corpoOriginal = corpoOriginal.Replace(url, string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", url));
        }

        // Dividir em parágrafos por dupla quebra de linha
        string[] paragrafos = corpoOriginal.Split(
            new string[] { "\n\n" },
            StringSplitOptions.None
        );

        string corpoFormatado = string.Empty;
        foreach (var conteudo in paragrafos)
        {
            corpoFormatado += "<p>" + conteudo + "</p>";
        }

        return corpoFormatado;
    }
}
