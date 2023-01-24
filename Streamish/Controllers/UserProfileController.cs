using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Streamish.Models;
using Streamish.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Streamish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileRepository _userProfileRepository;
        public UserProfileController(IUserProfileRepository uPRepo)
        {
            _userProfileRepository = uPRepo;
        }

        [Authorize]
        [HttpGet("DoesUserExist/{firebaseUserId}")]
        public IActionResult GetByFirebaseUserId(string firebaseUserId)
        {
            UserProfile userProfile = _userProfileRepository.GetByFirebaseUserId(firebaseUserId);

            if (userProfile == null)
            {
                return NotFound();
            }

            return Ok(userProfile);
        }

        [HttpPost]
        public IActionResult Register(UserProfile userProfile)
        {
            _userProfileRepository.Add(userProfile);
            return CreatedAtAction(nameof(GetByFirebaseUserId), new { firebaseUserId = userProfile.FirebaseUserId }, userProfile);
        }

        // GET: api/<UserProfileController>
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_userProfileRepository.GetAll());
        }

        // GET api/<UserProfileController>/5
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            UserProfile userProfile = _userProfileRepository.GetById(id);

            if (userProfile == null)
            {
                return NotFound();
            }

            return Ok(userProfile);
        }

        [Authorize]
        [HttpGet("GetWithVideos/{id}")]
        public IActionResult GetWithVideos(int id)
        {
            UserProfile userProfile = _userProfileRepository.GetByIdWithVideos(id);

            if (userProfile == null)
            {
                return NotFound();
            }

            return Ok(userProfile);
        }

        // POST api/<UserProfileController>
        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] UserProfile profile)
        {
            _userProfileRepository.Add(profile);
            return CreatedAtAction("Get", new { Id = profile.Id }, profile);
        }

        // PUT api/<UserProfileController>/5
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UserProfile profile)
        {
            if (profile == null || profile.Id != id)
            {
                return BadRequest();
            }

            _userProfileRepository.Update(profile);
            return NoContent();
        }

        // DELETE api/<UserProfileController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userProfileRepository.Delete(id);
            return NoContent();
        }
    }
}
