# שלב הבנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# העתקת כל הקבצים ומציאת קובץ הפרויקט באופן אוטומטי
COPY . .
RUN dotnet restore

# בנייה של הפרויקט (הוא יחפש בכל התיקיות)
RUN dotnet publish -c Release -o /app/out

# שלב ההרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# פקודת ההרצה
ENTRYPOINT ["dotnet", "TaskApp.dll"]