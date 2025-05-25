


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs;
using Project.Models;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseModelsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CourseModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CourseModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courses = await _context.CourseModels.ToListAsync();

            var courseDtos = courses.Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                UserId = c.UserId,
                MediaUrl = c.MediaUrl
               
            });

            return Ok(courseDtos);
        }

        // GET: api/CourseModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
        {
            var c = await _context.CourseModels.FindAsync(id);

            if (c == null)
                return NotFound();

            var dto = new CourseDto
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                UserId = c.UserId,
                MediaUrl = c.MediaUrl
            };

            return Ok(dto);
        }

        // POST: api/CourseModels
        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = new CourseModel
            {
                CourseId = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                UserId = dto.UserId,
                MediaUrl = dto.MediaUrl
            };

            _context.CourseModels.Add(course);
            await _context.SaveChangesAsync();

            var createdDto = new CourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                UserId = course.UserId,
                MediaUrl = course.MediaUrl
            };

            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, createdDto);
        }

        // PUT: api/CourseModels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] CreateCourseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _context.CourseModels.FindAsync(id);
            if (course == null)
                return NotFound();

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.UserId = dto.UserId;
            course.MediaUrl = dto.MediaUrl;

            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CourseModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var course = await _context.CourseModels.FindAsync(id);
            if (course == null)
                return NotFound();

            _context.CourseModels.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
