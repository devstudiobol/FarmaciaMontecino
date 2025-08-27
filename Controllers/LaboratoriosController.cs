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
	public class LaboratoriosController : ControllerBase
	{
		private readonly AppDbContext _context;

		public LaboratoriosController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet("ListarLaboratoriosActivos")]
		public async Task<ActionResult<IEnumerable<Laboratorio>>> ListarLaboratoriosActivos()
		{
			// Filtrar con estado "Activo"
			var laboratoriosActivos = await _context.Laboratorios
				.Where(e => e.Estado == "Activo")
				.ToListAsync();

			// Retornar la lista de activos
			return laboratoriosActivos;
		}

		// GET: api/Laboratorios
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Laboratorio>>> GetLaboratorios()
		{
			return await _context.Laboratorios.ToListAsync();
		}

		// GET: api/Laboratorios/5
		[HttpGet("ObtenerPorId/{id}")] // Ruta modificada para evitar conflicto
		public async Task<ActionResult<Laboratorio>> GetLaboratorio(int id)
		{
			var laboratorio = await _context.Laboratorios.FindAsync(id);

			if (laboratorio == null)
			{
				return NotFound();
			}

			return laboratorio;
		}

		[HttpPut("Actualizar")]
		public async Task<IActionResult> ActualizarLaboratorio(
		 [FromQuery] int id,
		 [FromQuery] string laboratorionombre,
		 [FromQuery] string direccion)
		{
			// 1. Verifica que los parámetros lleguen correctamente
			if (string.IsNullOrEmpty(laboratorionombre) || string.IsNullOrEmpty(direccion))
				return BadRequest("Nombre y dirección son requeridos");

			// 2. Busca el laboratorio (con tracking explícito)
			var laboratorioActual = await _context.Laboratorios
				.AsTracking()
				.FirstOrDefaultAsync(l => l.Id == id);

			if (laboratorioActual == null)
				return NotFound("Laboratorio no encontrado");

			// 3. Actualiza los campos
			laboratorioActual.LaboratorioNombre = laboratorionombre;
			laboratorioActual.Direccion = direccion;

			// 4. Guarda y verifica cambios
			try
			{
				int cambios = await _context.SaveChangesAsync();
				if (cambios > 0)
					return Ok(laboratorioActual);
				else
					return StatusCode(500, "No se realizaron cambios en la base de datos");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error interno: {ex.Message}");
			}
		}
		// POST: api/Laboratorios/Crear
		[HttpPost("Crear")] // Se mantiene igual
		public IActionResult CrearLaboratorio(
			[FromQuery] string laboratorionombre,
			[FromQuery] string direccion
		)
		{
			Laboratorio laboratorio = new Laboratorio
			{
				LaboratorioNombre = laboratorionombre,
				Direccion = direccion,
				Estado = "Activo"
			};

			_context.Laboratorios.Add(laboratorio);
			_context.SaveChanges();

			return Ok(laboratorio);
		}

		// DELETE: api/Laboratorios/5
		[HttpDelete("{id}")] // Ruta más descriptiva
		public async Task<IActionResult> DeleteLaboratorio(int id)
		{
			var tipo = await _context.Laboratorios.FindAsync(id);

			if (tipo == null)
			{
				return NotFound("El Laboratorio no fue encontrado.");
			}

			// Cambiar el estado a "Inactivo" en lugar de eliminar
			tipo.Estado = "Inactivo";

			// Guardar los cambios en la base de datos
			await _context.SaveChangesAsync();

			return Ok(new { message = "El Laboratorio ha sido desactivado." });
		}
	}
}