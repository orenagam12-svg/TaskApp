using Microsoft.EntityFrameworkCore;
using TaskApp.Data;
using TaskApp.Models;

namespace TaskApp.Services;

// ─── כל הלוגיקה של המשימות ────────────────────────────────────
public class TaskService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public TaskService(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    // ── קבלת כל המשימות (עם תגיות ותמונות) ──────────────────
    public async Task<List<SchoolTask>> GetAllAsync(
        bool? done = null,
        Priority? priority = null,
        TaskType? type = null,
        int? tagId = null,
        string? search = null)
    {
        var q = _db.Tasks
            .Include(t => t.TaskTags).ThenInclude(tt => tt.Tag)
            .Include(t => t.Images)
            .AsQueryable();

        if (done.HasValue)     q = q.Where(t => t.IsDone == done.Value);
        if (priority.HasValue) q = q.Where(t => t.Priority == priority.Value);
        if (type.HasValue)     q = q.Where(t => t.Type == type.Value);
        if (tagId.HasValue)    q = q.Where(t => t.TaskTags.Any(tt => tt.TagId == tagId.Value));
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(t => t.Name.Contains(search) || (t.Notes != null && t.Notes.Contains(search)));

        return await q.OrderBy(t => t.IsDone)
                      .ThenBy(t => t.DueDate == null)
                      .ThenBy(t => t.DueDate)
                      .ToListAsync();
    }

    // ── משימה בודדת ──────────────────────────────────────────
    public async Task<SchoolTask?> GetByIdAsync(int id) =>
        await _db.Tasks
            .Include(t => t.TaskTags).ThenInclude(tt => tt.Tag)
            .Include(t => t.Images)
            .FirstOrDefaultAsync(t => t.Id == id);

    // ── יצירת משימה חדשה ──────────────────────────────────────
    public async Task<SchoolTask> CreateAsync(SchoolTask task, List<int> tagIds)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        await SetTagsAsync(task.Id, tagIds);
        return task;
    }

    // ── עדכון משימה ───────────────────────────────────────────
    public async Task UpdateAsync(SchoolTask task, List<int> tagIds)
    {
        _db.Tasks.Update(task);
        await SetTagsAsync(task.Id, tagIds);
        await _db.SaveChangesAsync();
    }

    // ── מחיקת משימה (כולל תמונות) ────────────────────────────
    public async Task DeleteAsync(int id)
    {
        var task = await GetByIdAsync(id);
        if (task == null) return;

        // מחיקת קבצי תמונות מהדיסק
        foreach (var img in task.Images)
            DeleteImageFile(img.FileName);

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
    }

    // ── שינוי סטטוס בוצע/לא בוצע ────────────────────────────
    public async Task ToggleDoneAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return;
        task.IsDone = !task.IsDone;
        await _db.SaveChangesAsync();
    }

    // ── הוספת תמונה ───────────────────────────────────────────
    public async Task<TaskImage> AddImageAsync(int taskId, IFormFile file)
    {
        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsPath);

        // שם ייחודי כדי למנוע התנגשויות
        var ext      = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(uploadsPath, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        var image = new TaskImage
        {
            TaskId       = taskId,
            FileName     = fileName,
            OriginalName = file.FileName
        };
        _db.Images.Add(image);
        await _db.SaveChangesAsync();
        return image;
    }

    // ── מחיקת תמונה ───────────────────────────────────────────
    public async Task DeleteImageAsync(int imageId)
    {
        var img = await _db.Images.FindAsync(imageId);
        if (img == null) return;
        DeleteImageFile(img.FileName);
        _db.Images.Remove(img);
        await _db.SaveChangesAsync();
    }

    // ─── עזרים פרטיים ─────────────────────────────────────────
    private async Task SetTagsAsync(int taskId, List<int> tagIds)
    {
        // מחיקת תגיות ישנות
        var old = _db.TaskTags.Where(tt => tt.TaskId == taskId);
        _db.TaskTags.RemoveRange(old);

        // הוספת תגיות חדשות
        foreach (var tagId in tagIds.Distinct())
            _db.TaskTags.Add(new TaskTag { TaskId = taskId, TagId = tagId });

        await _db.SaveChangesAsync();
    }

    private void DeleteImageFile(string fileName)
    {
        var path = Path.Combine(_env.WebRootPath, "uploads", fileName);
        if (File.Exists(path)) File.Delete(path);
    }
}

// ─── שירות התגיות ─────────────────────────────────────────────
public class TagService
{
    private readonly AppDbContext _db;

    public TagService(AppDbContext db) => _db = db;

    public async Task<List<Tag>> GetAllAsync() =>
        await _db.Tags.OrderBy(t => t.Name).ToListAsync();

    public async Task<Tag> CreateAsync(Tag tag)
    {
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();
        return tag;
    }

    public async Task UpdateAsync(Tag tag)
    {
        _db.Tags.Update(tag);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var tag = await _db.Tags.FindAsync(id);
        if (tag == null) return;
        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync();
    }
}
