using IssueTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Domain;
using Models.DTO;

namespace IssueTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueTagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IssueTagController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/IssueTag

        [HttpPost]
        public async Task<IActionResult> CreateIssueTag([FromBody] IssueTagDTO issueTagDto)
        {
            try
            {
                var issueExists = await _context.Issues.AnyAsync(i => i.Id == issueTagDto.IssueId);
                if (!issueExists)
                {
                    return NotFound("Issue not found.");
                }

                var tagExists = await _context.Tags.AnyAsync(t => t.Id == issueTagDto.TagId);
                if (!tagExists)
                {
                    return NotFound("Tag not found.");
                }

                var existingIssueTag = await _context.IssueTag
                    .FirstOrDefaultAsync(it => it.IssueId == issueTagDto.IssueId && it.TagId == issueTagDto.TagId);

                if (existingIssueTag != null)
                {
                    return Conflict("Tag is already associated with the issue.");
                }

                var issueTag = new IssueTag
                {
                    IssueId = issueTagDto.IssueId,
                    TagId = issueTagDto.TagId
                };

                _context.Set<IssueTag>().Add(issueTag);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetIssueTagsByIssueId), new { issueId = issueTag.IssueId }, issueTag);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error occurred while creating issue-tag association.");
                return StatusCode(500, "An error occurred while creating the issue-tag association.");
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateIssueTag([FromBody] IssueTagDTO issueTagDto)
        //{
        //    // Check if the issue exists
        //    var issueExists = await _context.Issues.AnyAsync(i => i.Id == issueTagDto.IssueId);
        //    if (!issueExists)
        //    {
        //        return NotFound("Issue not found.");
        //    }

        //    // Check if the tag exists
        //    var tagExists = await _context.Tags.AnyAsync(t => t.Id == issueTagDto.TagId);
        //    if (!tagExists)
        //    {
        //        return NotFound("Tag not found.");
        //    }

        //    // Check if the tag is already associated with the issue
        //    var existingIssueTag = await _context.IssueTag
        //        .FirstOrDefaultAsync(it => it.IssueId == issueTagDto.IssueId && it.TagId == issueTagDto.TagId);

        //    if (existingIssueTag != null)
        //    {
        //        return Conflict("Tag is already associated with the issue.");
        //    }

        //    // Add the tag to the issue
        //    var issueTag = new IssueTag
        //    {
        //        IssueId = issueTagDto.IssueId,
        //        TagId = issueTagDto.TagId
        //    };

        //    _context.Set<IssueTag>().Add(issueTag);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetIssueTagsByIssueId), new { issueId = issueTag.IssueId }, issueTag);
        //}

        // GET: api/IssueTag/issue/{issueId}
        [HttpGet("issue/{issueId}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetIssueTagsByIssueId(int issueId)
        {
            var tags = await _context.IssueTag
                .Where(it => it.IssueId == issueId)
                .Include(it => it.Tag)
                .Select(it => it.Tag)
                .ToListAsync();

            if (!tags.Any())
            {
                return NotFound("No tags found for this issue.");
            }

            return tags;
        }

        // DELETE: api/IssueTag
        [HttpDelete]
        public async Task<IActionResult> DeleteIssueTag([FromBody] IssueTagDTO issueTagDto)
        {
            // Check if the issue-tag association exists
            var issueTag = await _context.IssueTag
                .FirstOrDefaultAsync(it => it.IssueId == issueTagDto.IssueId && it.TagId == issueTagDto.TagId);

            if (issueTag == null)
            {
                return NotFound("Tag association with the issue not found.");
            }

            // Remove the tag from the issue
            _context.IssueTag.Remove(issueTag);
            await _context.SaveChangesAsync();

            return Ok("Tag removed from the issue successfully.");
        }
    }

}
