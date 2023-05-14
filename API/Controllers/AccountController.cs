using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    
    public class AccountController : BaseApiController
    {
        private readonly DataContext _datacontext;

        public AccountController(DataContext context)
        {
            _datacontext = context;
        }

       [HttpPost("Register")]
        public async Task<ActionResult<UsuarioResponseDTO>> Registro(UsuarioRequestDTO usuarioDTO)
        {
           if(await ValidarUsuario (usuarioDTO.Nombre))
           return BadRequest("Usuario ya existe");


            using var hmac = new HMACSHA512();

            var usuario = new Usuario{
                Nombre = usuarioDTO.Nombre.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(usuarioDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _datacontext.Usuarios.Add(usuario);
            await _datacontext.SaveChangesAsync();

            return new UsuarioResponseDTO
            {
                Nombre = usuario.Nombre
            };
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO loginDTO)
        {
            //find busca por clave primaria id
            //firts busca el primer registro que coincida si no encuentra da error
            //firtsOrFoult retorna null
            //single busca un unico regostro que coincida, si este exite varias veces genera error
            //singleOrDefoult si exitemas de un registro se cae, si no devuelve null
            var usuario = await _datacontext.Usuarios.FirstOrDefaultAsync(u => u.Nombre == loginDTO.Nombre);
            if (usuario == null) return Unauthorized();

            using var hmac = new HMACSHA512(usuario.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
         
            for (int i=0 ; i< computeHash.Length; i++){
                if (usuario.PasswordHash[i] != computeHash[i]) return Unauthorized();
            }

            return new LoginResponseDTO{
                NombreUsuario = usuario.Nombre
            };


        }
        private async Task<bool> ValidarUsuario(string NombreUsuario)
        {
            return await _datacontext.Usuarios.AnyAsync(u => u.Nombre == NombreUsuario);
        }
    }
}