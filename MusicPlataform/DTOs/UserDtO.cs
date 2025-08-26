using MusicPlataform.Server.Models;

namespace MusicPlataform.Server.DTOs
{
    public class UserDtO { 
 
            // DTO de lectura (lo que devuelve la API)
            public record UserReadDto(string Username, string Email, DateTime CreatedAt);

            // DTO de creación (lo que recibe la API)
            public record UserCreateDto(string Username, string Email, string Password);
            public record UserLoginDto(int id,string Username, string Email);   
            public record UserLogeoDto(string Username, string Password);   
        }
    }

