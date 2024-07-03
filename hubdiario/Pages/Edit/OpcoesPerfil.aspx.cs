using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;

namespace hubdiario.Pages.Edit
{
    public partial class OpcoesPerfil : System.Web.UI.Page
    {
        // Conexão com a base de dados
        string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verifica se a página não é um postback
            if (!IsPostBack)
            {
                // Inicializa os dados da sessão
                int userId = GetUserIdFromSession();

                if (userId == 0)
                {
                    // Redireciona para a página de login se o utilizador não estiver autenticado
                    Response.Redirect("~/Default.aspx");
                }
            }
            else
            {
                // Verifica se o ID do utilizador ainda está na sessão durante um postback
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
            }
        }

        // Método para Atualizar o Email na base de dados
        public string UpdateUserEmail(int idUser, string newEmail)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Executa comando SQL para atualizar na base de dados
                SqlCommand cmd = new SqlCommand { CommandText = "p_UpdateUserEmail", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.Add(new SqlParameter("@idUser", idUser));
                cmd.Parameters.Add(new SqlParameter("@NewEmail", newEmail));
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    // Atualiza o email na sessão
                    Session["EmailUser"] = newEmail;

                    return "Email atualizado com sucesso.";
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 50000) // Número de erro definido no RAISERROR
                    {
                        return ex.Message; // Retorna a mensagem de erro definida no RAISERROR
                    }
                    else
                    {
                        throw; // Re-lança outras exceções
                    }
                }
            }
        }

        // Método para atualizar a palavra-passe
        public string UpdateUserPassword(int idUser, string currentPassword, string newPassword)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Executa comando SQL para atualizar na base de dados
                SqlCommand cmd = new SqlCommand { CommandText = "p_UpdateUserPassword", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.Add(new SqlParameter("@idUser", idUser));
                cmd.Parameters.Add(new SqlParameter("@CurrentPassword", HashPassword(currentPassword))); // Hash da palavra-passe atual
                cmd.Parameters.Add(new SqlParameter("@NewPassword", HashPassword(newPassword))); // Hash da nova palavra-passe
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return "Palavra-passe atualizada com sucesso.";
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 50000) // Número de erro definido no RAISERROR
                    {
                        return ex.Message; // Retorna a mensagem de erro definida no RAISERROR
                    }
                    else
                    {
                        throw; // Re-lançar outras exceções
                    }
                }
            }
        }

        // Método para apagar a conta do utilizador
        public string DeleteUserAccount(int idUser)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Executa comando para atualizar na base de dados
                SqlCommand cmd = new SqlCommand { CommandText = "p_DeleteUserAccount", CommandType = CommandType.StoredProcedure, Connection = con };
                cmd.Parameters.Add(new SqlParameter("@idUser", idUser));
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return "Conta apagada com sucesso.";
                }
                catch (SqlException ex)
                {
                    return "Erro ao apagar a conta: " + ex.Message;
                }
            }
        }

        // Método para obter o ID do utilizador
        private int GetUserIdFromSession()
        {
            if (Session["UserId"] != null)
            {
                return (int)Session["UserId"];
            }
            else
            {
                // Redirecionar para a página de login se o ID do utilizador não estiver na sessão
                Response.Redirect("~/Default.aspx");
                return 0;
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

        // Método para o botão de atualização de e-mail
        protected void btnUpdateEmail_Click(object sender, EventArgs e)
        {
            lblEmailMessage.Visible = false; // Esconde a mensagem
            lblPasswordMessage.Visible = false; // Esconde a mensagem

            int userId = GetUserIdFromSession();
            string newEmail = txtNewEmail.Text;
            string message = UpdateUserEmail(userId, newEmail);

            // Envia mensagem para a página
            lblEmailMessage.Text = message;
            lblEmailMessage.CssClass = "alert alert-success text-center";
            lblEmailMessage.Visible = true;
        }

        // Método para o botão de atualização de palavra-passe
        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            lblEmailMessage.Visible = false; // Esconde a mensagem
            lblPasswordMessage.Visible = false; // Esconde a mensagem

            int userId = GetUserIdFromSession();
            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Verifica se as palavra-passe são iguais
            if (newPassword != confirmPassword)
            {
                lblPasswordMessage.Text = "A nova palavra-passe e a confirmação não coincidem.";
                lblPasswordMessage.CssClass = "alert alert-danger text-center";
                lblPasswordMessage.Visible = true;
                return;
            }

            // Verifica se a palavra-passe tem letras e números
            if (!IsValidPassword(newPassword))
            {
                lblPasswordMessage.Text = "A palavra-passe deve ter pelo menos 8 caracteres e conter números e letras.";
                lblPasswordMessage.CssClass = "alert alert-danger text-center";
                lblPasswordMessage.Visible = true;
                return;
            }

            // Envia mensagem para a página
            string message = UpdateUserPassword(userId, currentPassword, newPassword);
            lblPasswordMessage.Text = message;
            lblPasswordMessage.CssClass = "alert alert-success text-center";
            lblPasswordMessage.Visible = true;
        }

        // Método para o botão de apagar conta
        protected void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            hfUserIdToDelete.Value = GetUserIdFromSession().ToString();
            mpeConfirmDelete.Show();    // Mostra a janela para confirmar a eliminação da conta
        }

        // Método para cancelar a eliminação do tema
        protected void btnCancelDelete_Click(object sender, EventArgs e)
        {
            mpeConfirmDelete.Hide(); // Esconde a janela
        }

        // Método de confirmação da eliminação da conta
        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            int userId = GetUserIdFromSession();
            string message = DeleteUserAccount(userId);

            // Envia mensagem para a página
            lblDeleteAccountMessage.Text = message;
            lblDeleteAccountMessage.Visible = true;

            if (message == "Conta apagada com sucesso.")
            {
                Session.Abandon();
                Response.Redirect("~/Default.aspx");
            }
        }

        // Método para o botão de voltar para o início
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/View/Temas.aspx");
        }
    }
}