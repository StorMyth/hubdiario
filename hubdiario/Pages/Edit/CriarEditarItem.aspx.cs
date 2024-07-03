using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace hubdiario.Pages.Edit
{
    public partial class CriarEditarItem : System.Web.UI.Page
    {
        // Conexão com a base de dados
        private string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        
        // Variáveis
        private int _categoryId;
        private int _userId;
        private int _itemId;
        private int _alertId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verifica se a página está a ser carregada pela primeira vez ou se é um postback
            if (!IsPostBack)
            {
                // Verifica se a query string contém a "categoryId"
                if (Request.QueryString["categoryId"] != null && Request.QueryString["themeId"] != null)
                {
                    // Tenta converter o valor da query string "categoryId" para um inteiro
                    if (int.TryParse(Request.QueryString["categoryId"], out _categoryId) &&
                        int.TryParse(Request.QueryString["themeId"], out int themeId))
                    {
                        Session["themeId"] = themeId; // Armazena o themeId na sessão
                        LoadUserId(_categoryId);    // Carrega o ID do utilizador associado à categoria
                        LoadItems(_categoryId);     // Carrega os itens associados à categoria
                        SetSelectedCategoryName(_categoryId);   // Indica o nome da categoria selecionada
                    }
                    else
                    {
                        Response.Redirect("~/Default.aspx");  // Redireciona para o login
                    }
                }
                else
                {
                    Response.Redirect("~/Default.aspx");  // Redireciona para o login
                }
            }
            else
            {
                // Em caso de postback, recupera o valor de "categoryId" da query string
                _categoryId = int.Parse(Request.QueryString["categoryId"]);
                int themeId = int.Parse(Request.QueryString["themeId"]);

                // Tenta converter o valor do hidden field "hfItemId" para um inteiro e armazena em _itemId
                if (int.TryParse(hfItemId.Value, out int itemId))
                {
                    _itemId = itemId;
                }
                // Tenta converter o valor do hidden field "hfAlertId" para um inteiro e armazena em _alertId
                if (int.TryParse(hfAlertId.Value, out int alertId))
                {
                    _alertId = alertId;
                }
                LoadUserId(_categoryId);  // Carrega novamente o ID do utilizador associado à categoria 
            }
        }

        // Método para selecionar o ID do utilizador com base no ID da categoria
        private void LoadUserId(int categoryId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand { CommandText = "p_GetUserIdByCategoryId", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                // Adiciona o parâmetro de saída @UserId
                SqlParameter userIdParam = new SqlParameter("@UserId", SqlDbType.Int);
                userIdParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(userIdParam);

                con.Open();
                var result = cmd.ExecuteScalar();
                con.Close();

                // Verifica se um resultado foi retornado
                if (userIdParam.Value != DBNull.Value)
                {
                    // Define o _userId com o valor retornado
                    _userId = (int)userIdParam.Value;
                }
                else
                {
                    Response.Redirect("~/VisualizarTema.aspx"); // Redireciona para trás
                }
            }
        }

        // Método para carregar os dados da tabela Itens da categoria
        private void LoadItems(int categoryId)
        {
            try 
            { 
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa comando SQL para verificar na base de dados
                    SqlCommand cmd = new SqlCommand { CommandText = "p_GetItemsByCategoryId", CommandType = CommandType.StoredProcedure, Connection = con };
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                    // Cria um adaptador de dados para executar o comando SQL e preencher um DataTable
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    // Preenche o DataTable com os dados
                    da.Fill(dt);

                    // Verifica se o DataTable está vazio
                    if (dt.Rows.Count == 0)
                    {
                        // Adicionar uma linha vazia se não houver itens, para a tabela não ficar sem linhas
                        DataRow newRow = dt.NewRow();
                        dt.Rows.Add(newRow);
                    }

                    // Define a fonte de dados do Repeater com o DataTable e faz o data bind
                    rptItems.DataSource = dt;
                    rptItems.DataBind();

                    // Carregar alerta do primeiro item, se disponível
                    if (dt.Rows.Count > 0 && dt.Rows[0]["idItem"] != DBNull.Value)
                    {
                        _itemId = (int)dt.Rows[0]["idItem"];    // Define o _itemId com o valor do primeiro item
                        _alertId = GetAlertIdByCategoryId(_categoryId);
                        LoadAlert(_alertId);    // Carrega o alerta associado à categoria
                    }
                    else
                    {
                        // Limpa os campos de alerta se não houver dados
                        ClearAlertFields();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger text-center";
                lblMessage.Visible = true;
            }
        }

        // Método para selecionar o nome da Categoria
        private void SetSelectedCategoryName(int categoryId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Executa comando SQL para verificar na base de dados
                SqlCommand cmd = new SqlCommand { CommandText = "p_GetCategoryNameById", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                con.Open();

                // Armazena o resultado como uma string
                string categoryName = cmd.ExecuteScalar()?.ToString();
                con.Close();

                // Define o texto do literal com o nome da categoria
                litCategoryName.Text = $"<h2 class='form-control fw-bolder text-center fs-4 bg-primary bg-gradient text-white mb-4 fontEncode'>{categoryName}</h2>";
            }
        }

        // Método para selecionar o ID do Alerta
        private int GetAlertIdByCategoryId(int categoryId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Executa comando SQL para verificar na base de dados
                SqlCommand cmd = new SqlCommand { CommandText = "p_GetAlertIdByCategoryId", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                cmd.Parameters.AddWithValue("@UserId", _userId);
                con.Open();

                // Armazena o resultado
                var result = cmd.ExecuteScalar();
                con.Close();

                // Retorna o ID do alerta encontrado, caso contrário retorna 0
                return result != null ? (int)result : 0;
            }
        }

        // Método para carregar os dados da tabela Alert da categoria
        private void LoadAlert(int alertId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa comando SQL para verificar na base de dados
                    SqlCommand cmd = new SqlCommand { CommandText = "p_GetAlertById", CommandType = CommandType.StoredProcedure, Connection = con };
                    cmd.Parameters.AddWithValue("@AlertId", alertId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Verifica se o reader tem resultados
                    if (reader.HasRows)
                    {
                        reader.Read();
                        // Define os valores dos controlos da página com os dados do alerta
                        hfAlertId.Value = reader["idAlert"].ToString();
                        hfItemId.Value = _itemId.ToString(); // Associa o item atual
                        txtAlertTime.Text = Convert.ToDateTime(reader["AlertTime"]).ToString("yyyy-MM-ddTHH:mm");
                        txtRepeatInterval.Text = reader["RepeatInterval"].ToString();
                        ddlRepeatUnit.SelectedValue = reader["RepeatUnit"].ToString();
                        chkActive.Checked = Convert.ToBoolean(reader["Active"]);
                    }
                    else
                    {
                        // Nenhum alerta encontrado, limpa os campos do formulário
                        ClearAlertFields();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger text-center";
                lblMessage.Visible = true;
            }
        }

        // Método para limpar os campos dos alertas, para ter a linha disponível
        private void ClearAlertFields()
        {
            hfAlertId.Value = "0";
            hfItemId.Value = "0";
            txtAlertTime.Text = string.Empty;
            txtRepeatInterval.Text = string.Empty;
            ddlRepeatUnit.SelectedIndex = 0;
            chkActive.Checked = false;
        }

        // Método para gravar um Alerta
        protected void btnSaveAlert_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Esconde a mensagem

            try
            {
                DateTime alertTime;
                // Tenta converter o texto do campo txtAlertTime para DateTime
                if (!DateTime.TryParse(txtAlertTime.Text, out alertTime))
                {
                    lblMessage.Text = "Tempo de alerta inválido.";
                    lblMessage.CssClass = "alert alert-danger text-center";
                    lblMessage.Visible = true;
                    return;
                }

                // A data do alerta não pode ser no passado
                if (alertTime < DateTime.Now)
                {
                    lblMessage.Text = "O tempo do alerta não pode ser no passado.";
                    lblMessage.CssClass = "alert alert-danger text-center";
                    lblMessage.Visible = true;
                    return;
                }

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    // Obtém os valores dos campos e inicializa as variáveis necessárias
                    int alertId = int.Parse(hfAlertId.Value);
                    int itemId = _itemId;
                    int? repeatInterval = null;
                    int tempRepeatInterval;
                    if (int.TryParse(txtRepeatInterval.Text, out tempRepeatInterval))
                    {
                        repeatInterval = tempRepeatInterval;
                    }
                    string repeatUnit = ddlRepeatUnit.SelectedValue;
                    bool active = chkActive.Checked;

                    SqlCommand cmd = new SqlCommand(alertId == 0 ? "p_InsertAlert" : "p_UpdateAlert", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Adiciona os parâmetros
                    cmd.Parameters.AddWithValue("@AlertTime", alertTime);
                    cmd.Parameters.AddWithValue("@RepeatInterval", repeatInterval.HasValue ? (object)repeatInterval.Value : DBNull.Value);

                    // Só adiciona o RepeatUnit se RepeatInterval tiver um valor atribuído
                    if (repeatInterval.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@RepeatUnit", repeatUnit);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@RepeatUnit", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@Active", active);

                    // Se não existir alerta
                    if (alertId == 0)
                    {
                        // Adiciona os parâmetros específicos para inserção
                        cmd.Parameters.AddWithValue("@UserId", _userId);
                        cmd.Parameters.AddWithValue("@CategoryId", _categoryId);

                        // Define o parâmetro de saída para obter o novo ID de alerta
                        SqlParameter newAlertIdParam = new SqlParameter("@NewAlertId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(newAlertIdParam);

                        // Executa o comando SQL para inserção
                        cmd.ExecuteNonQuery();
                        alertId = (int)newAlertIdParam.Value;
                        hfAlertId.Value = alertId.ToString();
                    }
                    else
                    {
                        // Adiciona os parâmetros específicos para atualização
                        cmd.Parameters.AddWithValue("@AlertId", alertId);
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();

                    lblMessage.Text = "Alterações efetuadas com sucesso.";
                    lblMessage.CssClass = "alert alert-success text-center";
                    lblMessage.Visible = true;
                }

                // Recarrega o alerta para refletir as mudanças
                LoadAlert(int.Parse(hfAlertId.Value));
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger text-center";
                lblMessage.Visible = true;
            }
        }


        // Método o botão gravar o Item
        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Esconde a mensagem

            try 
            { 
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    foreach (RepeaterItem item in rptItems.Items)
                    {
                        // Verifica se o item é do tipo normal ou alternado
                        if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        {
                            // Inicializa variáveis para armazenar os valores dos campos do item
                            int itemId = 0;
                            int.TryParse(((HiddenField)item.FindControl("hfItemId")).Value, out itemId);

                            string textItem = ((TextBox)item.FindControl("txtItem")).Text;
                            int quantity;
                            int.TryParse(((TextBox)item.FindControl("txtQuantity")).Text, out quantity);
                            DateTime? dateItem = null;
                            DateTime tempDateItem;
                            if (DateTime.TryParse(((TextBox)item.FindControl("txtDate")).Text, out tempDateItem))
                            {
                                dateItem = tempDateItem;
                            }
                            decimal? price = null;
                            decimal tempPrice;
                            if (decimal.TryParse(((TextBox)item.FindControl("txtPrice")).Text, out tempPrice))
                            {
                                price = tempPrice;
                            }

                            // Verifica se todos os campos estão vazios
                            if (string.IsNullOrEmpty(textItem) && quantity == 0 && !dateItem.HasValue && !price.HasValue)
                            {
                                lblMessage.Text = "Os campos do item não podem estar todos vazios.";
                                lblMessage.CssClass = "alert alert-danger text-center";
                                lblMessage.Visible = true;
                                return;
                            }

                            SqlCommand cmd = new SqlCommand(itemId == 0 ? "p_InsertItem" : "p_UpdateItem", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Adiciona os parâmetros 
                            cmd.Parameters.AddWithValue("@TextItem", string.IsNullOrEmpty(textItem) ? (object)DBNull.Value : textItem);
                            cmd.Parameters.AddWithValue("@Quantity", quantity == 0 ? (object)DBNull.Value : quantity);
                            cmd.Parameters.AddWithValue("@DateItem", dateItem.HasValue ? (object)dateItem.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@Price", price.HasValue ? (object)price.Value : DBNull.Value);

                            if (itemId == 0)
                            {
                                // Adiciona os parâmetros específicos para inserção
                                cmd.Parameters.AddWithValue("@CategoryId", _categoryId);
                            }
                            else
                            {
                                // Adiciona os parâmetros específicos para atualização
                                cmd.Parameters.AddWithValue("@ItemId", itemId);
                            }

                            cmd.ExecuteNonQuery();
                        }
                    }
                    con.Close();

                    // Envia mensagem para a página
                    lblMessage.Text = "Alterações efetuadas com sucesso.";
                    lblMessage.CssClass = "alert alert-success text-center";
                    lblMessage.Visible = true;
                }

                // Recarregar itens para refletir as mudanças
                LoadItems(_categoryId);
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger text-center";
                lblMessage.Visible = true;
            }
        }

        // Método para o botão adicionar um novo item numa nova linha 
        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Esconde a mensagem

            try
            {
                // Cria um novo DataTable para armazenar os itens
                DataTable dt = new DataTable();
                dt.Columns.Add("idItem", typeof(int));
                dt.Columns.Add("TextItem", typeof(string));
                dt.Columns.Add("Quantity", typeof(int));
                dt.Columns.Add("DateItem", typeof(DateTime));
                dt.Columns.Add("Price", typeof(decimal));

                // Itera sobre cada item no Repeater rptItems
                foreach (RepeaterItem item in rptItems.Items)
                {
                    // Verifica se o item é do tipo normal ou alternado
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        // Cria uma nova linha no DataTable
                        DataRow dr = dt.NewRow();

                        // Obtém o valor do HiddenField hfItemId e converte para int
                        string hfItemIdValue = ((HiddenField)item.FindControl("hfItemId")).Value;
                        int itemId;
                        if (int.TryParse(hfItemIdValue, out itemId))
                        {
                            dr["idItem"] = itemId;
                        }

                        // Obtém o valor do TextBox txtItem e define na coluna "TextItem"
                        dr["TextItem"] = ((TextBox)item.FindControl("txtItem")).Text;

                        // Obtém o valor do TextBox txtQuantity e converte para int
                        int quantity;
                        int.TryParse(((TextBox)item.FindControl("txtQuantity")).Text, out quantity);
                        dr["Quantity"] = quantity;

                        // Obtém o valor do TextBox txtDate e converte para DateTime
                        DateTime tempDateItem;
                        if (DateTime.TryParse(((TextBox)item.FindControl("txtDate")).Text, out tempDateItem))
                        {
                            dr["DateItem"] = tempDateItem;
                        }

                        // Obtém o valor do TextBox txtDate e converte para DateTime
                        decimal tempPrice;
                        if (decimal.TryParse(((TextBox)item.FindControl("txtPrice")).Text, out tempPrice))
                        {
                            dr["Price"] = tempPrice;
                        }

                        // Adiciona a linha ao DataTable
                        dt.Rows.Add(dr);
                    }
                }

                // Adiciona uma nova linha em branco ao DataTable
                DataRow newRow = dt.NewRow();
                dt.Rows.Add(newRow);

                // Define o DataTable como a fonte de dados do Repeater e faz o data bind
                rptItems.DataSource = dt;
                rptItems.DataBind();
            }
            catch (Exception ex)
            {
                // Log ou mensagem de erro para debug
                lblMessage.Text = "Ocorreu um erro: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger text-center";
                lblMessage.Visible = true;
            }
        }

        // Método para o botão apagar um item
        protected void btnDeleteItem_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Esconde a mensagem

            try
            {
                // Obtém o botão que foi clicado e o valor do argumento do comando (id do item)
                Button btnDelete = (Button)sender;
                string commandArgument = btnDelete.CommandArgument;

                // Verifica se o CommandArgument é um número inteiro válido
                int itemId;
                if (!int.TryParse(commandArgument, out itemId))
                {
                    lblMessage.Text = "Ainda não gravou as alterações do item.";
                    return;
                }

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Executa comando SQL para atualizar na base de dados
                    SqlCommand cmd = new SqlCommand { CommandText = "p_DeleteItem", CommandType = CommandType.StoredProcedure, Connection = con };
                    cmd.Parameters.AddWithValue("@ItemId", itemId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                // Recarregar itens para refletir as mudanças
                LoadItems(_categoryId);
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
            int themeId = (int)Session["themeId"];
            Response.Redirect($"~/Pages/View/VisualizarTema.aspx?id={themeId}");
        }
    }
}
