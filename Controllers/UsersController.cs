using AutoMapper;
using Job.it_classes.Data.Entities;
using Job.it_ClassLib.Data.DAL;
using Job.it_ClassLib.Data.DTO;
using Job.it_ClassLib.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.it_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public UsersController(IUserRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        } 

        [HttpGet]
        public async Task<ActionResult<UserDTO[]>> GetUsers()
        {
            try
            {
                var users = await _repository.GetAllUsersAsync();
                return _mapper.Map<UserDTO[]>(users);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get Users");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            try
            {
                var user = await _repository.GetUserAsync(id);
                if (user == null) return NotFound();
                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get User");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CreateUpdateUserDTO>> PostUser(CreateUpdateUserDTO user)
        {
            try
            {
                var existingUser = await _repository.GetUserAsync(user.Email);

                if (existingUser != null)
                {
                    return BadRequest("Email already in use");
                }

                var newUser = _mapper.Map<User>(user);

                // every new user by default has basic role
                var userRole = await _repository.GetRoleAsync("User");
                newUser.Role = userRole;
                _repository.Add(newUser);

                if (await _repository.SaveChangesAsync())
                {
                    var url = _linkGenerator.GetPathByAction("GetUser", "Users", new { id = newUser.UserId });

                    return Created(url, newUser);
                }

                else
                {
                    return BadRequest("Failed to see new User");
                }
            }

            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add User");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CreateUpdateUserDTO>> UpdateUser(int id, CreateUpdateUserDTO user)
        {
            try
            {
                var oldUser = await _repository.GetUserAsync(id);
                if (oldUser == null) NotFound($"Could not find User with an id of {id}");

                _mapper.Map(user, oldUser);

                if (await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<CreateUpdateUserDTO>(oldUser);
                }
                else
                {
                    return BadRequest();
                }


            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update User");
            }
        }




        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _repository.GetUserAsync(id);
                if (user == null) return NotFound();

                _repository.Delete(user);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete User");
            }
        }
    }
}
