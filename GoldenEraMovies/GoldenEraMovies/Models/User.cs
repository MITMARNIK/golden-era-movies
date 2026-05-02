using System.ComponentModel.DataAnnotations;

namespace GoldenEraMovies.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Utilizatorul este obligatoriu.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Utilizatorul trebuie sa aiba intre 3 si 50 de caractere.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Parola este obligatorie.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Parola trebuie sa aiba cel putin 6 caractere.")]
        public string Password { get; set; }
    }
}
