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
    public partial class _default : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            listarPosts();
        }

        private void listarPosts()
        {
            List<TabNewsContent> posts = TabNewsApi.Get10LastedPosts(nomeUsuarioTabNews, 10, 1);

            rptPosts.DataSource = posts;
            rptPosts.DataBind();
        }
    }
}