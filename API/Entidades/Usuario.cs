
namespace API.Entidades
{
    public class Usuario
    {
        public int Id {get; set;}
        public String Nombre {get; set;}

        public byte [] PasswordHash {get; set;}
        public byte [] PasswordSalt {get; set;}
    }
}