using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GoldenEraMovies.Models
{
    public class User : IdentityUser<int>
    {
        public string? ProfilePictureUrl { get; set; }
    }
}
