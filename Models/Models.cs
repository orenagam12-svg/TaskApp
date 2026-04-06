namespace TaskApp.Models;

// ─── המשימה הראשית ───────────────────────────────────────────
public class SchoolTask
{
    public int Id { get; set; }
    public string Name { get; set; } = "";          // שם המשימה
    public string? Notes { get; set; }              // הערות
    public DateTime? DueDate { get; set; }          // תאריך הגשה
    public Priority Priority { get; set; } = Priority.Medium;
    public TaskType Type { get; set; } = TaskType.Homework;
    public bool IsDone { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // קשרים
    public List<TaskTag> TaskTags { get; set; } = new();    // תגיות
    public List<TaskImage> Images { get; set; } = new();    // תמונות
}

// ─── תגית (נוצרת ע"י המשתמש) ────────────────────────────────
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = "";          // שם התגית
    public string Color { get; set; } = "#6366F1";  // צבע HEX
    public string Bg { get; set; } = "#EEF2FF";     // רקע HEX

    public List<TaskTag> TaskTags { get; set; } = new();
}

// ─── קשר רבים-לרבים בין משימה לתגית ────────────────────────
public class TaskTag
{
    public int TaskId { get; set; }
    public SchoolTask Task { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}

// ─── תמונה מצורפת למשימה ────────────────────────────────────
public class TaskImage
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public SchoolTask Task { get; set; } = null!;
    public string FileName { get; set; } = "";      // שם הקובץ בשרת
    public string OriginalName { get; set; } = "";  // שם מקורי
    public DateTime UploadedAt { get; set; } = DateTime.Now;
}

// ─── Enums ───────────────────────────────────────────────────
public enum Priority
{
    Urgent = 0,   // דחוף ⚡
    Medium = 1,   // בינוני 🟡
    Low    = 2    // נמוך 🟢
}

public enum TaskType
{
    Homework = 0,  // שיעורי בית 📚
    Exam     = 1,  // מבחן/בגרות 📝
    Project  = 2   // פרויקט 🗂️
}
