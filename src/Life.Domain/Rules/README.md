# Rules Directory Structure

This directory contains all cellular automaton rule implementations for the Game of Life system.

## Organization

### `/Standard` Directory
Contains well-known, classic cellular automaton rule sets:
- **ConwaysGameOfLifeRules.cs** - The original Conway's Game of Life (B3/S23)
- **HighLifeRules.cs** - HighLife variant with additional birth condition (B36/S23)
- **DayAndNightRules.cs** - Symmetric rules where patterns and inverses behave identically (B3678/S34678)
- **SeedsRules.cs** - Explosive rules where all cells die each generation (B2/S)

### `/Custom` Directory
Contains configurable rule implementations:
- **CustomRules.cs** - User-defined rules with arbitrary birth/survival conditions

### Root Files
- **ICellularAutomatonRules.cs** - Interface and base class for all rules
- **CellularAutomatonRuleFactory.cs** - Factory for creating rule instances

## Usage

```csharp
// Using factory with standard rules
var conwayRules = CellularAutomatonRuleFactory.CreateRules("conway");
var highLifeRules = CellularAutomatonRuleFactory.CreateRules("highlife");

// Using factory with notation
var customFromNotation = CellularAutomatonRuleFactory.CreateFromNotation("B36/S23");

// Direct instantiation
var customRules = new CustomRules(new[] { 3, 6 }, new[] { 2, 3 });

// With board
var board = new Board(10, 10, alivePositions);
var nextGen = board.GenerateNextGeneration(rules);
```

## Rule Notation

Rules follow the standard B/S notation:
- **B** (Birth): Numbers indicate neighbor counts that cause dead cells to become alive
- **S** (Survival): Numbers indicate neighbor counts that keep alive cells alive

Examples:
- B3/S23 - Conway's Game of Life
- B36/S23 - HighLife
- B2/S - Seeds (no survival conditions)

## Adding New Rules

### Standard Rules
1. Create a new file in `/Standard` directory
2. Inherit from `CellularAutomatonRulesBase`
3. Implement required methods and properties
4. Add to factory in `CellularAutomatonRuleFactory.cs`

### Custom Rules
Use the `CustomRules` class for runtime-configurable rules or create specialized implementations in the `/Custom` directory for complex logic.