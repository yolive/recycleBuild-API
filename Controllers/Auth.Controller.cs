using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecycleBuild.API.Data;
using RecycleBuild.API.DTO;
using RecycleBuild.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecycleBuild.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Auth : Base
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public Auth(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("registro")]
        public async  Task<IActionResult> Registro(UsuarioDto usuarioDto)
        {
            if (await _context.Usuario.AnyAsync(u => u.Email == usuarioDto.Email))
                return BadRequest("E-mail já registrado.");

            var usuario = new Usuario
            {
                Id = usuarioDto.Id,
                Email = usuarioDto.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha),
                Nome = usuarioDto.Nome
            };

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuário registrado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, usuario.Senha))
                return Unauthorized();

            var token = GenerateJwtToken(usuario);
            var usuarioResponse = new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Senha = "restrita"

            };

            return Ok(new { token, usuario = usuarioResponse });
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Name, usuario.Nome)
                },
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
