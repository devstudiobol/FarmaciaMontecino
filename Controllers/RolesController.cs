using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Farmacia.Context;
using Farmacia.Models;

namespace Farmacia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ListarRolesActivos")]
        public async Task<ActionResult<IEnumerable<Rol>>> ListarRolesActivos()
        {
            // Filtrar con estado "Activo"
            var rolesActivos = await _context.Roles
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return rolesActivos;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetRol()
        {
            return await _context.Roles.ToListAsync();
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> GetRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);

            if (rol == null)
            {
                return NotFound();
            }

            return rol;
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarRol(int id, string cargo)
        {
            // Busca la persona por su ID
            var rolActual = await _context.Roles.FindAsync(id);

            if (rolActual == null)
            {
                return NotFound("La configuracion no fue encontrado.");
            }

            // Actualiza los campos con los nuevos valores
            rolActual.Cargo = cargo;



            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(rolActual);
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearRol(string cargo)
        {
            Rol rol = new Rol()
            {
                Cargo = cargo,
                Estado = "Activo"
            };
            await _context.Roles.AddAsync(rol);
            await _context.SaveChangesAsync();
            return Ok(rol);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);

            if (rol == null)
            {
                return NotFound("El Rol no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            rol.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El Rol ha sido desactivado." });
        }
    }
}
