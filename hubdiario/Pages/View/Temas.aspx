<%@ Page Title="Temas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Temas.aspx.cs" Inherits="hubdiario.Pages.View.Temas" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">

        <!-- Titulo página -->
        <h2 id="title" class="text-center text-muted mt-5">Menu inicial</h2>

        <!-- Divisória do Título -->
        <div class="p-1 mb-3">
            <hr class="hr my-0 offset-3 w-50 mb-4" />
        </div>

        <div class="container w-75 w-md-100">
            <!-- Alertas -->
            <div class="d-flex justify-content-center">
                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center" Visible="false" />
                <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center" Visible="false" />
            </div>

            <!-- Tabela dos Temas -->
            <div class="mb-4">
                <asp:PlaceHolder ID="themesPlaceHolder" runat="server" />
            </div>
        </div>

        <!-- Botão Criar Tema -->
        <div class="mb-4 text-end me-md-5">
            <asp:Button ID="btnCreate_Theme" runat="server" Text="Criar Tema" CssClass="btn btn-success" OnClick="btnCreateTheme_Click" />
        </div>

        <!-- HiddenField para armazenar o ID do tema a ser excluído -->
        <asp:HiddenField ID="hfThemeIdToDelete" runat="server" />

        <!-- Painel de confirmação -->
        <asp:Panel ID="pnlConfirmDelete" runat="server" CssClass="modal-popup" Style="display:none;">
            <div class="modal-content">
                <h4>Confirmação</h4>
                <p>Tem certeza que deseja apagar este tema?</p>
                <asp:Button ID="btnConfirmDelete" runat="server" Text="Sim" OnClick="btnConfirmDelete_Click" CssClass="modal-button btn btn-danger" />
                <asp:Button ID="btnCancelDelete" runat="server" Text="Não" OnClick="btnCancelDelete_Click" CssClass="modal-button btn btn-secondary" />
            </div>
        </asp:Panel>

        <!-- ModalPopupExtender para confirmar a remoção do tema -->
        <ajaxToolkit:ModalPopupExtender ID="mpeConfirmDelete" runat="server" TargetControlID="hfThemeIdToDelete"
            PopupControlID="pnlConfirmDelete" BackgroundCssClass="modal-background" />

    </main>
</asp:Content>
