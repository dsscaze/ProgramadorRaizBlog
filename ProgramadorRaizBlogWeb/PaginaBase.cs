using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ProgramadorRaizBlogWeb
{
    public class PaginaBase : System.Web.UI.Page
    {
        public string nomeUsuarioTabNews = ConfigurationManager.AppSettings["nomeUsuarioTabNews"];
        public string session_id = ConfigurationManager.AppSettings["session_id"];

        protected string formatarCorpoPost(string corpo)
        {
            string corpoFormatado = string.Empty;

            corpo = corpo.Replace("\n```c#", "<pre class=\"prettyprint\">")
                         .Replace("\n```", "</pre>");

            corpo = corpo.Replace("-------------------------------------------------------------------------------------------", "<hr />");

            Regex urlRx = new Regex(@"(?<url>(http[s]?:[/][/]|www.)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])*)", RegexOptions.IgnoreCase);
            MatchCollection matches = urlRx.Matches(corpo);
            foreach (Match match in matches)
            {
                var url = match.Groups["url"].Value;
                corpo = corpo.Replace(url, string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", url));
            }

            string[] paragrafos = corpo.Split(
                    new string[] { "\n\n" },
                    StringSplitOptions.None
                );

            foreach (var conteudo in paragrafos)
            {
                corpoFormatado += "<p>" + conteudo + "</p>";
            }

            return corpoFormatado;
        }
    }
}