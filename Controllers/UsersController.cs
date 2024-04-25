using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Http;
using Since1999.Models;

namespace Since1999.Controllers
{
    public class UsersController : ApiController
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["Since1999Context"].ToString();

        public IEnumerable<User> Get()
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var command = new SqlCommand(@"SELECT UserID, Nome, Cognome, Username, Email, ProfileImage FROM Users", conn);
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
                        Email = (string)reader["Email"],
                        ProfileImage = reader["ProfileImage"].ToString(),
                    };
                    users.Add(user);
                }

                return users;
            }
        }

        public IHttpActionResult Get(int id)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var command = new SqlCommand(@"SELECT UserID, Nome, Cognome, Username, Email, ProfileImage FROM Users WHERE UserID = @UserID", conn);
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
                        Email = (string)reader["Email"],
                        ProfileImage = reader["ProfileImage"].ToString(),
                    };

                   

                    return Ok(user);
                }
                else
                {
                    return NotFound();
                }
            }
        }


        public IHttpActionResult Post([FromBody] User user)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var command = new SqlCommand(@"INSERT INTO Users (Nome, Cognome, Username, Email, ProfileImage) 
                                               VALUES (@Nome, @Cognome, @Username, @Email, @ProfileImage);
                                               SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);
                command.Parameters.AddWithValue("@Nome", user.Nome);
                command.Parameters.AddWithValue("@Cognome", user.Cognome);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@ProfileImage", user.ProfileImage); // Aggiungi il parametro per l'immagine del profilo
                var userId = (int)command.ExecuteScalar();

                user.UserID = userId;
                return Created(new Uri(Request.RequestUri + "/" + userId), user);
            }
        }

        [HttpPost]
        [Route("api/Users/{id}/ProfileImage")]
        public IHttpActionResult AddProfileImage(int id)
        {
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                var postedFile = httpRequest.Files[0];

                // Genera un nome univoco per l'immagine del profilo
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(postedFile.FileName);

                // Salva l'immagine del profilo nella directory delle immagini del profilo
                var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/ProfileImages/"), fileName);
                postedFile.SaveAs(filePath);

                // Aggiorna il percorso dell'immagine del profilo nel database
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    var command = new SqlCommand(@"UPDATE Users 
                               SET ProfileImage = @ProfileImage 
                               WHERE UserID = @UserID", conn);
                    command.Parameters.AddWithValue("@ProfileImage", "Content/ProfileImages/" + fileName);
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
            else
            {
                return BadRequest("Nessun file caricato.");
            }
        }


        public IHttpActionResult Put(int id, [FromBody] User user)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var command = new SqlCommand(@"UPDATE Users 
                                               SET Nome = @Nome, Cognome = @Cognome, Username = @Username, 
                                                   Email = @Email, ProfileImage = @ProfileImage 
                                               WHERE UserID = @UserID", conn);
                command.Parameters.AddWithValue("@Nome", user.Nome);
                command.Parameters.AddWithValue("@Cognome", user.Cognome);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@ProfileImage", user.ProfileImage);
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
