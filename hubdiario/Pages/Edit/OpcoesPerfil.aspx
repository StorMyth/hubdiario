<%@ Page Title="Opções do Perfil" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OpcoesPerfil.aspx.cs" Inherits="hubdiario.Pages.Edit.OpcoesPerfil" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">

        <!-- Titulo página -->
        <h2 id="title" class="text-center text-muted mt-5">Opções do Perfil</h2>

        <!-- Divisória do Título -->
        <div class="p-1">
            <hr class="hr my-0 offset-3 w-50 mb-4" />
        </div>

        <!-- Alertas -->
        <div class="d-flex justify-content-center">
            <asp:Label ID="lblPasswordMessage" runat="server" CssClass="alert alert-danger text-center" Visible="false" />
            <asp:Label ID="lblEmailMessage" runat="server" CssClass="alert alert-danger text-center" Visible="false" />
            <asp:Label ID="lblDeleteAccountMessage" runat="server" CssClass="alert alert-danger text-center" Visible="false" />
        </div>

        <div>
            <div class="container">
                <div class="d-flex justify-content-center align-items-center flex-column">
                    <!-- Título Email -->
                    <h5 class="fontEncode">Alterar Email</h5>
                    <!-- Email input -->
                    <asp:TextBox ID="txtNewEmail" runat="server" CssClass="form-control" placeholder="Novo Email" />
                    <!-- Botão atualizar Email -->
                    <asp:Button ID="btnUpdateEmail" runat="server" Text="Atualizar Email" CssClass="btn btn-primary mt-2" OnClick="btnUpdateEmail_Click" />

                    <!-- Título Palavra-passe -->
                    <h5 class="mt-5 fontEncode">Alterar Palavra-Passe</h5>
                    <!-- PW atual input -->
                    <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Palavra-passe Atual" />
                    <!-- Nova PW input -->
                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control mt-2" TextMode="Password" placeholder="Nova Palavra-passe" />
                    <!-- Confirmar PW input -->
                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control mt-2" TextMode="Password" placeholder="Confirmar Nova Palavra-passe" />
                    <!-- Botão atualizar PW -->
                    <asp:Button ID="btnUpdatePassword" runat="server" Text="Atualizar Palavra-Passe" CssClass="btn btn-primary mt-2" OnClick="btnUpdatePassword_Click" />
                </div>
            </div>

            <!-- Botões Apagar Conta e Voltar -->
            <div class="mb-4 text-end me-md-5 mt-3">
                <asp:Button ID="btnDeleteAccount" runat="server" Text="Eliminar Conta" CssClass="btn btn-danger" OnClick="btnDeleteAccount_Click" />
                <asp:Button ID="btnBack" runat="server" Text="Voltar" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
            </div>

            <!-- HiddenField para armazenar o ID do utilizador -->
            <asp:HiddenField ID="hfUserIdToDelete" runat="server" />

            <!-- Painel de confirmação -->
            <asp:Panel ID="pnlConfirmDelete" runat="server" CssClass="modal-popup" Style="display:none;">
                <div class="modal-content">
                    <h4>Confirmação</h4>
                    <p>Tem certeza que deseja apagar a sua conta?</p>
                    <asp:Button ID="btnConfirmDelete" runat="server" Text="Sim" OnClick="btnConfirmDelete_Click" CssClass="modal-button btn btn-danger" />
                    <asp:Button ID="btnCancelDelete" runat="server" Text="Não" OnClick="btnCancelDelete_Click" CssClass="modal-button btn btn-secondary" />
                </div>
            </asp:Panel>

            <!-- ModalPopupExtender para confirmar a eliminação da conta -->
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmDelete" runat="server" TargetControlID="hfUserIdToDelete"
                PopupControlID="pnlConfirmDelete" BackgroundCssClass="modal-background" />
        </div>
    </main>
</asp:Content>
