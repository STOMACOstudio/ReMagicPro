using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CardDatabase
{
    private static Dictionary<string, CardData> cardsByName = new Dictionary<string, CardData>();

    static CardDatabase()
    {
        //BASIC LANDS
            Add(new CardData //Plains
            {
                cardName = "Plains",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "White" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/plains"),
                artist = "Scott Bailey"
            });
            Add(new CardData //Island
            {
                cardName = "Island",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Blue" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/island"),
                artist = "Rob Alexander"
            });
            Add(new CardData //Swamp
            {
                cardName = "Swamp",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Black" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/swamp"),
                artist = "Alan Pollack"
            });
            Add(new CardData //Mountain
            {
                cardName = "Mountain",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Red" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/mountain"),
                artist = "John Avon"
            });
            Add(new CardData //Forest
            {
                cardName = "Forest",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Green" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/forest"),
                artist = "David Martin"
            });

        // Creatures
            //WHITE
            Add(new CardData //Voice of the provinces
                    {
                    cardName = "Voice of the Provinces",
                    artist = "Igor Kieryluk",
                    rarity = "Common",
                    manaCost = 6,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Angel" },
                    keywordAbilities = new List<KeywordAbility> {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/voice_of_the_provinces"),
                    flavorText = "Her horn is heard across Innistrad, lifing the hearts of the righteous",
                    abilities = new List<CardAbility>
                    {
                    new CardAbility
                    {
                        timing = TriggerTiming.OnEnter,
                        description = "create a white 1/1 Human token.",
                        effect = (Player owner, Card unused) =>
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                Card human = CardFactory.Create("Human");
                                if (human == null)
                                {
                                    Debug.LogError("Failed to spawn Human Token — check card database!");
                                    return;
                                }

                                GameManager.Instance.SummonToken(human, owner);
                            }
                        }
                    }
                    }
                    });
            Add(new CardData // Holy strength
                    {
                        cardName = "Holy Strength",
                        artist = "Scott M. Fisher",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string> { "White" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = 1,
                        toughnessBuff = 2,
                        artwork = Resources.Load<Sprite>("Art/holy_strength"),
                        flavorText = "Such power protects the body with the strength of the soul.",
                        rulesText = "Enchanted creature gets +1/+2."
                    });
                Add(new CardData // Divine transformation
                    {
                        cardName = "Divine Transformation",
                        artist = "NèNè Thomas",
                        rarity = "Uncommon",
                        manaCost = 4,
                        color = new List<string> { "White", "White" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = 3,
                        toughnessBuff = 3,
                        artwork = Resources.Load<Sprite>("Art/divine_transformation"),
                        flavorText = "Glory surged through her and radiance surrounded her. All things were possible with the blessing of the Divine.",
                        rulesText = "Enchanted creature gets +3/+3."
                    });
                Add(new CardData // Spirit Away
                    {
                        cardName = "Spirit Away",
                        artist = "Greg Staples",
                        rarity = "Rare",
                        manaCost = 7,
                        color = new List<string> { "Blue", "Blue" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        gainControlOfCreature = true,
                        powerBuff = 2,
                        toughnessBuff = 2,
                        keywordBuff = KeywordAbility.Flying,
                        artwork = Resources.Load<Sprite>("Art/spirit_away"),
                        flavorText = "The fear of slipping from the geist's tenuous grip overwhelmed Tolo's joy at his first flight.",
                        rulesText = "You control enchanted creature. Enchanted creature gets +2/+2 and has flying."
                    });
                Add(new CardData // Eager cadet
                    {
                    cardName = "Eager Cadet",
                    artist = "Greg & Tim Hildebrandt",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human", "Soldier" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Training? Seeing my crops burnt to cinders was all the 'training' I needed.",
                    artwork = Resources.Load<Sprite>("Art/eager_cadet")
                    });
                Add(new CardData // Glory seeker
                    {
                    cardName = "Glory Seeker",
                    artist = "Dave Dorman",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Human", "Soldier" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "The turning of the tide always begins with one soldier's decision to head back into the fray.",
                    artwork = Resources.Load<Sprite>("Art/glory_seeker")
                    });
                Add(new CardData // Alaborn trooper
                    {
                    cardName = "Alaborn Trooper",
                    artist = "Lubov",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 3,
                    subtypes = new List<string> { "Human", "Soldier" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "I dedicate my body to my country\nAnd my life to the King.\nAlaborn Soldier's Oath",
                    artwork = Resources.Load<Sprite>("Art/alaborn_trooper")
                    });
                Add(new CardData // Intrepid Hero
                    {
                    cardName = "Intrepid Hero",
                    artist = "Mike Ploog",
                    rarity = "Rare",
                    manaCost = 3,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human", "Soldier" },
                    flavorText = "A fool knows no fear. A hero shows no fear.",
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.TapToDestroyPower4OrGreater
                    },
                    artwork = Resources.Load<Sprite>("Art/intrepid_hero")
                    });
                
                Add(new CardData //Capashen templar
                    {
                    cardName = "Capashen Templar",
                    artist = "Todd Lockwood",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Human", "Knight" },
                    manaToPayToActivate = 1,
                    toughnessBuff = 1,
                    flavorText = "Their shields are Benalia's outermost battlements.",
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.PayToBuffSelf
                    },
                    artwork = Resources.Load<Sprite>("Art/capashen_templar")
                    });
                Add(new CardData // Foot soldiers
                    {
                    cardName = "Foot Soldiers",
                    artist = "Kev Walker",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 4,
                    subtypes = new List<string> { "Human", "Soldier" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Infantry deployment is the art of putting your troops in the wrong place at the right time.",
                    artwork = Resources.Load<Sprite>("Art/foot_soldiers")
                    });
                Add(new CardData //Angelic wall
                    {
                    cardName = "Angelic Wall",
                    artist = "John Avon",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 4,
                    subtypes = new List<string> { "Wall" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                        KeywordAbility.Flying
                    },
                    flavorText = "The ancestor protects us in ways we only begin to comprehend.\n-Mystic Elder",
                    artwork = Resources.Load<Sprite>("Art/angelic_wall")
                    });
                Add(new CardData //Wall of swords
                    {
                    cardName = "Wall of Swords",
                    artist = "Zoltan Boros & Gabor Szikszai",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 5,
                    subtypes = new List<string> { "Wall" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                        KeywordAbility.Flying
                    },
                    flavorText = "The air hummed with the scissoring sound of uncounted blades that hovered in front of the invaders as though wielded by a phalanx of unseen hands.",
                    artwork = Resources.Load<Sprite>("Art/wall_of_swords")
                    });
                Add(new CardData // Abbey griffin
                    {
                    cardName = "Abbey Griffin",
                    artist = "Jaime Jones",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Griffin" },
                    keywordAbilities = new List<KeywordAbility> {
                        KeywordAbility.Flying,
                        KeywordAbility.Vigilance
                    },
                    flavorText = "The darkness crawls with vampires and ghouls, but we are not without allies.\n-Mikaeus, the Lunarch",
                    artwork = Resources.Load<Sprite>("Art/abbey_griffin")
                    });
                Add(new CardData // Serra angel
                    {
                    cardName = "Serra Angel",
                    artist = "Mark Zug",
                    rarity = "Rare",
                    manaCost = 5,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 4,
                    subtypes = new List<string> { "Angel" },
                    keywordAbilities = new List<KeywordAbility> {
                        KeywordAbility.Flying,
                        KeywordAbility.Vigilance
                    },
                    flavorText = "Her sword sings more beautifully than any choir.",
                    artwork = Resources.Load<Sprite>("Art/serra_angel")
                    });
                Add(new CardData // Archangel
                    {
                    cardName = "Archangel",
                    artist = "Quinton Hoover",
                    rarity = "Uncommon",
                    manaCost = 7,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
                    subtypes = new List<string> { "Angel" },
                    keywordAbilities = new List<KeywordAbility> {
                        KeywordAbility.Flying,
                        KeywordAbility.Vigilance
                    },
                    flavorText = "The sky rang with the cries of armored seraphs, and the darkness made a tactical retreat.",
                    artwork = Resources.Load<Sprite>("Art/archangel")
                    });
                Add(new CardData //Angel of mercy
                    {
                    cardName = "Angel of Mercy",
                    artist = "Melissa A. Benson",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Angel" },
                    flavorText = "A song of life soars over fields of blood",
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/angel_of_mercy"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "Gain 3 life.",
                            effect = (Player owner, Card unused) =>
                            {
                                GameManager.Instance.TryGainLife(owner, 3, false);
                            }
                        },
                    }
                    });
                Add(new CardData //Venerable monk
                    {
                    cardName = "Venerable Monk",
                    artist = "D. Alexander Gregory",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Human", "Cleric" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/venerable_monk"),
                    flavorText = "His presence brings not only a strong arm but also renewed hope.",
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "Gain 2 life.",
                            effect = (Player owner, Card unused) =>
                            {
                                GameManager.Instance.TryGainLife(owner, 2, false);
                            }
                        },
                    }
                    });
                Add(new CardData //Staunch defenders
                    {
                    cardName = "Staunch Defenders",
                    artist = "Tristan Elwall",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 4,
                    subtypes = new List<string> { "Human", "Soldier" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/staunch_defenders"),
                    flavorText = "The key to winning any fight is simply staying alive.\n-The Southern Paladin",
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "Gain 4 life.",
                            effect = (Player owner, Card unused) =>
                            {
                                GameManager.Instance.TryGainLife(owner, 4, false);
                            }
                        },
                    }
                    });
                Add(new CardData //Iron tusk elephant
                    {
                    cardName = "Iron Tusk Elephant",
                    artist = "Tony Roberts",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Elephant" },
                    flavorText = "The fury in the lion's eye;\nthe patience in the hippo's yawn:\nthe pride within the griffin's cry;\nare one within the iron tusk's stride.\n-'Iron Tusk', Femeref song",
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Trample
                    },
                    artwork = Resources.Load<Sprite>("Art/iron_tusk_elephant")
                    });
                Add(new CardData //Luminous angel
                    {
                    cardName = "Luminous Angel",
                    artist = "Matthew D. Wilson",
                    rarity = "Rare",
                    manaCost = 7,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 4,
                    subtypes = new List<string> { "Angel" },
                    keywordAbilities = new List<KeywordAbility> { 
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/luminous_angel"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnUpkeep,
                            description = "create a 1/1 white Spirit creature token with flying.",
                            effect = (Player owner, Card unused) =>
                            {
                                Card spirit = CardFactory.Create("Spirit");
                                if (spirit == null)
                                {
                                    Debug.LogError("Failed to spawn Human Spirit Token — check card database!");
                                    return;
                                }

                                GameManager.Instance.SummonToken(spirit, owner);
                            }
                        }
                        
                    }
                    });

                Add(new CardData // Spirit Token
                    {
                        cardName = "Spirit",
                        artist = "Jason A. engle",
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = new List<string> { "White" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Spirit" },
                        keywordAbilities = new List<KeywordAbility> { KeywordAbility.Flying },
                        artwork = Resources.Load<Sprite>("Art/spirit_token")
                    });
            
            //BLUE
                Add(new CardData // Fugitive wizard
                    {
                    cardName = "Fugitive Wizard",
                    artist = "Jim Nelson",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human", "Wizard" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "The College of Lat-Nam is often forced to expel students whose experiments grow too risky or too cruel.",
                    artwork = Resources.Load<Sprite>("Art/fugitive_wizard")
                    });
                Add(new CardData // Archivist
                    {
                    cardName = "Archivist",
                    artist = "Donato Giancola",
                    rarity = "Rare",
                    manaCost = 4,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human", "Wizard" },
                    manaToPayToActivate = 0,
                    cardsToDraw = 1,
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.TapToDrawCards
                    },
                    flavorText = "Knowledge is a feast for the mind. Savor every swallow.",
                    artwork = Resources.Load<Sprite>("Art/archivist")
                    });
                Add(new CardData // Coral Eel
                    {
                    cardName = "Coral Eel",
                    artist = "Una Fricker",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 1,
                    subtypes = new List<string> { "Fish" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Some fishers like to eat eels, and some eels like to eat fishers.",
                    artwork = Resources.Load<Sprite>("Art/coral_eel")
                    });
                Add(new CardData // Giant octopus
                    {
                    cardName = "Giant Octopus",
                    artist = "Heather Hudson",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Octopus" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Before my eyes was a horrible monster, worthy to figure in the legends of the marvellous... Its eight arms, or rather feet, fixed to its head... were twice as long as its body, and were twisted like the furies' hair.\n-Jules Verne, Twenty Thousands Leagues under the Sea",
                    artwork = Resources.Load<Sprite>("Art/giant_octopus")
                    });
                Add(new CardData // Sea Monster
                    {
                    cardName = "Sea Monster",
                    artist = "John Howe",
                    rarity = "Common",
                    manaCost = 6,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 6,
                    toughness = 6,
                    subtypes = new List<string> { "Serpent" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Abandon ship!",
                    rulesText = "Thsi creature cannot attack unless defending player controls an island",
                    artwork = Resources.Load<Sprite>("Art/sea_monster")
                    });
                Add(new CardData //Sea eagle
                    {
                    cardName = "Sea Eagle",
                    artist = "Anthony S. Waters",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Bird" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    flavorText = "Where air meets water, fish meets talon.",
                    artwork = Resources.Load<Sprite>("Art/sea_eagle")
                    });
                Add(new CardData //Phantom warrior
                    {
                    cardName = "Phantom Warrior",
                    artist = "Greg Staples",
                    rarity = "Uncommon",
                    manaCost = 3,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Illusion", "Warrior" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBeBlocked
                    },
                    flavorText = "It can pass though solid matter-but that doesn't mean it's harmless.",
                    artwork = Resources.Load<Sprite>("Art/phantom_warrior")
                    });
                Add(new CardData //Tidal kraken
                    {
                    cardName = "Tidal Kraken",
                    artist = "Cristopher Moeller",
                    rarity = "Rare",
                    manaCost = 8,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 6,
                    toughness = 6,
                    subtypes = new List<string> { "Kraken" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBeBlocked
                    },
                    flavorText = "To merfolk, pirates are a nuisance. To pirates, merfolk are a threat. To the kraken, they're both appetizers.",
                    artwork = Resources.Load<Sprite>("Art/tidal_kraken")
                    });
                Add(new CardData //Wind drake
                    {
                    cardName = "Wind Drake",
                    artist = "Tom Wanerstrand",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Drake" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    flavorText = "But high she shoots through air and light,\nAbove all low delay,\nWhere nothing earthly bounds her flight,\nNor shadow dims her way.\n-Thomas Moore,\n'Oh that I had Wings'",
                    artwork = Resources.Load<Sprite>("Art/wind_drake")
                    });
                Add(new CardData //Fighting drake
                    {
                    cardName = "Fighting Drake",
                    artist = "Matt Cavotta",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 4,
                    subtypes = new List<string> { "Drake" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    flavorText = "Scholars in their ivory towers call them 'sharks of the sky'. Scholars on the road don't call them at all.",
                    artwork = Resources.Load<Sprite>("Art/fighting_drake")
                    });
                Add(new CardData //Air elemental
                    {
                    cardName = "Air Elemental",
                    artist = "Wayne England",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 4,
                    subtypes = new List<string> { "Elemental" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    flavorText = "Where psycho meets cyclone.",
                    artwork = Resources.Load<Sprite>("Art/air_elemental")
                    });
                Add(new CardData //Mahamoti djinn
                    {
                    cardName = "Mahamoti Djinn",
                    artist = "Eric Peterson",
                    rarity = "Rare",
                    manaCost = 6,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 6,
                    subtypes = new List<string> { "Djinn" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    flavorText = "Of royal blood among the spirits of the air, the Mahamoti djinn rides on the wings of the winds. As dangerous in the gambling hall as he is in battle, he is a master of trickery and misdirection.",
                    artwork = Resources.Load<Sprite>("Art/mahamoti_djinn")
                    });
                Add(new CardData //Wall of air
                    {
                    cardName = "Wall of Air",
                    artist = "John Avon",
                    rarity = "Uncommon",
                    manaCost = 3,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 5,
                    subtypes = new List<string> { "Wall" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                        KeywordAbility.Flying
                    },
                    flavorText = "Let the air itself protect you, for it is everywhere.\n-Master Wizard",
                    artwork = Resources.Load<Sprite>("Art/wall_of_air")
                    });
                Add(new CardData //Ancient carp
                    {
                    cardName = "Ancient Carp",
                    artist = "Cristopher Burdett",
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 5,
                    subtypes = new List<string> { "Fish" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Why eat now what could one day grow into a feast?\n-Ojutai, translated from Draconic",
                    artwork = Resources.Load<Sprite>("Art/ancient_carp")
                    });
                Add(new CardData //Killer whale
                        {
                        cardName = "Killer Whale",
                        artist = "Stephen Daniele",
                        rarity = "Uncommon",
                        manaCost = 5,
                        color = new List<string> { "Blue", "Blue" },
                        cardType = CardType.Creature,
                        power = 3,
                        toughness = 5,
                        manaToPayToActivate = 1,
                        subtypes = new List<string> { "Fish" },
                        abilityToGain = KeywordAbility.Flying,
                        keywordAbilities = new List<KeywordAbility> { },
                        activatedAbilities = new List<ActivatedAbility>
                        {
                            ActivatedAbility.PayToGainAbility
                        },
                        flavorText = "Hunger is like the sea: deep, endless, and unforgiving.",
                        artwork = Resources.Load<Sprite>("Art/killer_whale")
                        });
                Add(new CardData //Merchant of secrets
                    {
                        cardName = "Merchant of Secrets",
                        artist = "Greg Hildenbrandt",
                        rarity = "Common",
                        manaCost = 3,
                        color = new List<string> { "Blue" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Human", "Wizard" },
                        keywordAbilities = new List<KeywordAbility> { },
                        flavorText = "To scrape out a living in Aphetto, wizards are reduced to selling rumors, lies, forgeries, or -if they get desparate enough- the truth.",
                        artwork = Resources.Load<Sprite>("Art/merchant_of_secrets"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                description = "draw a card.",
                                effect = (Player owner, Card unused) =>
                                {
                                    GameManager.Instance.DrawCard(owner);
                                    Debug.Log("Merchant of Secrets enters: draw a card.");
                                }
                            }
                        }
                    });
            //BLACK
                Add(new CardData { //Maggot Carrier
                    cardName = "Maggot Carrier",
                    artist = "Ron Spencer",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Zombie" },
                    artwork = Resources.Load<Sprite>("Art/maggot_carrier"),
                    flavorText = "The mere sight of our undead allies sickens me. What unholy bargain have you struck?\n-Grizzlegom, to Agnate",
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "each player loses 1 life.",
                            effect = (Player owner, Card selfCard) =>
                            {
                                GameManager.Instance.humanPlayer.Life -= 1;
                                GameManager.Instance.aiPlayer.Life -= 1;

                                GameManager.Instance.ShowFloatingDamage(1, GameManager.Instance.playerLifeContainer);
                                GameManager.Instance.ShowFloatingDamage(1, GameManager.Instance.enemyLifeContainer);
                                GameManager.Instance.UpdateUI();
                                GameManager.Instance.CheckForGameEnd();
                            }
                        }
                    }
                });
                Add(new CardData { //Cyclopean Mummy
                    cardName = "Cyclopean Mummy",
                    artist = "Edward Beard, Jr.",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 1,
                    subtypes = new List<string> { "Zombie" },
                    exileSelfOnDeath = true,
                    rulesText = "When this creature dies, exile it.",
                    flavorText = "The ritual of plucking out an eye to gain future sight is but a curse that enables the living to see their own deaths.",
                    artwork = Resources.Load<Sprite>("Art/cyclopean_mummy")
                });
                Add(new CardData { //Scathe zombies
                    cardName = "Scathe Zombies",
                    artist = "Kev Walker",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Zombie" },
                    flavorText = "They groaned, they stirred, they all uprose,\nNor spake, nor moved their eyes;\nIt had been strange, evein a dream,\nTo have seen those dead men rise.\n-Samuel Taylor Coleridge,\n'The Rime of the Ancient Mariner'",
                    artwork = Resources.Load<Sprite>("Art/scathe_zombies")
                });
                Add(new CardData //Frozen Shade
                        {
                        cardName = "Frozen Shade",
                        artist = "Douglas Shuler",
                        rarity = "Common",
                        manaCost = 3,
                        color = new List<string> { "Black" },
                        cardType = CardType.Creature,
                        power = 0,
                        toughness = 1,
                        subtypes = new List<string> { "Shade" },
                        manaToPayToActivate = 1,
                        powerBuff = 1,
                        toughnessBuff = 1,
                        flavorText = "There are some qualities, some incorporate things,\nThat have a doble life, which thus is made\nA type of twin entity which springs\nFrom matter and light, evinced in solid and shade.\n-Edgar Allan Poe, 'Silence'",
                        activatedAbilities = new List<ActivatedAbility>
                        {
                            ActivatedAbility.PayToBuffSelf
                        },
                        artwork = Resources.Load<Sprite>("Art/frozen_shade")
                        });
                Add(new CardData //Bog imp
                    {
                    cardName = "Bog Imp",
                    artist = "Carl Critchlow",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Imp", },
                    flavorText = "Think of it as a butcher knife with wings.",
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/bog_imp")
                    });
                Add(new CardData //Scavenging scarab
                    {
                    cardName = "Scavenging Scarab",
                    artist = "Jeff Easley",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Insect", },
                    flavorText = "The beetles feed not on the flesh of corpses but on the metal, grinding out the iron and steel to add to their own bulky shells.",
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlock
                    },
                    artwork = Resources.Load<Sprite>("Art/scavenging_scarab")
                    });
                Add(new CardData //Giant cockroach
                    {
                    cardName = "Giant Cockroach",
                    artist = "Heather Hudson",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 2,
                    subtypes = new List<string> { "Insect", },
                    flavorText = "Toren had stepped on a lot of bugs during his life, so he couldn't help feeling ambarassed when a bug stepped on him.",
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/giant_cockroach")
                    });
                Add(new CardData //Nightmare
                    {
                    cardName = "Nightmare",
                    artist = "Carl Critchlow",
                    rarity = "Rare",
                    manaCost = 6,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 0,
                    subtypes = new List<string> { "Nightmare", "Horse" },
                    rulesText = "This creature has power and toughness each equal to the number of swamps you control.",
                    flavorText = "The thunder of its hooves beats dreams into despair.",
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/nightmare"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            usesStack = false,
                            description = "",
                            effect = (Player owner, Card selfCard) =>
                            {
                                if (selfCard is CreatureCard creature)
                                {
                                    int swamp = owner.Battlefield.Count(c => c.cardName == "Swamp");
                                    creature.basePower = swamp;
                                    creature.baseToughness = swamp;
                                    creature.RecalculateStats();
                                    GameManager.Instance.UpdateUI();
                                    GameManager.Instance.CheckDeaths(owner);
                                }
                            }
                        },
                        new CardAbility
                        {
                            timing = TriggerTiming.OnLandEnter,
                            description = "",
                            effect = (Player owner, Card selfCard) =>
                            {
                                if (selfCard is CreatureCard creature)
                                {
                                    int swamp = owner.Battlefield.Count(c => c.cardName == "Swamp");
                                    creature.basePower = swamp;
                                    creature.baseToughness = swamp;
                                    creature.RecalculateStats();
                                    GameManager.Instance.UpdateUI();
                                    GameManager.Instance.CheckDeaths(owner);
                                }
                            }
                        },
                        new CardAbility
                        {
                            timing = TriggerTiming.OnLandLeave,
                            description = "",
                            effect = (Player owner, Card selfCard) =>
                            {
                                if (selfCard is CreatureCard creature)
                                {
                                    int swamp = owner.Battlefield.Count(c => c.cardName == "Swamp");
                                    creature.basePower = swamp;
                                    creature.baseToughness = swamp;
                                    creature.RecalculateStats();
                                    GameManager.Instance.UpdateUI();
                                    GameManager.Instance.CheckDeaths(owner);
                                }
                            }
                        }
                    }
                    });

            //RED
                Add(new CardData //Crazed Goblin
                    {
                    cardName = "Crazed Goblin",
                    artist = "Darrell Riche",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Goblin", "Warrior" },
                    flavorText = "Because fighting is easier than figuiring out what else to do.",
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.MustAttackEachTurnIfAble
                    },
                    artwork = Resources.Load<Sprite>("Art/crazed_goblin")
                    });
                Add(new CardData //Wall of earth
                    {
                    cardName = "Wall of Earth",
                    artist = "Richard Thomas",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 6,
                    subtypes = new List<string> { "Wall" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                    },
                    flavorText = "The ground shuddered violently and the earth seemed to come to life. The elemental force contained in the vast wall of earth was trapped, bent to its controller's will.",
                    artwork = Resources.Load<Sprite>("Art/wall_of_earth")
                    });
                Add(new CardData //Goblin sky raider
                    {
                    cardName = "Goblin Sky Raider",
                    artist = "Daren Bader",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 2,
                    subtypes = new List<string> { "Goblin", "Warrior" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    flavorText = "The goblin word for 'flying' is more accurately translated as 'falling slowly'.",
                    artwork = Resources.Load<Sprite>("Art/goblin_sky_raider")
                    });
                Add(new CardData //Anaba Shaman
                    {
                    cardName = "Anaba Shaman",
                    artist = "Simon Bisley",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    manaToPayToActivate = 1,
                    damageToCreature = 1,
                    subtypes = new List<string> { "Minotaur", "Shaman" },
                    flavorText = "The shamans? Ha! They are craven cows not capable of true magic.\n-Irini Sengir",
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.TapToDealDamageAnyTarget
                    },
                    artwork = Resources.Load<Sprite>("Art/anaba_shaman")
                    });
                Add(new CardData //Hill giant
                    {
                    cardName = "Hill Giant",
                    artist = "Orizio Daniele",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Giant" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Fortunately, hill giants have large blind spots in which a human can easily hide. Unfortunately, these blind spots are beneath the bottoms of their feet.",
                    artwork = Resources.Load<Sprite>("Art/hill_giant")
                    });
                Add(new CardData //Goblin raider
                    {
                    cardName = "Goblin Raider",
                    artist = "Arnie Swekel",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Goblin", "Warrior" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlock
                    },
                    flavorText = "He's smart for a goblin. Ge can do two things: hit and run.",
                    artwork = Resources.Load<Sprite>("Art/goblin_raider")
                    });
                Add(new CardData //Balduvian Barbarians
                    {
                    cardName = "Balduvian Barbarians",
                    artist = "Jim Nelson",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Red", "Red" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 2,
                    subtypes = new List<string> { "Human", "Barbarian" },
                    flavorText = "From the snowy slopes of Kaelor,\nTo the canyons of Bandu,\nWe drink and fight and feast and die\nAs we were born to do.\n-Balduvian tavern song",
                    artwork = Resources.Load<Sprite>("Art/balduvian_barbarians")
                    });
                Add(new CardData //Shivan dragon
                    {
                    cardName = "Shivan Dragon",
                    artist = "Melissa Benson",
                    rarity = "Rare",
                    manaCost = 6,
                    color = new List<string> { "Red", "Red" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
                    subtypes = new List<string> { "Dragon" },
                    manaToPayToActivate = 1,
                    powerBuff = 1,
                    flavorText = "While it's true most dragons are cruel, the Shivan dragon seems to take particular glee in the misery of others, often tormenting its victims much like a cat plays wih a mouse before delivering the final blow.",
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.PayToBuffSelf
                    },
                    artwork = Resources.Load<Sprite>("Art/shivan_dragon")
                    });
            //GREEN
                Add(new CardData //Wall of wood
                    {
                    cardName = "Wall of Wood",
                    artist = "Mark Tedin",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 3,
                    subtypes = new List<string> { "Wall" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                    },
                    flavorText = "Everybody knows that to ward off trouble, you knock on wood. But usually it's better to make a wall out of the wood and let trouble do the knocking.",
                    artwork = Resources.Load<Sprite>("Art/wall_of_wood")
                    });
                Add(new CardData //Argothian swine
                    {
                    cardName = "Argothian Swine",
                    artist = "Randy Elliot",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Boar" },
                    keywordAbilities = new List<KeywordAbility> {
                        KeywordAbility.Trample
                    },
                    flavorText = "In Argoth, the shortest path between two points is the one the swine make.",
                    artwork = Resources.Load<Sprite>("Art/argothian_swine")
                    });

                Add(new CardData //Canopy spider
                    {
                    cardName = "Canopy Spider",
                    artist = "Mike Raabe",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 3,
                    subtypes = new List<string> { "Spider" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Reach
                    },
                    flavorText = "It keeps the upper reaches of the forest free of every menace... except for the spider itself.",
                    artwork = Resources.Load<Sprite>("Art/canopy_spider")
                    });

                Add(new CardData //Giant spider
                    {
                    cardName = "Giant Spider",
                    artist = "Randy Gallegos",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 4,
                    subtypes = new List<string> { "Spider" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Reach
                    },
                    flavorText = "Watching the spider's web.\nLlanowar expression meaning 'focusing on the wrong thing'",
                    artwork = Resources.Load<Sprite>("Art/giant_spider")
                    });

                Add(new CardData //Grizzly bears
                    {
                    cardName = "Grizzly Bears",
                    artist = "D. J. Cleland-Hura",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Bear" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "They've got claws as long as your arm. And they're grouchy. Really, really grouchy.",
                    artwork = Resources.Load<Sprite>("Art/grizzly_bears")
                    });

                Add(new CardData //Moss monster
                    {
                    cardName = "Moss Monster",
                    artist = "Glen Angus",
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 6,
                    subtypes = new List<string> { "Elemental" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "After the battle, an eerie silence griped the forest. The losers' remains were lightly dusted with green.",
                    artwork = Resources.Load<Sprite>("Art/moss_monster")
                    });

                Add(new CardData //Craw wurm
                    {
                    cardName = "Craw Wurm",
                    artist = "Heather Hudson",
                    rarity = "Common",
                    manaCost = 6,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 6,
                    toughness = 4,
                    subtypes = new List<string> { "Wurm" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "The most terrifying thing about the craw wurm is probably the horrible crashing sound it makes as it speeds though the forest. This noise is so loud it echoes through the trees and seems to come from all directions at once.",
                    artwork = Resources.Load<Sprite>("Art/craw_wurm")
                    });

                Add(new CardData //Wall of ice
                    {
                    cardName = "Wall of Ice",
                    artist = "Richard Thomas",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 7,
                    subtypes = new List<string> { "Wall" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                    },
                    flavorText = "And through the drifts the snowy cliffs\nDid send a dismal sheen:\nNor shapes of men nor beasts we ken-\nThe ice was all between.\n-Samuel Coleridge, 'The Rime of the Ancient Mariner'",
                    artwork = Resources.Load<Sprite>("Art/wall_of_ice")
                    });

                Add(new CardData //Dosans oldest chant
                    {
                    cardName = "Dosan's Oldest Chant",
                    artist = "Tim Hildebrandt",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 5,
                    color = new List<string> { "Green" },
                    lifeToGain = 5,
                    cardsToDraw = 1,
                    //rulesText = "You gain 6 life. Draw a card.",
                    flavorText = "As Dosan's chant grew in volume, a second, deeper voice rose up in harmony behind it, strong enough to shake the earth and yet vibrant enough to fill the spirit.",
                    artwork = Resources.Load<Sprite>("Art/dosans_oldest_chant"),
                    });

                Add(new CardData //Trained armodon
                    {
                    cardName = "Trained Armodon",
                    artist = "Gary Leach",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Elephant" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Armodons are trained to step on things. Enemy things.",
                    artwork = Resources.Load<Sprite>("Art/trained_armodon")
                    });

                Add(new CardData // Human Token
                    {
                        cardName = "Human",
                        artist = "Ben Maier",
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = new List<string> { "White" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Human" },
                        keywordAbilities = new List<KeywordAbility> { },
                        artwork = Resources.Load<Sprite>("Art/human_token")
                    });

            //ARTIFACT
                
                Add(new CardData //Phyrexian hulk
                    {
                    cardName = "Phyrexian Hulk",
                    artist = "Brian Snoddy",
                    rarity = "Uncommon",
                    manaCost = 6,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 4,
                    subtypes = new List<string> { "Phyrexian", "Golem" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "It doesen't think. It doesn't feel.\nIt doesen't laugh or cry.\nAll it does from dusk till dawn\nIs make the soldiers die.\n-Onean children's rhyme",
                    artwork = Resources.Load<Sprite>("Art/phyrexian_hulk")
                    });

                Add(new CardData //Juggernaut
                    {
                    cardName = "Juggernaut",
                    artist = "Jonas De Ro",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 3,
                    subtypes = new List<string> { "Juggernaut" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.MustAttackEachTurnIfAble,
                        KeywordAbility.CantBeBlockedByWalls,
                    },
                    flavorText = "Urza's machines have a splendid habit of excavating themselves.\n-Rona, disciple of Gix",
                    artwork = Resources.Load<Sprite>("Art/juggernaut")
                    });

                Add(new CardData //Glass golem
                    {
                    cardName = "Glass Golem",
                    artist = "Glen Angus",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 6,
                    toughness = 2,
                    subtypes = new List<string> { "Golem" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Izzet artificers have learned to steer their beautiful contructs clear of Boros warhammers-and the opera house.",
                    artwork = Resources.Load<Sprite>("Art/glass_golem")
                    });

                Add(new CardData //Yotian soldier
                    {
                    cardName = "Yotian Soldier",
                    artist = "Luca Zontini",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 4,
                    subtypes = new List<string> { "Soldier" },
                    keywordAbilities = new List<KeywordAbility> {
                        KeywordAbility.Vigilance
                    },
                    flavorText = "Poets dream the verses of otherworldy stories. Artificers dream the blueprints of otherplanar artifacts.",
                    artwork = Resources.Load<Sprite>("Art/yotian_soldier")
                    });
                
                Add(new CardData //Dancing scimitar
                    {
                    cardName = "Dancing Scimitar",
                    artist = "Ron Spears",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 5,
                    subtypes = new List<string> { "Spirit" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying,
                    },
                    flavorText = "A blade that has never known sheath, a hilt that has never known hand.",
                    artwork = Resources.Load<Sprite>("Art/dancing_scimitar")
                    });
                
            //MULTI

        // Sorceries
            //WHITE
                
                Add(new CardData //Wrath of God
                    {
                        cardName = "Wrath of God",
                        artist = "Kev Walker",
                        rarity = "Rare",
                        cardType = CardType.Sorcery,
                        manaCost = 4,
                        color = new List<string> { "White", "White" },
                        artwork = Resources.Load<Sprite>("Art/wrath_of_god"),
                        typeOfPermanentToDestroyAll = SorceryCard.PermanentTypeToDestroy.Creature
                    });
                
                Add(new CardData { //Sacred nectar
                    cardName = "Sacred Nectar",
                    artist = "Dana Knutson",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "White" },
                    lifeToGain = 4,
                    flavorText = "For he on honey-dew hath fed,\nAnd drunk the milk of Paradise.\n-Samuel Taylor Coleridge,\n'Kubla Khan'",
                    artwork = Resources.Load<Sprite>("Art/sacred_nectar"),
                    abilities = new List<CardAbility>(),
                    });
                Add(new CardData //Demystify
                        {
                            cardName = "Demystify",
                            artist = "Cristopher Rush",
                            rarity = "Common",
                            cardType = CardType.Instant,
                            manaCost = 1,
                            color = new List<string> { "White" },
                            requiresTarget = true,
                            requiredTargetType = SorceryCard.TargetType.Enchantment,
                            destroyTargetIfTypeMatches = true,
                            flavorText = "The truth will outshine all lies.",
                            artwork = Resources.Load<Sprite>("Art/demystify"),
                        });
                
                Add(new CardData // Archangel's Light
                    {
                        cardName = "Archangel's Light",
                        artist = "Volkan Baga",
                        rarity = "Mythic",
                        cardType = CardType.Sorcery,
                        manaCost = 8,
                        color = new List<string> { "White" },
                        lifeToGainPerCardInOwnGraveyard = 2,
                        shuffleOwnGraveyardIntoLibrary = true,
                        artwork = Resources.Load<Sprite>("Art/archangels_light"),
                        flavorText = "This is the light of Avacyn. Even in her absence she offers us hope.\n-Radulf, priest of Avacyn",
                        rulesText = "Gain 2 life for each card in your graveyard, then shuffle your graveyard into your library."
                    });
                
                Add(new CardData //Holy day
                    {
                    cardName = "Holy Day",
                    artist = "Pete Venters",
                    rarity = "Common",
                    cardType = CardType.Instant,
                    manaCost = 1,
                    color = new List<string> { "White" },
                    preventAllCombatDamageThisTurn = true,
                    rulesText = "Prevent all combat damage that would be dealt this turn.",
                    flavorText = "The day of Spirits; my soul's calm retreat\nWhich none disturb!\n-Henry Vaughan, 'The Night'",
                    artwork = Resources.Load<Sprite>("Art/holy_day"),
                    });

                Add(new CardData //Blessed Reversal
                    {
                    cardName = "Blessed Reversal",
                    artist = "Cristopher Moeller",
                    rarity = "Rare",
                    cardType = CardType.Instant,
                    manaCost = 2,
                    color = new List<string> { "White", "White" },
                    lifeToGainPerCreatureAttackingYou = 3,
                    rulesText = "You gain 3 life for each creature attacking you.",
                    flavorText = "A battle's outcome is never certain.\n-The Southern Paladin",
                    artwork = Resources.Load<Sprite>("Art/blessed_reversal"),
                    });

                Add(new CardData //Charge
                    {
                    cardName = "Charge",
                    artist = "Zehou Chen",
                    rarity = "Common",
                    cardType = CardType.Instant,
                    manaCost = 1,
                    color = new List<string> { "White" },
                    controlledCreaturesPowerBuff = 1,
                    controlledCreaturesToughnessBuff = 1,
                    rulesText = "Creatures you control get +1/+1 until the end of turn.",
                    flavorText = "Honor rides before us. All we have to do is catch up.\n-Danitha Capashen",
                    artwork = Resources.Load<Sprite>("Art/charge"),
                    });

                Add(new CardData //Resupply
                    {
                    cardName = "Resupply",
                    artist = "Filip Burburan",
                    rarity = "Common",
                    cardType = CardType.Instant,
                    manaCost = 6,
                    color = new List<string> { "White" },
                    lifeToGain = 6,
                    cardsToDraw = 1,
                    //rulesText = "You gain 6 life. Draw a card.",
                    flavorText = "If the scalelords are the brains of Dromoka's army, the supply caravans are its beating heart.\n-Baihir, Dromaka mage",
                    artwork = Resources.Load<Sprite>("Art/resupply"),
                    });

                Add(new CardData //Purify
                    {
                    cardName = "Purify",
                    artist = "Doug Chaffee",
                    rarity = "Rare",
                    cardType = CardType.Sorcery,
                    manaCost = 5,
                    color = new List<string> { "White", "White" },
                    typeOfPermanentToDestroyAll = SorceryCard.PermanentTypeToDestroy.ArtifactAndEnchantment,
                    rulesText = "Destroy all artifacts and enchantments.",
                    flavorText = "We have no need for these trinkets, we need only the strength of our swords and the virtue of our hearts.\n-The Northern Paladin",
                    artwork = Resources.Load<Sprite>("Art/demystify"),
                    });
            //BLUE

                Add(new CardData { //Inspiration
                    cardName = "Inspiration",
                    artist = "Matt Cavotta",
                    rarity = "Common",
                    cardType = CardType.Instant,
                    manaCost = 4,
                    color = new List<string> { "Blue" },
                    cardsToDraw = 2,
                    artwork = Resources.Load<Sprite>("Art/inspiration"),
                    });
                
                Add(new CardData //Unsummon
                    {
                        cardName = "Unsummon",
                        artist = "Ron Spencer",
                        rarity = "Common",
                        cardType = CardType.Instant,
                        manaCost = 1,
                        color = new List<string> { "Blue" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        returnTargetCreatureToOwnerHand = true,
                        rulesText = "Return target creature to its owner's hand.",
                        artwork = Resources.Load<Sprite>("Art/unsummon"),
                    });
            //BLACK
                
                Add(new CardData //Terror
                    {
                        cardName = "Terror",
                        artist = "Ron Spencer",
                        rarity = "Common",
                        cardType = CardType.Instant,
                        manaCost = 2,
                        color = new List<string> { "Black" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        destroyTargetIfTypeMatches = true,
                        excludeArtifactCreatures = true,
                        excludedTargetColor = "Black",
                        rulesText = "Destroy target nonartifact, nonblack creature.",
                        artwork = Resources.Load<Sprite>("Art/terror"),
                    });

                
                Add(new CardData //Raise Dead
                    {
                        cardName = "Raise Dead",
                        artist = "Jeff A. Menges",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 1,
                        color = new List<string> { "Black" },
                        rulesText = "Return a random creature card from your graveyard to your hand.",
                        returnRandomCreatureFromGraveyard = true,
                        artwork = Resources.Load<Sprite>("Art/raise_dead")
                    });
                    
            // RED
                Add(new CardData //Stone rain
                        {
                            cardName = "Stone Rain",
                            artist = "Tony Szczudlo",
                            rarity = "Common",
                            cardType = CardType.Sorcery,
                            manaCost = 3,
                            color = new List<string> { "Red" },
                            requiresTarget = true,
                            requiredTargetType = SorceryCard.TargetType.Land,
                            destroyTargetIfTypeMatches = true,
                            artwork = Resources.Load<Sprite>("Art/stone_rain"),
                        });
                
                    Add(new CardData //Shatter
                        {
                            cardName = "Shatter",
                            artist = "Michael Koelsch",
                            rarity = "Common",
                            cardType = CardType.Instant,
                            manaCost = 2,
                            color = new List<string> { "Red" },
                            requiresTarget = true,
                            requiredTargetType = SorceryCard.TargetType.Artifact,
                            canTargetArtifactCreatures = true,
                            destroyTargetIfTypeMatches = true,
                            flavorText = "Days of planning. Weeks of building. Months of perfecting. Seconds of smashing.",
                            artwork = Resources.Load<Sprite>("Art/shatter"),
                        });
                
                Add(new CardData //Accelerate
                    {
                        cardName = "Accelerate",
                        artist = "Gary Ruddell",
                        rarity = "Common",
                        cardType = CardType.Instant,
                        manaCost = 2,
                        color = new List<string> { "Red" },
                        rulesText = "Target creature gains haste until the end of turn. Draw a card.",
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        keywordToGrant = KeywordAbility.Haste,
                        cardsToDraw = 1,
                        flavorText = "I've seen lightning move slower.\n-Nomad sentry",
                        artwork = Resources.Load<Sprite>("Art/accelerate"),
                    });
                
                Add(new CardData
                    {
                        cardName = "Shock",
                        artist = "Mike Sass",
                        rarity = "Common",
                        cardType = CardType.Instant,
                        manaCost = 1,
                        color = new List<string> { "Red" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.CreatureOrPlayer,
                        damageToTarget = 2,
                        flavorText = "I'm shocked. SHOCKED! Well, not that shocked.\nPhilip J. Fry",
                        artwork = Resources.Load<Sprite>("Art/shock"),
                    });
                
                Add(new CardData //Lava Axe
                    {
                        cardName = "Lava Axe",
                        artist = "Ray Lago",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 5,
                        color = new List<string> { "Red" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Player,
                        damageToTarget = 5,
                        rulesText = "Deal 5 damage to target player.",
                        flavorText = "Catch!",
                        artwork = Resources.Load<Sprite>("Art/lava_axe"),
                    });
                
                Add(new CardData //Inferno
                    {
                        cardName = "Inferno",
                        artist = "Don Hazeltine",
                        rarity = "Rare",
                        cardType = CardType.Instant,
                        manaCost = 7,
                        color = new List<string> { "Red", "Red" },
                        rulesText = "Inferno deals 6 damage to each creature and each player.",
                        damageToEachCreatureAndPlayer = 6,
                        flavorText = "Some have said there is no subtley to destruction. You know what? They're dead.\n-Wandering mage",
                        artwork = Resources.Load<Sprite>("Art/inferno"),
                    });
                
            // GREEN
                
                Add(new CardData { //Rampant Growth
                    cardName = "Rampant Growth",
                    artist = "Scott M. Fischer",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "Green" },
                    searchRandomBasicLandToBattlefieldTapped = true,
                    rulesText = "Search your library for a random basic land card and put it onto the battlefield tapped. Then shuffle.",
                    flavorText = "Nature grows solutions to her problems.",
                    artwork = Resources.Load<Sprite>("Art/rampant_growth"),
                    });
                
                Add(new CardData { //Giant growth
                    cardName = "Giant Growth",
                    artist = "Terese Nielsen",
                    rarity = "Common",
                    cardType = CardType.Instant,
                    manaCost = 1,
                    color = new List<string> { "Green" },
                    requiresTarget = true,
                    rulesText = "Target creature gets +3/+3 until the end of turn.",
                    requiredTargetType = SorceryCard.TargetType.Creature,
                    powerBuff = 3,
                    toughnessBuff = 3,
                    flavorText = "Only the most effective tactics stand the test of time.\n-Gamelen, Citanul elder",
                    artwork = Resources.Load<Sprite>("Art/giant_growth"),
                    });

                Add(new CardData { //Might of oaks
                    cardName = "Might of Oaks",
                    artist = "Greg Staples",
                    rarity = "Rare",
                    cardType = CardType.Instant,
                    manaCost = 4,
                    color = new List<string> { "Green" },
                    requiresTarget = true,
                    rulesText = "Target creature gets +7/+7 until the end of turn.",
                    requiredTargetType = SorceryCard.TargetType.Creature,
                    powerBuff = 7,
                    toughnessBuff = 7,
                    flavorText = "Guess where I'm going to plant this!",
                    artwork = Resources.Load<Sprite>("Art/might_of_oaks"),
                    });
                
            ///MULTI

        // Artifacts
            Add(new CardData // Fountain of Youth
                {
                    cardName = "Fountain of Youth",
                    artist = "Daniel Gelon",
                    rarity = "Uncommon",
                    manaCost = 0,
                    color = new List<string>(),
                    cardType = CardType.Artifact,
                    manaToPayToActivate = 2,
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.TapToGainLife
                    },
                    rulesText = "2, TAP: Gain 1 life.",
                    flavorText = "The Fountain had stood in the town square fr centuries, but only the pigeons knew its secret.",
                    artwork = Resources.Load<Sprite>("Art/fountain_of_youth")
                });

                Add(new CardData //Meteorite
                    {
                        cardName = "Meteorite",
                        artist = "Scott Murphy",
                        rarity = "Uncommon",
                        manaCost = 5,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        activatedAbilities = new List<ActivatedAbility>
                        {
                            ActivatedAbility.TapForMana
                        },
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                description = " deal 2 damage to any target.",
                                requiresTarget = true,
                                requiredTargetType = SorceryCard.TargetType.CreatureOrPlayer,
                                effect = (Player owner, Card target) =>
                                {
                                    Player pTarget = GameManager.Instance.optionalTargetPlayer;
                                    if (pTarget != null)
                                    {
                                        pTarget.Life -= 2;
                                        GameObject ui = (pTarget == GameManager.Instance.humanPlayer)
                                            ? GameManager.Instance.playerLifeContainer
                                            : GameManager.Instance.enemyLifeContainer;
                                        GameManager.Instance.ShowFloatingDamage(2, ui);
                                        GameManager.Instance.CheckForGameEnd();
                                        GameManager.Instance.optionalTargetPlayer = null;
                                    }
                                    else if (target is CreatureCard creature)
                                    {
                                        creature.TakeDamage(2);
                                        var vis = GameManager.Instance.FindCardVisual(creature);
                                        if (vis != null)
                                            GameManager.Instance.ShowFloatingDamage(2, vis.gameObject);
                                        GameManager.Instance.CheckDeaths(GameManager.Instance.humanPlayer);
                                        GameManager.Instance.CheckDeaths(GameManager.Instance.aiPlayer);
                                    }
                                    GameManager.Instance.UpdateUI();
                                }
                            }
                        },
                        rulesText = "When this artifact enters, it deals 2 damage to any target.\nTAP: Add 1 mana.",
                        flavorText = "'And if I'm lying,' he began...",
                        artwork = Resources.Load<Sprite>("Art/meteorite")
                    });

                Add(new CardData // Icy Manipulator
                    {
                        cardName = "Icy Manipulator",
                        artist = "Mark Zug",
                        rarity = "Uncommon",
                        manaCost = 4,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        manaToPayToActivate = 1,
                        activatedAbilities = new List<ActivatedAbility>
                        {
                            ActivatedAbility.TapTargetArtifactCreatureOrLand
                        },
                        rulesText = "1T: Tap target artifact, creature, or land.",
                        flavorText = "Ice may thaw, but malice never does.",
                        artwork = Resources.Load<Sprite>("Art/icy_manipulator")
                    });

                Add(new CardData // Jayemdae Tome
                    {
                        cardName = "Jayemdae Tome",
                        artist = "Mark Tedin",
                        rarity = "Uncommon",
                        manaCost = 4,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        manaToPayToActivate = 4,
                        cardsToDraw = 1,
                        activatedAbilities = new List<ActivatedAbility>
                        {
                            ActivatedAbility.TapToDrawCards
                        },
                        //rulesText = "4, TAP: Draw a card.",
                        flavorText = "Knowledge is power.\n-Sir Francis Bacon,\nMeditationes Sacrae",
                        artwork = Resources.Load<Sprite>("Art/jayemdae_tome")
                    });

                    Add(new CardData // Short Sword
                    {
                        cardName = "Short Sword",
                        artist = "John Severin Brassell",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        subtypes = new List<string> { "Equipment" },
                        powerBuff = 1,
                        toughnessBuff = 1,
                        manaToPayToActivate = 1,
                        flavorText = "Sometimes the only difference between a martyr and a hero is a sword.\n-Captain Sisay, Memoirs",
                        activatedAbilities = new List<ActivatedAbility> { ActivatedAbility.Equip },
                        rulesText = "Equipped creature gets +1/+1.",
                        artwork = Resources.Load<Sprite>("Art/short_sword")
                    });

                Add(new CardData // Kite Shield
                    {
                        cardName = "Kite Shield",
                        artist = "Jim Pavelec",
                        rarity = "Uncommon",
                        manaCost = 0,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        subtypes = new List<string> { "Equipment" },
                        powerBuff = 0,
                        toughnessBuff = 3,
                        manaToPayToActivate = 3,
                        activatedAbilities = new List<ActivatedAbility> { ActivatedAbility.Equip },
                        rulesText = "Equipped creature gets +0/+3.",
                        flavorText = "To my sword I owe my glory, but to my shield I owe my life.\n-Sarlena, paladine of the Northern Verge",
                        artwork = Resources.Load<Sprite>("Art/kite_shield")
                    });

                Add(new CardData // Marauder's axe
                    {
                        cardName = "Marauder's Axe",
                        artist = "Mitchell Malloy",
                        rarity = "Common",
                        manaCost = 2,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        subtypes = new List<string> { "Equipment" },
                        powerBuff = 2,
                        toughnessBuff = 0,
                        manaToPayToActivate = 2,
                        activatedAbilities = new List<ActivatedAbility> { ActivatedAbility.Equip },
                        rulesText = "Equipped creature gets +2/+0.",
                        flavorText = "A sharp axe solves most problems.",
                        artwork = Resources.Load<Sprite>("Art/marauders_axe")
                    });

                Add(new CardData // Ogre's cleaver
                    {
                        cardName = "Ogre's Cleaver",
                        artist = "Adi Granov",
                        rarity = "Uncommon",
                        manaCost = 2,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        subtypes = new List<string> { "Equipment" },
                        powerBuff = 5,
                        toughnessBuff = 0,
                        manaToPayToActivate = 5,
                        activatedAbilities = new List<ActivatedAbility> { ActivatedAbility.Equip },
                        rulesText = "Equipped creature gets +5/+0.",
                        flavorText = "She adopted the weapon of the slave-lord Kazuul, and with it, all his cruelty.",
                        artwork = Resources.Load<Sprite>("Art/ogres_axe")
                    });

                Add(new CardData // Greatsword
                    {
                        cardName = "Greatsword",
                        artist = "Nick Klein",
                        rarity = "Uncommon",
                        manaCost = 3,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        subtypes = new List<string> { "Equipment" },
                        powerBuff = 3,
                        toughnessBuff = 0,
                        manaToPayToActivate = 3,
                        activatedAbilities = new List<ActivatedAbility> { ActivatedAbility.Equip },
                        rulesText = "Equipped creature gets +3/+0.",
                        flavorText = "The only blow that matters is the killing blow.",
                        artwork = Resources.Load<Sprite>("Art/greatsword")
                    });

                Add(new CardData // Glorious Anthem
                    {
                        cardName = "Glorious Anthem",
                        artist = "Kev Walker",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "White", "White" },
                        cardType = CardType.Enchantment,
                        artwork = Resources.Load<Sprite>("Art/glorious_anthem"),
                        rulesText = "Creatures you control get +1/+1.",
                        flavorText = "Once heard, the battle song of an angel becomes part of the listener forever.",
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                usesStack = false,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    foreach (var c in owner.Battlefield.OfType<CreatureCard>())
                                    {
                                        c.AddAuraBuff(1, 1);
                                        GameManager.Instance.FindCardVisual(c)?.UpdateVisual();
                                    }
                                }
                            },
                            new CardAbility
                            {
                                timing = TriggerTiming.OnCreatureEnter,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    Card entering = GameManager.Instance.lastEnteredCreature;
                                    if (entering is CreatureCard creature && entering != selfCard &&
                                        GameManager.Instance.GetOwnerOfCard(entering) == owner)
                                    {
                                        creature.AddAuraBuff(1, 1);
                                        GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                                    }
                                }
                            },
                            new CardAbility
                            {
                                timing = TriggerTiming.OnDeath,
                                usesStack = false,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    foreach (var c in owner.Battlefield.OfType<CreatureCard>())
                                    {
                                        c.RemoveAuraBuff(1, 1);
                                        GameManager.Instance.FindCardVisual(c)?.UpdateVisual();
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData // Pacifism
                    {
                        cardName = "Pacifism",
                        artist = "Matthew D. Wilson",
                        rarity = "Common",
                        manaCost = 2,
                        color = new List<string> { "White" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        keywordBuff = KeywordAbility.Defender,
                        artwork = Resources.Load<Sprite>("Art/pacifism"),
                        rulesText = "Enchanted creature cannot attack or block.",
                        flavorText = "Even those born to battle could only lay their blades at Akroma's juicy feet",
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                description = string.Empty,
                                effect = (Player owner, Card source) =>
                                {
                                    if (source is AuraCard aura && aura.attachedTo is CreatureCard creature)
                                        creature.AddAuraKeyword(KeywordAbility.CantBlock);
                                }
                            },
                            new CardAbility
                            {
                                timing = TriggerTiming.OnDeath,
                                description = string.Empty,
                                effect = (Player owner, Card source) =>
                                {
                                    if (source is AuraCard aura && aura.attachedTo is CreatureCard creature)
                                        creature.RemoveAuraKeyword(KeywordAbility.CantBlock);
                                }
                            }
                        }
                    });
                
                Add(new CardData // Feast of the unicorn
                    {
                        cardName = "Feast of the Unicorn",
                        artist = "Dennis Detwiller",
                        rarity = "Common",
                        manaCost = 4,
                        color = new List<string> { "Black" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = 4,
                        toughnessBuff = 0,
                        artwork = Resources.Load<Sprite>("Art/feast_of_the_unicorn"),
                        rulesText = "Enchanted creature gets +4/+0.",
                        flavorText = "Could there be a fouler act? No doubt the baron knows of one.\n-Autumn Willow",
                    });
                Add(new CardData // Unholy strength
                    {
                        cardName = "Unholy Strength",
                        artist = "Tom Kyffin",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string> { "Black" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = 2,
                        toughnessBuff = 1,
                        artwork = Resources.Load<Sprite>("Art/unholy_strength"),
                        flavorText = "Such power grows the body as it shrinks the soul.",
                        rulesText = "Enchanted creature gets +2/+1."
                    });

                Add(new CardData // Granite Grip
                    {
                        cardName = "Granite Grip",
                        artist = "Mike Raabe",
                        rarity = "Common",
                        manaCost = 3,
                        color = new List<string> { "Red" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        artwork = Resources.Load<Sprite>("Art/granite_grip"),
                        flavorText = "Let me introduce you to Rocky.",
                        rulesText = "Enchanted creature gets +1/+0 for each mountain you control.",
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                usesStack = false,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is AuraCard aura && selfCard is EnchantmentCard enchantment && aura.attachedTo is CreatureCard creature)
                                    {
                                        if (enchantment.brotherhoodBuffs.TryGetValue(creature, out int oldBuff) && oldBuff != 0)
                                            creature.RemoveAuraBuff(oldBuff, 0);

                                        int mountainCount = owner.Battlefield.Count(c => c.cardName == "Mountain");
                                        if (mountainCount > 0)
                                        {
                                            creature.AddAuraBuff(mountainCount, 0);
                                            enchantment.brotherhoodBuffs[creature] = mountainCount;
                                        }
                                        else
                                        {
                                            enchantment.brotherhoodBuffs.Remove(creature);
                                        }

                                        GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                                        GameManager.Instance.CheckDeaths(owner);
                                    }
                                }
                            },
                            new CardAbility
                            {
                                timing = TriggerTiming.OnLandEnter,
                                usesStack = false,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is AuraCard aura && selfCard is EnchantmentCard enchantment && aura.attachedTo is CreatureCard creature)
                                    {
                                        if (enchantment.brotherhoodBuffs.TryGetValue(creature, out int oldBuff) && oldBuff != 0)
                                            creature.RemoveAuraBuff(oldBuff, 0);

                                        int mountainCount = owner.Battlefield.Count(c => c.cardName == "Mountain");
                                        if (mountainCount > 0)
                                        {
                                            creature.AddAuraBuff(mountainCount, 0);
                                            enchantment.brotherhoodBuffs[creature] = mountainCount;
                                        }
                                        else
                                        {
                                            enchantment.brotherhoodBuffs.Remove(creature);
                                        }

                                        GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                                        GameManager.Instance.CheckDeaths(owner);
                                    }
                                }
                            },
                            new CardAbility
                            {
                                timing = TriggerTiming.OnLandLeave,
                                usesStack = false,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is AuraCard aura && selfCard is EnchantmentCard enchantment && aura.attachedTo is CreatureCard creature)
                                    {
                                        if (enchantment.brotherhoodBuffs.TryGetValue(creature, out int oldBuff) && oldBuff != 0)
                                            creature.RemoveAuraBuff(oldBuff, 0);

                                        int mountainCount = owner.Battlefield.Count(c => c.cardName == "Mountain");
                                        if (mountainCount > 0)
                                        {
                                            creature.AddAuraBuff(mountainCount, 0);
                                            enchantment.brotherhoodBuffs[creature] = mountainCount;
                                        }
                                        else
                                        {
                                            enchantment.brotherhoodBuffs.Remove(creature);
                                        }

                                        GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                                        GameManager.Instance.CheckDeaths(owner);
                                    }
                                }
                            },
                            new CardAbility
                            {
                                timing = TriggerTiming.OnDeath,
                                usesStack = false,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is AuraCard aura && selfCard is EnchantmentCard enchantment && aura.attachedTo is CreatureCard creature)
                                    {
                                        if (enchantment.brotherhoodBuffs.TryGetValue(creature, out int oldBuff) && oldBuff != 0)
                                        {
                                            creature.RemoveAuraBuff(oldBuff, 0);
                                            enchantment.brotherhoodBuffs.Remove(creature);
                                            GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                                            GameManager.Instance.CheckDeaths(owner);
                                        }
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData // Oakenform
                    {
                        cardName = "Oakenform",
                        artist = "Wayne Reynolds",
                        rarity = "Common",
                        manaCost = 3,
                        color = new List<string> { "Green" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = 3,
                        toughnessBuff = 3,
                        artwork = Resources.Load<Sprite>("Art/oakenform"),
                        flavorText = "When the beast cloaks itself in the mighty oak, what good is a bow? When the oak wraps itself around the snarling beast, what good is s hatchet?\n-Dionus, elvish archdruid",
                        rulesText = "Enchanted creature gets +3/+3.",
                    });

                Add(new CardData // Dehydratation
                    {
                        cardName = "Dehydratation",
                        artist = "Arnie Swekel",
                        rarity = "Common",
                        manaCost = 4,
                        color = new List<string> { "Blue" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        keywordBuff = KeywordAbility.CantUntap,
                        artwork = Resources.Load<Sprite>("Art/dehydratation"),
                        flavorText = "Cry to the sun and watch as even your tears forsake you.\n-Acolyte of Marit Lage",
                        rulesText = "Enchanted creature cannot untap.",
                    });

                Add(new CardData // Inertia Bubble
                    {
                        cardName = "Inertia Bubble",
                        artist = "Hugh Jamieson",
                        rarity = "Common",
                        manaCost = 2,
                        color = new List<string> { "Blue" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        requiredTargetType = SorceryCard.TargetType.Artifact,
                        keywordBuff = KeywordAbility.CantUntap,
                        artwork = Resources.Load<Sprite>("Art/inertia_bubble"),
                        rulesText = "Enchanted artifact doesn't untap during its controller's untap step.",
                        flavorText = "I wouldn't want you to hurt yourself.\nBruenna, Neurok leader",
                    });

                Add(new CardData // Flight
                    {
                        cardName = "Flight",
                        artist = "Jerry Tiritilli",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string> { "Blue" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        keywordBuff = KeywordAbility.Flying,
                        artwork = Resources.Load<Sprite>("Art/flight"),
                        rulesText = "Enchanted creature has flying."
                    });
                
                Add(new CardData // Conquer
                    {
                        cardName = "Conquer",
                        artist = "Randy Gallegos",
                        rarity = "Uncommon",
                        manaCost = 5,
                        color = new List<string> { "Red", "Red" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        requiredTargetType = SorceryCard.TargetType.Land,
                        targetMustBeOpponentPermanent = true,
                        gainControlOfLand = true,
                        rulesText = "Enchant land an opponent controls\nYou control enchanted land.",
                        flavorText = "Why do we trade with those despicable elves? You don't live in forests, you burn them!\n-Avram Garrison,\nLeader of the Knights of Stromgald",
                        artwork = Resources.Load<Sprite>("Art/conquer")
                    });
            }

    private static void Add(CardData data)
        {
            cardsByName[data.cardName] = data;
        }

    public static CardData GetCardData(string name)
        {
            if (cardsByName.TryGetValue(name, out var data))
            {
                return data;
            }

            Debug.LogError("Card not found in database: " + name);
            return null;
        }

    public static IEnumerable<CardData> GetAllCards()
        {
            return cardsByName.Values;
        }
}