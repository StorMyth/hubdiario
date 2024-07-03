using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.UI;
using System.Web;
using System.Collections.Generic;

namespace hubdiario.Pages.Edit
{
    public partial class CriarTema : System.Web.UI.Page
    {
        // Conexão com a base de dados
        string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

        // Variável para armazenar o ID do tema atual
        private int themeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verifica se a página está a ser carregada pela primeira vez ou se é um postback
            if (!IsPostBack)
            {
                // Verifica o email foi enviado da autenticação
                if (Request.QueryString["email"] != null)
                {
                    // Decodifica o email e armazena na sessão
                    string email = HttpUtility.UrlDecode(Request.QueryString["email"]);
                    Session["EmailUser"] = email;
                    Session["themeId"] = 0;
                    ViewState["CategoriesCount"] = 1;
                    // Inicialmente carrega a primeira categoria
                    LoadCategories(1);
                }
                else
                {
                    Response.Redirect("~/Default.aspx");    // Redireciona para o login
                }
            }
            else
            {
                // Recarrega as categorias no postback
                if (Session["themeId"] != null && int.TryParse(Session["themeId"].ToString(), out themeId))
                {
                    int categoriesCount = (int)ViewState["CategoriesCount"];
                    LoadCategories(categoriesCount);
                }
            }
        }

        // Método para carregar as categorias dinamicamente
        private void LoadCategories(int count)
        {
            categoriesPlaceHolder.Controls.Clear();

            // Recupera os valores atuais das categorias do ViewState
            var categoryValues = ViewState["CategoryValues"] as Dictionary<int, string> ?? new Dictionary<int, string>();

            for (int i = 1; i <= count; i++)
            {
                // Caixa do nome da Categoria
                TextBox txtCategoryName = new TextBox
                {
                    ID = $"txtCategory_{i}",
                    Width = Unit.Pixel(200),
                    CssClass = "form-control",
                    Text = categoryValues.ContainsKey(i) ? categoryValues[i] : string.Empty // Preserva o valor se já estiver no ViewState
                };

                // Botão para apagar a Categoria
                Button btnDeleteCategory = new Button
                {
                    ID = $"btnDelete_{i}",
                    Text = "Remover",
                    CssClass = "btn btn-sm btn-danger ms-2",
                    CommandArgument = i.ToString(),
                    Visible = i != 1 // Desativa o botão remover para a primeira categoria
                };
                btnDeleteCategory.Click += new EventHandler(DeleteCategory_Click);

                // Mostra a categoria na página
                categoriesPlaceHolder.Controls.Add(new LiteralControl("<div class='d-flex align-items-center p-1'>"));
                categoriesPlaceHolder.Controls.Add(new LiteralControl($"<label class='me-2 fw-bolder fontEncode'>Categoria #{i}:</label>"));
                categoriesPlaceHolder.Controls.Add(txtCategoryName);
                categoriesPlaceHolder.Controls.Add(new LiteralControl(" "));
                categoriesPlaceHolder.Controls.Add(btnDeleteCategory);
                categoriesPlaceHolder.Controls.Add(new LiteralControl("</div>"));
            }
        }

        // Método para o botão gravar o novo tema e as suas categorias
        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Esconde a mensagem
            string newThemeName = txtThemeName.Text.Trim();

            // Verifica se o nome do tema está vazio
            if (string.IsNullOrEmpty(newThemeName))
            {
                lblMessage.Text = "O nome do tema não pode estar vazio.";
                lblMessage.CssClass = "alert alert-danger text-center";
                lblMessage.Visible = true;
                return;
            }

            // Verifica se todas as categorias têm o nome preenchido
            foreach (Control control in categoriesPlaceHolder.Controls)
            {
                if (control is TextBox txtCategoryName)
                {
                    if (string.IsNullOrEmpty(txtCategoryName.Text.Trim()))
                    {
                        lblMessage.Text = "Os nomes das categorias devem ser preenchidos.";
                        lblMessage.CssClass = "alert alert-danger text-center";
                        lblMessage.Visible = true;
                        return;
                    }
                }
            }

            string email = Session["EmailUser"].ToString();

