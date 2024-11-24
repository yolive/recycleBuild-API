using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecycleBuild.API.Data;
using RecycleBuild.API.DTO;
using RecycleBuild.API.Models;

namespace RecycleBuild.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class Reciclagem : Base
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public Reciclagem(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<LixoRecicladoDto>> AdicionarReciclagem(LixoRecicladoDto dto)
        {
            try
            {
                var idUsuario = GetIdUsuario();

                var lixoReciclado = new LixoReciclado
                {
                    IdUsuario = idUsuario,
                    Peso = dto.Peso,
                    DataReciclagem = DateTime.UtcNow.AddHours(-3)
                };

                _context.LixoReciclado.Add(lixoReciclado);
                await _context.SaveChangesAsync();

                return Ok(_mapper.Map<LixoRecicladoDto>(lixoReciclado));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex + "Erro interno ao processar a requisição");
            }

        }

        [HttpGet("total-mensal")]
        public async Task<ActionResult<PerfilUsuarioDto>> GetTotalMensal()
        {
            try
            {
                var idUsuario = GetIdUsuario();
                var agora = DateTime.UtcNow;
                var usuario = await _context.Usuario
                    .FirstOrDefaultAsync(u => u.Id == idUsuario);

                if (usuario == null)
                    return NotFound("Usuário não encontrado");

                var total = await _context.LixoReciclado
                    .Where(lr => lr.IdUsuario == idUsuario &&
                           lr.DataReciclagem.Month == agora.Month &&
                           lr.DataReciclagem.Year == agora.Year)
                    .SumAsync(lr => lr.Peso);                

                var perfil = new PerfilUsuarioDto
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    Nome = usuario.Nome,
                    TotalMensal = total,
                    PontosMes = (int)total
                };

                return Ok(perfil);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex + "Erro interno ao processar a requisição");
            }
        }

        [HttpGet("top-doadores")]
        public async Task<ActionResult<TopDoadoresDto>> GetTopDoadores()
        {
            try
            {
                var agora = DateTime.UtcNow;

                var topDoadores = await _context.LixoReciclado
                    .Where(lr => lr.DataReciclagem.Month == agora.Month && lr.DataReciclagem.Year == agora.Year)
                    .GroupBy(lr => lr.IdUsuario)
                    .Select(g => new
                    {
                        IdUsuario = g.Key,
                        NomeUsuario = g.First().Usuario.Nome,
                        Total = g.Sum(lr => lr.Peso)
                    })
                    .OrderByDescending(x => x.Total)
                    .Take(5)
                    .ToListAsync();

                return Ok(topDoadores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex + "Erro interno ao processar a requisição");
            }

        }
    }
}
