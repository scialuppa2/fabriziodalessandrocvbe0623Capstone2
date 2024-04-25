using BCryptNet = BCrypt.Net.BCrypt;
using System.Data.SqlClient;
using System.Configuration;
using Since1999.Models;
using System;

namespace Since1999.Services
{
    public class UserService
    {
        private readonly string _connString;

        public UserService()
        {
            _connString = ConfigurationManager.ConnectionStrings["Since1999Context"].ToString();
        }

        public bool IsValidUser(string email, string password)
        {
            var user = GetUserByEmail(email);
            if (user != null)
            {
                // Verifica la corrispondenza della password
                return BCryptNet.Verify(password, user.PasswordHash);
            }
            return false;
        }

        public User GetUserByEmail(string email)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                var query = "SELECT * FROM Users WHERE Email = @Email";
                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var user = new User
                            {
                                UserID = (int)reader["UserID"],
                                Nome = (string)reader["Nome"],
                                Cognome = (string)reader["Cognome"],
                                Username = (string)reader["Username"],
                                Email = (string)reader["Email"],
                                PasswordHash = (string)reader["PasswordHash"],
                                ProfileImage = reader["ProfileImage"] as string // Aggiungi il campo per l'immagine del profilo
                            };
                            // Aggiungi log dei dati dell'utente
                            Console.WriteLine("Dati dell'utente recuperati:", user);
                            return user;
                        }
                    }
                }
            }

            // Nessun utente trovato con quell'email
            return null;
        }
    }
}
