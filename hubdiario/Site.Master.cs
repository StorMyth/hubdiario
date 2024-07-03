using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace hubdiario
{
    public partial class SiteMaster : MasterPage
    {
        string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verifica se há um valor na sessão e indica que o utilizador está logado
            if (Session["IsLoggedIn"] != null && (bool)Session["IsLoggedIn"])
            {
                // Mostrar o menu se o utilizador está logado
                navbar.Visible = true;

                int userId = GetUserIdByEmail(Session["EmailUser"].ToString());
                if (userId != 0)
                {
                    Session["UserId"] = userId;
                }
                else
                {
                    // Redirecionar para a página de login se o UserId não puder ser obtido
                    Response.Redirect("~/Default.aspx");
                }
            }
            else
            {
                // Esconder o menu se o utilizador não está logado
                navbar.Visible = false;
            }
        }

        // Método para obter o ID do utilizador pelo nome do email
        private int GetUserIdByEmail(string email)
        {
            int userId = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand { CommandText = "p_GetUserIdByEmail", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.Add(new SqlParameter("@EmailUser", email));
                SqlParameter userIdParam = new SqlParameter("@UserId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(userIdParam);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    userId = (int)userIdParam.Value;
                }
                catch (Exception ex)
                {
                    // Log ou mensagem de erro para debug
                    Console.WriteLine("Ocorreu um erro: " + ex.Message);
                }
            }
            return userId;
        }

        // Método para terminar a sessão do utilizador
        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            // Limpar a sessão e redirecionar para a página de login
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect("~/Default.aspx");
        }
    }
}