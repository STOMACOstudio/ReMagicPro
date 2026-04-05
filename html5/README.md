# ReMagic Lite HTML5 Prototype

This folder contains a **first implementation shell** for a lightweight HTML5 roguelike card game loop inspired by the Unity project.

## Implemented loop

1. Choose 1-2 colors
2. Build starter deck from deck database entries
3. Fight escalating PvE encounters until you lose

## What's included

- `src/data/cards.js`:
  - Initial card database subset in plain JS objects.
  - Card names and deck composition patterns align with the Unity card/deck database style.
- `src/data/decks.js`:
  - Starter deck entries per color.
  - Enemy archetype generation with difficulty scaling.
- `src/game/engine.js`:
  - Minimal battle engine (draw, land drop, cast first playable, attack/block, damage, win/loss).
- `src/game/roguelike.js`:
  - Run state and encounter progression.
- `src/main.js`:
  - UI wiring and event flow.

## Run locally

Because this uses ES modules, run it with a local static server.

### Option A: Python

```bash
cd html5
python3 -m http.server 8080
```

Open `http://localhost:8080`.

### Option B: Node (serve)

```bash
cd html5
npx serve .
```

## Important notes

- This is an MVP shell, not rules-complete MTG.
- Current engine intentionally simplifies many mechanics.
- The project is structured to allow incremental replacement of simplified logic with full rules.

## Suggested next steps

1. Migrate full `CardDatabase` + `DeckDatabase` to JSON
2. Add deterministic action model (`actions`, `stack`, `priority`)
3. Replace heuristic spell handling with per-card effect resolvers
4. Add richer card UI component (hover previews, zones, targeting)
5. Add save/load and meta progression
