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
    public class PresentacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PresentacionesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ListarPresentacionesActivos")]
        public async Task<ActionResult<IEnumerable<Presentacion>>> ListarPresentacionesActivos()
        {
            // Filtrar con estado "Activo"
            var presentacionesActivos = await _context.Presentaciones
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return presentacionesActivos;
        }

        // GET: api/Presentaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Presentacion>>> GetPresentaciones()
        {
            return await _context.Presentaciones.ToListAsync();
        }

        // GET: api/Presentaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Presentacion>> GetPresentacion(int id)
        {
            var presentacion = await _context.Presentaciones.FindAsync(id);

            if (presentacion == null)
            {
                return NotFound();
            }

            return presentacion;
        }

        // PUT: api/Presentaciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarPresentacion(int id, string nombre, string nombrecorto)
        {
            // Busca la persona por su ID
            var presentacionActual = await _context.Presentaciones.FindAsync(id);

            if (presentacionActual == null)
            {
                return NotFound("La presentacion no fue encontrado.");
            }

            // Actualiza los campos con los nuevos valores
            presentacionActual.Nombre = nombre;
            presentacionActual.NombreCorto = nombrecorto;



            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(presentacionActual);
        }

        // POST: api/Permisos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearPresentacion(string nombre, string nombrecorto)
        {
            Presentacion presentacion = new Presentacion()
            {
                Nombre = nombre,
                NombreCorto = nombrecorto,
                Estado = "Activo"
            };
            await _context.Presentaciones.AddAsync(presentacion);
            await _context.SaveChangesAsync();
            return Ok(presentacion);
        }

        // DELETE: api/Permisos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePresentacion(int id)
        {
            var presentacion = await _context.Presentaciones.FindAsync(id);

            if (presentacion == null)
            {
                return NotFound("La presentacion no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            presentacion.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "La presentacion ha sido desactivado." });
        }
    }
}
