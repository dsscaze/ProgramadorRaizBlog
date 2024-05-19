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
    public partial class post : PaginaBase
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
            TabNewsContent conteudo = TabNewsApi.GetContent(nomeUsuarioTabNews, slug);

            h1Titulo.InnerHtml = conteudo.title;
            spanDataCriacao.InnerHtml = conteudo.created_at.AddHours(-3).ToString();
            divCorpo.InnerHtml = formatarCorpoPost(conteudo.body);

        }
    }
}