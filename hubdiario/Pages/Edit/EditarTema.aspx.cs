using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace hubdiario.Pages.Edit
{
    public partial class EditarTema : System.Web.UI.Page
    {
        // Conexão com a base de dados
        string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

        // Variável para armazenar o ID do tema atual
        private int themeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verifica se a página está a ser carregada pela primeira vez
            if (!IsPostBack)
            {
                // Verifica se foi indicado um ID de tema é válido
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out themeId))
                {
                    // Armazenar o ID do tema na sessão para acessos futuros
                    Session["themeId"] = themeId;

                    // Carrega os detalhes do tema e as categorias associadas
                    LoadThemeDetails(themeId);
                    LoadCategories(themeId);
                }
                else
                {
                    Response.Redirect("~/Pages/View/Temas.aspx"); // Redireciona para trás, se não houver ID válido
                }
            }
            else
            {
                // Se a página estiver em postback, recupera o ID do tema da sessão
                if (Session["themeId"] != null && int.TryParse(Session["themeId"].ToString(), out themeId))
                {
                    // Apenas recarrega as categorias associadas ao tema
                    LoadCategories(themeId);
                }
            }
        }

        // Método para carregar o nome do Tema
        private void LoadThemeDetails(int themeId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Executa comando SQL para verificar a base de dados
                SqlCommand cmd = new SqlCommand { CommandText = "p_GetThemeNameById", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.Add(new SqlParameter("@ThemeId", themeId));
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    txtThemeName.Text = reader.GetString(reader.GetOrdinal("NameTheme"));
                }
                con.Close();
            }
        }

        // Método para carregar as categorias associadas ao tema
        private void LoadCategories(int themeId)
        {
            // Limpa os controles existentes antes de recarregar
            categoriesPlaceHolder.Controls.Clear();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Executa comando SQL para verificar a base de dados
                SqlCommand cmd = new SqlCommand { CommandText = "p_GetCategoriesByTheme", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.Add(new SqlParameter("@idTheme", themeId));
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<int> categoryIds = new List<int>();
                int catcount = 1; // Faz a contagem do nº de Categorias visíveis na página
                while (reader.Read())
                {
                    int categoryId = reader.GetInt32(reader.GetOrdinal("idCategory"));
                    string categoryName = reader.GetString(reader.GetOrdinal("NameCategory"));

                    categoryIds.Add(categoryId);

                    // Cria um controlo TextBox para editar o nome da categoria
                    TextBox txtCategoryName = new TextBox
                    {
                        ID = $"txtCategory_{categoryId}",
                        Text = categoryName,
                        Width = Unit.Pixel(200),
                        CssClass = "form-control"
                    };

                    // Cria um botão para apagar a categoria
                    Button btnDeleteCategory = new Button
                    {
                        ID = $"btnDelete_{categoryId}",
                        Text = "Remover",
                        CssClass = "btn btn-sm btn-danger ms-2",
                        CommandArgument = categoryId.ToString(),
                        OnClientClick = "return confirm('Tem certeza que deseja eliminar esta categoria?');"
                    };

                    btnDeleteCategory.Click += new EventHandler(DeleteCategory_Click);

                    // Adiciona os controlos ao PlaceHolder
                    categoriesPlaceHolder.Controls.Add(new LiteralControl("<div class='p-1 d-flex align-items-center p-1'>"));
                    categoriesPlaceHolder.Controls.Add(new LiteralControl($"<label class='me-2 fw-bolder fontEncode'>Categoria #{catcount}:</label>"));
                    categoriesPlaceHolder.Controls.Add(txtCategoryName);
                    categoriesPlaceHolder.Controls.Add(new LiteralControl("&nbsp;"));
                    categoriesPlaceHolder.Controls.Add(btnDeleteCategory);
                    categoriesPlaceHolder.Controls.Add(new LiteralControl("</div>"));

                    catcount++;
                }

                // Habilita/desabilita os botões de exclusão
                if (categoryIds.Count == 1)
                {
                    foreach (Control control in categoriesPlaceHolder.Controls)
                    {
                        if (control is Button btn && btn.ID.StartsWith("btnDelete_"))
                        {
                            btn.Enabled = false;
                        }
                    }
                }
                else
                {
                    foreach (Control control in categoriesPlaceHolder.Controls)
                    {
                        if (control is Button btn && btn.ID.StartsWith("btnDelete_"))
                        {
                            btn.Enabled = true;
                        }
                    }
                }
                con.Close();
            }
        }

        // Método para gravar as alterações do tema e das categorias
        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Esconde a mensagem

            try
            {
                string newThemeName = txtThemeName.Text.Trim();

                // Verifica se o nome do tema está vazio
                if (string.IsNullOrEmpty(newThemeName))
                {
                    lblMessage.Text = "O nome do tema não pode estar vazio.";
                    lblMessage.CssClass = "alert alert-danger text-center";
                    lblMessage.Visible = true;
                    return;
                }

                // Verificar se o ID do tema está corretamente inicializado na sessão
                if (Session["themeId"] == null || !int.TryParse(Session["themeId"].ToString(), out themeId) || themeId <= 0)
                {
                    lblMessage.Text = "Erro no ID do tema.";
                    lblMessage.CssClass = "alert alert-danger text-center";
                    lblMessage.Visible = true;
                    return;
                }

                // Grava o Tema
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa comando SQL para atualizar na base de dados
                    SqlCommand cmd = new SqlCommand { CommandText = "p_UpdateThemeName", CommandType = CommandType.StoredProcedure, Connection = con };
                    cmd.Parameters.AddWithValue("@newName", newThemeName);
                    cmd.Parameters.AddWithValue("@idTheme", themeId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                // Grava as categorias
                foreach (Control control in categoriesPlaceHolder.Controls)
                {
                    if (control is TextBox txtCategoryName)
                    {
                        int categoryId = int.Parse(txtCategoryName.ID.Split('_')[1]); // Extrai o ID da categoria do ID do controle

                        string newCategoryName = txtCategoryName.Text.Trim();

                        using (SqlConnection con = new SqlConnection(_connectionString))
                        {
                            SqlCommand cmd = new SqlCommand { CommandType = CommandType.StoredProcedure, Connection = con };
                            // Executa comando para atualizar na base de dados
                            cmd.CommandText = "p_UpdateCategory";
                            cmd.Parameters.AddWithValue("@idCategory", categoryId);
                            cmd.Parameters.AddWithValue("@NameCategory", newCategoryName);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

                // Envia mensagem para página
                lblMessage.Text = "Alterações gravadas com sucesso.";
                lblMessage.CssClass = "alert alert-success text-center";
                lblMessage.Visible = true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Erro ao atualizar: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger text-center";
                lblMessage.Visible = true;
            }
        }

        // Método para o botão adicionar uma nova categoria
        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Esconde a mensagem
            string newCategoryName = txtNewCategory.Text.Trim();

            // Verifica se o campo de nova categoria está vazio
            if (string.IsNullOrEmpty(newCategoryName))
            {
                lblMessage.Text = "O nome da nova categoria não pode estar vazio.";
                lblMessage.CssClass = "alert alert-danger text-center";
                lblMessage.Visible = true;
                return;
            }

            int themeId = int.Parse(Session["themeId"].ToString());

            // Insere a nova categoria
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa comando SQL para atualizar na base de dados
                    SqlCommand cmd = new SqlCommand { CommandType = CommandType.StoredProcedure, Connection = con };
                    cmd.CommandText = "p_InsertCategory";
                    cmd.Parameters.AddWithValue("@idTheme", themeId);
                    cmd.Parameters.AddWithValue("@NameCategory", newCategoryName);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    // Envia mensagem para a página
                    lblMessage.Text = "Categoria inserida com sucesso.";
                    lblMessage.CssClass = "alert alert-success text-center";
                    lblMessage.Visible = true;

                    // Limpa o campo de nova categoria após adicionar
                    txtNewCategory.Text = string.Empty;
                }
                // Recarregar categorias após inserir a nova categoria
                LoadCategories(themeId);
            }
            catch (SqlException ex)
            {
                // Verifica se a exceção levantada é devido à categoria já existente
                if (ex.Number == 50000) // Número do erro do RAISERROR
                {
                    lblMessage.Text = ex.Message;
                    lblMessage.CssClass = "alert alert-danger text-center";
                    lblMessage.Visible = true;
                }
                else
                {
                    lblMessage.Text = "Erro ao inserir a categoria: " + ex.Message;
                    lblMessage.CssClass = "alert alert-danger text-center";
                    lblMessage.Visible = true;
                }
            }
        }

        // Método para o botão apagar uma categoria
        protected void DeleteCategory_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Esconde a mensagem

            Button btn = (Button)sender;
            int categoryId = int.Parse(btn.CommandArgument);

            try
            {
                // Apaga a categoria
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa comando SQL para atualiazar na base de dados
                    SqlCommand cmd = new SqlCommand("p_DeleteCategory", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idCategory", categoryId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                // Recarregar categorias
                LoadCategories(themeId);
                lblMessage.Text = "Categoria apagada com sucesso.";
                lblMessage.CssClass = "alert alert-success text-center";
                lblMessage.Visible = true;
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger text-center";
                lblMessage.Visible = true;
            }
        }

        // Método para o botão de voltar para o menu anterior
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/View/Temas.aspx");
        }
    }
}
