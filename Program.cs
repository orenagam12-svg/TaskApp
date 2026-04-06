using Microsoft.EntityFrameworkCore;
using TaskApp.Data;
using TaskApp.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── שירותים ──────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // שמות camelCase ב-JSON
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Enum כמחרוזת (priority: "Urgent" ולא 0)
        o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// SQLite — הקובץ נשמר בתיקיית האפליקציה
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=tasks.db"));

builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<TagService>();

// גישה מכל מכשיר ברשת המקומית
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

// ─── יצירת/עדכון בסיס הנתונים אוטומטית בהפעלה ─────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // יוצר את הטבלאות אם לא קיימות
}

app.UseStaticFiles();   // קבצי CSS/JS/תמונות מ-wwwroot
app.MapControllers();

Console.WriteLine("✅ האפליקציה רצה!");
Console.WriteLine("📱 פתח בדפדפן: http://localhost:5000");
Console.WriteLine("📱 מטלפון ברשת: http://<IP של המחשב>:5000");

app.Run();
