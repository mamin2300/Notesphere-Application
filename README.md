# 📓 Notesphere

A full-featured student productivity web application that helps students manage notes, plan schedules, and collaborate with peers — all in one place.

> Built as a collaborative academic project at Sheridan College, Fall 2025.

---

## Key Features

- **Notes** — Create and organize notes with tags, version history, and templates
- **Planner** — Weekly event planner with recurring events and real-time conflict detection
- **Dashboard** — Personal workspace with reminders and quick actions
- **Sharing** — Collaborate and share notes with other users
- **Authentication** — Secure login and registration

---

## Tech Stack

- ASP.NET Core MVC
- C#
- SQLite
- Entity Framework Core
- HTML / CSS / JavaScript

---

## Getting Started

### Prerequisites
- Visual Studio 2022 or later
- .NET 8 SDK

### Running the Project

1. Clone the repository
   ```bash
   git clone https://github.com/mamin2300/Notesphere-Application.git
   ```
2. Open the solution in Visual Studio
3. Apply database migrations
   ```bash
   dotnet ef database update
   ```
4. Press **F5** to run

---

## Project Structure

```
Notesphere/
├── Controllers/          # MVC Controllers
├── Models/               # Entity models
├── Views/                # Razor views
├── Services/             # Business logic & repositories
├── Migrations/           # EF Core migrations
└── NotesphereDbContext   # Database context
```

---

## Team

Collaborative academic project — Sheridan College, Fall 2025.

| Name | Role |
|------|------|
| Mamin | Team Member |
| Malika Muskan | Planner Module |
| Talha | Team Member |
| Saad | Team Member |


