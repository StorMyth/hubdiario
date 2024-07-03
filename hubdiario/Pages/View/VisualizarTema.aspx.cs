using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace hubdiario.Pages.View
{
    public partial class VisualizarTema : System.Web.UI.Page
    {
        // Conexão com a base de dados
        string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private int _firstCategoryId;   // ID da primeira categoria
        private string _firstCategoryName;  // Nome da primeira categoria

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verifica se a página não é um postback(primeira carga da página)
            if (!IsPostBack)
            {
                // Verifica se o parâmetro "id" está presente na query string
                if (Request.QueryString["id"] != null)
                {
                    int themeId;
                    // Tenta converter o valor do parâmetro "id" para um inteiro
                    if (int.TryParse(Request.QueryString["id"], out themeId))
                    {
                        DisplayThemeDetails(themeId);    // Exibe os detalhes do tema
                        LoadCategories(themeId);         // Carrega as categorias associadas ao tema

                        Session["CurrentThemeId"] = themeId; // Armazena o themeId na sessão para uso em postbacks

                        // Carrega itens para a primeira categoria por padrão
                        if (_firstCategoryId > 0)
                        {
                            LoadItems(_firstCategoryId);
                            lblSelectedCategory.Text = $"{_firstCategoryName}";
                            SetSelectedCategoryName(_firstCategoryId);
                            
                            Session["CurrentCategoryId"] = _firstCategoryId;    // Armazena o categoryId na sessão
                        }
                    }
                    else
                    {
                        Response.Redirect("~/Pages/Edit/OpcoesPerfil");  // Redireciona para trás
                    }
                }
                else
                {
                    Response.Redirect("~/Pages/View/Temas.aspx");  // Redireciona para trás
                }
            }
            else
            {
                // Verifica se o themeId está armazenado na sessão
                if (Session["CurrentThemeId"] != null)
                {
                    int themeId = (int)Session["CurrentThemeId"];
                    LoadCategories(themeId);    // Carrega as categorias associadas ao tema

                    // Verifica se o postback foi causado por um click na categoria
                    if (Request["__EVENTTARGET"] == "CategoryClick")
                    {
                        int categoryId;

                        // Tenta converter o argumento do evento para um inteiro
                        if (int.TryParse(Request["__EVENTARGUMENT"], out categoryId))
                        {
                            LoadItems(categoryId);  // Carrega itens para a categoria selecionada
                            SetSelectedCategoryName(categoryId);
                            
                            Session["CurrentCategoryId"] = categoryId;   // Armazena o categoryId na sessão
                        }
                    }
                }
                else
                {
                    Response.Redirect("~/Default.aspx");  // Redireciona para o login
                }
            }
        }

        // Método para exibir o nome do tema
        private void DisplayThemeDetails(int themeId)
        {
            try 
            { 
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("p_GetThemeNameById", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ThemeId", themeId);
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    // Verifica se o reader tem resultados
                    if (reader.Read())
                    {
                        // Define o texto do label lblThemeName com o nome do tema obtido
                        lblThemeName.Text = reader.GetString(reader.GetOrdinal("NameTheme"));
                    }
                    else
                    {
                        Response.Redirect("~/Pages/View/Temas.aspx"); // Redireciona para trás
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
            }
        }

        // Método para carregar os nomes das categorias do tema
        private void LoadCategories(int themeId)
        {
            // Limpa os controles anteriores no painel de categorias
            categoriesPanel.Controls.Clear();

            try 
            { 
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa comando SQL para verificar na base de dados
                    SqlCommand cmd = new SqlCommand("p_GetCategoriesByThemeId", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ThemeId", themeId);
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    bool isFirstCategory = true;
                    while (reader.Read())
                    {
                        int categoryId = reader.GetInt32(reader.GetOrdinal("idCategory"));
                        string categoryName = reader.GetString(reader.GetOrdinal("NameCategory"));

                        // Verifica se esta é a primeira categoria
                        if (isFirstCategory)
                        {
                            _firstCategoryId = categoryId;
                            _firstCategoryName = categoryName;
                            isFirstCategory = false;
                        }

                        // Cria um painel para a categoria e adiciona um link
                        Panel categoryPanel = new Panel();
                        categoryPanel.CssClass = "category-item";
                        categoryPanel.Controls.Add(new Literal
                        {
                            Text = $"<a href='javascript:showItems({categoryId})' class='category-link'>{categoryName}</a>"
                        });

                        // Adiciona o painel da categoria ao painel de categorias
                        categoriesPanel.Controls.Add(categoryPanel);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
            }
        }

        // Método para carregar os detalhes dos itens
        private void LoadItems(int categoryId)
        {
            // Limpa os itens anteriores no painel
            itemsPanel.Controls.Clear();

            // Flags para verificar se as colunas têm valores
            bool hasTextItem = false;
            bool hasQuantity = false;
            bool hasDateItem = false;
            bool hasPrice = false;

            // Armazena temporariamente as linhas dos itens
            List<Dictionary<string, object>> itemRowsData = new List<Dictionary<string, object>>();

            // Dados do alerta
            Dictionary<string, object> alertRowData = null;

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa comando SQL para verificar na base de dados
                    SqlCommand cmd = new SqlCommand("p_GetItemsAndAlerts", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Dictionary<string, object> itemRowData = new Dictionary<string, object>();

                        // Verifica e adiciona os dados dos itens, caso não tenham valor não mostra a coluna
                        if (!reader.IsDBNull(reader.GetOrdinal("TextItem")))
                        {
                            itemRowData["TextItem"] = reader.GetString(reader.GetOrdinal("TextItem"));
                            hasTextItem = true;
                        }
                        else
                        {
                            itemRowData["TextItem"] = string.Empty;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("Quantity")))
                        {
                            itemRowData["Quantity"] = reader.GetInt32(reader.GetOrdinal("Quantity")).ToString();
                            hasQuantity = true;
                        }
                        else
                        {
                            itemRowData["Quantity"] = string.Empty;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("DateItem")))
                        {
                            itemRowData["DateItem"] = reader.GetDateTime(reader.GetOrdinal("DateItem")).ToShortDateString();
                            hasDateItem = true;
                        }
                        else
                        {
                            itemRowData["DateItem"] = string.Empty;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("Price")))
                        {
                            itemRowData["Price"] = reader.GetDecimal(reader.GetOrdinal("Price")).ToString("C");
                            hasPrice = true;
                        }
                        else
                        {
                            itemRowData["Price"] = string.Empty;
                        }

                        itemRowsData.Add(itemRowData);

                        // Verifica e adiciona os dados dos alertas
                        if (alertRowData == null && !reader.IsDBNull(reader.GetOrdinal("AlertTime")))
                        {
                            alertRowData = new Dictionary<string, object>
                            {
                                ["AlertTime"] = reader.GetDateTime(reader.GetOrdinal("AlertTime")).ToString("yyyy-MM-dd HH:mm:ss"),
                                ["RepeatInterval"] = !reader.IsDBNull(reader.GetOrdinal("RepeatInterval")) ? reader.GetInt32(reader.GetOrdinal("RepeatInterval")).ToString() : string.Empty,
                                ["RepeatUnit"] = !reader.IsDBNull(reader.GetOrdinal("RepeatUnit")) ? reader.GetString(reader.GetOrdinal("RepeatUnit")) : string.Empty
                            };
                        }
                    }

                    con.Close();
                }

                if (itemRowsData.Count == 0)
                {
                    // Exibe mensagem se não houver itens
                    itemsPanel.Controls.Add(new Literal { Text = "<div class='alert alert-warning text-center'>Não existem itens criados.</div>" });
                }
                else
                {
                    // Cria a tabela de itens
                    Table itemsTable = new Table { CssClass = "table table-hover" };
                    TableHeaderRow itemsHeaderRow = new TableHeaderRow();
                    if (hasTextItem) itemsHeaderRow.Cells.Add(new TableHeaderCell { Text = "Texto", CssClass = "bg-secondary text-white bg-gradient text-center fw-bolder" });
                    if (hasQuantity) itemsHeaderRow.Cells.Add(new TableHeaderCell { Text = "Quantidade", CssClass = "bg-secondary text-white bg-gradient text-center fw-bolder" });
                    if (hasDateItem) itemsHeaderRow.Cells.Add(new TableHeaderCell { Text = "Data", CssClass = "bg-secondary text-white bg-gradient text-center fw-bolder" });
                    if (hasPrice) itemsHeaderRow.Cells.Add(new TableHeaderCell { Text = "Preço", CssClass = "bg-secondary text-white bg-gradient text-center fw-bolder" });

                    if (itemsHeaderRow.Cells.Count > 0)
                    {
                        itemsTable.Rows.Add(itemsHeaderRow);
                    }

                    // Adiciona as linhas à tabela de itens
                    foreach (var rowData in itemRowsData)
                    {
                        TableRow row = new TableRow();

                        if (hasTextItem) row.Cells.Add(new TableCell { Text = rowData["TextItem"].ToString(), CssClass = "text-center form-control-sm-text" });
                        if (hasQuantity) row.Cells.Add(new TableCell { Text = rowData["Quantity"].ToString(), CssClass = "text-center form-control-sm-text" });
                        if (hasDateItem) row.Cells.Add(new TableCell { Text = rowData["DateItem"].ToString(), CssClass = "text-center form-control-sm-text" });
                        if (hasPrice) row.Cells.Add(new TableCell { Text = rowData["Price"].ToString(), CssClass = "text-center form-control-sm-text" });

                        itemsTable.Rows.Add(row);
                    }
                    // Adiciona a tabela de itens ao painel de itens dentro de uma div
                    itemsPanel.Controls.Add(new Literal { Text = "<div class='d-flex justify-content-center w-auto'>" });
                    itemsPanel.Controls.Add(itemsTable);
                    itemsPanel.Controls.Add(new Literal { Text = "</div>" });
                }

                if (alertRowData == null)
                {
                    // Exibe mensagem se não houver alertas
                    itemsPanel.Controls.Add(new Literal { Text = "<div class='alert alert-warning text-center w-100'>Não existe alerta configurado.</div>" });
                }
                else
                {
                    // Cria a tabela de alertas
                    Table alertsTable = new Table { CssClass = "table table-striped mt-3" };
                    TableHeaderRow alertsHeaderRow = new TableHeaderRow();
                    alertsHeaderRow.Cells.Add(new TableHeaderCell { Text = "Alerta", CssClass = "bg-warning bg-gradient text-center fw-bolder text-white" });
                    alertsHeaderRow.Cells.Add(new TableHeaderCell { Text = "Repetição", CssClass = "bg-warning bg-gradient text-center fw-bolder text-white" });
                    alertsHeaderRow.Cells.Add(new TableHeaderCell { Text = "Tipo", CssClass = "bg-warning bg-gradient text-center fw-bolder text-white" });

                    alertsTable.Rows.Add(alertsHeaderRow);

                    // Adiciona a linha à tabela de alertas
                    TableRow row = new TableRow();

                    row.Cells.Add(new TableCell { Text = alertRowData["AlertTime"].ToString(), CssClass = "text-center" });
                    row.Cells.Add(new TableCell { Text = alertRowData["RepeatInterval"].ToString(), CssClass = "text-center" });
                    row.Cells.Add(new TableCell { Text = alertRowData["RepeatUnit"].ToString(), CssClass = "text-center" });

                    alertsTable.Rows.Add(row);

                    // Adiciona a tabela de alertas ao painel de itens dentro de uma div
                    itemsPanel.Controls.Add(new Literal { Text = "<div class='d-flex justify-content-center'>" });
                    itemsPanel.Controls.Add(new Literal { Text = "<div class='container w-75 w-md-100 m-md-0'>" });
                    itemsPanel.Controls.Add(alertsTable);
                    itemsPanel.Controls.Add(new Literal { Text = "</div>" });
                    itemsPanel.Controls.Add(new Literal { Text = "</div>" });
                }
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
            }
        }

        // Método para obter o nome da Categoria
        private void SetSelectedCategoryName(int categoryId)
        {
            try 
            { 
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa o comando SQL para verificar a base de dados
                    SqlCommand cmd = new SqlCommand("p_GetCategoryNameById", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Define o texto do label lblSelectedCategory com o nome da categoria obtida
                        lblSelectedCategory.Text = $"{reader.GetString(reader.GetOrdinal("NameCategory"))}";
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
            }
        }

        // Método para o botão de voltar para o menu anterior
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/View/Temas.aspx");
        }

        // Método para o botão de criar ou editar os itens de uma Categoria
        protected void btnCreateEditItem_Click(object sender, EventArgs e)
        {
            try 
            { 
                // Verifica se tem o id da Categoria
                if (Session["CurrentCategoryId"] != null)
                {
                    int categoryId = (int)Session["CurrentCategoryId"];
                    int themeId = (int)Session["CurrentThemeId"];

                    // Redirecionar para a página de criação/edição de itens da categoria selecionada
                    Response.Redirect($"~/Pages/Edit/CriarEditarItem.aspx?categoryId={categoryId}&themeId={themeId}");
                }
                else
                {
                    lblMessage.Text = "Existe um problema com a Categoria selecionada";
                }
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
            }
        }
    }
}
