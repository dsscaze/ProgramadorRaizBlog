<%@ Page Title="" Language="C#" MasterPageFile="~/BlogPadrao.Master" AutoEventWireup="true" CodeBehind="post.aspx.cs" Inherits="ProgramadorRaizBlogWeb.post" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <!-- Page Header-->
        <header class="masthead" style="background-image: url('assets/img/post-bg.jpg')">
            <div class="container position-relative px-4 px-lg-5">
                <div class="row gx-4 gx-lg-5 justify-content-center">
                    <div class="col-md-10 col-lg-8 col-xl-7">
                        <div class="post-heading">
                            <h1 runat="server" id="h1Titulo"></h1>
                            <span class="meta">
                                criador por
                                <a href="autor.aspx">programador raiz</a>
                                em <span runat="server" id="spanDataCriacao"></span>
                            </span>
                        </div>
                    </div>
                </div>
            </div>
        </header>
        <!-- Post Content-->
        <article class="mb-4">
            <div class="container px-4 px-lg-5">
                <div class="row gx-4 gx-lg-5 justify-content-center">
                    <div class="col-md-10 col-lg-8 col-xl-7" runat="server" id="divCorpo">
                        
                       <%-- <p>
                            Placeholder text by
                            <a href="http://spaceipsum.com/">Space Ipsum</a>
                            &middot; Images by
                            <a href="https://www.flickr.com/photos/nasacommons/">NASA on The Commons</a>
                        </p>--%>
                    </div>
                </div>
            </div>
        </article>
        
</asp:Content>
