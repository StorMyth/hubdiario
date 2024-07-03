using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.UI;

namespace hubdiario
{
    public partial class Registo : Page
    {
        // Conexão com a base de dados
        private string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // Método para o botão de registar o novo utilizador
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // Limpa a mensagem anterior
            lblMessage.Text = string.Empty;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            // Verifica se todos os campos foram preenchidos
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                lblMessage.Text = "Todos os campos são obrigatórios.";
                return;
            }

            // Verifica se as palavras-passe coincidem
            if (password != confirmPassword)
            {
                lblMessage.Text = "As palavras-passe não coincidem.";
                return;
            }

            // Verifica se a palavra-passe atende aos critérios de complexidade
            if (!IsValidPassword(password))
            {
                lblMessage.Text = "A palavra-passe deve ter pelo menos 8 caracteres e conter números e letras.";
                return;
            }

            try
            {
                // Converte a password colocada para hash
                string passwordHash = HashPassword(password);

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand { CommandText = "p_RegisterUser", CommandType = CommandType.StoredProcedure, Connection = con };

                    cmd.Parameters.Add(new SqlParameter("@Email", email));
                    cmd.Parameters.Add(new SqlParameter("@PasswordHash", passwordHash));

                    SqlParameter successParam = new SqlParameter("@Success", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(successParam);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    bool success = (bool)successParam.Value;
                    con.Close();

                    if (success)
                    {
                        lblMessage.Text = "Registo bem-sucedido! Faça login.";
                    }
                    else
                    {
                        lblMessage.Text = "Erro ao registar. Tente novamente.";
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000) // Nº de erro personalizado para o caso do email já estiver registado
                {
                    lblMessage.Text = ex.Message;
                }
                else
                {
                    lblMessage.Text = "Erro: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Erro: " + ex.Message;
            }
        }

        // Método para validar a complexidade da palavra-passe
        private bool IsValidPassword(string password)
        {
            // Tem de ter mais de 8 caracteres
            if (password.Length < 8) return false;
            bool hasLetter = false;
            bool hasDigit = false;
            // Tem de ter letras e números
            foreach (char c in password)
            {
                if (char.IsLetter(c)) hasLetter = true;
                if (char.IsDigit(c)) hasDigit = true;
            }
            return hasLetter && hasDigit;
        }

        // Método para hash de palavra-passe com SHA-256
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
