<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="hubdiario.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>
        <!-- Layout de login retirado de https://mdbootstrap.com/docs/standard/extended/login/ -->
        <section class="vh-100 gradient-custom">
          <div class="container py-5 h-100">
            <div class="row d-flex justify-content-center align-items-center h-100">
              <div class="col-12 col-md-8 col-lg-6 col-xl-5">
                <div class="card bg-dark text-white" style="border-radius: 1rem;">
                  <div class="card-body p-5 text-center">

                    <div class="mb-md-5 mt-md-4 pb-5">
                      <!-- Título do layout -->
                      <h2 class="fw-bold mb-2 text-uppercase">Login</h2>
                      <p class="text-white-50 mb-5">Por favor coloque o email e palavra-passe!</p>

                      <!-- Email input -->
                      <div data-mdb-input-init class="form-outline form-white mb-4 text-center">
                          <asp:TextBox type="email" runat="server" id="txtEmail" class="form-control form-control-lg mx-auto fieldPWLog"></asp:TextBox>
                          <asp:Label class="form-label" runat="server" for="txtEmail">Email address</asp:Label>
                      </div>

                      <!-- Password input -->
                      <div data-mdb-input-init class="form-outline form-white mb-4 text-center">
                          <asp:TextBox type="password" runat="server" id="txtPassword" class="form-control form-control-lg mx-auto fieldPWLog"></asp:TextBox>
                          <asp:Label class="form-label" runat="server" for="txtPassword">Password</asp:Label>
                      </div>

                      <!-- Botão Login -->
                      <asp:Button data-mdb-button-init data-mdb-ripple-init class="btn btn-outline-light btn-lg px-5" runat="server" OnClick="btnLogin_Click" type="submit" Text="Entrar"></asp:Button>
                      
                      <!-- Alertas -->
                      <div class="d-flex justify-content-center mt-4">
                          <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
                      </div>
                    </div>

                    <!-- Link para o registo da conta -->
                    <div>
                      <p class="mb-0">Não tem conta? <a href="Registo.aspx" class="text-white-50 fw-bold">Registe-se</a></p>
                    </div>

                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>
    </main>

</asp:Content>
