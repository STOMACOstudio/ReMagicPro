import { getCard } from './cards.js';

const starterDeckEntries = {
  White: [
    ['Plains', 16], ['Eager Cadet', 3], ['Glory Seeker', 3], ['Angelic Wall', 3],
    ['Foot Soldiers', 2], ['Abbey Griffin', 2], ['Serra Angel', 1], ['Charge', 3], ['Sacred Nectar', 2]
  ],
  Blue: [
    ['Island', 16], ['Fugitive Wizard', 3], ['Coral Eel', 3], ['Sea Eagle', 3],
    ['Wind Drake', 3], ['Giant Octopus', 2], ['Unsummon', 3], ['Mahamoti Djinn', 1], ['Inertia Bubble', 2]
  ],
  Black: [
    ['Swamp', 16], ['Maggot Carrier', 3], ['Bog Imp', 3], ['Cyclopean Mummy', 3],
    ['Frozen Shade', 3], ['Giant Cockroach', 2], ['Nightmare', 1], ['Terror', 2], ['Raise Dead', 3]
  ],
  Red: [
    ['Mountain', 16], ['Goblin Sky Raider', 3], ['Crazed Goblin', 3], ['Wall of Earth', 3],
    ['Shock', 3], ['Goblin Raider', 3], ['Anaba Shaman', 2], ['Hill Giant', 2], ['Shivan Dragon', 1]
  ],
  Green: [
    ['Forest', 16], ['Argothian Swine', 2], ['Wall of Wood', 3], ['Canopy Spider', 3],
    ['Trained Armodon', 3], ['Grizzly Bears', 3], ['Rampant Growth', 2], ['Giant Spider', 2], ['Giant Growth', 3]
  ]
};

const enemyArchetypes = [
  { name: 'Farmhand', difficulty: 1, colors: ['White'] },
  { name: 'Village Mage', difficulty: 2, colors: ['Blue'] },
  { name: 'Crypt Dweller', difficulty: 3, colors: ['Black'] },
  { name: 'Raider', difficulty: 4, colors: ['Red'] },
  { name: 'Warden', difficulty: 5, colors: ['Green'] },
  { name: 'Duelist', difficulty: 6, colors: ['Red', 'White'] },
  { name: 'Skybinder', difficulty: 7, colors: ['Blue', 'White'] },
  { name: 'Grimcaller', difficulty: 8, colors: ['Black', 'Green'] }
];

function expandEntries(entries) {
  const deck = [];
  for (const [cardName, count] of entries) {
    for (let i = 0; i < count; i += 1) {
      deck.push(getCard(cardName));
    }
  }
  return deck;
}

function mergeEntries(colors) {
  const picked = colors.slice(0, 2);
  if (picked.length === 1) return starterDeckEntries[picked[0]];

  const [a, b] = picked;
  const first = starterDeckEntries[a].map(([name, count]) => [name, Math.max(1, Math.floor(count / 2))]);
  const second = starterDeckEntries[b].map(([name, count]) => [name, Math.max(1, Math.ceil(count / 2))]);
  return [...first, ...second];
}

export function buildStarterDeck(colors) {
  return expandEntries(mergeEntries(colors));
}

export function buildEnemyDeck(encounterIndex) {
  const archetype = enemyArchetypes[encounterIndex % enemyArchetypes.length];
  const baseEntries = mergeEntries(archetype.colors);

  // Scale by giving enemies extra copies of high-cost creatures as run goes on.
  const scaleBonus = Math.floor(encounterIndex / enemyArchetypes.length);
  const scaled = baseEntries.map(([name, count]) => {
    const card = getCard(name);
    if (card.type === 'Creature' && (card.manaCost ?? 0) >= 4) {
      return [name, count + Math.min(2, scaleBonus)];
    }
    return [name, count];
  });

  return {
    archetype,
    deck: expandEntries(scaled)
  };
}
