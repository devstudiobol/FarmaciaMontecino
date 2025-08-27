using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Farmacia.Context;
using Farmacia.Models;
using System.Globalization;

namespace Farmacia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

		[HttpGet("listarVentaDetalleVenta")]
		public async Task<ActionResult<List<Detalle_Venta>>> listarDetalle_Venta(int idventa)
		{
			try
			{
				// Verificar si la venta existe
				var venta = await _context.Ventas.FindAsync(idventa);
				if (venta == null)
				{
					return NotFound("Venta no encontrada");
				}

				// Buscar todos los detalles de venta que tengan el
                //
                //
                // mismo idventa
				var detalles_Venta = await _context.Detalle_Ventas
					.Where(d => d.idventa == idventa) // Filtra por idventa
					.ToListAsync();

				// Si no se encuentran detalles de venta, retornar un mensaje
				if (detalles_Venta == null || !detalles_Venta.Any())
				{
					return NotFound("No se encontraron detalles de venta para la venta especificada");
				}

				// Retornar la lista de detalles de venta
				return detalles_Venta;
			}
			catch (Exception ex)
			{
				// Capturar la excepción y retornar un error 500 con detalles
				return StatusCode(500, $"Error interno del servidor: {ex.Message}");
			}
		}

		[HttpGet]
		[Route("ListarVentasFecha")]
		public async Task<ActionResult<IEnumerable<object>>> ListarVentasFecha(string fechaIni, string fechafin)
		{
			if (string.IsNullOrWhiteSpace(fechaIni) || string.IsNullOrWhiteSpace(fechafin))
			{
				return BadRequest("Las fechas no pueden estar vacías.");
			}

			// Consulta para obtener las ventas en el rango de fechas
			var ventasConDetalles = await _context.Ventas
				.Where(v => string.Compare(v.Fecha, fechaIni) >= 0 && string.Compare(v.Fecha, fechafin) <= 0 && v.Estado=="Activo" )
				.Select(v => new
				{
					VentaId = v.Id,
					FechaVenta = v.Fecha,
					TotalVenta = v.Total,
					Detalles = _context.Detalle_Ventas
						.Where(dv => dv.idventa == v.Id && dv.Estado=="Activo")
						.Select(dv => new
						{
							ProductoId = dv.idproducto,
							Cantidad = dv.Cantidad,
							PrecioUnitario = dv.Precio,
							TotalDetalle = dv.Cantidad * dv.Precio
						}).ToList()
				}).ToListAsync();

			// Calcular el total de ventas en el rango de fechas
			var totalVentas = ventasConDetalles.Sum(v => v.TotalVenta);

			// Retornar el resultado con los detalles de las ventas y el total de ventas
			return Ok(new
			{
				Ventas = ventasConDetalles,
				TotalVentas = totalVentas
			});
		}































		[HttpGet]
        [Route("ListarVentasActivos")]
        public async Task<ActionResult<IEnumerable<Venta>>> ListarVentasActivos()
        {
            // Filtrar con estado "Activo"
            var ventasActivos = await _context.Ventas
    .Where(e => e.Estado == "Activo")
    .OrderByDescending(e => e.Id) // Ordenar por VentaId de forma descendente
    .ToListAsync();

            // Retornar la lista de activos
            return ventasActivos;
        }

        // GET: api/Ventas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentas()
        {
            return await _context.Ventas.ToListAsync();
        }

        // GET: api/Ventas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Venta>> GetVenta(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);

            if (venta == null)
            {
                return NotFound();
            }

            return venta;
        }

        // PUT: api/Ventas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarVenta(int id, int total, string fecha)
        {
            // Buscar por su ID
            var ventaActual = await _context.Ventas.FindAsync(id);

            if (ventaActual == null)
            {
                return NotFound("La venta no fue encontrada.");
            }

            // Actualiza los campos con los nuevos valores
            ventaActual.Total = total;
            ventaActual.Fecha = fecha;


            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(ventaActual);
        }

        // POST: api/Permisos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearDetalleVenta(int total, string fecha, int idcliente, int idusuario)
        {
            Venta venta = new Venta()
            {
                Total = total,
                Fecha = fecha,
                idcliente = idcliente,
                idusuario = idusuario,
                Estado = "Activo"
            };
            await _context.Ventas.AddAsync(venta);
            await _context.SaveChangesAsync();
            return Ok(venta);
        }
		[HttpGet("SumarTotalesPorMes")]
		public async Task<ActionResult<Dictionary<string, int>>> SumarTotalesPorMes()
		{
			try
			{
				// Obtener todas las ventas activas
				var ventasActivas = await _context.Ventas
					.Where(v => v.Estado == "Activo")
					.ToListAsync();

				// Agrupar las ventas por mes y sumar los totales
var totalesPorMes = ventasActivas
    .GroupBy(v => new { Year = v.Fecha.Substring(0, 4), Month = v.Fecha.Substring(5, 2) })
    .OrderBy(g => g.Key.Year) // Ordenar por año
    .ThenBy(g => g.Key.Month) // Ordenar por mes
    .ToDictionary(
        g => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(g.Key.Month)), // Clave: Nombre del mes
        g => g.Sum(v => v.Total) // Valor: Suma de los totales
    );
				// Retornar el diccionario con los totales por mes
				return Ok(totalesPorMes); // Usar Ok() para devolver un ActionResult
			}
			catch (Exception ex)
			{
				// Capturar la excepción y retornar un error 500 con detalles
				return StatusCode(500, $"Error interno del servidor: {ex.Message}");
			}
		}
		// DELETE: api/Permisos/5
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);

            if (venta == null)
            {
                return NotFound("La Venta no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            venta.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "La Venta ha sido desactivado." });
        }
    }
}
