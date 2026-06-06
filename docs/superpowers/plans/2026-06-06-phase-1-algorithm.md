# Phase 1 Algorithm Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build and verify the pure C# algorithm core for the NinjaTrader order helper.

**Architecture:** Keep the algorithm independent from NinjaTrader runtime APIs so it can be compiled and tested locally first. Use small data classes and calculators that can later be copied into or referenced by a NinjaScript Strategy.

**Tech Stack:** C# targeting .NET Framework-compatible syntax, Windows PowerShell, system C# compiler `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`.

---

### Task 1: Create Failing Algorithm Tests

**Files:**
- Create: `tests/NinjaTrader/TradePlanCalculatorTests.cs`

- [ ] **Step 1: Write the failing test runner**

Create a console test runner with test cases for bullish, bearish, doji, invalid range, invalid quantity, and tick rounding behavior. The test runner should reference the production namespace `NinjaTraderOrderHelper`.

- [ ] **Step 2: Run test compile to verify it fails**

Run:

```powershell
& 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe' /nologo /out:tests\NinjaTrader\TradePlanCalculatorTests.exe tests\NinjaTrader\TradePlanCalculatorTests.cs src\NinjaTrader\*.cs
```

Expected: FAIL because production source files do not exist yet.

### Task 2: Implement Trade Plan Core

**Files:**
- Create: `src/NinjaTrader/TradeDirection.cs`
- Create: `src/NinjaTrader/TradePlan.cs`
- Create: `src/NinjaTrader/TradePlanCalculator.cs`

- [ ] **Step 1: Implement minimal production code**

Add the direction enum, immutable trade plan data object, and calculator methods needed by the tests.

- [ ] **Step 2: Run tests to verify plan calculation passes**

Run:

```powershell
& 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe' /nologo /out:tests\NinjaTrader\TradePlanCalculatorTests.exe tests\NinjaTrader\TradePlanCalculatorTests.cs src\NinjaTrader\*.cs
& '.\tests\NinjaTrader\TradePlanCalculatorTests.exe'
```

Expected: PASS for plan calculation tests.

### Task 3: Implement Risk Preview Core

**Files:**
- Create: `src/NinjaTrader/RiskPreview.cs`
- Create: `src/NinjaTrader/RiskPreviewCalculator.cs`
- Modify: `tests/NinjaTrader/TradePlanCalculatorTests.cs`

- [ ] **Step 1: Add failing risk preview tests**

Add tests for long and short SL/TP amount calculation and invalid quantity rejection.

- [ ] **Step 2: Run tests to verify risk preview fails**

Run the compile and test commands from Task 2. Expected: FAIL because risk preview classes do not exist yet.

- [ ] **Step 3: Implement risk preview code**

Add `RiskPreview` and `RiskPreviewCalculator`, returning Chinese blocking messages for invalid quantity and non-orderable plans.

- [ ] **Step 4: Run all tests**

Run the compile and test commands from Task 2. Expected: all tests pass.

### Task 4: Document Usage

**Files:**
- Modify: `README.md`

- [ ] **Step 1: Update README**

Document the new source/test layout and the local test command.

- [ ] **Step 2: Run final verification**

Run:

```powershell
& 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe' /nologo /out:tests\NinjaTrader\TradePlanCalculatorTests.exe tests\NinjaTrader\TradePlanCalculatorTests.cs src\NinjaTrader\*.cs
& '.\tests\NinjaTrader\TradePlanCalculatorTests.exe'
```

Expected: all tests pass with Chinese success output.
