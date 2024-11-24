using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RecycleBuild.API.Controllers
{
    [ApiController]
    public abstract class Base : ControllerBase
    {
        protected int GetIdUsuario()
        {
            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idUsuarioClaim == null)
                throw new UnauthorizedAccessException("Id de Usuário não encontrado no token");

            return int.Parse(idUsuarioClaim.Value);
        }
    }
}
