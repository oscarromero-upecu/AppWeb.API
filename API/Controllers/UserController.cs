
using API.Data;
using API.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly DataContext _DbContext;
        public UserController(DataContext dataContext)
        {
            _DbContext = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> ObtenerUsuarios()
        {
            return await _DbContext.Usuarios.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> ObtenerUsuario(int id)
        {
            var user = await _DbContext.Usuarios.FindAsync(id);

            if (user == null) return BadRequest("Usuario no existe");

            return user;
        }
    }
}