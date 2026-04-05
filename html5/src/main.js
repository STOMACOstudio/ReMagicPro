import { playRound } from './game/engine.js';
import { createRun, resolveEncounterResult, startEncounter } from './game/roguelike.js';

const colors = ['White', 'Blue', 'Black', 'Red', 'Green'];
const selectedColors = new Set();
let run = null;

const colorPicker = document.getElementById('color-picker');
const startRunBtn = document.getElementById('start-run');
const setupSection = document.getElementById('setup');
const runSection = document.getElementById('run');
const battleSection = document.getElementById('battle');
const playTurnBtn = document.getElementById('play-turn');
const nextEncounterBtn = document.getElementById('next-encounter');
const restartRunBtn = document.getElementById('restart-run');
const logElement = document.getElementById('log');

const fields = {
  wins: document.getElementById('wins'),
  encounter: document.getElementById('encounter'),
  selectedColors: document.getElementById('selected-colors'),
  playerLife: document.getElementById('player-life'),
  playerDeck: document.getElementById('player-deck'),
  playerHand: document.getElementById('player-hand'),
  playerField: document.getElementById('player-field'),
  playerLands: document.getElementById('player-lands'),
  enemyLife: document.getElementById('enemy-life'),
  enemyDeck: document.getElementById('enemy-deck'),
  enemyHand: document.getElementById('enemy-hand'),
  enemyField: document.getElementById('enemy-field'),
  enemyLands: document.getElementById('enemy-lands')
};

function appendLog(lines) {
  const items = Array.isArray(lines) ? lines : [lines];
  for (const line of items) {
    const li = document.createElement('li');
    li.textContent = line;
    logElement.prepend(li);
  }
}

function renderColorPicker() {
  colorPicker.innerHTML = '';
  for (const color of colors) {
    const button = document.createElement('button');
    button.type = 'button';
    button.className = 'color-button';
    button.textContent = color;

    button.addEventListener('click', () => {
      if (selectedColors.has(color)) {
        selectedColors.delete(color);
      } else {
        if (selectedColors.size >= 2) {
          const first = selectedColors.values().next().value;
          selectedColors.delete(first);
        }
        selectedColors.add(color);
      }

      renderColorPicker();
      startRunBtn.disabled = selectedColors.size === 0;
    });

    if (selectedColors.has(color)) {
      button.classList.add('selected');
    }

    colorPicker.appendChild(button);
  }
}

function renderBattle() {
  if (!run?.activeBattle) return;

  const { player, enemy } = run.activeBattle;

  fields.wins.textContent = String(run.wins);
  fields.encounter.textContent = String(run.encounterIndex + 1);
  fields.selectedColors.textContent = run.selectedColors.join(', ');

  fields.playerLife.textContent = String(player.life);
  fields.playerDeck.textContent = String(player.deck.length);
  fields.playerHand.textContent = String(player.hand.length);
  fields.playerField.textContent = String(player.battlefield.filter((c) => c.type === 'Creature').length);
  fields.playerLands.textContent = String(player.landsInPlay);

  fields.enemyLife.textContent = String(enemy.life);
  fields.enemyDeck.textContent = String(enemy.deck.length);
  fields.enemyHand.textContent = String(enemy.hand.length);
  fields.enemyField.textContent = String(enemy.battlefield.filter((c) => c.type === 'Creature').length);
  fields.enemyLands.textContent = String(enemy.landsInPlay);
}

function startNewEncounter() {
  startEncounter(run);
  appendLog(`--- Encounter ${run.encounterIndex + 1} vs ${run.activeEnemy.name} ---`);
  appendLog(run.activeBattle.log);
  run.activeBattle.log.length = 0;
  renderBattle();

  playTurnBtn.disabled = false;
  nextEncounterBtn.classList.add('hidden');
}

startRunBtn.addEventListener('click', () => {
  run = createRun([...selectedColors]);
  logElement.innerHTML = '';
  setupSection.classList.add('hidden');
  runSection.classList.remove('hidden');
  battleSection.classList.remove('hidden');

  startNewEncounter();
});

playTurnBtn.addEventListener('click', () => {
  if (!run?.activeBattle || run.activeBattle.over) return;

  const oldLogLen = run.activeBattle.log.length;
  playRound(run.activeBattle);
  const newLines = run.activeBattle.log.slice(oldLogLen);
  appendLog(newLines);
  renderBattle();

  if (run.activeBattle.over) {
    resolveEncounterResult(run);

    if (run.over) {
      appendLog(`Run over. Final wins: ${run.wins}.`);
      playTurnBtn.disabled = true;
      nextEncounterBtn.classList.add('hidden');
      return;
    }

    appendLog(`Victory. Wins: ${run.wins}. Click Next Encounter.`);
    playTurnBtn.disabled = true;
    nextEncounterBtn.classList.remove('hidden');
  }
});

nextEncounterBtn.addEventListener('click', () => {
  startNewEncounter();
});

restartRunBtn.addEventListener('click', () => {
  run = null;
  selectedColors.clear();
  logElement.innerHTML = '';
  setupSection.classList.remove('hidden');
  runSection.classList.add('hidden');
  battleSection.classList.add('hidden');
  startRunBtn.disabled = true;
  renderColorPicker();
});

renderColorPicker();
