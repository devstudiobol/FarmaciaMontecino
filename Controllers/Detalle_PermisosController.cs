using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Farmacia.Context;
using Farmacia.Models;
using Farmacia.DTO;

namespace Farmacia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Detalle_PermisosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Detalle_PermisosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ListarDetallePermisosActivos")]
        public async Task<ActionResult<IEnumerable<Detalle_Permiso>>> ListarDetallePermisosActivos()
        {
            // Filtrar con estado "Activo"
            var detallePermisosActivos = await _context.Detalle_Permisos
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return detallePermisosActivos;
        }

		[HttpGet]
		[Route("ListarDetallePermisosActivosUsuario")]
		public async Task<ActionResult<IEnumerable<Detalle_Permiso>>> ListarDetallePermisosActivosUsuario(int id)
		{
			// Filtrar con estado "Activo"
			var detallePermisosActivos = await _context.Detalle_Permisos
				.Where(e => e.idusuario == id)
				.ToListAsync();

			// Retornar la lista de activos
			return detallePermisosActivos;
		}


		// GET: api/Detalle_Permisos
		[HttpGet]
        public async Task<ActionResult<IEnumerable<Detalle_Permiso>>> GetDetalle_Permisos()
        {
            return await _context.Detalle_Permisos.ToListAsync();
        }

        // GET: api/Detalle_Permisos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Detalle_Permiso>> GetDetalle_Permiso(int id)
        {
            var detalle_Permiso = await _context.Detalle_Permisos.FindAsync(id);

            if (detalle_Permiso == null)
            {
                return NotFound();
            }

            return detalle_Permiso;
        }

        // PUT: api/Detalle_Permisos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarPermiso(int id, int idpermiso, int idusuario)
        {
            // Busca la persona por su ID
            var detallePermisoActual = await _context.Detalle_Permisos.FindAsync(id);

            if (detallePermisoActual == null)
            {
                return NotFound("El Detalle Permiso no fue encontrado.");
            }

            // Actualiza los campos con los nuevos valores
            detallePermisoActual.idpermiso = idpermiso;
            detallePermisoActual.idusuario = idusuario;



            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(detallePermisoActual);
        }

		// POST: api/Permisos
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost("Crear")]
		public async Task<IActionResult> CrearDetallePermiso([FromBody] AsignarPermisosRequest request)
		{
			if (request == null || request.Permisos == null || !request.Permisos.Any())
			{
				return BadRequest("La solicitud no es válida.");
			}

			try
			{
				// Obtener los permisos actuales del usuario
				var permisosActuales = await _context.Detalle_Permisos
					.Where(dp => dp.idusuario == request.IdUsuario)
					.ToListAsync();

				// Eliminar permisos que ya no están seleccionados
				var permisosAEliminar = permisosActuales
					.Where(pa => !request.Permisos.Contains(pa.idpermiso))
					.ToList();

				if (permisosAEliminar.Any())
				{
					_context.Detalle_Permisos.RemoveRange(permisosAEliminar);
					await _context.SaveChangesAsync();
				}

				// Agregar nuevos permisos
				var permisosAAgregar = request.Permisos
					.Where(id => !permisosActuales.Any(pa => pa.idpermiso == id))
					.Select(id => new Detalle_Permiso
					{
						idpermiso = id,
						idusuario = request.IdUsuario,
						Estado = "Activo"
					})
					.ToList();

				if (permisosAAgregar.Any())
				{
					await _context.Detalle_Permisos.AddRangeAsync(permisosAAgregar);
					await _context.SaveChangesAsync();
				}

				return Ok(new { Message = "Permisos asignados correctamente." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error interno del servidor: {ex.Message}");
			}
		}
		[HttpPost("ActualizarPermisos")]
		public IActionResult ActualizarPermisos([FromBody] UpdatePermissionsRequest request)
		{
			try
			{
				// Lógica para añadir permisos
				foreach (var permisoId in request.AddedPermissions)
				{
					// Añadir permiso al usuario
				}

				// Lógica para eliminar permisos
				foreach (var permisoId in request.RemovedPermissions)
				{
					// Eliminar permiso del usuario
				}

				return Ok("Permisos actualizados exitosamente");
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error al actualizar los permisos");
			}
		}

		public class UpdatePermissionsRequest
		{
			public int IdUsuario { get; set; }
			public List<int> AddedPermissions { get; set; }
			public List<int> RemovedPermissions { get; set; }
		}
		// DELETE: api/Permisos/5
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetallePermiso(int id)
        {
            var detallePermiso = await _context.Detalle_Permisos.FindAsync(id);

            if (detallePermiso == null)
            {
                return NotFound("El Detalle Permiso no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            detallePermiso.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El Detalle Permiso ha sido desactivado." });
        }
    }
}
