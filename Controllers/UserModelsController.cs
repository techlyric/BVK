using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.DTOs;
using Microsoft.CodeAnalysis.Scripting;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.UserModels.ToListAsync();

            var userDtos = users.Select(u => new UserDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            });

            return Ok(userDtos);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await _context.UserModels.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(userDto);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new UserModel
            {
                UserId = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Role = dto.Role,
                PasswordHash = dto.Password
                //PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.UserModels.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, userDto);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] CreateUserDto dto)
        {
            var user = await _context.UserModels.FindAsync(id);
            if (user == null)
                return NotFound();

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Role = dto.Role;
            user.PasswordHash = dto.Password;
            //user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.UserModels.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.UserModels.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.UserModels
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            // If passwords are hashed, use:
            // if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))

            // For now, compare directly
            if (user.PasswordHash != loginDto.Password)
                return Unauthorized(new { message = "Invalid email or password." });

            // Simulated JWT/token (in real apps, generate a real token)
            var token = Guid.NewGuid().ToString();

            return Ok(new
            {
                token,
                role = user.Role,
                userId = user.UserId,
                email=user.Email
            });
        }



    }
}


































