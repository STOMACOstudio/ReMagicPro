export const cards = {
  Plains: { name: 'Plains', type: 'Land', colors: ['White'] },
  Island: { name: 'Island', type: 'Land', colors: ['Blue'] },
  Swamp: { name: 'Swamp', type: 'Land', colors: ['Black'] },
  Mountain: { name: 'Mountain', type: 'Land', colors: ['Red'] },
  Forest: { name: 'Forest', type: 'Land', colors: ['Green'] },

  // White
  'Eager Cadet': { name: 'Eager Cadet', type: 'Creature', manaCost: 1, power: 1, toughness: 1, colors: ['White'] },
  'Glory Seeker': { name: 'Glory Seeker', type: 'Creature', manaCost: 2, power: 2, toughness: 2, colors: ['White'] },
  'Angelic Wall': { name: 'Angelic Wall', type: 'Creature', manaCost: 2, power: 0, toughness: 4, colors: ['White'], tags: ['Defender'] },
  'Foot Soldiers': { name: 'Foot Soldiers', type: 'Creature', manaCost: 3, power: 2, toughness: 3, colors: ['White'] },
  'Abbey Griffin': { name: 'Abbey Griffin', type: 'Creature', manaCost: 4, power: 2, toughness: 2, colors: ['White'], tags: ['Flying'] },
  'Serra Angel': { name: 'Serra Angel', type: 'Creature', manaCost: 5, power: 4, toughness: 4, colors: ['White'], tags: ['Flying'] },
  Charge: { name: 'Charge', type: 'Spell', manaCost: 1, effect: 'buff_attackers' },
  Pacifism: { name: 'Pacifism', type: 'Spell', manaCost: 2, effect: 'pacify' },
  Demystify: { name: 'Demystify', type: 'Spell', manaCost: 1, effect: 'noop' },
  'Sacred Nectar': { name: 'Sacred Nectar', type: 'Spell', manaCost: 2, effect: 'gain_life_4' },

  // Blue
  'Fugitive Wizard': { name: 'Fugitive Wizard', type: 'Creature', manaCost: 1, power: 1, toughness: 1, colors: ['Blue'] },
  'Coral Eel': { name: 'Coral Eel', type: 'Creature', manaCost: 2, power: 2, toughness: 1, colors: ['Blue'] },
  'Sea Eagle': { name: 'Sea Eagle', type: 'Creature', manaCost: 3, power: 2, toughness: 3, colors: ['Blue'], tags: ['Flying'] },
  'Wind Drake': { name: 'Wind Drake', type: 'Creature', manaCost: 3, power: 2, toughness: 2, colors: ['Blue'], tags: ['Flying'] },
  'Giant Octopus': { name: 'Giant Octopus', type: 'Creature', manaCost: 4, power: 3, toughness: 3, colors: ['Blue'] },
  'Mahamoti Djinn': { name: 'Mahamoti Djinn', type: 'Creature', manaCost: 6, power: 5, toughness: 6, colors: ['Blue'], tags: ['Flying'] },
  Unsummon: { name: 'Unsummon', type: 'Spell', manaCost: 1, effect: 'bounce' },
  Flight: { name: 'Flight', type: 'Spell', manaCost: 1, effect: 'noop' },
  'Inertia Bubble': { name: 'Inertia Bubble', type: 'Spell', manaCost: 3, effect: 'noop' },

  // Black
  'Maggot Carrier': { name: 'Maggot Carrier', type: 'Creature', manaCost: 1, power: 1, toughness: 1, colors: ['Black'] },
  'Bog Imp': { name: 'Bog Imp', type: 'Creature', manaCost: 2, power: 1, toughness: 1, colors: ['Black'], tags: ['Flying'] },
  'Cyclopean Mummy': { name: 'Cyclopean Mummy', type: 'Creature', manaCost: 3, power: 2, toughness: 1, colors: ['Black'] },
  'Frozen Shade': { name: 'Frozen Shade', type: 'Creature', manaCost: 3, power: 0, toughness: 1, colors: ['Black'] },
  'Giant Cockroach': { name: 'Giant Cockroach', type: 'Creature', manaCost: 4, power: 4, toughness: 2, colors: ['Black'] },
  Nightmare: { name: 'Nightmare', type: 'Creature', manaCost: 6, power: 5, toughness: 5, colors: ['Black'] },
  Terror: { name: 'Terror', type: 'Spell', manaCost: 2, effect: 'destroy' },
  'Raise Dead': { name: 'Raise Dead', type: 'Spell', manaCost: 1, effect: 'raise_dead' },

  // Red
  'Goblin Sky Raider': { name: 'Goblin Sky Raider', type: 'Creature', manaCost: 2, power: 1, toughness: 2, colors: ['Red'], tags: ['Flying'] },
  'Crazed Goblin': { name: 'Crazed Goblin', type: 'Creature', manaCost: 1, power: 1, toughness: 1, colors: ['Red'] },
  'Wall of Earth': { name: 'Wall of Earth', type: 'Creature', manaCost: 3, power: 0, toughness: 4, colors: ['Red'], tags: ['Defender'] },
  'Goblin Raider': { name: 'Goblin Raider', type: 'Creature', manaCost: 2, power: 2, toughness: 2, colors: ['Red'] },
  'Anaba Shaman': { name: 'Anaba Shaman', type: 'Creature', manaCost: 4, power: 2, toughness: 2, colors: ['Red'] },
  'Hill Giant': { name: 'Hill Giant', type: 'Creature', manaCost: 4, power: 3, toughness: 3, colors: ['Red'] },
  'Shivan Dragon': { name: 'Shivan Dragon', type: 'Creature', manaCost: 6, power: 5, toughness: 5, colors: ['Red'], tags: ['Flying'] },
  Shock: { name: 'Shock', type: 'Spell', manaCost: 1, effect: 'deal_2' },
  Shatter: { name: 'Shatter', type: 'Spell', manaCost: 2, effect: 'noop' },

  // Green
  'Wall of Wood': { name: 'Wall of Wood', type: 'Creature', manaCost: 1, power: 0, toughness: 3, colors: ['Green'], tags: ['Defender'] },
  'Canopy Spider': { name: 'Canopy Spider', type: 'Creature', manaCost: 2, power: 1, toughness: 3, colors: ['Green'] },
  'Trained Armodon': { name: 'Trained Armodon', type: 'Creature', manaCost: 3, power: 3, toughness: 3, colors: ['Green'] },
  'Grizzly Bears': { name: 'Grizzly Bears', type: 'Creature', manaCost: 2, power: 2, toughness: 2, colors: ['Green'] },
  'Giant Spider': { name: 'Giant Spider', type: 'Creature', manaCost: 4, power: 2, toughness: 4, colors: ['Green'] },
  'Argothian Swine': { name: 'Argothian Swine', type: 'Creature', manaCost: 3, power: 3, toughness: 3, colors: ['Green'] },
  'Might of Oaks': { name: 'Might of Oaks', type: 'Spell', manaCost: 4, effect: 'noop' },
  'Giant Growth': { name: 'Giant Growth', type: 'Spell', manaCost: 1, effect: 'noop' },
  'Rampant Growth': { name: 'Rampant Growth', type: 'Spell', manaCost: 2, effect: 'ramp' }
};

export function getCard(name) {
  const card = cards[name];
  if (!card) {
    throw new Error(`Card '${name}' not found in prototype card DB.`);
  }
  return { ...card };
}
