using Microsoft.AspNetCore.Mvc;
using TaskApp.Models;
using TaskApp.Services;

namespace TaskApp.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly TaskService _svc;
    public TasksController(TaskService svc) => _svc = svc;

    // GET /api/tasks — כל המשימות עם פילטרים
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? done,
        [FromQuery] Priority? priority,
        [FromQuery] TaskType? type,
        [FromQuery] int? tagId,
        [FromQuery] string? search)
    {
        var tasks = await _svc.GetAllAsync(done, priority, type, tagId, search);
        return Ok(tasks.Select(TaskDto));
    }

    // GET /api/tasks/5 — משימה בודדת
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var task = await _svc.GetByIdAsync(id);
        return task == null ? NotFound() : Ok(TaskDto(task));
    }

    // POST /api/tasks — יצירת משימה חדשה
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskRequest req)
    {
        var task = new SchoolTask
        {
            Name     = req.Name,
            Notes    = req.Notes,
            DueDate  = req.DueDate,
            Priority = req.Priority,
            Type     = req.Type,
        };
        await _svc.CreateAsync(task, req.TagIds);
        var created = await _svc.GetByIdAsync(task.Id);
        return CreatedAtAction(nameof(GetOne), new { id = task.Id }, TaskDto(created!));
    }

    // PUT /api/tasks/5 — עדכון משימה
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskRequest req)
    {
        var task = await _svc.GetByIdAsync(id);
        if (task == null) return NotFound();

        task.Name     = req.Name;
        task.Notes    = req.Notes;
        task.DueDate  = req.DueDate;
        task.Priority = req.Priority;
        task.Type     = req.Type;

        await _svc.UpdateAsync(task, req.TagIds);
        return Ok(TaskDto((await _svc.GetByIdAsync(id))!));
    }

    // DELETE /api/tasks/5 — מחיקת משימה
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }

    // PATCH /api/tasks/5/toggle — שינוי בוצע/לא בוצע
    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        await _svc.ToggleDoneAsync(id);
        var task = await _svc.GetByIdAsync(id);
        return task == null ? NotFound() : Ok(TaskDto(task));
    }

    // POST /api/tasks/5/images — העלאת תמונה
    [HttpPost("{id}/images")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("לא נבחר קובץ");

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowed.Contains(ext))
            return BadRequest("סוג קובץ לא נתמך");

        var img = await _svc.AddImageAsync(id, file);
        return Ok(new { img.Id, img.FileName, img.OriginalName, Url = $"/uploads/{img.FileName}" });
    }

    // DELETE /api/tasks/images/3 — מחיקת תמונה
    [HttpDelete("images/{imageId}")]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        await _svc.DeleteImageAsync(imageId);
        return NoContent();
    }

    // ─── DTO — מה שנשלח ל-Frontend ──────────────────────────
    private static object TaskDto(SchoolTask t) => new
    {
        t.Id, t.Name, t.Notes,
        t.DueDate, t.Priority, t.Type, t.IsDone, t.CreatedAt,
        Tags   = t.TaskTags.Select(tt => new { tt.Tag.Id, tt.Tag.Name, tt.Tag.Color, tt.Tag.Bg }),
        Images = t.Images.Select(i => new { i.Id, i.OriginalName, Url = $"/uploads/{i.FileName}" })
    };
}

// ─── Request body ─────────────────────────────────────────────
public record TaskRequest(
    string Name,
    string? Notes,
    DateTime? DueDate,
    Priority Priority,
    TaskType Type,
    List<int> TagIds
);
