using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Since1999.Filters;
using Since1999.Models;

namespace Since1999.Controllers
{
    public class UsersController : ApiController
    {
        // Connessione al database
        private readonly string connString = ConfigurationManager.ConnectionStrings["Since1999Context"].ToString();

        // GET: api/Users
        public IEnumerable<User> Get()
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var command = new SqlCommand(@"SELECT UserID, Nome, Cognome, Username, Email FROM Users", conn);
                var reader = command.ExecuteReader();

                var users = new List<User>();
                while (reader.Read())
                {
                    var user = new User
                    {
                        UserID = (int)reader["UserID"],
                        Nome = (string)reader["Nome"],
                        Cognome = (string)reader["Cognome"],
                        Username = (string)reader["Username"],
                        Email = (string)reader["Email"]
                    };
                    users.Add(user);
                }

                return users;
            }
        }

        // GET: api/Users/5
        public IHttpActionResult Get(int id)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var command = new SqlCommand(@"SELECT UserID, Nome, Cognome, Username, Email FROM Users WHERE UserID = @UserID", conn);
                command.Parameters.AddWithValue("@UserID", id);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    var user = new User
                    {
                        UserID = (int)reader["UserID"],
                        Nome = (string)reader["Nome"],
                        Cognome = (string)reader["Cognome"],
                        Username = (string)reader["Username"],
                        Email = (string)reader["Email"]
                    };
                    return Ok(user);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        // POST: api/Users
        public IHttpActionResult Post([FromBody] User user)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var command = new SqlCommand(@"INSERT INTO Users (Nome, Cognome, Username, Email) 
                                               VALUES (@Nome, @Cognome, @Username, @Email);
                                               SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);
                command.Parameters.AddWithValue("@Nome", user.Nome);
                command.Parameters.AddWithValue("@Cognome", user.Cognome);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                var userId = (int)command.ExecuteScalar();

                user.UserID = userId;
                return Created(new Uri(Request.RequestUri + "/" + userId), user);
            }
        }

        // PUT: api/Users/5
        public IHttpActionResult Put(int id, [FromBody] User user)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var command = new SqlCommand(@"UPDATE Users 
                                               SET Nome = @Nome, Cognome = @Cognome, Username = @Username, 
                                                   Email = @Email 
                                               WHERE UserID = @UserID", conn);
                command.Parameters.AddWithValue("@Nome", user.Nome);
                command.Parameters.AddWithValue("@Cognome", user.Cognome);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@UserID", id);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        // DELETE: api/Users/5
        public IHttpActionResult Delete(int id)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var command = new SqlCommand(@"DELETE FROM Users WHERE UserID = @UserID", conn);
                command.Parameters.AddWithValue("@UserID", id);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
        }
    }
}
