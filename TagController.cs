using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Models;
using Models.Domain;
using System.Threading.Tasks;
using Models.DTO;
using Microsoft.AspNetCore.Authorization;

//[Route("api/[controller]")]
//[ApiController]
//public class TagController : ControllerBase
//{
//    private readonly ApplicationDbContext _context;

//    public TagController(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    // POST: api/Tag
//    [HttpPost]
//    [Authorize(Roles = "Admin")]
//    public async Task<ActionResult<Tag>> CreateTag([FromBody] TagDTO tagDto)
//    {
//        if (tagDto == null || string.IsNullOrEmpty(tagDto.Name))
//        {
//            return BadRequest("Tag name is required.");
//        }

//        // Check if the tag already exists
//        var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagDto.Name);
//        if (existingTag != null)
//        {
//            return Conflict("Tag with the same name already exists.");
//        }

//        var tag = new Tag
//        {
//            Name = tagDto.Name
//        };

//        _context.Tags.Add(tag);
//        await _context.SaveChangesAsync();

//        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
//    }

//    // GET: api/Tag/{id}
//    [HttpGet("{id}")]
//    public async Task<ActionResult<Tag>> GetTag(int id)
//    {
//        var tag = await _context.Tags.FindAsync(id);
//        if (tag == null)
//        {
//            return NotFound();
//        }
//        return tag;
//    }
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class TagController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TagController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/Tag
    [HttpPost]
    public async Task<ActionResult<Tag>> CreateTag([FromBody] TagDTO tagDto)
    {
        if (tagDto == null || string.IsNullOrEmpty(tagDto.Name))
        {
            return BadRequest("Tag name is required.");
        }

        // Check if the tag already exists
        var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagDto.Name);
        if (existingTag != null)
        {
            return Conflict("Tag with the same name already exists.");
        }

        var tag = new Tag
        {
            Name = tagDto.Name
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
    }

    // GET: api/Tag
    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<IEnumerable<Tag>>> GetAllTags()
    {
        var tags = await _context.Tags.ToListAsync();
        return Ok(tags);
    }

    // GET: api/Tag/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<Tag>> GetTag(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound("Tag not found.");
        }
        return tag;
    }

    // GET: api/Tag/{tagId}/issues
    [HttpGet("{tagId}/issues")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<IEnumerable<Issue>>> GetIssuesByTag(int tagId)
    {
        var issues = await _context.IssueTag
            .Where(it => it.TagId == tagId)
            .Include(it => it.Issue)
            .Select(it => it.Issue)
            .ToListAsync();

        if (!issues.Any())
        {
            return NotFound("No issues found for this tag.");
        }

        return Ok(issues);
    }
}

