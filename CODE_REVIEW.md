# Dice Probability API — Code Review

**Date:** 2026-06-10
**Reviewed at commit:** `d211e03`
**Scope:** Full solution (`Dice` API + `DiceUnitTests`)

## Summary

The project is in good shape: it builds with zero warnings, all 24 unit tests pass, and the
API returns mathematically correct results. The architecture is clean — interfaces, dependency
injection, separated business logic, and a reasonable test suite. The inclusion-exclusion
formula in `WaysToRoll` is correct, and the win/lose/tie partition is sound.

The recommendations below are improvements, not blockers. They're ordered roughly by impact.

| Severity | Area | Count |
|----------|------|-------|
| High | Correctness / dead error handling | 2 |
| Medium | Numeric type choices, performance | 4 |
| Low | Design, code hygiene, tests | 7 |

---

## High

### 1. The `OverflowException` → 422 handling is effectively dead code

Both controller actions wrap the calculation in `catch (OverflowException)` and return
`422 Unprocessable Entity`. In practice this path is never reachable within the configured
validation ranges:

- `Math.Pow(sides, d1 + d2)` returns `double.PositiveInfinity` on overflow — it does **not**
  throw. With `sides ≤ 20` and `d1 + d2 ≤ 62`, the largest value is `20^62 ≈ 4.6e80`, far
  below `double.MaxValue (~1.8e308)`. Overflow can't happen.
- `(double)(a / (b * c))` in `MathHelper.Combinations` casts a `BigInteger` to `double`. That
  cast throws `OverflowException` only if the value exceeds `double.MaxValue`. The largest
  binomial reachable here, `C(620, 310) ≈ 10^186`, is still under the limit.

**Consequence:** if inputs ever *did* exceed `double`, `totalPossible` would become `Infinity`
and the API would silently return `{win:0, lose:0, tie:0}` — a wrong answer with a 200 status,
not the intended 422.

**Recommendation:** either (a) remove the dead `catch` blocks and rely on the range validation,
or (b) make the guard real by validating against an explicit complexity ceiling, or moving the
counts to `BigInteger`/`checked` arithmetic so an over-limit input is genuinely detected. Pick
one; today the code implies protection that doesn't exist.

`Dice/Controllers/DiceController.cs:38`, `:60`

### 2. Exact integer counts are carried as `double`, losing precision past 2^53

`IMathHelper.WaysToRoll` and `Combinations` return `double`, but both compute **exact integer
counts**. Any count above `2^53 (~9.0e15)` can no longer be represented exactly, so results
become approximate. `WaysToRoll(targetSum, 20, 100)` is well within the validated range and
produces counts far beyond `2^53`, meaning the `/api/dice/WaysToRoll` endpoint can return
subtly wrong integers for large inputs.

**Recommendation:** return `long` (or `BigInteger`, serialized as a string) for ways-to-roll
counts. Keep `double` only where a probability (0–1) is the actual output.

`Dice/BusinessLogic/MathHelper.cs:29`, `Dice/BusinessLogic/Interfaces/IMathHelper.cs:6`

---

## Medium

### 3. `Combinations` via full factorials is needlessly expensive

`Combinations(n, k)` computes `n! / (k! · (n−k)!)` by building three full factorials as
`BigInteger`s and dividing. For the probability calc this is called repeatedly with `n` up to
~620, so each call multiplies hundreds of large BigIntegers. This is the main reason
`31d20 vs 31d20` takes ~450 ms.

**Recommendation:** use the multiplicative formula, iterating only `min(k, n−k)` terms:

```csharp
public static BigInteger Combinations(int n, int k)
{
    if (k < 0 || k > n) return 0;
    k = Math.Min(k, n - k);
    BigInteger result = 1;
    for (int i = 0; i < k; i++)
        result = result * (n - i) / (i + 1);   // exact at each step
    return result;
}
```

This is O(k) multiplications on far smaller intermediate values and removes the need for a
separate `Factorial`.

`Dice/BusinessLogic/MathHelper.cs:8`

### 4. `Combinations` has no guard for `k > n` or `k < 0`

`Factorial(n - k)` throws `ArgumentOutOfRangeException` when `k > n`. Current callers stay in
range, but the public method is a latent trap. The multiplicative version in #3 returns `0`
for out-of-range `k`, which is the conventional and safe binomial definition.

