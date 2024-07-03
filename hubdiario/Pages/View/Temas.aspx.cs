using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace hubdiario.Pages.View
{
    public partial class Temas : System.Web.UI.Page
    {
        // Conexão com a base de dados
        string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verifica se o utilizador está autenticado
                if (Session["EmailUser"] != null)
                {
                    string email = Session["EmailUser"].ToString();
                    DisplayUserThemes(email);   // Mostra os Temas do utilizador autenticado

                    // Exibe mensagem de erro, da tentativa de apagar tema
                    if (Request.QueryString["error"] != null)
                    {
                        lblMessage.Text = HttpUtility.UrlDecode(Request.QueryString["error"]);
                        lblMessage.Visible = true;
                    }
                    // Exibe mensagem de sucesso, ao apagar tema
                    else if (Request.QueryString["message"] != null)
                    {
                        lblSuccess.Text = HttpUtility.UrlDecode(Request.QueryString["message"]);
                        lblSuccess.Visible = true;
                    }
                }
                else
                {
                    Response.Redirect("~/Default.aspx"); // Redireciona para login se não autenticado
                }
            }
            else
            {
                // Recarrega a lista de temas se for um postback
                if (Session["EmailUser"] != null)
                {
                    string email = Session["EmailUser"].ToString();
                    DisplayUserThemes(email);
                }
            }
        }
        
        // Método para mostrar os Temas do utilizador
        private void DisplayUserThemes(string email)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Executa comando SQL para verificar a base de dados
                SqlCommand cmd = new SqlCommand { CommandText = "p_GetUserThemes", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.Add(new SqlParameter("@EmailUser", email));
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    // Cria uma tabela
                    Table themesTable = new Table { CssClass = "table table-hover" };

                    // Cria o thead para os títulos das colunas
                    TableHeaderRow headerRow = new TableHeaderRow();
                    headerRow.TableSection = TableRowSection.TableHeader;

                    // Nomes dos títulos das colunas
                    headerRow.Cells.Add(new TableHeaderCell { Text = "Temas", CssClass = "bg-secondary bg-gradient text-center text-white" });
                    headerRow.Cells.Add(new TableHeaderCell { Text = "Ações", CssClass = "bg-secondary bg-gradient text-center text-white" });

                    themesTable.Rows.Add(headerRow);

                    while (reader.Read())
                    {
                        int themeId = reader.GetInt32(reader.GetOrdinal("idTheme"));
                        string themeName = reader.GetString(reader.GetOrdinal("NameTheme"));

                        // Cria uma linha na tabela para o tema
                        TableRow row = new TableRow();

                        // Cria um link para o nome do tema
                        TableCell themeNameCell = new TableCell { CssClass = "text-center" };
                        HyperLink themeNameLink = new HyperLink
                        {
                            Text = themeName,
                            NavigateUrl = $"VisualizarTema.aspx?id={themeId}",
                            CssClass = "fs-3 text-decoration-none text-body fontEncode"
                        };
                        themeNameCell.Controls.Add(themeNameLink);

                        // Cria botão de editar o tema
                        TableCell actionsCell = new TableCell { CssClass = "actions-cell" };
                        Button editButton = new Button
                        {
                            Text = "Editar",
                            PostBackUrl = $"~/Pages/Edit/EditarTema.aspx?id={themeId}",
                            CssClass = "btn btn-primary btn-sm"
                        };

                        // Cria botão de apagar o tema
                        LinkButton deleteButton = new LinkButton
                        {
                            Text = "Apagar",
                            CssClass = "btn btn-danger btn-sm",
                            CommandArgument = themeId.ToString()
                        };
                        deleteButton.Click += DeleteButton_Click;

                        // Adiciona os botões à célula de ações
                        actionsCell.Controls.Add(editButton);
                        actionsCell.Controls.Add(new Literal { Text = " " });
                        actionsCell.Controls.Add(deleteButton);

                        // Adiciona as células à linha
                        row.Cells.Add(themeNameCell);
                        row.Cells.Add(actionsCell);

                        // Adiciona a linha à tabela
                        themesTable.Rows.Add(row);
                    }

                    // Adiciona a tabela ao PlaceHolder
                    themesPlaceHolder.Controls.Add(themesTable);
                }
                else
                {
                    // Envia mensagem caso não existem temas criados
                    themesPlaceHolder.Controls.Add(new Literal { Text = "<div class='alert alert-warning text-center'>Nenhum tema encontrado.</div>" });
                }

                con.Close();
            }
        }

        // Método para o botão de apagar o tema
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // Obtem o ID do tema a partir do CommandArgument
            int themeId = int.Parse((sender as LinkButton).CommandArgument);
            hfThemeIdToDelete.Value = themeId.ToString();
            mpeConfirmDelete.Show();
        }

        // Método para a confirmação que pretende apagar o tema
        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            int themeId = int.Parse(hfThemeIdToDelete.Value);
            try
            {
                string themeName = GetThemeName(themeId); // Obtem o nome do tema antes de apagar
                DeleteTheme(themeId);   // Chamar o método para apagar o tema
                Response.Redirect("~/Pages/View/Temas.aspx?message=" + HttpUtility.UrlEncode($"O Tema '{themeName}' foi apagado com sucesso."), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Response.Redirect("~/Pages/View/Temas.aspx?error=" + HttpUtility.UrlEncode("Falha ao apagar o tema: " + ex.Message), false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        // Método para cancelar a eliminação do tema
        protected void btnCancelDelete_Click(object sender, EventArgs e)
        {
            mpeConfirmDelete.Hide();
        }

        // Método para apagar o Tema selecionado
        private void DeleteTheme(int themeId)
        {
            lblMessage.Visible = false; // Esconde a mensagem
            lblSuccess.Visible = false; // Esconde a mensagem

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa comando SQL para atualizar a base de dados
                    SqlCommand cmd = new SqlCommand("p_DeleteThemeById", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@idTheme", themeId));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        // Método para obter o nome do Tema pelo ID
        private string GetThemeName(int themeId)
        {
            string themeName = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa comando SQL para verificar na base de dados
                    SqlCommand cmd = new SqlCommand("p_GetThemeNameById", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ThemeId", themeId));
                    con.Open();
                    // Armazena o nome do Tema
                    themeName = cmd.ExecuteScalar()?.ToString();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
                lblMessage.Visible = true;
            }
            return themeName;
        }

        // Método para o botão de criar um novo Tema
        protected void btnCreateTheme_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Esconde a mensagem
            lblSuccess.Visible = false; // Esconde a mensagem

            // Verifica se a sessão de EmailUser está definida
            if (Session["EmailUser"] != null)
            {
                // Obtém o email do utilizador da sessão
                string email = Session["EmailUser"].ToString();

                // Redireciona para a página de criação de tema com o parâmetro userId na query string
                Response.Redirect("~/Pages/Edit/CriarTema.aspx?email=" + HttpUtility.UrlEncode(email));
            }
            else
            {
                // Se a sessão de EmailUser não estiver definida, redireciona para a página de login
                Response.Redirect("~/Default.aspx");
            }
        }
    }
}