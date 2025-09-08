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
using System.Globalization;

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
		[HttpGet("listarProductosConMenorStockHome")]
		public async Task<ActionResult<List<object>>> ListarProductosConMenorStockHome()
		{
			try
			{
				// Obtener los productos ordenados por stock (de menor a mayor) con información de presentación y laboratorio
				var productos = await _context.Productos
					.Where(p => p.Stock > 0 && p.Estado == "Activo")
					.OrderBy(p => p.Stock)
					.Take(5)
					.Join(_context.Presentaciones,
						producto => producto.idpresentacion,
						presentacion => presentacion.Id,
						(producto, presentacion) => new
						{
							producto.Codigo,
							producto.Nombre,
							producto.Vencimiento,
							producto.idtipo,
							producto.idpresentacion,
							producto.Precio,
							producto.Stock,
							PresentacionNombre = presentacion.Nombre,
							LaboratorioNombre = _context.Laboratorios
								.Where(l => l.Id == producto.idlaboratorio) // Ajusta según el nombre de tu campo
								.Select(l => l.LaboratorioNombre)
								.FirstOrDefault() ?? "Sin laboratorio"
						})
					.ToListAsync();

				// Si no hay productos, retornar un mensaje
				if (productos == null || !productos.Any())
				{
					return NotFound("No se encontraron productos con stock disponible.");
				}

				// Retornar la lista de productos
				return Ok(productos);
			}
			catch (Exception ex)
			{
				// Capturar la excepción y retornar un error 500 con detalles
				return StatusCode(500, $"Error interno del servidor: {ex.Message}");
			}
		}
		[HttpGet("listarProductosConMenorStock")]
		public async Task<ActionResult<List<Producto>>> ListarProductosConMenorStock()
		{
			try
			{
				// Obtener los productos ordenados por stock (de menor a mayor)
				var productos = await _context.Productos
					.Where(p => p.Stock > 0 && p.Estado == "Activo") // Filtra solo productos con stock mayor a 0
					.OrderBy(p => p.Stock)   // Ordena por stock de menor a mayor
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

		[HttpGet("ProductosProximosAVencer")]
		public async Task<ActionResult<IEnumerable<object>>> GetProductosProximosAVencer()
		{
			try
			{
				// Obtener la fecha actual
				var fechaActual = DateTime.Now;

				// Calcular la fecha límite (fecha actual + días especificados)
				var fechaLimite = fechaActual.AddDays(90);

				// Obtener todos los productos activos incluyendo las relaciones con Laboratorio y Presentacion
				var productos = await _context.Productos
					.Include(p => p.Laboratorio) // Incluir la relación con Laboratorio
					.Include(p => p.Presentacion) // Incluir la relación con Presentacion
					.Where(p => p.Estado == "Activo" && !string.IsNullOrEmpty(p.Vencimiento))
					.ToListAsync();

				// Filtrar productos que están próximos a vencer y tomar los 10 más cercanos
				var productosProximosAVencer = productos
					.Select(p =>
					{
						// Intentar parsear la fecha de vencimiento (asumiendo formato yyyy-MM-dd)
						if (DateTime.TryParseExact(p.Vencimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaVencimiento))
						{
							return new
							{
								Producto = p,
								FechaVencimiento = fechaVencimiento,
								DiasParaVencer = (fechaVencimiento - fechaActual).Days
							};
						}
						return null;
					})
					.Where(p => p != null &&
							   p.FechaVencimiento >= fechaActual &&
							   p.FechaVencimiento <= fechaLimite)
					.OrderBy(p => p.FechaVencimiento) // Ordenar por fecha de vencimiento más próxima
					.Take(10) // Tomar solo los 10 productos más próximos a vencer
					.Select(p => new
					{
						p.Producto.Id,
						p.Producto.Nombre,
						p.Producto.Descripcion,
						p.Producto.Stock,
						FechaVencimiento = p.FechaVencimiento.ToString("yyyy-MM-dd"),
						DiasParaVencer = p.DiasParaVencer,
						p.Producto.Precio,
						// Obtener nombre del laboratorio (manejar nulls)
						Laboratorio = p.Producto.Laboratorio != null ? p.Producto.Laboratorio.LaboratorioNombre : "Sin laboratorio",
						// Obtener nombre de la presentación (manejar nulls)
						Presentacion = p.Producto.Presentacion != null ? p.Producto.Presentacion.Nombre : "Sin presentación",
						Estado = p.DiasParaVencer <= 7 ? "Crítico" :
								 p.DiasParaVencer <= 15 ? "Advertencia" : "Normal"
					})
					.ToList();

				return Ok(productosProximosAVencer);
			}
			catch (Exception ex)
			{
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
	 int idtipo,
   int precio_compra)        // Añadido
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
   productoActual.Precio_compra = precio_compra;

			await _context.SaveChangesAsync();

			return Ok(productoActual);
		}

		// POST: api/Permisos
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		[Route("Crear")]
		public async Task<IActionResult> CrearProducto(string codigo, string nombre, string descripcion, int precio, int stock, string vencimiento, int idtipo, int idlaboratorio, int concentracion, string casilla, int idpresentacion,int precio_compra)
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
				casilla = casilla,
				idpresentacion = idpresentacion,
				Estado = "Activo",
	Precio_compra = precio_compra
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
