using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Railway_Reservation_Sys.DTOs;
using Railway_Reservation_Sys.Interfaces;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RailwayReservationDbContext _context;
        private readonly ITokenRepository tokenRepository;


        public AuthController(UserManager<IdentityUser> userManager, RailwayReservationDbContext context, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            _context = context;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerDTO)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.UserName
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerDTO.Password);

            if (identityResult.Succeeded)
            {
                if (registerDTO.Roles != null && registerDTO.Roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerDTO.Roles);

                    if (identityResult.Succeeded)
                    {
                        var user = new User
                        {
                            UserID = Guid.Parse(identityUser.Id),
                            Email = registerDTO.UserName,
                            Name = registerDTO.FullName,
                            Role = registerDTO.Roles.FirstOrDefault() 
                        };

                        // Save User to Main database
                        await _context.Users.AddAsync(user);
                        await _context.SaveChangesAsync();

                        return Ok(new{user});
                    }
                }
            }
            return BadRequest(identityResult.Errors);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.UserName);
            if (user != null)
            {
                var CheckPassword = await userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (CheckPassword)
                {
                    //Get Roles ofs User
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());
                        var response = new LoginResponseDTO
                        {
                            JwtToken = jwtToken
                        };
                        return Ok(response);
                        // return Ok();
                    }
                }
            }
            return BadRequest("Invalid Username or Password");
        }
    }
}