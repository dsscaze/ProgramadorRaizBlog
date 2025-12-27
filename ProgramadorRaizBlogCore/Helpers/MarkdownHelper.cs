using System.Text.RegularExpressions;

namespace ProgramadorRaizBlogCore.Helpers;

public static class MarkdownHelper
{
    private static Dictionary<string, string> _blocosDeCodigoProtegidos = new();

    public static string FormatarCorpoPost(string? corpo)
    {
        if (string.IsNullOrEmpty(corpo))
        {
            return corpo ?? string.Empty;
        }

        _blocosDeCodigoProtegidos.Clear();
        string corpoOriginal = corpo;

        // ETAPA 1: Proteger blocos de código com placeholders
        corpoOriginal = ProtegerBlocosDeCodigoMarkdown(corpoOriginal);

        // ETAPA 2: Formatar headers (fora dos blocos de código protegidos)
        corpoOriginal = Regex.Replace(corpoOriginal, @"^### (.+)$", m => $"<h3>{m.Groups[1].Value}</h3>", RegexOptions.Multiline);
        corpoOriginal = Regex.Replace(corpoOriginal, @"^## (.+)$", m => FormatarH2(m.Groups[1].Value), RegexOptions.Multiline);
        corpoOriginal = Regex.Replace(corpoOriginal, @"^# (.+)$", m => $"<h1>{m.Groups[1].Value}</h1>", RegexOptions.Multiline);

        // ETAPA 3: Remover linhas de divisão
        corpoOriginal = corpoOriginal.Replace("-------------------------------------------------------------------------------------------", "<hr />");

        // ETAPA 4: Formatar negrito: **texto** para <strong>texto</strong>
        corpoOriginal = Regex.Replace(corpoOriginal, @"\*\*(.+?)\*\*", "<strong>$1</strong>");

        // ETAPA 5: Formatar links markdown: [texto](url) para <a>texto</a>
        corpoOriginal = Regex.Replace(corpoOriginal, @"\[([^\]]+)\]\(([^)]+)\)", 
            m => $"<a href=\"{m.Groups[2].Value}\" target=\"_blank\">{m.Groups[1].Value}</a>");

        // ETAPA 6: Formatar URLs soltas (que não estão em markdown)
        Regex urlRx = new Regex(@"(?<url>(http[s]?:[/][/]|www.)([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])*)", RegexOptions.IgnoreCase);
        MatchCollection matches = urlRx.Matches(corpoOriginal);
        foreach (Match match in matches)
        {
            var url = match.Groups["url"].Value;
            // Evitar substituir URLs que já estão dentro de tags <a>
            if (!corpoOriginal.Contains($"<a href=\"{url}\""))
            {
                corpoOriginal = corpoOriginal.Replace(url, string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", url));
            }
        }

        // ETAPA 7: Converter quebras de linha simples em <br>
        // Mas preservar duplas quebras de linha para criar parágrafos
        corpoOriginal = corpoOriginal.Replace("\n\n", "|||PARAGRAFO_BREAK|||");
        corpoOriginal = corpoOriginal.Replace("\n", "<br>\n");
        corpoOriginal = corpoOriginal.Replace("|||PARAGRAFO_BREAK|||", "\n\n");

        // ETAPA 8: Dividir em parágrafos por dupla quebra de linha
        string[] paragrafos = corpoOriginal.Split(
            new string[] { "\n\n" },
            StringSplitOptions.None
        );

        string corpoFormatado = string.Empty;
        foreach (var conteudo in paragrafos)
        {
            if (!string.IsNullOrWhiteSpace(conteudo))
            {
                corpoFormatado += "<p>" + conteudo.Trim() + "</p>";
            }
        }

        // ETAPA 9: Restaurar blocos de código protegidos
        corpoFormatado = RestaurarBlocosDeCodigoMarkdown(corpoFormatado);

        return corpoFormatado;
    }

    private static string ProtegerBlocosDeCodigoMarkdown(string corpo)
    {
        // Padrão mais flexível: ``` linguagem ... ``` com suporte a múltiplas linhas e diferentes quebras de linha
        string pattern = @"```\s*([a-zA-Z0-9#]*?)\s*[\r\n]+(.*?)[\r\n]+```";
        
        int contador = 0;
        return Regex.Replace(corpo, pattern, match =>
        {
            string linguagem = !string.IsNullOrEmpty(match.Groups[1].Value) ? match.Groups[1].Value.Trim() : "plaintext";
            string codigo = match.Groups[2].Value;

            // Escapar HTML especiais
            codigo = System.Net.WebUtility.HtmlEncode(codigo);

            string bloco = $"<pre><code class=\"language-{linguagem}\">{codigo}</code></pre>";
            string placeholder = $"|||CODIGO_PROTEGIDO_{contador}|||";
            _blocosDeCodigoProtegidos[placeholder] = bloco;
            contador++;

            return placeholder;
        }, RegexOptions.Singleline);
    }

    private static string RestaurarBlocosDeCodigoMarkdown(string corpo)
    {
        foreach (var kvp in _blocosDeCodigoProtegidos)
        {
            corpo = corpo.Replace(kvp.Key, kvp.Value);
        }
        return corpo;
    }

    private static string FormatarH2(string titulo)
    {
        string id = GerarId(titulo);
        return $"<h2 id=\"{id}\">{titulo}</h2>";
    }

    private static string GerarId(string texto)
    {
        // Remove caracteres especiais e converte para minúsculas
        string id = Regex.Replace(texto.ToLower(), @"[^\w\s-]", "");
        // Substitui espaços por hífens
        id = Regex.Replace(id, @"\s+", "-");
        // Remove hífens duplicados
        id = Regex.Replace(id, @"-+", "-");
        return id.Trim('-');
    }
}
