using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace hubdiario
{
    public partial class Default : Page
    {
        // Variavel para estado de autenticação a falso
        public bool _acessoBD = false;
        // Conexão com a base de dados
        string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // Método para o botão de validar a autenticação do utilizador
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string passwordHash = HashPassword(password); // Converte a password fornecida para hash

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Executa comnado SQL para verificar na base de dados
                SqlCommand cmd = new SqlCommand { CommandText = "p_AuthenticateUser", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.Add(new SqlParameter("@EmailUser", email));
                cmd.Parameters.Add(new SqlParameter("@PasswordUser", passwordHash));
                con.Open();
                _acessoBD = Convert.ToBoolean(cmd.ExecuteScalar());
                con.Close();
            }

            // Se a autenticação for bem-sucedida, obtém o ID do utilizador
            if (_acessoBD)
            {
                // Define o valor da sessão para indicar que o utilizador está logado
                Session["IsLoggedIn"] = true;

                lblMessage.Text = "Login com sucesso!";
                // Armazenar o email na sessão e redirecionar para a página Temas
                Session["EmailUser"] = email;
                HttpContext.Current.Response.Redirect("/Pages/View/Temas.aspx"); // Redirecionar para página Temas
            }
            else
            {
                lblMessage.Text = "Email ou palavra-passe inválidos.";
            }
        }

        // Método para hash da palavra-passe com SHA-256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
