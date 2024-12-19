using IssueTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Models.Domain;
using Models.DTO;
using System.Threading.Tasks;
using ILogger = Microsoft.Build.Framework.ILogger;

namespace IssueTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IssueController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Issue>> CreateIssue([FromBody] IssueDTO issueDto)
        {
            var issue = new Issue
            {
                Title = issueDto.Title,
                Description = issueDto.Description,
                Status = issueDto.Status,
                ProjectId = issueDto.ProjectId,
                UserId = issueDto.UserId
            };

            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetIssues), new { id = issue.Id }, issue);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> UpdateIssue(int id, [FromBody] IssueDTO issueDto)
        {
            try
            {
                var issue = await _context.Issues.FindAsync(id);
                if (issue == null)
                    return NotFound();

                issue.Title = issueDto.Title;
                issue.Description = issueDto.Description;
                issue.Status = issueDto.Status;
                issue.ProjectId = issueDto.ProjectId;

                _context.Entry(issue).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                //ILogger.LogError(ex, "Error occurred while updating issue.");
                return StatusCode(500, "An error occurred while updating the issue.");
            }
        }

        //[HttpPut("{id}")]
        //[Authorize(Roles = "User,Admin")]
        //public async Task<IActionResult> UpdateIssue(int id, [FromBody] IssueDTO issueDto)
        //{
        //    var issue = await _context.Issues.FindAsync(id);
        //    if (issue == null) return NotFound();

        //    issue.Title = issueDto.Title;
        //    issue.Description = issueDto.Description;
        //    issue.Status = issueDto.Status;
        //    issue.ProjectId = issueDto.ProjectId;

        //    _context.Entry(issue).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        // GET: api/Issue
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Issue>>> GetIssues()
        {
            return await _context.Issues.ToListAsync();
        }

        // PUT: api/Issue/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles ="User,Admin")]
        public async Task<IActionResult> UpdateIssueStatus(int id, [FromBody] string status)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound("Issue not found.");
            }

            var allowedStatuses = new[] { "open", "in progress", "resolved" };
            if (!allowedStatuses.Contains(status.ToLower()))
            {
                return BadRequest("Invalid status. Valid statuses are 'open', 'in progress', and 'resolved'.");
            }

            issue.Status = status;
            _context.Entry(issue).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<Issue>> GetIssue(int id)
        {
            var issue = await _context.Issues
                .Include(i => i.IssueTags)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (issue == null)
            {
                return NotFound();
            }

            return issue;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Issue>>> SearchIssues([FromQuery] string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Keyword is required.");
            }

            var issues = await _context.Issues
                .Where(i => EF.Functions.Like(i.Title, $"%{keyword}%") || EF.Functions.Like(i.Description, $"%{keyword}%"))
                .Include(i => i.IssueTags) // Include related tags if necessary
                .ToListAsync();

            if (!issues.Any())
            {
                return NotFound("No issues found matching the provided keyword.");
            }

            return Ok(issues);
        }
        [HttpGet("searchByStatusOrTag")]
        public async Task<ActionResult<IEnumerable<Issue>>> SearchIssues(
    //[FromQuery] string keyword = null,
    [FromQuery] string status = null,
    [FromQuery] int? tagId = null)
        {
            // Start with a base query for issues
            var query = _context.Issues.AsQueryable();

            // Filter by status if provided
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(i => i.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by tagId if provided
            if (tagId.HasValue)
            {
                query = query.Where(i => i.IssueTags.Any(it => it.TagId == tagId.Value));
            }

            // Execute the query and include IssueTags if necessary
            var issues = await query
                .Include(i => i.IssueTags)  // Include IssueTags to fetch related tags if needed
                .ThenInclude(it => it.Tag)  // Optionally include Tag details
                .ToListAsync();

            // Return results or NotFound if no issues match the filters
            if (!issues.Any())
            {
                return NotFound("No issues found matching the provided filters.");
            }

            return Ok(issues);
        }

    }

    // DELETE: 
    //[HttpDelete("api/Issue/{id}")]
    //public async Task<IActionResult> DeleteIssue(int id, [FromQuery] int userId)
    //{
    //    var issue = await _context.Issues.FindAsync(id);
    //    if (issue == null)
    //    {
    //        return NotFound("Issue not found.");
    //    }

    //    // Get the user making the request
    //    var user = await _context.Users.FindAsync(userId);
    //    if (user == null)
    //    {
    //        return Unauthorized("User not found.");
    //    }

    //    // Check if the user has permission to delete the issue
    //    if (user.Role != "Admin" && issue.UserId != user.Id)
    //    {
    //        return Forbid("You do not have permission to delete this issue.");
    //    }

    //    _context.Issues.Remove(issue);
    //    await _context.SaveChangesAsync();

    //    return NoContent();
    //}

}


