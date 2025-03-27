# 🎧 PopApp API

PopApp API is the backend solution for the [PopApp Mobile](https://github.com/RafaelZacca/PopApp-Mobile) application. It handles user authentication, audio recognition, song metadata, and recommendations through external music APIs (AudD & LastFM), and persists data using Entity Framework Core and SQL Server.
This is a PoC, which means this has been developed under the purpose of trying and learning the technology, and there is no intention of putting this App on production.

---

## ✨ Features

- JWT-based authentication
- Audio recognition using [AudD](https://audd.io/)
- Song recommendations via [Last.fm](https://www.last.fm/api)
- Modular architecture: API / Core / Database
- Auto-generated Swagger documentation via Swashbuckle
- Entity Framework Core (Code First)
- CORS-enabled for frontend integration

---

## 🧠 Tech Stack

- **.NET 5**
- **ASP.NET Core Web API**
- **Entity Framework Core** (SQL Server)
- **Swashbuckle.AspNetCore** for Swagger UI
- **Newtonsoft.Json** for advanced JSON handling

---

## 🗂️ Project Structure

```
PopApp-API/
├── API/             # Main entry point (controllers, startup)
├── Core/            # Business logic, interfaces, services
├── Database/        # Entity models, EF Core context
├── appsettings.json # Configuration (connection strings, API keys)
├── PopApp.sln       # Solution file
└── README.md
```

---

## ⚙️ Configuration

Before running the API, create an `appsettings.Local.json` or use the included example:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "YOUR-DB-CONNECTION-STRING"
  },
  "Api": {
    "Url": "YOUR-API-URL",
    "AudDUrl": "https://api.audd.io/",
    "LastFMUrl": "http://ws.audioscrobbler.com/2.0/"
  },
  "Security": {
    "AudDKey": "",
    "LastFMKey": "YOUR-LASTFM-KEY",
    "JwtKey": "YOUR-JWT-KEY"
  }
}
```

---

## 🚀 Getting Started

### 1. Clone the repository
```bash
git clone https://github.com/RafaelZacca/PopApp-API.git
cd PopApp-API
```

### 2. Open in Visual Studio (or VS Code)

### 3. Set `API` as startup project

### 4. Apply migrations and seed the database (if needed)
```bash
dotnet ef database update --project Database --startup-project API
```

### 5. Run the project
```bash
dotnet run --project API
```

The API will run at:
```
https://localhost:5001
```

> Use Swagger UI at `https://localhost:5001/swagger` to explore the endpoints.

---

## 🔒 Authentication

The API uses JWT tokens. After logging in via `/auth/login`, use the returned token in the `Authorization` header of your requests:

```
Authorization: Bearer <your-token>
```

---

## 📦 Dependencies

### API Project
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Microsoft.AspNetCore.Mvc.Abstractions`
- `Swashbuckle.AspNetCore`

### Core Project
- `Microsoft.Extensions.Configuration.Abstractions`
- `Microsoft.Extensions.Identity.Core`
- `Newtonsoft.Json`

### Database Project
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.EntityFrameworkCore.Tools`

---

## 🔗 Frontend Integration

This backend is consumed by the [PopApp Mobile](https://github.com/RafaelZacca/PopApp-Mobile) frontend via REST API.

Make sure CORS is enabled to allow mobile app requests.

---

## 🧭 API Documentation

Swagger UI is available at:
```
https://localhost:5001/swagger
```
