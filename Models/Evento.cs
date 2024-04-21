using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Since1999.Models
{
    public class Evento
    {
        public int EventiID { get; set; }
        public string Titolo { get; set; }
        public DateTime Data { get; set; }
        public string Luogo { get; set; }
        public string Descrizione { get; set; }
        public string ImageURL { get; set; }

    }
}