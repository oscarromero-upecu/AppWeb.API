using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class LoginRequestDTO
    {
        public string Nombre { get; set; }
        public string Password { get; set; }
    }
}