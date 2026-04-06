using Microsoft.EntityFrameworkCore;
using TaskApp.Models;

namespace TaskApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // טבלאות בבסיס הנתונים
    public DbSet<SchoolTask> Tasks { get; set; }
    public DbSet<Tag>        Tags  { get; set; }
    public DbSet<TaskTag>    TaskTags { get; set; }
    public DbSet<TaskImage>  Images { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        // מפתח מורכב לטבלת הקשר TaskTag
        b.Entity<TaskTag>()
            .HasKey(tt => new { tt.TaskId, tt.TagId });

        b.Entity<TaskTag>()
            .HasOne(tt => tt.Task)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TaskId);

        b.Entity<TaskTag>()
            .HasOne(tt => tt.Tag)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TagId);

        // נתוני ברירת מחדל — תגיות המקצועות
        b.Entity<Tag>().HasData(
            new Tag { Id=1,  Name="עברית",           Color="#8B5CF6", Bg="#F3F0FF" },
            new Tag { Id=2,  Name="אנגלית",          Color="#3B82F6", Bg="#EFF6FF" },
            new Tag { Id=3,  Name="מתמטיקה",         Color="#EF4444", Bg="#FEF2F2" },
            new Tag { Id=4,  Name="מחשבים C#",       Color="#0EA5E9", Bg="#F0F9FF" },
            new Tag { Id=5,  Name="מחשבים Android",  Color="#10B981", Bg="#ECFDF5" },
            new Tag { Id=6,  Name="היסטוריה",        Color="#F59E0B", Bg="#FFFBEB" },
            new Tag { Id=7,  Name="ספרות",           Color="#EC4899", Bg="#FDF2F8" },
            new Tag { Id=8,  Name="ספורט",           Color="#F97316", Bg="#FFF7ED" },
            new Tag { Id=9,  Name="פיזיקה",          Color="#6366F1", Bg="#EEF2FF" }
        );
    }
}
