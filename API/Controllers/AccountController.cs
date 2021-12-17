using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            this._context = context;
            this._tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register([FromForm]RegisterDTO registerDTO)
        {

            if (await UserExists(registerDTO.UserName))
                return BadRequest("Username is taken");


            using (var hmac = new HMACSHA512())
            {
                var user = new AppUser()
                {
                    UserName = registerDTO.UserName.ToLower(),
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                    PasswordSalt = hmac.Key
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new UserDTO
                {
                    UserName = registerDTO.UserName,
                    Token = _tokenService.CreateToken(user)
                };

            }
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user=await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid Username");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password.ToLower()));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized();

               
            }

            return new UserDTO
            {
                UserName = loginDTO.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }

    }



}
