using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Services;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IJwtTokenService tokenService, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
                {
                    return BadRequest(ApiResponse.ErrorResponse("Email and password are required", StatusCodes.Status400BadRequest));
                }

                // For demo purposes - In production, validate credentials against database
                // This is a simplified example
                if (loginRequest.Email == "admin@example.com" && loginRequest.Password == "Admin@123")
                {
                    var userId = Guid.NewGuid();
                    var token = _tokenService.GenerateToken(userId, loginRequest.Email, "Admin");
                    
                    _logger.LogInformation($"User {loginRequest.Email} logged in successfully");
                    
                    return Ok(ApiResponse<LoginResponse>.SuccessResponse(
                        new LoginResponse 
                        { 
                            Token = token,
                            ExpiresIn = 3600,
                            TokenType = "Bearer"
                        },
                        "Login successful"));
                }

                _logger.LogWarning($"Failed login attempt for user: {loginRequest.Email}");
                return Unauthorized(ApiResponse.ErrorResponse("Invalid email or password", StatusCodes.Status401Unauthorized));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse.ErrorResponse("An error occurred during login", StatusCodes.Status500InternalServerError));
            }
        }
    }
}