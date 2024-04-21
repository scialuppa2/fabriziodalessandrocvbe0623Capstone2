using BCryptNet = BCrypt.Net.BCrypt;
using System.Data.SqlClient;
using System.Configuration;

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
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                var query = "SELECT PasswordHash FROM Users WHERE Email = @Email";
                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var storedHash = reader.GetString(0);
                            if (string.IsNullOrEmpty(storedHash))
                            {
                                // Gestire il caso in cui l'hash della password recuperato è nullo o vuoto
                                return false;
                            }
                            return VerifyPasswordHash(password, storedHash);
                        }
                    }
                }
            }

            // Nessun utente trovato con quell'email
            return false;
        }


        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCryptNet.Verify(password, storedHash);
        }
    }
}
