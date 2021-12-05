using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
       

        public async Task<IEnumerable<AppUser>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }


        [HttpGet("{id}")]
        
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return await _context.Users.FindAsync(id);
        }



        [Route("edit-user")]
        [HttpPost]

        public async Task<HttpStatusCode> Edit([FromForm] AppUser appUser)
        {

            var user = await _context.Users.FindAsync(appUser.Id);
            if (user == null)
            {
                return HttpStatusCode.UnprocessableEntity;
            }

            user.UserName = appUser.UserName;
            await _context.SaveChangesAsync();
            return HttpStatusCode.OK;

        }

        [Route("delete-user")]
        [HttpPost]
        public async Task<HttpStatusCode> Delete([FromForm] int id)
        {
            if (id > 0)
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return HttpStatusCode.BadRequest;
                }

                _context.Remove(user);
                _context.SaveChanges();
                return HttpStatusCode.OK;

            }
            else
            {
                return HttpStatusCode.UnprocessableEntity;
            }
        }




        [HttpPost]
        [Route("add-user")]

        public async Task<IEnumerable<AppUser>> AddUser([FromForm] AppUser appUser)
        {
            await _context.AddAsync(appUser);
            _context.SaveChanges();

            return await _context.Users.ToListAsync();

        }

    }
}

