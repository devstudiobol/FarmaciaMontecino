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
    public class Detalle_VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Detalle_VentasController(AppDbContext context)
        {
            _context = context;
        }

		[HttpGet]
		[Route("ProductosMasVendidos")]
		public async Task<IActionResult> ObtenerProductosMasVendidos()
		{
			// Agrupar las ventas por idproducto y sumar las cantidades vendidas
			var productosVendidos = await _context.Detalle_Ventas
				.Where(dv=>dv.Estado=="Activo")
				.GroupBy(dv => dv.idproducto)
				.Select(g => new
				{
					ProductoId = g.Key,
					TotalVendido = g.Sum(dv => dv.Cantidad)
				})
				.OrderByDescending(p => p.TotalVendido) // Ordenar de mayor a menor
				.ToListAsync();

			if (productosVendidos == null || !productosVendidos.Any())
			{
				return NotFound("No se encontraron ventas.");
			}

			// Obtener la información completa de los productos
			var productosIds = productosVendidos.Select(p => p.ProductoId).ToList();
			var productos = await _context.Productos
				.Where(p => productosIds.Contains(p.Id)&& p.Estado == "Activo")
				.ToListAsync();

			// Combinar la información de los productos con las cantidades vendidas
			var resultado = productosVendidos
				.Join(productos,
					  pv => pv.ProductoId,
					  p => p.Id,
					  (pv, p) => new
					  {
						  ProductoId = p.Id,
						  Nombre = p.Nombre,
						  TotalVendido = pv.TotalVendido
					  })
				.OrderByDescending(p => p.TotalVendido) // Ordenar nuevamente por si acaso
				.ToList();

			return Ok(resultado);
		}

		[HttpGet]
        [Route("ListarDetalleVentasActivos")]
        public async Task<ActionResult<IEnumerable<Detalle_Venta>>> ListarDetallesVentasActivos()
        {
            // Filtrar con estado "Activo"
            var detalleVentasActivos = await _context.Detalle_Ventas
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return detalleVentasActivos;
        }

        // GET: api/Detalle_Ventas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Detalle_Venta>>> GetDetalle_Ventas()
        {
            return await _context.Detalle_Ventas.ToListAsync();
        }

        // GET: api/Detalle_Ventas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Detalle_Venta>> GetDetalle_Venta(int id)
        {
            var detalle_Venta = await _context.Detalle_Ventas.FindAsync(id);

            if (detalle_Venta == null)
            {
                return NotFound();
            }

            return detalle_Venta;
        }

		

		// PUT: api/Detalle_Ventas/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarDetalleVenta(int id, int cantidad, int descuento, int precio, int total)
        {
            // Buscar por su ID
            var detalleVentaActual = await _context.Detalle_Ventas.FindAsync(id);

            if (detalleVentaActual == null)
            {
                return NotFound("El Detalle Venta no fue encontrado.");
            }

            // Actualiza los campos con los nuevos valores
            detalleVentaActual.Cantidad = cantidad;
            detalleVentaActual.Descuento = descuento;
            detalleVentaActual.Precio = precio;
            detalleVentaActual.Total = total;

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(detalleVentaActual);
        }

		// POST: api/Permisos
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		[Route("Crear")]
		public async Task<IActionResult> CrearDetalleVenta(int cantidad, int descuento, int precio, int total, int idproducto, int idventa)
		{
			// Obtener el producto correspondiente al idproducto
			var producto = await _context.Productos.FindAsync(idproducto);
			if (producto == null)
			{
				return NotFound("Producto no encontrado");
			}

			// Verificar si hay suficiente stock
			if (producto.Stock < cantidad)
			{
				return BadRequest("No hay suficiente stock disponible");
			}

			// Restar la cantidad vendida del stock disponible
			producto.Stock -= cantidad;

			// Actualizar el producto en la base de datos
			_context.Productos.Update(producto);

			// Crear el detalle de venta
			Detalle_Venta detalleVenta = new Detalle_Venta()
			{
				Cantidad = cantidad,
				Descuento = descuento,
				Precio = precio,
				Total = total,
				idproducto = idproducto,
				idventa = idventa,
				Estado = "Activo"
			};

			// Agregar el detalle de venta a la base de datos
			await _context.Detalle_Ventas.AddAsync(detalleVenta);

			// Guardar los cambios en la base de datos
			await _context.SaveChangesAsync();

			return Ok(detalleVenta);
		}


		// DELETE: api/Permisos/5
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetalleVenta(int id)
        {
            var detalleVenta = await _context.Detalle_Ventas.FindAsync(id);

            if (detalleVenta == null)
            {
                return NotFound("El Detalle Venta no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            detalleVenta.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El Detalle Venta ha sido desactivado." });
        }
    }
}
