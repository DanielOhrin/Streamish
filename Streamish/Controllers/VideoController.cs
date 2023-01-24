using System;
using Microsoft.AspNetCore.Mvc;
using Streamish.Repositories;
using Streamish.Models;
using Microsoft.AspNetCore.Authorization;

namespace Streamish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoRepository _videoRepository;
        public VideoController(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        [Authorize]
        [HttpGet("search")]
        public IActionResult Search(string q, bool sortDesc)
        {
            return Ok(_videoRepository.Search(q, sortDesc));
        }

        [Authorize]
        [HttpGet("hottest")]
        public IActionResult Since(DateTime? since)
        {
            return Ok(_videoRepository.Since(since));
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_videoRepository.GetAll());
        }

        [Authorize]
        [HttpGet("GetWithComments")]
        public IActionResult GetWithComments()
        {
            var videos = _videoRepository.GetAllWithComments();
            return Ok(videos);
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] int id)
        {
            var video = _videoRepository.GetById(id);
            if (video == null)
            {
                return NotFound();
            }
            return Ok(video);
        }

        [Authorize]
        [HttpGet("GetWithComments/{id}")]
        public IActionResult GetByIdWithComments([FromRoute] int id)
        {
            Video video = _videoRepository.GetByIdWithComments(id);

            if (video == null)
            {
                return NotFound();
            }

            return Ok(video);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] Video video)
        {
            // NOTE: This is only temporary to set the UserProfileId until we implement login
            // TODO: After we implement login, use the id of the current user
            video.UserProfileId = 1;

            video.DateCreated = DateTime.Now;
            if (string.IsNullOrWhiteSpace(video.Description))
            {
                video.Description = null;
            }

            try
            {
                // Handle the video URL

                // A valid video link might look like this:
                //  https://www.youtube.com/watch?v=sstOXCQ-EG0&list=PLdo4fOcmZ0oVGRpRwbMhUA0KAvMA2mLyN
                // 
                // Our job is to pull out the "v=XXXXX" part to get the get the "code/id" of the video
                //  So we can construct an URL that's appropriate for embedding a video

                // An embeddable Video URL looks something like this:
                //  https://www.youtube.com/embed/sstOXCQ-EG0

                // If this isn't a YouTube video, we should just give up
                if (!video.Url.Contains("youtube"))
                {
                    return BadRequest();
                }

                // If it's not already an embeddable URL, we have some work to do
                if (!video.Url.Contains("embed"))
                {
                    var videoCode = video.Url.Split("v=")[1].Split("&")[0];
                    video.Url = $"https://www.youtube.com/embed/{videoCode}";
                }
            }
            catch // Something went wrong while creating the embeddable url
            {
                return BadRequest();
            }

            _videoRepository.Add(video);

            return CreatedAtAction("Get", new { id = video.Id }, video);
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(int id, Video video)
        {
            if (id != video.Id)
            {
                return BadRequest();
            }

            _videoRepository.Update(video);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _videoRepository.Delete(id);
            return NoContent();
        }
    }
}