using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;
using static MusicPlataform.Server.DTOs.UserDtO;

namespace MusicPlataform.Server.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MusicContext _context;

        public UsersController(MusicContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new UserReadDto(u.Username, u.Email, u.CreatedAt))
                .ToList();

            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id:int}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserReadDto(u.Username, u.Email, u.CreatedAt))
                .FirstOrDefault();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // POST: api/users/register
        [HttpPost("register")]
        public IActionResult Register(UserCreateDto dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
                return BadRequest("El nombre de usuario ya existe");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password), // 👈 aquí hasheamos
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new UserReadDto(user.Username, user.Email, user.CreatedAt));
        }

        // POST: api/users/login
        [HttpPost("login")]
        public IActionResult Login(UserLogeoDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == dto.Username);
            if (user == null)
                return Unauthorized("Usuario no encontrado");

            // 👇 Verificamos el hash
            if (!VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Contraseña incorrecta");

            return Ok(new UserLoginDto(user.Id,user.Username, user.Email));
        }

        // PUT: api/users/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, UserCreateDto dto)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            // Username NO se cambia
            user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = HashPassword(dto.Password); // 👈 rehash si se cambia

            _context.SaveChanges();

            return Ok(new UserReadDto(user.Username, user.Email, user.CreatedAt));
        }

        // DELETE: api/users/5
        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok("Usuario eliminado correctamente");
        }

        //  Métodos privados de seguridad
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }
    }
}
