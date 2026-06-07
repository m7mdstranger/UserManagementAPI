using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Models;
using UserManagementAPI.Utilities;
using UserManagementAPI.Exceptions;

namespace UserManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Add this to require authentication
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users?pageNumber=1&pageSize=10
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Cap max page size

            var users = await _context.Users
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Role = u.Role,
                    FullName = u.FullName,
                    Email = u.Email,
                    Department = u.Department,
                    Created = u.Created
                })
                .ToListAsync();

            return Ok(ApiResponse<List<UserDto>>.SuccessResponse(users, "Users retrieved successfully"));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ValidationException("Invalid user ID");
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Role = user.Role,
                FullName = user.FullName,
                Email = user.Email,
                Department = user.Department,
                Created = user.Created
            };

            return Ok(ApiResponse<UserDto>.SuccessResponse(userDto, "User retrieved successfully"));
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, [FromForm] UpdateUserDto updateUserDto)
        {
            if (id == Guid.Empty)
            {
                throw new ValidationException("Invalid user ID");
            }

            if (updateUserDto == null)
            {
                throw new ValidationException("User data is required");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                throw new ValidationException(errors);
            }

            // Validate password
            PasswordValidator.Validate(updateUserDto.PasswordHash);

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                throw new NotFoundException("User not found");
            }

            // Map DTO to entity
            existingUser.Role = updateUserDto.Role;
            existingUser.FullName = updateUserDto.FullName;
            existingUser.Email = updateUserDto.Email;
            existingUser.PasswordHash = updateUserDto.PasswordHash;
            existingUser.Department = updateUserDto.Department;

            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.SuccessResponse($"Update user: {id}"));
        }

        // POST: api/Users
        [HttpPost("AddUser")]
        public async Task<ActionResult<ApiResponse<UserDto>>> PostUser([FromForm] User user)
        {
            if (user == null)
            {
                throw new ValidationException("User data is required");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                throw new ValidationException(errors);
            }

            // Validate password
            PasswordValidator.Validate(user.PasswordHash);

            user.Id = Guid.NewGuid();
            user.Created = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Role = user.Role,
                FullName = user.FullName,
                Email = user.Email,
                Department = user.Department,
                Created = user.Created
            };

            return CreatedAtAction("GetUser", new { id = user.Id },
                ApiResponse<UserDto>.SuccessResponse(userDto, "User created successfully", StatusCodes.Status201Created));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ValidationException("Invalid user ID");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.SuccessResponse($"User with id: {id} is deleted."));
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}