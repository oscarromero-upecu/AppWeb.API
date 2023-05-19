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
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseApiController
    {
        private readonly DataContext _datacontext;
        private readonly ITokenServices _tokenService;

        public AccountController(DataContext context, ITokenServices tokenServices)
        {
            _datacontext = context;
            _tokenService = tokenServices;
        }

       [HttpPost("Register")]
        public async Task<ActionResult<UsuarioResponseDTO>> Registro(UsuarioRequestDTO usuarioDTO)
        {
           if(await ValidarUsuario (usuarioDTO.Nombre))
           return BadRequest("Usuario ya existe");


            using var hmac = new HMACSHA512();

            var usuario = new Usuario{
                Nombre = usuarioDTO.Nombre,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(usuarioDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _datacontext.Usuarios.Add(usuario);
            await _datacontext.SaveChangesAsync();

            return new UsuarioResponseDTO
            {
                Nombre = usuario.Nombre,
                Token = _tokenService.CrearToken(usuario)
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
                NombreUsuario = usuario.Nombre,
                Token = _tokenService.CrearToken(usuario)
            };


        }
        private async Task<bool> ValidarUsuario(string NombreUsuario)
        {
            return await _datacontext.Usuarios.AnyAsync(u => u.Nombre == NombreUsuario);
        }
    }
}