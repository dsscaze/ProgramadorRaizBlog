using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TabNewsCSharpSDK;
using TabNewsCSharpSDK.Entities;

namespace ProgramadorRaizBlogWeb
{
    public partial class post : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["id"] == null)
            {
                h1Titulo.InnerHtml = "Nenhum post selecionado";
            }
            else
            {
                carregarPost();
            }

        }

        private void carregarPost()
        {
            string slug = Request["id"];
            TabNewsContent conteudo = TabNewsApi.GetContent("programadorraiz", slug);

            h1Titulo.InnerHtml = conteudo.title;
            spanDataCriacao.InnerHtml = conteudo.created_at.AddHours(-3).ToString();
            divCorpo.InnerHtml = formatarCorpoPost(conteudo.body);

        }

        private string formatarCorpoPost(string corpo)
        {
            string corpoFormatado = string.Empty;

            corpo = corpo.Replace("\n```c#", "<pre class=\"prettyprint\">")
                         .Replace("\n```", "</pre>");

            corpo = corpo.Replace("-------------------------------------------------------------------------------------------", "<hr />");

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