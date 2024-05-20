<%@ Page Title="" Language="C#" MasterPageFile="~/BlogPadrao.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="ProgramadorRaizBlogWeb._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Page Header-->
    <header class="masthead" style="background-image: url('assets/img/home-bg.jpg')">
        <div class="container position-relative px-4 px-lg-5">
            <div class="row gx-4 gx-lg-5 justify-content-center">
                <div class="col-md-10 col-lg-8 col-xl-7">
                    <div class="site-heading">
                        <h1>Programador Raiz</h1>
                        <span class="subheading">Um blog sobre programação</span>
                    </div>
                </div>
            </div>
        </div>
    </header>
    <!-- Main Content-->
    <div class="container px-4 px-lg-5">
        <div class="row gx-4 gx-lg-5 justify-content-center">
            <div class="col-md-10 col-lg-8 col-xl-7">
                <asp:Repeater runat="server" ID="rptPosts">
                    <ItemTemplate>
                        <!-- Post preview-->
                        <div class="post-preview">
                            <a href='<%# "post.aspx?id=" + Eval("slug") %>'>
                                <h2 class="post-title"><%# Eval("title") %></h2>
                                <%--<h3 class="post-subtitle">Problems look mighty small from 150 miles up</h3>--%>
                            </a>
                            <p class="post-meta">
                                criado por
                                <a href="autor.aspx">programador raiz</a>
                                        em <%# Convert.ToDateTime(Eval("created_at")).AddHours(-3).ToString()%>
                            </p>
                        </div>
                        <!-- Divider-->
                        <hr class="my-4" />
                    </ItemTemplate>
                </asp:Repeater>


                <!-- Pager-->
                <%--<div class="d-flex justify-content-end mb-4"><a class="btn btn-primary text-uppercase" href="#!">Ver antigos →</a></div>--%>
            </div>
        </div>
    </div>

</asp:Content>
