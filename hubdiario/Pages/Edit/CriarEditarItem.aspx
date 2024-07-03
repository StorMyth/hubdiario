<%@ Page Title="Criar e Editar Item" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CriarEditarItem.aspx.cs" Inherits="hubdiario.Pages.Edit.CriarEditarItem" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <div class="container gx-sm-1">

            <!-- Título Nome da Categoria -->
            <div class="text-center mt-5 offset-3 w-50">
                <asp:Literal runat="server" ID="litCategoryName"></asp:Literal>
            </div>

            <!-- Alertas -->
            <div class="d-flex justify-content-center">
                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center" Visible="false" />
            </div>

            <!-- Tabela dos Itens -->
            <div>
                <asp:Repeater runat="server" ID="rptItems">
                    <HeaderTemplate>
                        <table class="table">
                            <tr>
                                <!-- Títulos das Colunas de Itens -->
                                <th class="bg-secondary text-white bg-gradient text-center fw-bolder">Texto</th>
                                <th class="bg-secondary text-white bg-gradient text-center fw-bolder">Quantidade</th>
                                <th class="bg-secondary text-white bg-gradient text-center fw-bolder">Data</th>
                                <th class="bg-secondary text-white bg-gradient text-center fw-bolder">Preço</th>
                                <th class="bg-secondary text-white bg-gradient text-center fw-bolder">Ação</th> 
                            </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <!-- Campos para inserir os valores -->
                                <tr>
                                    <td>
                                        <!-- HiddenField para o Id do item -->
                                        <asp:HiddenField runat="server" ID="hfItemId" Value='<%# Eval("idItem") %>' />
                                        <!-- Texto input -->
                                        <asp:TextBox runat="server" ID="txtItem" CssClass="form-control text-center form-control-sm-text" Text='<%# Eval("TextItem") %>'></asp:TextBox>
                                    </td>
                                    <!-- Quantidade input -->
                                    <td><asp:TextBox runat="server" ID="txtQuantity" CssClass="form-control text-center form-control-sm-text" Text='<%# Eval("Quantity") %>'></asp:TextBox></td>
                                    <!-- Data input -->
                                    <td><asp:TextBox runat="server" ID="txtDate" CssClass="form-control text-center form-control-sm-text" Text='<%# Bind("DateItem", "{0:dd/MM/yyyy}") %>'></asp:TextBox></td>
                                    <!-- Preço input -->
                                    <td><asp:TextBox runat="server" ID="txtPrice" CssClass="form-control text-center form-control-sm-text" Text='<%# Eval("Price") %>'></asp:TextBox></td>
                                    <td>
                                        <!-- Botão para remover item -->
                                        <asp:Button ID="btnDeleteItem" runat="server" Text="Remover" CssClass="btn btn-danger btn-sm ms-4 ms-md-0 ms-sm-0" CommandArgument='<%# Eval("idItem") %>' OnClick="btnDeleteItem_Click" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
           
            <!-- Botão Adicionar Item -->
            <div class="mb-4 text-end me-md-5">
                <asp:Button runat="server" ID="btnAddItem" Text="Adicionar Item" CssClass="btn btn-success btn-sm" OnClick="btnAddItem_Click" />
            </div>

            <hr />
            <!-- Secção do Alerta -->
            <div class="row justify-content-center">
                <div class="col-auto">
                    <!-- Título -->
                    <div class="text-start mb-3">
                        <h3 class="fontEncode">Alerta:</h3>
                    </div>
                    <!-- Tabela do Alerta -->
                    <asp:Panel runat="server" ID="pnlAlert">
                        <table class="table w-auto">
                            <tr>
                                <!-- Títulos das Colunas de Alerta -->
                                <th class="bg-secondary bg-gradient text-center fw-bolder text-white">Data do Alerta</th>
                                <th class="bg-secondary bg-gradient text-center fw-bolder text-white">Repetições</th>
                                <th class="bg-secondary bg-gradient text-center fw-bolder text-white">Tipo</th>
                                <th class="bg-secondary bg-gradient text-center fw-bolder text-white">Ativo</th>
                            </tr>
                            <tr>
                                <td>
                                    <!-- HiddenField para o id do alerta -->
                                    <asp:HiddenField runat="server" ID="hfAlertId" />
                                    <!-- HiddenField para o id do item -->
                                    <asp:HiddenField runat="server" ID="hfItemId" />
                                    <!-- Data input -->
                                    <asp:TextBox runat="server" ID="txtAlertTime" CssClass="form-control text-center fieldDate form-control-sm-text" TextMode="DateTimeLocal"></asp:TextBox>
                                </td>
                                <!-- Nº de repetições input -->
                                <td><asp:TextBox runat="server" CssClass="form-control text-center" ID="txtRepeatInterval"></asp:TextBox></td>
                                <td>
                                    <!-- Tipo de repetição input -->
                                    <asp:DropDownList runat="server" CssClass="form-select" ID="ddlRepeatUnit">
                                        <asp:ListItem Text="Hora" Value="Hora"></asp:ListItem>
                                        <asp:ListItem Text="Diário" Value="Diário"></asp:ListItem>
                                        <asp:ListItem Text="Semanal" Value="Semanal"></asp:ListItem>
                                        <asp:ListItem Text="Mensal" Value="Mensal"></asp:ListItem>
                                        <asp:ListItem Text="Anual" Value="Anual"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <!-- Ativação do alerta -->
                                <td><asp:CheckBox runat="server" CssClass="formcheck text-center" ID="chkActive" /></td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
            </div>
            
            <!-- Botões para Gravar e Voltar -->
            <div class="mb-4 text-end me-md-5 mt-4">
                <asp:Button ID="btnBack" runat="server" Text="Voltar" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
                <asp:Button runat="server" ID="btnSaveAlert" Text="Gravar Alerta" CssClass="btn bg-warning text-white" OnClick="btnSaveAlert_Click" />
                <asp:Button runat="server" ID="btnSave" Text="Gravar Alterações" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            </div>
        </div>
    </main>
</asp:Content>
