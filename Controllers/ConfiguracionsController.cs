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
    public class ConfiguracionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConfiguracionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ListarConfiguracionActivos")]
        public async Task<ActionResult<IEnumerable<Configuracion>>> ListarConfiguracionesActivos()
        {
            // Filtrar con estado "Activo"
            var configuracionActivos = await _context.Configuraciones
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return configuracionActivos;
        }

        // GET: api/Configuracions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Configuracion>>> GetConfiguraciones()
        {
            return await _context.Configuraciones.ToListAsync();
        }

        // GET: api/Configuracions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Configuracion>> GetConfiguracion(int id)
        {
            var configuracion = await _context.Configuraciones.FindAsync(id);

            if (configuracion == null)
            {
                return NotFound();
            }

            return configuracion;
        }

        // PUT: api/Configuracions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarConfiguracion(int id, string nombre, int telefono, string email, string direccion)
        {
            // Busca la persona por su ID
            var configuracionActual = await _context.Configuraciones.FindAsync(id);

            if (configuracionActual == null)
            {
                return NotFound("La configuracion no fue encontrada.");
            }

            // Actualiza los campos con los nuevos valores
            configuracionActual.Nombre = nombre;
            configuracionActual.Telefono = telefono;
            configuracionActual.Email = email;
            configuracionActual.Direccion = direccion;


            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(configuracionActual);
        }

        // POST: api/Configuracions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearConfiguracion(string nombre, int telefono, string email, string direccion)
        {
            Configuracion configuracion = new Configuracion()
            {
                Nombre = nombre,
                Telefono = telefono,
                Email = email,
                Direccion = direccion,
                Estado = "Activo"
            };
            await _context.Configuraciones.AddAsync(configuracion);
            await _context.SaveChangesAsync();
            return Ok(configuracion);
        }

        // DELETE: api/Configuracions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfiguracion(int id)
        {
            var configuracion = await _context.Configuraciones.FindAsync(id);

            if (configuracion == null)
            {
                return NotFound("La configuracion no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            configuracion.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "La configuracion ha sido desactivado." });
        }
    }
}
