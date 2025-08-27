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
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ListarClientesActivos")]
        public async Task<ActionResult<IEnumerable<Cliente>>> ListarClientesActivos()
        {
            // Filtrar con estado "Activo"
            var clientesActivos = await _context.Clientes
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return clientesActivos;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes.ToListAsync();
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        // PUT: api/Clientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarCliente(int id, string nombre, string apellido, int telefono, string direccion)
        {
            // Busca la persona por su ID
            var clienteActual = await _context.Clientes.FindAsync(id);

            if (clienteActual == null)
            {
                return NotFound("La configuracion no fue encontrada.");
            }

            // Actualiza los campos con los nuevos valores
            clienteActual.Nombre = nombre;
            clienteActual.Apellido = apellido;
            clienteActual.Telefono = telefono;
            clienteActual.Direccion = direccion;


            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(clienteActual);
        }

		// POST: api/Clientes
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost("Crear")]
		public IActionResult CrearCliente(
		[FromQuery] string nombre,
		[FromQuery] string apellido,
		[FromQuery] int telefono,
		[FromQuery] string direccion)
		{
			var cliente = new Cliente
			{
				Nombre = nombre,
				Apellido = apellido,
				Telefono = telefono,
				Direccion = direccion,
				Estado = "Activo"
			};

			_context.Clientes.Add(cliente);
			_context.SaveChanges();

			return Ok(cliente);
		}

		// DELETE: api/Clientes/5
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var Cliente = await _context.Clientes.FindAsync(id);

            if (Cliente == null)
            {
                return NotFound("El cliente no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            Cliente.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El cliente ha sido desactivado." });
        }
    }
}
