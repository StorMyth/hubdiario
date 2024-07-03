using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Timers;
using System.Configuration;

namespace Hubdiario.AlertService
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private static Timer _timer;

        static void Main(string[] args)
        {
            // Configure o timer para verificar os alertas a cada minuto
            _timer = new Timer(60000);
            _timer.Elapsed += CheckAlerts;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            Console.WriteLine("Serviço de alerta iniciado. Pressione [Enter] para sair.");
            Console.ReadLine();
        }

        // Método para verificar se existem alertas
        private static void CheckAlerts(object source, ElapsedEventArgs e)
        {
            // Envia mensagem para a consola com a data e hora
            Console.WriteLine("A verificar alertas às " + DateTime.Now);
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Executa comando SQL para verificar na base de dados
                connection.Open();
                string query = "EXEC p_GetAlertsToSend @CurrentTime";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CurrentTime", DateTime.Now);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int alertId = reader.GetInt32(0);
                        int userId = reader.GetInt32(1);
                        DateTime alertTime = reader.GetDateTime(2);
                        string email = reader.GetString(3);

                        // Envia mensagem para a consola quando um email é enviado
                        Console.WriteLine($"A enviar email para {email} para o alerta {alertId} às {alertTime}");

                        // Corpo do email enviado
                        SendEmail(email, "Alerta de Notificação", "Este é um lembrete para o seu alerta.");

                        // Atualiza o próximo alerta se for recorrente
                        UpdateNextAlert(alertId, alertTime);
                    }
                }
            }
        }

        // Método para envio de email
        private static void SendEmail(string to, string subject, string body)
        {
            try
            {
                // Usa o SmtpClient configurado a partir de mailSettings no App.config
                SmtpClient client = new SmtpClient();
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["SmtpFrom"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                client.Send(mailMessage);
                // Envia mensagem para a consola para quem está a enviar o email
                Console.WriteLine("Email enviado para " + to);
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine("Erro SMTP ao enviar email: " + smtpEx.Message);
                if (smtpEx.InnerException != null)
                {
                    Console.WriteLine("Erro interno: " + smtpEx.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar email: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Erro interno: " + ex.InnerException.Message);
                }
            }
        }

        // Método para atualizar o alerta para a próxima data
        private static void UpdateNextAlert(int alertId, DateTime alertTime)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                // Executa comando SQL para verificar na base de dados
                string query = "EXEC p_GetAlertDetails @AlertId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AlertId", alertId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int? repeatInterval = reader["RepeatInterval"] as int?;
                        string repeatUnit = reader["RepeatUnit"] as string;

                        // Lógica para calcular o próximo alerta baseado em RepeatInterval e RepeatUnit
                        if (repeatInterval.HasValue && !string.IsNullOrEmpty(repeatUnit))
                        {
                            DateTime nextAlertTime = alertTime;

                            switch (repeatUnit)
                            {
                                case "Hora":
                                    nextAlertTime = alertTime.AddHours(repeatInterval.Value);
                                    break;
                                case "Diário":
                                    nextAlertTime = alertTime.AddDays(repeatInterval.Value);
                                    break;
                                case "Semanal":
                                    nextAlertTime = alertTime.AddDays(7 * repeatInterval.Value);
                                    break;
                                case "Mensal":
                                    nextAlertTime = alertTime.AddMonths(repeatInterval.Value);
                                    break;
                                case "Anual":
                                    nextAlertTime = alertTime.AddYears(repeatInterval.Value);
                                    break;
                            }

                            reader.Close(); // Fecha o reader antes de executar outra operação

                            // Executa comando SQL para atualizar a base de dados
                            string updateQuery = "EXEC p_UpdateNextAlertTime @AlertId, @NextAlertTime";
                            SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                            updateCommand.Parameters.AddWithValue("@NextAlertTime", nextAlertTime);
                            updateCommand.Parameters.AddWithValue("@AlertId", alertId);

                            updateCommand.ExecuteNonQuery();
                            Console.WriteLine($"Alerta {alertId} atualizado para a próxima execução em {nextAlertTime}");
                        }
                        else
                        {
                            reader.Close(); // Fecha o reader antes de executar outra operação

                            // Se não houver repetição, desativa o alerta
                            string deactivateQuery = "EXEC p_DeactivateAlert @AlertId";
                            SqlCommand deactivateCommand = new SqlCommand(deactivateQuery, connection);
                            deactivateCommand.Parameters.AddWithValue("@AlertId", alertId);

                            deactivateCommand.ExecuteNonQuery();
                            // Envia mensagem para a consola caso não exista repetições do alerta
                            Console.WriteLine($"Alerta {alertId} desativado, pois não possui repetição.");
                        }
                    }
                }
            }
        }
    }
}
