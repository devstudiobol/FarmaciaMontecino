using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Farmacia.Context;
using Farmacia.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Farmacia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IConfiguration _configuration;
        public UsuariosController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("ListarUsuariosActivos")]
        public async Task<ActionResult<IEnumerable<Usuario>>> ListarVentasActivos()
        {
            // Filtrar con estado "Activo"
            var usuarioActivos = await _context.Usuarios
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return usuarioActivos;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }
        //Login

        //Login
		[HttpGet("Login")]
		public async Task<IActionResult> Login(
			[FromQuery] string correo,
			[FromQuery] string password)
		{
			// Validación básica de parámetros
			if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(password))
				return BadRequest(new { message = "Correo y contraseña son requeridos" });

			// Buscar usuario (¡preferiblemente con hash de contraseña!)
			var usuario = await _context.Usuarios
				.SingleOrDefaultAsync(u => u.Correo == correo && u.Password == password);

			if (usuario == null)
				return Unauthorized(new { message = "Credenciales inválidas" });

			// Generar token JWT
			string jwtToken = GenerarToken(usuario);

			return Ok(new
			{
				token = jwtToken,
				id = usuario.Id,
				nombre = usuario.Nombre,
				expiresIn = 3600 // Tiempo de expiración en segundos (1 hora)
			});
		}
		private string GenerarToken(Usuario usuario)
		{
			var claims = new[]
			{
		new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
		new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
		new Claim(JwtRegisteredClaimNames.GivenName, usuario.Nombre),
		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
	};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
				_configuration["Jwt:Key"]));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddHours(1),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarUsuario(int id, string nombre, string correo,string usuarionombre, string password)
        {
            // Busca la persona por su ID
            var usuarioActual = await _context.Usuarios.FindAsync(id);

            if (usuarioActual == null)
            {
                return NotFound("El usuario no fue encontrada.");
            }

            // Actualiza los campos con los nuevos valores
            usuarioActual.Nombre = nombre;
            usuarioActual.Correo = correo;
            usuarioActual.UsuarioNombre = usuarionombre;
            usuarioActual.Correo = correo;
            usuarioActual.Password = password;


            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(usuarioActual);
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

		[HttpPost("Crear")]
		public IActionResult CrearCliente(
		[FromQuery] string nombre,
		[FromQuery] string correo,
		[FromQuery] string usuarionombre,
		[FromQuery] string password,
		[FromQuery] int idrol
				)
		{
			var usuario = new Usuario
			{
				Nombre = nombre,
				Correo = correo,
				UsuarioNombre = usuarionombre,
				Password = password,
                idrol=idrol,
				Estado = "Activo"
			};

			_context.Usuarios.Add(usuario);
			_context.SaveChanges();

			return Ok(usuario);
		}

		// DELETE: api/Usuarios/5
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound("El usuario no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            usuario.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El usuario ha sido desactivado." });
        }
    }
}
