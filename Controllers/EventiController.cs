using Newtonsoft.Json;
using Since1999.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Since1999.Controllers
{
    public class EventiController : ApiController
    {
        // Connessione al database
        private readonly string connString = ConfigurationManager.ConnectionStrings["Since1999Context"].ToString();

        // GET: api/Eventi
        [HttpGet]
        public HttpResponseMessage GetEventi()
        {
            DataTable eventiTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                string query = "SELECT * FROM Eventi";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(eventiTable);
                }
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(eventiTable), Encoding.UTF8, "application/json");
            return response;
        }


        // Aggiungi altri metodi per gestire le altre operazioni CRUD sugli eventi
    }
}
