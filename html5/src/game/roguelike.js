import { buildEnemyDeck, buildStarterDeck } from '../data/decks.js';
import { createBattle } from './engine.js';

export function createRun(selectedColors) {
  return {
    selectedColors,
    wins: 0,
    encounterIndex: 0,
    activeBattle: null,
    activeEnemy: null,
    over: false
  };
}

export function startEncounter(run) {
  const starterDeck = buildStarterDeck(run.selectedColors);
  const enemyPack = buildEnemyDeck(run.encounterIndex);

  run.activeBattle = createBattle(starterDeck, enemyPack.deck);
  run.activeEnemy = enemyPack.archetype;

  run.activeBattle.log.push(
    `Encounter ${run.encounterIndex + 1}: ${enemyPack.archetype.name} (difficulty ${enemyPack.archetype.difficulty})`
  );

  return run;
}

export function resolveEncounterResult(run) {
  if (!run.activeBattle?.over) return run;

  if (run.activeBattle.winner === 'player') {
    run.wins += 1;
    run.encounterIndex += 1;
  } else {
    run.over = true;
  }

  return run;
}
