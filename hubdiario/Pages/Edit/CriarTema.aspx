<%@ Page Title="Novo Tema" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CriarTema.aspx.cs" Inherits="hubdiario.Pages.Edit.CriarTema" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <div>
            <!-- Título da página -->
            <h2 class="text-center text-muted">Criar Novo Tema</h2>

            <!-- Divisória do Título -->
            <div class="p-1">
                <hr class="hr my-0 offset-3 w-50 mb-4" />
            </div>

            <!-- Alertas -->
            <div class="d-flex justify-content-center">
                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center" Visible="false" />
            </div>

            <!-- Inserir nome do Tema -->
            <div class="container w-50 mx-auto bg-light p-3 border rounded">
                <div class="d-flex justify-content-center align-items-center">
                    <div class="row g-3 align-items-center">
                        <div class="col-auto">
                            <label class="fw-bolder fontEncode">Nome do Tema: &nbsp;</label>
                        </div>
                        <div class="col-auto">
                            <asp:TextBox ID="txtThemeName" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvThemeName" runat="server" ControlToValidate="txtThemeName"
                                ErrorMessage="&nbsp;O nome do tema é obrigatório." Display="Dynamic" CssClass="text-danger align-content-center" />                
                        </div>
                    </div>
                </div>
            </div>

            <!-- Lista de Categorias -->
            <div class="container w-50 mx-auto bg-light p-3 mt-3 border rounded">
                <div class="d-flex justify-content-center align-items-center flex-column">
                    <h4 class="mt-2 fontOrbitron">Categorias:</h4>
                    <div id="categoriesPlaceHolder" runat="server">
                        <!-- Categorias serão adicionadas dinamicamente aqui -->
                    </div>
                </div>

                <!-- Botão adicionar categoria -->
                <div class="d-flex justify-content-end mt-3">
                    <asp:Button ID="btnAddCategory" runat="server" Text="Adicionar Categoria" CssClass="btn btn-success btn-sm" OnClick="btnAddCategory_Click" />
                </div>
            </div>

            <!-- Botões Gravar e Voltar -->
            <div class="mb-4 text-end me-md-5 mt-4">
                <asp:Button ID="btnBack" runat="server" Text="Voltar" CssClass="btn btn-secondary" OnClick="btnBack_Click" CausesValidation="false" />
                <asp:Button ID="btnSave" runat="server" Text="Gravar" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            </div>

        </div>
    </main>
</asp:Content>
