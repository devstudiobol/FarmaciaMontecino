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
    public class TiposController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TiposController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ListarTiposActivos")]
        public async Task<ActionResult<IEnumerable<Tipo>>> ListarTipoActivos()
        {
            // Filtrar con estado "Activo"
            var tiposActivos = await _context.Tipos
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return tiposActivos;
        }

        // GET: api/Tipos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tipo>>> GetTipos()
        {
            return await _context.Tipos.ToListAsync();
        }

        // GET: api/Tipos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tipo>> GetTipo(int id)
        {
            var tipo = await _context.Tipos.FindAsync(id);

            if (tipo == null)
            {
                return NotFound();
            }

            return tipo;
        }

        // PUT: api/Tipos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarPermiso(int id, string nombre)
        {
            // Busca la persona por su ID
            var tipoActual = await _context.Tipos.FindAsync(id);

            if (tipoActual == null)
            {
                return NotFound("El Tipo no fue encontrado.");
            }

            // Actualiza los campos con los nuevos valores
            tipoActual.Nombre = nombre;



            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(tipoActual);
        }

        // POST: api/Permisos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearTipo(string nombre)
        {
            Tipo tipo = new Tipo()
            {
                Nombre = nombre,
                Estado = "Activo"
            };
            await _context.Tipos.AddAsync(tipo);
            await _context.SaveChangesAsync();
            return Ok(tipo);
        }

        // DELETE: api/Permisos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipo(int id)
        {
            var tipo = await _context.Tipos.FindAsync(id);

            if (tipo == null)
            {
                return NotFound("El Tipo no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            tipo.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El Tipo ha sido desactivado." });
        }
    }
}
