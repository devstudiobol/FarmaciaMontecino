using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Farmacia.Context;
using Farmacia.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Farmacia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ListarProductosActivos")]
        public async Task<ActionResult<IEnumerable<Producto>>> ListarProductosActivos()
        {
            // Filtrar con estado "Activo"
            var productosActivos = await _context.Productos
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return productosActivos;
        }

		[HttpGet("listarProductosConMenorStock")]
		public async Task<ActionResult<List<Producto>>> ListarProductosConMenorStock()
		{
			try
			{
				// Obtener los productos ordenados por stock (de menor a mayor)
				var productos = await _context.Productos
					.Where(p => p.Stock > 0 && p.Estado=="Activo") // Filtra solo productos con stock mayor a 0
					.OrderBy(p => p.Stock)   // Ordena por stock de menor a mayor
					.Take(5)
                    .ToListAsync();

				// Si no hay productos, retornar un mensaje
				if (productos == null || !productos.Any())
				{
					return NotFound("No se encontraron productos con stock disponible.");
				}

				// Retornar la lista de productos
				return productos;
			}
			catch (Exception ex)
			{
				// Capturar la excepción y retornar un error 500 con detalles
				return StatusCode(500, $"Error interno del servidor: {ex.Message}");
			}
		}


		// GET: api/Productos
		[HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

		// PUT: api/Productos/5
		[HttpPut]
		[Route("Actualizar")]
		public async Task<IActionResult> ActualizarProducto(
	 int id,
	 string codigo,
	 string nombre,
	 string descripcion,
	 int precio,
	 int stock,
	 int concentracion,
	 int casilla, // Cambiado de string a int
	 string vencimiento,
	 int idpresentacion, // Añadido
	 int idlaboratorio,  // Añadido
	 int idtipo)        // Añadido
		{
			var productoActual = await _context.Productos.FindAsync(id);

			if (productoActual == null)
			{
				return NotFound("El Producto no fue encontrado.");
			}

			// Actualiza todos los campos
			productoActual.Codigo = codigo;
			productoActual.Nombre = nombre;
			productoActual.Descripcion = descripcion;
			productoActual.Precio = precio;
			productoActual.Stock = stock;
			productoActual.concentracion = concentracion;
			productoActual.casilla = casilla.ToString(); // Convertir a string si es necesario
			productoActual.Vencimiento = vencimiento;
			productoActual.idpresentacion = idpresentacion;
			productoActual.idlaboratorio = idlaboratorio;
			productoActual.idtipo = idtipo;

			await _context.SaveChangesAsync();

			return Ok(productoActual);
		}
		// POST: api/Permisos
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearProducto(string codigo, string nombre,string descripcion, int precio, int stock, string vencimiento, int idtipo, int idlaboratorio,int concentracion,string casilla, int idpresentacion)
        {
            Producto producto = new Producto()
            {
                Codigo = codigo,
                Nombre = nombre,
                Descripcion = descripcion,
                Precio = precio,
                Stock = stock,
                Vencimiento = vencimiento,
                idtipo = idtipo,
                idlaboratorio = idlaboratorio,
                concentracion = concentracion,
                casilla= casilla,
                idpresentacion = idpresentacion,
                Estado = "Activo"
            };
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
            return Ok(producto);
        }

        // DELETE: api/Permisos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound("El Producto no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            producto.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El Producto ha sido desactivado." });
        }
    }
}
