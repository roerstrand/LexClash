YouTube: https://youtube.com/shorts/HhwuGdsO7Lg

# LexClash — Multiplayer Word Game

A real-time multiplayer word game built with **ASP.NET Core**, **Blazor**, and **SignalR**. Two players compete by taking turns submitting words from a given category, scoring points based on word length and letter values over 20 rounds.

---

## Features

- User registration and cookie-based authentication
- Create or join game sessions using a unique game code
- Real-time game updates via SignalR (with HTTP polling fallback)
- Category-based word challenges (Animals, Countries, Fruits/Vegetables, etc.)
- Scoring system with per-letter values, long-word bonuses, and pass penalties
- Game history tracking
- LAN multiplayer support — play on multiple devices on the same Wi-Fi network

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Blazor Server, Razor Components, Bootstrap 5 |
| Backend | ASP.NET Core 10, REST API, SignalR |
| Auth | ASP.NET Core Identity, JWT, Cookie-based sessions |
| Database | SQL Server, Entity Framework Core 10 |
| Testing | xUnit, Moq, Reqnroll (BDD), Playwright (E2E) |

---

## Architecture

The solution follows a **3-tier architecture** split across five projects:

```
OrdSpel/              # Blazor UI — Razor components, service layer, app state
OrdSpel.API/          # REST API — controllers, SignalR hub, DI setup
OrdSpel.BLL/          # Business Logic Layer — game rules, scoring, auth logic
OrdSpel.DAL/          # Data Access Layer — EF Core, repositories, migrations
OrdSpel.Shared/       # Shared DTOs, enums, game rule constants
```

Test projects mirror this structure:
```
OrdSpel.API.Test/         # xUnit unit + integration tests
OrdSpel.BLL.Test/         # xUnit unit tests
OrdSpel.DAL.Test.xUnit/   # DAL unit tests
OrdSpel.DAL.Test.BDD/     # Reqnroll BDD tests
OrdSpel.UI.Test/          # Playwright + Reqnroll E2E tests
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local instance or Docker)
- Visual Studio 2022 or VS Code with the C# extension

### Local Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/MammaGula/OrdSpel.git
   cd OrdSpel
   ```

2. **Configure the database**

   Update the connection strings in `OrdSpel.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "AppDbConnection": "Server=localhost;Database=ordspel;Trusted_Connection=True;TrustServerCertificate=True",
     "AuthDbConnection": "Server=localhost;Database=ordspel;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```

3. **Run the API**
   ```bash
   cd OrdSpel.API
   dotnet run --launch-profile https
   # API available at https://localhost:7099
   ```

4. **Run the UI** (separate terminal)
   ```bash
   cd OrdSpel
   dotnet run --launch-profile https
   # UI available at https://localhost:7265
   ```

5. Open your browser at `https://localhost:7265`

The database is seeded automatically on first run with categories and words.

### LAN Play (Windows)

To play across multiple devices on the same Wi-Fi network, run the included PowerShell script from the repo root:

```powershell
.\Starta-LAN.ps1
```

This script auto-detects your local IP address, starts both API and UI on LAN-accessible addresses, and opens the browser with a shareable link.

---

## API Overview

Base URL: `https://localhost:7099`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login |
| POST | `/api/auth/logout` | Logout |
| GET | `/api/auth/me` | Get current user |
| GET | `/api/category` | List all categories |
| GET | `/api/category/{id}/words` | Get words for a category |
| POST | `/api/game/create` | Create a new game session |
| POST | `/api/game/join` | Join a game by code |
| GET | `/api/game/{code}` | Get game status |
| GET | `/api/game/{code}/lobby` | Get lobby status |
| GET | `/api/game/{code}/result` | Get game results |
| GET | `/api/game/history` | Get game history for user |
| POST | `/api/games/{code}/turns` | Submit a word for the current turn |

A Postman collection (`OrdSpel API.postman_collection.json`) is included in the repo root for manual API testing.

### Real-time (SignalR)

Hub URL: `/hubs/game`

| Event | Direction | Description |
|---|---|---|
| `JoinGame(code)` | Client → Server | Join a game group |
| `TurnUpdated(code)` | Server → Client | A turn was played |
| `GameStateChanged(code)` | Server → Client | Game status changed |

---

## Game Rules

Defined in `OrdSpel.Shared/Constraints/GameRules.cs` and `LetterScores.cs`:

- **20 rounds** per game
- Words are scored per letter using a Scrabble-like letter value system
- **Long word bonus:** +3 points for words longer than 12 characters
- **Pass penalty:** -5 points for skipping a turn

---

## Running Tests

Run all tests:
```bash
dotnet test
```

Run a specific project:
```bash
dotnet test OrdSpel.API.Test
dotnet test OrdSpel.BLL.Test
dotnet test OrdSpel.DAL.Test.xUnit
dotnet test OrdSpel.DAL.Test.BDD
dotnet test OrdSpel.UI.Test
```

Tests use an in-memory database. To run the API itself against an in-memory database:
```bash
cd OrdSpel.API
dotnet run --launch-profile "OrdSpel.API (Test)"
```

Code coverage is collected via `coverlet.collector`.

---

## Project Highlights

- **Two separate DbContexts** — `AppDbContext` for game data, `AuthDbContext` for ASP.NET Identity, keeping concerns cleanly separated
- **Repository pattern** — all data access goes through typed repository interfaces
- **SignalR + polling fallback** — the UI polls every 3 seconds if the SignalR connection drops
- **Shared DTO library** — `OrdSpel.Shared` is referenced by both API and UI, ensuring type-safe contracts across the boundary
- **Environment-aware seeding** — the Test launch profile uses an in-memory database that resets on startup

---

## License

This project was developed initially as a group project and is not licensed for commercial use.