            try
            {
                int userId = GetUserIdByEmail(email);

                // Verifica se tem o ID do utilizador
                if (userId <= 0)
                {
                    lblMessage.Text = "Utilizador não foi encontrado.";
                    lblMessage.CssClass = "alert alert-danger text-center";
                    lblMessage.Visible = true;
                    return;
                }

                // Grava o Tema
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa o comando SQL para atualizar na base de dados
                    SqlCommand cmd = new SqlCommand { CommandType = CommandType.StoredProcedure, Connection = con };
                    cmd.CommandText = "p_InsertTheme";
                    cmd.Parameters.AddWithValue("@NameTheme", newThemeName);
                    cmd.Parameters.AddWithValue("@idUser", userId);

                    SqlParameter idThemeParam = new SqlParameter
                    {
                        ParameterName = "@idTheme",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idThemeParam);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    int themeId = (int)idThemeParam.Value;
                    Session["themeId"] = themeId;

                    // Grava as categorias
                    foreach (Control control in categoriesPlaceHolder.Controls)
                    {
                        if (control is TextBox txtCategoryName)
                        {
                            string categoryName = txtCategoryName.Text.Trim();

                            if (!string.IsNullOrEmpty(categoryName))
                            {
                                // Executa comando SQL para atualizar na base de dados
                                cmd.CommandText = "p_InsertCategory";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@idTheme", themeId);
                                cmd.Parameters.AddWithValue("@NameCategory", categoryName);
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                    }

                    // Envia mensagem para a página
                    lblMessage.Text = "Tema e categorias criados com sucesso.";
                    lblMessage.CssClass = "alert alert-sucess text-center";
                    lblMessage.Visible = true;

                    // Redireciona para Temas.aspx após gravar
                    Response.Redirect("~/Pages/View/Temas.aspx");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Erro: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger text-center";
                lblMessage.Visible = true;
            }
        }

        // Método para obter o ID do utilizador pelo email
        private int GetUserIdByEmail(string email)
        {
            int userId = 0;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Executa comando SQL para verificar a base de dados
                SqlCommand cmd = new SqlCommand { CommandText = "p_GetUserIdByEmail", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.AddWithValue("@EmailUser", email);
                SqlParameter userIdParam = new SqlParameter
                {
                    ParameterName = "@UserId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(userIdParam);
                con.Open();
                cmd.ExecuteNonQuery();

                // Atribui o valor do parâmetro de saída ao userId
                userId = (int)userIdParam.Value;

                con.Close();
            }
            // Retorna o valor do ID do utilizador
            return userId;
        }

        // Método para o botão adicionar uma nova categoria
        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Esconde a mensagem

            // Grava os valores atuais das categorias no ViewState
            var categoryValues = new Dictionary<int, string>();
            foreach (Control control in categoriesPlaceHolder.Controls)
            {
                if (control is TextBox txtCategoryName)
                {
                    int index = int.Parse(txtCategoryName.ID.Split('_')[1]);
                    categoryValues[index] = txtCategoryName.Text.Trim();
                }
            }
            ViewState["CategoryValues"] = categoryValues;

            // Adiciona uma nova categoria
            int itemCount = (int)ViewState["CategoriesCount"] + 1;
            ViewState["CategoriesCount"] = itemCount;

            // Recarrega as categorias
            LoadCategories(itemCount);
        }

        // Método para botão remover uma categoria
        protected void DeleteCategory_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Esconde a mensagem

            Button btn = (Button)sender;
            int itemCount = int.Parse(btn.CommandArgument);

            // Remove os controlos associados à categoria
            categoriesPlaceHolder.Controls.RemoveAt((itemCount - 1) * 6);
            categoriesPlaceHolder.Controls.RemoveAt((itemCount - 1) * 6);
            categoriesPlaceHolder.Controls.RemoveAt((itemCount - 1) * 6);
            categoriesPlaceHolder.Controls.RemoveAt((itemCount - 1) * 6);
            categoriesPlaceHolder.Controls.RemoveAt((itemCount - 1) * 6);

            int count = (int)ViewState["CategoriesCount"] - 1;
            ViewState["CategoriesCount"] = count;

            // Recarrega as categorias
            LoadCategories(count);
        }

        // Método para o botão de voltar para o menu anterior
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/View/Temas.aspx");
        }
    }
}
