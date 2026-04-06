using Microsoft.AspNetCore.Mvc;
using TaskApp.Models;
using TaskApp.Services;

namespace TaskApp.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly TagService _svc;
    public TagsController(TagService svc) => _svc = svc;

    // GET /api/tags — כל התגיות
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _svc.GetAllAsync();
        return Ok(tags);
    }

    // POST /api/tags — יצירת תגית חדשה
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagRequest req)
    {
        var tag = new Tag { Name = req.Name, Color = req.Color, Bg = req.Bg };
        await _svc.CreateAsync(tag);
        return Ok(tag);
    }

    // PUT /api/tags/5 — עדכון תגית
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TagRequest req)
    {
        var tags = await _svc.GetAllAsync();
        var tag  = tags.FirstOrDefault(t => t.Id == id);
        if (tag == null) return NotFound();

        tag.Name  = req.Name;
        tag.Color = req.Color;
        tag.Bg    = req.Bg;
        await _svc.UpdateAsync(tag);
        return Ok(tag);
    }

    // DELETE /api/tags/5 — מחיקת תגית
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}

public record TagRequest(string Name, string Color, string Bg);
