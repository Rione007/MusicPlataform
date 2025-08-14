using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;
using System.Security.Cryptography;
using System.Text;

namespace MusicPlataform.Server.Controllers
{
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
                .Include(u => u.Playlists)
                .Include(u => u.LikedTracks)
                .ToList();

            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id:int}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users
                .Include(u => u.Playlists)
                .Include(u => u.LikedTracks)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // POST: api/users/register
        [HttpPost("register")]
        public IActionResult Register(User user, string password)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
                return BadRequest("El nombre de usuario ya existe");

            user.PasswordHash = HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user);
        }

        // POST: api/users/login
        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return Unauthorized("Usuario no encontrado");

            if (!VerifyPassword(password, user.PasswordHash))
                return Unauthorized("Contraseña incorrecta");

            return Ok(new
            {
                Message = "Login exitoso",
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email
            });
        }

        // PUT: api/users/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, User updatedUser)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;

            _context.SaveChanges();
            return Ok(user);
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

            return Ok();
        }

        // Funciones de hash
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
