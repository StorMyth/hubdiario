<%@ Page Title="Detalhes do Tema" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VisualizarTema.aspx.cs" Inherits="hubdiario.Pages.View.VisualizarTema" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">

        <!-- Referência ao script externo -->
        <asp:PlaceHolder runat="server">
            <script src="<%= ResolveUrl("~/Scripts/custom-script.js") %>" type="text/javascript"></script>
        </asp:PlaceHolder>

        <div class="container gx-sm-1">
            
            <!-- Título Nome Tema-->
            <div class="text-center text-success mt-5">
                <asp:Label ID="lblThemeName" runat="server" CssClass="fs-1 fw-bolder fontOrbitron"></asp:Label>
            </div>

            <!-- Divisória -->
            <div class="p-1">
                <hr class="hr my-0 offset-3 w-50" />
            </div>

            <!-- Alertas -->
            <div class="p-1">
                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" />
            </div>

            <!-- Nomes de todas as Categorias do Tema -->
            <div class="mt-4 d-flex justify-content-center">
                <div class="categories-container fontEncode" id="categoriesPanel" runat="server"></div>
            </div>
            
            <!-- Categoria Selecionada -->
            <div class="hr my-0 offset-3 w-50">
                <asp:Label ID="lblSelectedCategory" runat="server" CssClass="form-control text-center fs-4 bg-primary bg-gradient text-white fontEncode"></asp:Label>
            </div>

            <!-- Tabela que lista os Itens -->
            <div class="container w-75 w-md-100">
                <asp:Panel ID="itemsPanel" runat="server" class="item-list mt-3"></asp:Panel>
            </div>

            <!-- Botões Criar/Editar e Voltar -->
            <div class="mb-4 text-end me-md-5 mt-4">
                <asp:Button ID="btnBack" runat="server" Text="Voltar" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
                <asp:Button ID="btnCreateEditItem" runat="server" CssClass="btn btn-success" Text="Criar/Editar" OnClick="btnCreateEditItem_Click" />
            </div>
        </div>
    </main>
</asp:Content>
