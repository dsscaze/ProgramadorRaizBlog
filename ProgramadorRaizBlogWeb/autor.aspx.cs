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
    public partial class autor : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            carregarDadosAutor();
        }

        private void carregarDadosAutor()
        {
            string slug = Request["id"];
            TabNewsUser usuarioAutor = TabNewsApi.GetUser(session_id);

            divDescricao.InnerHtml = formatarCorpoPost(usuarioAutor.description);
        }
    }
}