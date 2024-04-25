using System.ComponentModel.DataAnnotations;

namespace Since1999.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Il campo Nome è obbligatorio.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Il campo Cognome è obbligatorio.")]
        public string Cognome { get; set; }

        [Required(ErrorMessage = "Il campo Username è obbligatorio.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Il campo Email è obbligatorio.")]
        [EmailAddress(ErrorMessage = "Il campo Email non è in un formato valido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Il campo Password è obbligatorio.")]
        [MinLength(5, ErrorMessage = "La password deve essere lunga almeno 5 caratteri.")]
        public string PasswordHash { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ProfileImage { get; set; } // Rendere nullable con '?'

    }
}
