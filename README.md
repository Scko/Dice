# Dice Probability API

A .NET 8 Web API for calculating dice roll probabilities.

## Endpoints

### `GET /api/dice`

Calculates the win, lose, and tie probabilities when one player rolls a set of dice against another player's roll.

**Query parameters**

| Parameter | Type | Range | Description |
|-----------|------|-------|-------------|
| `dice1` | int | 1–31 | Number of dice for player 1 |
| `dice2` | int | 1–31 | Number of dice for player 2 |
| `sides` | int | 1–20 | Number of sides on each die |

**Response**

```json
{
  "win": 0.4167,
  "lose": 0.4167,
  "tie": 0.1667
}
```

---

### `GET /api/dice/WaysToRoll`

Calculates the number of ways to roll a target sum with a given number of dice.

**Query parameters**

| Parameter | Type | Range | Description |
|-----------|------|-------|-------------|
| `targetSum` | int | ≥1 | The target sum to roll |
| `dice` | int | 1–20 | Number of dice |
| `sides` | int | 1–100 | Number of sides on each die |

**Response**

```json
6
```

Returns `0` if the target sum is impossible with the given dice and sides.

---

## Running locally

```bash
dotnet run --project Dice
```

Swagger UI is available at `/swagger` when running in the Development environment.

## Running tests

```bash
dotnet test
```

## Tech stack

- .NET 8 Web API
- Swashbuckle (Swagger)
- xUnit + Moq (unit tests)
