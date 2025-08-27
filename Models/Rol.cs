using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Farmacia.Models
{
    public class Rol
    {
        public int Id { get; set; }
        public string Cargo { get; set; }
        public string Estado { get; set; }
        public ICollection<Usuario> Usuarios { get; set; }

    }
}
