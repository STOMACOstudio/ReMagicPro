function shuffle(array) {
  const copy = [...array];
  for (let i = copy.length - 1; i > 0; i -= 1) {
    const j = Math.floor(Math.random() * (i + 1));
    [copy[i], copy[j]] = [copy[j], copy[i]];
  }
  return copy;
}

function createPlayer(deck, label) {
  const library = shuffle(deck).map((card, index) => ({ ...card, id: `${label}-${index}-${card.name}` }));
  return {
    label,
    life: 20,
    deck: library,
    hand: [],
    battlefield: [],
    graveyard: [],
    landsInPlay: 0,
    turn: 0
  };
}

function draw(player, amount = 1) {
  for (let i = 0; i < amount; i += 1) {
    const card = player.deck.shift();
    if (!card) return;
    player.hand.push(card);
  }
}

function playLandIfPossible(player) {
  const idx = player.hand.findIndex((card) => card.type === 'Land');
  if (idx >= 0) {
    const [land] = player.hand.splice(idx, 1);
    player.battlefield.push(land);
    player.landsInPlay += 1;
    return `${player.label} plays ${land.name}.`;
  }
  return null;
}

function spendableMana(player) {
  return player.landsInPlay;
}

function castFirstPlayable(player, opponent) {
  const mana = spendableMana(player);
  const idx = player.hand.findIndex((card) => (card.manaCost ?? 0) <= mana && card.type !== 'Land');
  if (idx < 0) return null;

  const [card] = player.hand.splice(idx, 1);
  if (card.type === 'Creature') {
    player.battlefield.push({ ...card, summoningSick: true, tapped: false, pacified: false, damage: 0 });
    return `${player.label} casts ${card.name}.`;
  }

  // Minimal spell support for first prototype.
  switch (card.effect) {
    case 'deal_2':
      opponent.life -= 2;
      break;
    case 'gain_life_4':
      player.life += 4;
      break;
    case 'bounce': {
      const targetIndex = opponent.battlefield.findIndex((c) => c.type === 'Creature');
      if (targetIndex >= 0) {
        const [bounced] = opponent.battlefield.splice(targetIndex, 1);
        opponent.hand.push({ ...bounced, summoningSick: false, tapped: false, damage: 0 });
      }
      break;
    }
    case 'destroy': {
      const targetIndex = opponent.battlefield.findIndex((c) => c.type === 'Creature');
      if (targetIndex >= 0) {
        const [dead] = opponent.battlefield.splice(targetIndex, 1);
        opponent.graveyard.push(dead);
      }
      break;
    }
    case 'raise_dead': {
      const targetIndex = player.graveyard.findIndex((c) => c.type === 'Creature');
      if (targetIndex >= 0) {
        const [raised] = player.graveyard.splice(targetIndex, 1);
        player.hand.push(raised);
      }
      break;
    }
    case 'pacify': {
      const target = opponent.battlefield.find((c) => c.type === 'Creature' && !c.pacified);
      if (target) target.pacified = true;
      break;
    }
    case 'buff_attackers': {
      for (const creature of player.battlefield) {
        if (creature.type === 'Creature') {
          creature.tempPowerBonus = (creature.tempPowerBonus ?? 0) + 1;
        }
      }
      break;
    }
    default:
      break;
  }

  player.graveyard.push(card);
  return `${player.label} casts ${card.name}.`;
}

function readyCreatures(player) {
  for (const permanent of player.battlefield) {
    if (permanent.type !== 'Creature') continue;
    permanent.tapped = false;
    if (permanent.summoningSick) permanent.summoningSick = false;
    permanent.damage = 0;
    permanent.tempPowerBonus = 0;
  }
}

function creaturePower(creature) {
  return (creature.power ?? 0) + (creature.tempPowerBonus ?? 0);
}

function creatureToughness(creature) {
  return creature.toughness ?? 0;
}

function attackStep(attacker, defender) {
  const attackers = attacker.battlefield.filter(
    (c) => c.type === 'Creature' && !c.summoningSick && !c.tapped && !c.pacified && !(c.tags || []).includes('Defender')
  );

  if (attackers.length === 0) {
    return `${attacker.label} attacks with no creatures.`;
  }

  const blockers = defender.battlefield
    .filter((c) => c.type === 'Creature' && !c.tapped && !c.pacified)
    .sort((a, b) => creaturePower(b) - creaturePower(a));

  let directDamage = 0;
  const deadAttackers = [];
  const deadBlockers = [];

  for (const atk of attackers) {
    atk.tapped = true;
    const block = blockers.shift();
    if (!block) {
      directDamage += creaturePower(atk);
      continue;
    }

    if (creaturePower(atk) >= creatureToughness(block)) deadBlockers.push(block);
    if (creaturePower(block) >= creatureToughness(atk)) deadAttackers.push(atk);
  }

  defender.life -= directDamage;

  for (const dead of deadAttackers) {
    const idx = attacker.battlefield.findIndex((c) => c.id === dead.id);
    if (idx >= 0) attacker.graveyard.push(...attacker.battlefield.splice(idx, 1));
  }

  for (const dead of deadBlockers) {
    const idx = defender.battlefield.findIndex((c) => c.id === dead.id);
    if (idx >= 0) defender.graveyard.push(...defender.battlefield.splice(idx, 1));
  }

  return `${attacker.label} attacks with ${attackers.length}, dealing ${directDamage} direct damage.`;
}

function upkeepTurn(active, other) {
  active.turn += 1;
  readyCreatures(active);
  draw(active, 1);
  const landLog = playLandIfPossible(active);
  const castLog = castFirstPlayable(active, other);
  return [
    `${active.label} starts turn ${active.turn}.`,
    landLog,
    castLog
  ].filter(Boolean);
}

export function createBattle(playerDeck, enemyDeck) {
  const player = createPlayer(playerDeck, 'Player');
  const enemy = createPlayer(enemyDeck, 'Enemy');

  draw(player, 5);
  draw(enemy, 5);

  return {
    over: false,
    winner: null,
    turnNumber: 1,
    player,
    enemy,
    log: ['Battle starts. Both players draw 5 cards.']
  };
}

export function playRound(state) {
  if (state.over) return state;

  const lines = [];
  lines.push(...upkeepTurn(state.player, state.enemy));
  lines.push(attackStep(state.player, state.enemy));

  if (state.enemy.life <= 0) {
    state.over = true;
    state.winner = 'player';
    lines.push('Enemy is defeated.');
    state.log.push(...lines);
    return state;
  }

  lines.push(...upkeepTurn(state.enemy, state.player));
  lines.push(attackStep(state.enemy, state.player));

  if (state.player.life <= 0) {
    state.over = true;
    state.winner = 'enemy';
    lines.push('Player is defeated.');
  }

  if (state.player.deck.length === 0) {
    state.over = true;
    state.winner = 'enemy';
    lines.push('Player decked out.');
  }

  if (state.enemy.deck.length === 0) {
    state.over = true;
    state.winner = 'player';
    lines.push('Enemy decked out.');
  }

  state.turnNumber += 1;
  state.log.push(...lines);
  return state;
}
