# 📚 אפליקציית המשימות | כיתה יא

אפליקציה מבוססת **ASP.NET Core + C#** עם בסיס נתונים SQLite.

---

## 🚀 איך להריץ

### דרישות מוקדמות
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)

### הרצה
```bash
# כנס לתיקיית הפרויקט
cd TaskApp

# הרץ
dotnet run
```

פתח בדפדפן: **http://localhost:5000**

מטלפון באותה רשת WiFi: **http://[IP של המחשב]:5000**

---

## 📁 מבנה הפרויקט

```
TaskApp/
│
├── Program.cs                  ← נקודת הכניסה, הגדרת השירותים
│
├── Models/
│   └── Models.cs               ← SchoolTask, Tag, TaskTag, TaskImage, Enums
│
├── Data/
│   └── AppDbContext.cs         ← EF Core + SQLite, נתוני ברירת מחדל
│
├── Services/
│   └── TaskService.cs          ← כל הלוגיקה (CRUD משימות + תגיות + תמונות)
│
├── Controllers/
│   ├── TasksController.cs      ← API: /api/tasks
│   ├── TagsController.cs       ← API: /api/tags
│   └── HomeController.cs       ← מחזיר את דף ה-HTML
│
└── wwwroot/
    ├── index.html              ← כל ה-Frontend (HTML + CSS + JS)
    └── uploads/                ← תמונות שהועלו (נוצר אוטומטית)
```

---

## 🔌 API Endpoints

### משימות
| Method | URL | תיאור |
|--------|-----|--------|
| GET | `/api/tasks` | כל המשימות (עם פילטרים) |
| GET | `/api/tasks/{id}` | משימה בודדת |
| POST | `/api/tasks` | יצירת משימה |
| PUT | `/api/tasks/{id}` | עדכון משימה |
| DELETE | `/api/tasks/{id}` | מחיקת משימה |
| PATCH | `/api/tasks/{id}/toggle` | בוצע / לא בוצע |
| POST | `/api/tasks/{id}/images` | העלאת תמונה |
| DELETE | `/api/tasks/images/{imgId}` | מחיקת תמונה |

### תגיות
| Method | URL | תיאור |
|--------|-----|--------|
| GET | `/api/tags` | כל התגיות |
| POST | `/api/tags` | יצירת תגית |
| PUT | `/api/tags/{id}` | עדכון תגית |
| DELETE | `/api/tags/{id}` | מחיקת תגית |

### פילטרים ל-GET /api/tasks
- `?done=true/false`
- `?priority=Urgent/Medium/Low`
- `?type=Homework/Exam/Project`
- `?tagId=3`
- `?search=מילה`

---

## 🗄️ בסיס הנתונים
הקובץ `tasks.db` נוצר אוטומטית בהרצה הראשונה.
ניתן לפתוח אותו עם [DB Browser for SQLite](https://sqlitebrowser.org/).