`Dice/BusinessLogic/MathHelper.cs:8`

### 5. Memoization is per-request only

`ProbabilityToWinLoseTie` builds a fresh `cache` dictionary on every call, and `WaysToRoll`
recomputes its `Combinations` from scratch each time. Since `DiceProbabilityCalculator` and
`MathHelper` are registered as singletons (good — they're stateless), a process-lifetime cache
of `WaysToRoll(sum, dice, sides)` results would eliminate repeated work across requests.

**Recommendation:** add a thread-safe `ConcurrentDictionary` cache inside `MathHelper`, or use
`IMemoryCache`. (If you do, drop the local per-call dictionary in the calculator.)

`Dice/BusinessLogic/DiceProbabilityCalculator.cs:17`

### 6. `Math.Pow(-1, k)` for sign alternation

Minor, but `Math.Pow(-1, k)` does a floating-point call per term. Prefer
`((k & 1) == 0) ? 1 : -1`, which is exact and free.

`Dice/BusinessLogic/MathHelper.cs:36`

---

## Low

### 7. Controller depends on `IMathHelper` directly

`DiceController` injects `IMathHelper` to serve `/WaysToRoll`, leaking the math layer into the
presentation layer. Consider routing it through the business-logic abstraction (e.g. add a
`WaysToRoll` method to a service interface) so the controller depends on one cohesive boundary.

`Dice/Controllers/DiceController.cs:14`

### 8. Interface / visibility inconsistency in `MathHelper`

`Factorial` is `public` but not on `IMathHelper`; `Combinations` is on the interface but only
used internally. If `Combinations`/`Factorial` are implementation details, make them non-public
and shrink `IMathHelper` to just `WaysToRoll`. Smaller interfaces are easier to mock and reason
about (ISP).

`Dice/BusinessLogic/Interfaces/IMathHelper.cs:5`

### 9. Error responses are plain strings, not `ProblemDetails`

`UnprocessableEntity("Inputs too large to calculate.")` returns a bare string. ASP.NET Core's
`ProblemDetails` (RFC 7807) gives clients a consistent, machine-readable error shape and is
what the `[ApiController]` model-validation failures already produce — mixing the two is
inconsistent.

`Dice/Controllers/DiceController.cs:40`, `:62`

### 10. Redundant `using` directives

With `ImplicitUsings` enabled, `using System;` and `using Microsoft.AspNetCore.Http;` in
`DiceController.cs` are unnecessary.

`Dice/Controllers/DiceController.cs:1`, `:4`

### 11. The "overflow" tests only exercise mocks, hiding issue #1

`DiceControllerTests` verifies the 422 path by having the mock *throw* `OverflowException`.
That proves the catch block maps the exception correctly, but it can never reveal that real
math never throws it. An end-to-end test driving genuine large inputs would have surfaced the
dead-code gap.

**Recommendation:** add an integration test (`WebApplicationFactory`) that hits the real
endpoints with boundary inputs and asserts on actual status + body.

`DiceUnitTests/Controllers/DiceControllerTests.cs:39`

### 12. Missing edge-case coverage in `MathHelperTests`

No tests for: `Combinations` with `k > n` (current behavior throws), `WaysToRoll` for a sum
below the minimum (`sum < dice`), or large-input precision. Adding these would pin down the
behavior you choose for #2/#4.

`DiceUnitTests/BusinessLogic/MathHelperTests.cs`

### 13. Package currency

`Swashbuckle.AspNetCore 6.5.0` is somewhat dated. Note also that .NET 9+ ships OpenAPI document
generation in the box (`Microsoft.AspNetCore.OpenApi`); worth considering at the next framework
bump. Not urgent on .NET 8.

`Dice/Dice.csproj:10`

---

## What's already good

- Clean DI + interface seam; stateless services correctly registered as singletons.
- Correct inclusion-exclusion implementation and a sound win/lose/tie partition.
- Input validation via data annotations with a genuinely useful explanatory comment on the
  `Sides` cap.
- Readable, well-named tests with descriptive `DisplayName`s, and a real `README`.
- Swagger wired up and gated to Development.
