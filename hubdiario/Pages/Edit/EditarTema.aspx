<%@ Page Title="Temas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditarTema.aspx.cs" Inherits="hubdiario.Pages.Edit.EditarTema" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <div>
            <!-- Título da página -->
            <h2 class="text-center text-muted mt-5">Editar Tema</h2>
           
            <!-- Divisória do Título -->
            <div class="p-1 mb-3">
                <hr class="hr my-0 offset-3 w-50 mb-4" />
            </div>
            
            <!-- Alertas -->
            <div class="d-flex justify-content-center">
                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center" Visible="false" />
            </div>

            <!-- Modificar nome do Tema -->
            <div class="container w-50 mx-auto bg-light p-3 mt-3 border rounded">
                <div class="d-flex justify-content-center align-items-center">
                    <div class="row g-3 align-items-center">
                        <div class="col-auto">
                            <label class="fw-bolder fontEncode">Nome do Tema: &nbsp;</label>
                        </div>
                        <div class="col-auto">
                            <asp:TextBox ID="txtThemeName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Lista de Categorias -->
            <div class="container w-50 mx-auto bg-light p-3 mt-3 border rounded">
                <div class="d-flex justify-content-center align-items-center flex-column">
                    <h4 class="mt-2 fontOrbitron">Categorias:</h4>
                    <asp:PlaceHolder ID="categoriesPlaceHolder" runat="server"></asp:PlaceHolder>

                    <!-- Botão Nova Categoria -->
                    <div class="d-flex align-items-center p-1">
                        <label class="me-2 fw-bolder fontEncode">Nova Categoria:</label>
                        <asp:TextBox ID="txtNewCategory" runat="server" CssClass="form-control fieldCatsize"></asp:TextBox>
                        <asp:Button ID="btnAddCategory" runat="server" Text="Adicionar" CssClass="btn btn-sm btn-success ms-2" OnClick="btnAddCategory_Click" />
                    </div>
                </div>
            </div>

            <!-- Botões Gravar e Voltar -->
            <div class="mb-4 text-end me-md-5 mt-3">
                <asp:Button ID="btnBack" runat="server" Text="Voltar" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
                <asp:Button ID="btnSave" runat="server" Text="Gravar Alterações" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            </div>                
        </div>
    </main>
</asp:Content>
