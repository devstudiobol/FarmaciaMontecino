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
    public class Detalle_TempsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Detalle_TempsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ListarProductosActivos")]
        public async Task<ActionResult<IEnumerable<Detalle_Temp>>> ListarDetalleTemporalesActivos()
        {
            // Filtrar con estado "Activo"
            var detalleTempsActivos = await _context.Detalle_Temp
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return detalleTempsActivos;
        }

        // GET: api/Detalle_Temps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Detalle_Temp>>> GetDetalle_Temp()
        {
            return await _context.Detalle_Temp.ToListAsync();
        }

        // GET: api/Detalle_Temps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Detalle_Temp>> GetDetalle_Temp(int id)
        {
            var detalle_Temp = await _context.Detalle_Temp.FindAsync(id);

            if (detalle_Temp == null)
            {
                return NotFound();
            }

            return detalle_Temp;
        }

        // PUT: api/Detalle_Temps/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarDetalleVenta(int id, int cantidad, int descuento, int precio, int total)
        {
            // Buscar por su ID
            var detalleTempActual = await _context.Detalle_Temp.FindAsync(id);

            if (detalleTempActual == null)
            {
                return NotFound("El Detalle Temporal no fue encontrado.");
            }

            // Actualiza los campos con los nuevos valores
            detalleTempActual.Cantidad = cantidad;
            detalleTempActual.Descuento = descuento;
            detalleTempActual.Precio = precio;
            detalleTempActual.Total = total;

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(detalleTempActual);
        }

        // POST: api/Permisos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearDetalleVenta(int cantidad, int descuento, int precio, int total, int idproducto, int idusuario)
        {
            Detalle_Temp detalleTemp = new Detalle_Temp()
            {
                Cantidad = cantidad,
                Descuento = descuento,
                Precio = precio,
                Total = total,
                idproducto = idproducto,
                idusuario = idusuario,
                Estado = "Activo"
            };
            await _context.Detalle_Temp.AddAsync(detalleTemp);
            await _context.SaveChangesAsync();
            return Ok(detalleTemp);
        }

        // DELETE: api/Permisos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetalleTemporal(int id)
        {
            var detalleTemp = await _context.Detalle_Temp.FindAsync(id);

            if (detalleTemp == null)
            {
                return NotFound("El Detalle Temporal no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            detalleTemp.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El Detalle Temporal ha sido desactivado." });
        }
    }
}
