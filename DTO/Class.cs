namespace Farmacia.DTO
{
    public class AsignarPermisosRequest
    {
        public int IdUsuario { get; set; } // ID del usuario
        public List<int> Permisos { get; set; } // Lista de IDs de permisos
    }
}
