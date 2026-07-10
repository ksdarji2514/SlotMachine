# Slot Machine API

A .NET slot machine backend built for the BlazeSoft coding challenge. Players can add funds, spin the reels, and receive payouts based on straight and zigzag win lines across a configurable matrix.

## Prerequisites

Before running the project, make sure you have:

| Requirement | Version / Notes |
|-------------|-----------------|
| [.NET SDK](https://dotnet.microsoft.com/download) | **.NET 10** (`net10.0`) |
| [MongoDB](https://www.mongodb.com/try/download/community) | Running locally on `mongodb://localhost:27017` (default) |

Optional:

- **Visual Studio 2022+** or **VS Code** with the C# extension
- **curl**, **Postman**, or a similar tool for testing the API

## Quick Start

### 1. Start MongoDB

Ensure MongoDB is running on your machine. The default connection string is:

```
mongodb://localhost:27017
```

### 2. Configure the application (optional)

Connection settings live in `SlotMachine.API/appsettings.json`:

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "slotmachine"
  }
}
```

Update these values if your MongoDB instance uses a different host, port, or database name.

### 3. Restore, build, and run

From the solution root:

```bash
dotnet restore
dotnet build
dotnet run --project SlotMachine.API
```

The API starts at:

- HTTP: `http://localhost:5217`
- HTTPS: `https://localhost:7083` (when using the `https` launch profile)

### 4. Open Swagger (Development)

When running in the Development environment, Swagger UI is available at:

```
http://localhost:5217/swagger
```

## API Endpoints

### Add funds to a player

Creates the player if they do not exist.

```http
POST /api/player/balance
Content-Type: application/json

{
  "playerId": "player-1",
  "amount": 1000
}
```

### Spin the slot machine

Deducts the bet, generates a random matrix, calculates the win, and credits the player's balance.

```http
POST /api/slot/spin
Content-Type: application/json

{
  "playerId": "player-1",
  "bet": 10
}
```

**Example response:**

```json
{
  "success": true,
  "data": {
    "matrix": [
      [3, 3, 3, 4, 5],
      [2, 3, 2, 3, 3],
      [1, 2, 3, 3, 3]
    ],
    "win": 270,
    "balance": 1260
  }
}
```

## Running Tests

```bash
dotnet test
```

Unit tests cover win-line calculation (straight and zigzag lines) and core spin flow behavior.

## Project Structure

```
SlotMachine/
├── SlotMachine.API/          # Web API, controllers, middleware, validators
├── SlotMachine.Service/      # Business logic (spin, win calculation, player)
├── SlotMachine.Repository/   # MongoDB data access
├── SlotMachine.Database/     # MongoDB context and entities
├── SlotMachine.DTO/          # Request/response models
├── SlotMachine.Common/       # Shared constants and exceptions
└── SlotMachine.Tests/        # Unit tests
```

## How Wins Are Calculated

- The slot grid is **configurable** (default: 5 columns × 3 rows) and stored in MongoDB.
- Each cell contains a random digit from **0–9**.
- A paying line must have **more than 2 identical digits starting at column 0**.
- Payout per line: `bet × (sum of digits in the winning run)`.
- Two line types are evaluated:
  - **Straight** — one horizontal line per row
  - **Zigzag** — one bouncing path per starting row (skipped when height is 1)

## Error Handling

| Scenario | HTTP Status |
|----------|-------------|
| Validation failure | `400 Bad Request` |
| Player not found | `404 Not Found` |
| Insufficient balance | `409 Conflict` |
| Missing slot configuration | `500 Internal Server Error` |

## Notes

- On first startup, the API seeds a default game configuration (`5×3`) into the `config` collection if one does not exist.
- Balance updates use atomic MongoDB operations to safely handle concurrent spins.
