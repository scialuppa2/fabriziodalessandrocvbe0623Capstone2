using Since1999.Filters;
using Since1999.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Since1999.Controllers
{
    
    public class TourStadi2022Controller : ApiController
    {
        // GET: api/TourStadi2022
        public IEnumerable<TourStadi2022> Get()
        {
            string connString = ConfigurationManager.ConnectionStrings["Since1999Context"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();

            var command = new SqlCommand(@"SELECT * FROM TourStadi2022", conn);
            var reader = command.ExecuteReader();

            List<TourStadi2022> concerts = new List<TourStadi2022>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var concert = new TourStadi2022();
                    concert.ConcertID = (int)reader["ConcertID"];
                    concert.Title = (string)reader["Title"];
                    concert.Date = (DateTime)reader["Date"];
                    concert.Location = (string)reader["Location"];
                    concert.Description = (string)reader["Description"];
                    concert.ImageURL = (string)reader["ImageURL"];
                    concerts.Add(concert);
                }
            }

            return concerts;
        }

        // GET: api/TourStadi2022/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TourStadi2022
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/TourStadi2022/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/TourStadi2022/5
        public void Delete(int id)
        {
        }
    }
}
