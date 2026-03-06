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
                artist = "Sora AI"
            });
            Add(new CardData //Island
            {
                cardName = "Island",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Blue" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/island"),
                artist = "Sora AI"
            });
            Add(new CardData //Swamp
            {
                cardName = "Swamp",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Black" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/swamp"),
                artist = "Sora AI"
            });
            Add(new CardData //Mountain
            {
                cardName = "Mountain",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Red" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/mountain"),
                artist = "Sora AI"
            });
            Add(new CardData //Forest
            {
                cardName = "Forest",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Green" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/forest"),
                artist = "Sora AI"
            });

        // Creatures
            //WHITE
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
                Add(new CardData //Iconoclast monk
                    {
                        cardName = "Iconoclast Monk",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 3,
                        color = new List<string> { "White" },
                        cardType = CardType.Creature,
                        power = 2,
                        toughness = 2,
                        subtypes = new List<string> { "Human", "Cleric" },
                        artwork = Resources.Load<Sprite>("Art/iconoclast_monk"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                description = " you may destroy target non-creature artifact.",
                                requiresTarget = true,
                                requiredTargetType = SorceryCard.TargetType.Artifact,
                                effect = (Player owner, Card target) =>
                                {
                                    Player controller = GameManager.Instance.GetOwnerOfCard(target);
                                    if (!target.keywordAbilities.Contains(KeywordAbility.Indestructible))
                                        GameManager.Instance.SendToGraveyard(target, controller);
                                }
                            }
                        }
                    });
                Add (new CardData { // Beasthunter
                    cardName = "Beasthunter",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Human", "Warrior" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.ProtectionFromRed,
                        KeywordAbility.ProtectionFromGreen,
                    },
                    flavorText = "When hunting, reflexes and training matter more than your sword.",
                    artwork = Resources.Load<Sprite>("Art/beasthunter")
                    });
                Add(new CardData // Angry farmer
                    {
                    cardName = "Angry Farmer",
                    artist = "Anna Camattari",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "The Lich Queen took his land, his livestock, and his family. Now, all that remains are vengeance and a rusted fork.",
                    artwork = Resources.Load<Sprite>("Art/angry_farmer")
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
                Add(new CardData //Trinkets Collector
                    {
                    cardName = "Trinkets Collector",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 1,
                    subtypes = new List<string> { "Spirit" },
                    artwork = Resources.Load<Sprite>("Art/trinkets_collector"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnArtifactEnter,
                            description = "gain 1 life.",
                            effect = (Player owner, Card artifact) =>
                            {
                                GameManager.Instance.TryGainLife(owner, 1, false);
                            }
                        }
                    }
                    });
                Add(new CardData //Gallant lord
                    {
                    cardName = "Gallant Lord",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 3,
                    subtypes = new List<string> { "Human", "Knight" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Vigilance
                    },
                    artwork = Resources.Load<Sprite>("Art/gallant_lord")
                    });
                Add(new CardData //Gentle giant
                    {
                    cardName = "Gentle Giant",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 4,
                    subtypes = new List<string> { "Giant" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender
                    },
                    artwork = Resources.Load<Sprite>("Art/gentle_giant")
                    });
                Add(new CardData //Waterbearer
                    {
                    cardName = "Waterbearer",
                    artist = "Anna Camattari",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 2,
                    subtypes = new List<string> { "Human" },
                    keywordAbilities = new List<KeywordAbility> {},
                    artwork = Resources.Load<Sprite>("Art/waterbearer"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "gain 1 life.",
                            effect = (Player owner, Card unused) =>
                            {
                                GameManager.Instance.TryGainLife(owner, 1, false);
                                //Debug.Log("Waterbearer enters: gain 1 life.");
                            }
                        },
                        new CardAbility
                        {
                            timing = TriggerTiming.OnDeath,
                            description = "gain 1 life.",
                            effect = (Player owner, Card unused) =>
                            {
                                GameManager.Instance.TryGainLife(owner, 1, false);
                                //Debug.Log("Waterbearer dies: gain 1 life.");
                            }
                        }
                    }
                    });
                Add(new CardData //Virgins procession
                    {
                    cardName = "Virgins Procession",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 4,
                    subtypes = new List<string> { "Human", "Cleric" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Vigilance
                    },
                    artwork = Resources.Load<Sprite>("Art/virgins_procession"),
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
                Add(new CardData //Realm protector
                    {
                    cardName = "Realm Protector",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 7,
                    subtypes = new List<string> { "Human", "Soldier" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Vigilance
                    },
                    artwork = Resources.Load<Sprite>("Art/realm_protector")
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
                Add(new CardData //Hamlet Recruiter
                    {
                    cardName = "Hamlet Recruiter",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human", "Soldier" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/hamlet_recruiter"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnUpkeep,
                            description = "create a Human Soldier.",
                            effect = (Player owner, Card unused) =>
                            {
                                Card humanSoldier = CardFactory.Create("Human Soldier");
                                if (humanSoldier == null)
                                {
                                    Debug.LogError("Failed to spawn Human Soldier Token — check card database!");
                                    return;
                                }

                                GameManager.Instance.SummonToken(humanSoldier, owner);
                            }
                        }
                        
                    }
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
                Add(new CardData //Skyhunter unicorn
                    {
                    cardName = "Skyhunter Unicorn",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Unicorn" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/skyhunter_unicorn")
                    });
                Add(new CardData //Pure angel
                    {
                    cardName = "Pure Angel",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 7,
                    color = new List<string> { "White", "White" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
                    subtypes = new List<string> { "Angel" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/pure_angel"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnUpkeep,
                            description = "gain 5 life.",
                            effect = (Player owner, Card unused) =>
                            {
                                GameManager.Instance.TryGainLife(owner, 5, false);
                                Debug.Log("Gain 5 life at upkeep.");
                            }
                        }
                    }
                    });
                Add(new CardData //Untamed Unicorn
                {
                    cardName = "Untamed Unicorn",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 6,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 0,
                    subtypes = new List<string> { "Unicorn" },
                    rulesText = "This creature has power and toughness equal to the number of Plains you control.",
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Vigilance,
                        KeywordAbility.Lifelink
                    },
                    artwork = Resources.Load<Sprite>("Art/untamed_unicorn"),
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
                                    int plains = owner.Battlefield.Count(c => c.cardName == "Plains");
                                    creature.basePower = plains;
                                    creature.baseToughness = plains;
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
                                    int plains = owner.Battlefield.Count(c => c.cardName == "Plains");
                                    creature.basePower = plains;
                                    creature.baseToughness = plains;
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
                                    int plains = owner.Battlefield.Count(c => c.cardName == "Plains");
                                    creature.basePower = plains;
                                    creature.baseToughness = plains;
                                    creature.RecalculateStats();
                                    GameManager.Instance.UpdateUI();
                                    GameManager.Instance.CheckDeaths(owner);
                                }
                            }
                        }
                    }
                });
                Add(new CardData // Human Soldier Token
                    {
                        cardName = "Human Soldier",
                        artist = "Sora AI",
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = new List<string> { "White" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Human", "Soldier" },
                        keywordAbilities = new List<KeywordAbility> { },
                        artwork = Resources.Load<Sprite>("Art/human_soldier_token")
                    });

                Add(new CardData // Spirit Token
                    {
                        cardName = "Spirit",
                        artist = "Sora AI",
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
                Add(new CardData //Skyward whale
                    {
                    cardName = "Skyward Whale",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 6,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
                    subtypes = new List<string> { "Leviathan" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/skyward_whale")
                    });
                Add(new CardData //Arcane barrier
                    {
                    cardName = "Arcane Barrier",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 2,
                    subtypes = new List<string> { "Wall" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender
                    },
                    artwork = Resources.Load<Sprite>("Art/arcane_barrier")
                    });
                Add(new CardData //Deepwood owl
                    {
                    cardName = "Deepwood Owl",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 3,
                    subtypes = new List<string> { "Leviathan" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/deepwood_owl")
                    });
                Add(new CardData //Wandering squid
                    {
                    cardName = "Wandering Squid",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 6,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 6,
                    subtypes = new List<string> { "Cephalopod" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/wandering_squid")
                    });
                Add(new CardData //Giant crab
                    {
                    cardName = "Giant Crab",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 4,
                    subtypes = new List<string> { "Crab" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/giant_crab")
                    });
                Add(new CardData //Wandering cloud
                    {
                    cardName = "Wandering Cloud",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 1,
                    subtypes = new List<string> { "Elemental" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying,
                        KeywordAbility.CanOnlyBlockFlying
                    },
                    artwork = Resources.Load<Sprite>("Art/wandering_cloud")
                    });
                Add(new CardData //Lucky fisherman
                    {
                        cardName = "Lucky Fisherman",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 2,
                        color = new List<string> { "Blue" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Human" },
                        keywordAbilities = new List<KeywordAbility> { },
                        artwork = Resources.Load<Sprite>("Art/lucky_fisherman"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                description = "draw a card.",
                                effect = (Player owner, Card unused) =>
                                {
                                    GameManager.Instance.DrawCard(owner);
                                    Debug.Log("Lucky Fisherman enters: draw a card.");
                                }
                            }
                        }
                    });    
                Add(new CardData //Colossal Octopus
                    {
                    cardName = "Colossal Octopus",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 9,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 8,
                    toughness = 8,
                    subtypes = new List<string> { "Cephalopod" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/colossal_octopus")
                    });
                Add(new CardData //Replicator
                    {
                    cardName = "Replicator",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 3,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Wizard" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "You can't get rid of me.",
                    artwork = Resources.Load<Sprite>("Art/replicator"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnUpkeep,
                            description = "create a copy of this creature.",
                            effect = (Player owner, Card unused) =>
                            {
                                Card replicator = CardFactory.Create("Replicator");
                                if (replicator == null)
                                {
                                    Debug.LogError("Failed to spawn Copy Token — check card database!");
                                    return;
                                }

                                replicator.isToken = true;
                                replicator.manaCost = 0;
                                GameManager.Instance.SummonToken(replicator, owner);
                            }
                        }
                        
                    }
                    });
                Add(new CardData //Sharkmen tribe
                    {
                    cardName = "Sharkmen Tribe",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 3,
                    subtypes = new List<string> { "Merfolk", "Warrior" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Their spears are for fishing: their real weapons are rows of sharpened teeth and a cold thirst for blood.",
                    artwork = Resources.Load<Sprite>("Art/sharkmen_tribe")
                    });
                Add(new CardData //Cosmic Whale
                    {
                    cardName = "Cosmic Whale",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 8,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
                    subtypes = new List<string> { "Leviathan" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/cosmic_whale"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "its controller takes an extra turn after this.",
                            effect = (Player owner, Card unused) =>
                            {
                                owner.extraTurns += 1;
                            }
                        }
                    }
                    });
                Add(new CardData //Tide Spirit
                    {
                    cardName = "Tide Spirit",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Spirit", "Blue" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/tide_spirit"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnCardDraw,
                            description = " if it is not your draw step, this creature gets +2/+2 until end of turn.",
                            effect = (Player owner, Card selfCard) =>
                            {
                                if (selfCard is CreatureCard creature &&
                                    TurnSystem.Instance.currentPhase != TurnSystem.TurnPhase.Draw)
                                {
                                    creature.AddTemporaryBuff(2, 2);
                                    var vis = GameManager.Instance.FindCardVisual(creature);
                                    if (vis != null)
                                        vis.UpdateVisual();
                                }
                            }
                        }
                    }
                    });
                Add(new CardData //Apprentice potionist
                    {
                    cardName = "Apprentice Potionist",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human", "Wizard" },
                    artwork = Resources.Load<Sprite>("Art/apprentice_potionist"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "search your library for a random Potion card, reveal it, put it into your hand, then shuffle your library.",
                            effect = (Player owner, Card unused) =>
                            {
                                GameManager.Instance.SearchLibraryForRandomPotion(owner);
                            }
                        }
                    }
                    });
                Add(new CardData //Master potionist
                    {
                    cardName = "Master Potionist",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 4,
                    color = new List<string> { "Blue", "Blue" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Human", "Wizard" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.PotionSpellsCostOneLess
                    },
                    artwork = Resources.Load<Sprite>("Art/master_potionist"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnArtifactEnter,
                            description = "if it's a Potion, draw a card.",
                            effect = (Player owner, Card selfCard) =>
                            {
                                Card entering = GameManager.Instance.lastEnteredArtifact;
                                if (entering != null && entering.subtypes.Contains("Potion") &&
                                    GameManager.Instance.GetOwnerOfCard(entering) == owner)
                                {
                                    GameManager.Instance.DrawCard(owner);
                                }
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
                Add(new CardData { //Hired assassin
                    cardName = "Hired Assassin",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 6,
                    color = new List<string> { "Black", "Black" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Human", "Rogue" },
                    artwork = Resources.Load<Sprite>("Art/hired_assassin"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "you may destroy target non-artifact creature.",
                            requiresTarget = true,
                            requiredTargetType = SorceryCard.TargetType.Creature,
                            excludeArtifactCreatures = true,
                            effect = (Player owner, Card target) =>
                            {
                                if (target is CreatureCard creature && !creature.color.Contains("Artifact")
                                    && !target.keywordAbilities.Contains(KeywordAbility.Indestructible))
                                {
                                    Player controller = GameManager.Instance.GetOwnerOfCard(target);
                                    GameManager.Instance.SendToGraveyard(target, controller);
                                }
                            }
                        }
                    }
                });
                Add (new CardData { //Flayed Deer
                    cardName = "Flayed Deer",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Zombie", "Beast" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.ProtectionFromGreen
                    },
                    artwork = Resources.Load<Sprite>("Art/flayed_deer")
                    });
                Add(new CardData //Giant Bat
                        {
                        cardName = "Giant Bat",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 4,
                        color = new List<string> { "Black" },
                        cardType = CardType.Creature,
                        power = 3,
                        toughness = 2,
                        manaToPayToActivate = 1,
                        subtypes = new List<string> { "Bat" },
                        abilityToGain = KeywordAbility.Flying,
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.Lifelink,
                        },
                        activatedAbilities = new List<ActivatedAbility>
                        {
                            ActivatedAbility.PayToGainAbility
                        },
                        artwork = Resources.Load<Sprite>("Art/giant_bat")
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
                Add(new CardData //Bog crocodile
                        {
                        cardName = "Bog Crocodile",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        manaCost = 4,
                        color = new List<string> { "Black" },
                        cardType = CardType.Creature,
                        power = 2,
                        toughness = 3,
                        subtypes = new List<string> { "Reptile" },
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.Swampwalk,
                            KeywordAbility.Forestwalk,
                        },
                        artwork = Resources.Load<Sprite>("Art/bog_crocodile")
                        });
                Add(new CardData //Undead gorilla
                    {
                    cardName = "Undead Gorilla",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Black", "Black" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 4,
                    subtypes = new List<string> { "Monkey", "Zombie" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlockWithoutForest,
                    },
                    artwork = Resources.Load<Sprite>("Art/undead_Gorilla")
                    });
                Add(new CardData //Rotting whale
                    {
                    cardName = "Rotting Whale",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 6,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 6,
                    toughness = 1,
                    subtypes = new List<string> { "Zombie", "Leviathan" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Not even the largest creatures escape the cold touch of the Lich Queen.",
                    artwork = Resources.Load<Sprite>("Art/rotting_whale")
                    });
                Add(new CardData //Rotting Dragon
                    {
                    cardName = "Rotting Dragon",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "Black", "Black" },
                    cardType = CardType.Creature,
                    power = 6,
                    toughness = 6,
                    subtypes = new List<string> { "Dragon", "Zombie" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/rotting_dragon"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnUpkeep,
                            description = "put a -1/-1 counter on this creature.",
                            effect = (Player owner, Card selfCard) =>
                            {
                                if (selfCard is CreatureCard c)
                                {
                                    c.AddMinusOneCounter();
                                    var vis = GameManager.Instance.FindCardVisual(c);
                                    if (vis != null) vis.UpdateVisual();
                                    GameManager.Instance.CheckDeaths(owner);
                                }
                            }
                        }
                    }
                    });
                Add(new CardData //Limping corpse
                    {
                    cardName = "Limping Corpse",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 1,
                    entersTapped = true,
                    subtypes = new List<string> { "Zombie" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Beware the night when the dead begin crawling out of their graves.",
                    artwork = Resources.Load<Sprite>("Art/limping_corpse")
                    });
                Add(new CardData //Stubborn Skeleton
                    {
                    cardName = "Stubborn Skeleton",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    entersTapped = true,
                    subtypes = new List<string> { "Skeleton" },
                    manaToPayToActivate = 1,
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.ReturnSelfFromGraveyard
                    },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/stubborn_skeleton")
                    });
                Add(new CardData //Wall of Putrid Flesh
                    {
                    cardName = "Wall of Putrid Flesh",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 1,
                    subtypes = new List<string> { "Zombie", "Wall" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender
                    },
                    manaToPayToActivate = 1,
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.ReturnSelfFromGraveyardToHand
                    },
                    artwork = Resources.Load<Sprite>("Art/wall_of_putrid_flesh")
                    });
                Add(new CardData //Famished crow
                    {
                    cardName = "Famished Crow",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Zombie", "Bird" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlock,
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/famished_crow")
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
                Add(new CardData //Giant crow
                    {
                    cardName = "Giant Crow",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Bird" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlock,
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/giant_crow"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "opponent discards a card at random.",
                            effect = (Player owner, Card unused) =>
                            {
                                Player opponent = GameManager.Instance.GetOpponentOf(owner);
                                opponent.DiscardRandomCard();
                            }
                        }
                    }
                    });
                Add(new CardData //Possessed innocent
                    {
                    cardName = "Possessed Innocent",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 5,
                    color = new List<string> { "Black", "Black" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/possessed_innocent"),
                    abilities = new List<CardAbility>
                    {
                    new CardAbility
                        {
                            timing = TriggerTiming.OnDeath,
                            description = "create a Demon.",
                            effect = (Player owner, Card unused) =>
                            {
                                Card demon = CardFactory.Create("Demon");
                                if (demon == null)
                                {
                                    Debug.LogError("Failed to spawn Demon Token — check card database!");
                                    return;
                                }

                                GameManager.Instance.SummonToken(demon, owner);
                            }

                        }
                    }
                    });
                Add(new CardData //Lunatic necromancer
                    {
                    cardName = "Lunatic Necromancer",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human", "Wizard" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/lunatic_necromancer"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnUpkeep,
                            description = "create a tapped Zombie.",
                            effect = (Player owner, Card unused) =>
                            {
                                Card zombie = CardFactory.Create("Zombie");
                                if (zombie == null)
                                {
                                    Debug.LogError("Failed to spawn Zombie Token — check card database!");
                                    return;
                                }

                                GameManager.Instance.SummonToken(zombie, owner);
                            }
                        }
                        
                    }
                    });
                Add(new CardData //Sad clown
                    {
                    cardName = "Sad Clown",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = new List<string> { "Black", "Black" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Spirit" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/sad_clown"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "opponent discards a card at random.",
                            effect = (Player owner, Card unused) =>
                            {
                                Player opponent = GameManager.Instance.GetOpponentOf(owner);
                                opponent.DiscardRandomCard();
                            }
                        },
                        new CardAbility
                        {
                            timing = TriggerTiming.OnDeath,
                            description = "opponent discards a card at random.",
                            effect = (Player owner, Card unused) =>
                            {
                                Player opponent = GameManager.Instance.GetOpponentOf(owner);
                                opponent.DiscardRandomCard();
                            }
                        },
                    }
                    });
                Add(new CardData //Ratbat
                    {
                    cardName = "Ratbat",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Rat", "Bat" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlock,
                        KeywordAbility.Flying,
                        KeywordAbility.Lifelink
                    },
                    artwork = Resources.Load<Sprite>("Art/ratbat")
                    });
                Add(new CardData //Giant rat
                    {
                    cardName = "Giant Rat",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 1,
                    subtypes = new List<string> { "Rat" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "It lives to feed: and the more it feeds, the more it remembers the taste of flesh.",
                    artwork = Resources.Load<Sprite>("Art/giant_rat")
                    });
                Add(new CardData //Dump People
                    {
                    cardName = "Dump People",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 1,
                    subtypes = new List<string> { "Human", "Rogue" },
                    artwork = Resources.Load<Sprite>("Art/dump_people"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "return a random noncreature artifact card from your graveyard to your hand.",
                            effect = (Player owner, Card unused) =>
                            {
                                GameManager.Instance.ReturnRandomNonCreatureArtifactFromGraveyard(owner);
                            }
                        }
                    }
                    });
                Add(new CardData //Bog mosquito
                    {
                    cardName = "Bog Mosquito",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Insect" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlock,
                        KeywordAbility.Flying,
                        KeywordAbility.Lifelink
                    },
                    artwork = Resources.Load<Sprite>("Art/bog_mosquito")
                    });
                Add(new CardData { //Wicked witch
                    cardName = "Wicked Witch",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    tapLifeLossAmount = 1,
                    subtypes = new List<string> { "Human", "Wizard" },
                    activatedAbilities = new List<ActivatedAbility> {
                        ActivatedAbility.TapToLoseLife
                    },
                    artwork = Resources.Load<Sprite>("Art/wicked_witch")
                    });
                Add(new CardData //Undead Army
                    {
                    cardName = "Undead Army",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 8,
                    color = new List<string> { "Black", "Black" },
                    cardType = CardType.Creature,
                    power = 8,
                    toughness = 8,
                    subtypes = new List<string> { "Zombie" },
                    rulesText = "Whenever this creature attacks or blocks, put a -1/-1 counter on this creature at the end of combat.",
                    artwork = Resources.Load<Sprite>("Art/undead_army")
                    });
                Add(new CardData // Zombie Token
                    {
                        cardName = "Zombie",
                        artist = "Sora AI",
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = new List<string> { "Black" },
                        cardType = CardType.Creature,
                        power = 2,
                        toughness = 1,
                        entersTapped = true,
                        subtypes = new List<string> { "Zombie" },
                        keywordAbilities = new List<KeywordAbility> { },
                        artwork = Resources.Load<Sprite>("Art/zombie_token")
                    });

                Add(new CardData //Nocturnal Spectre
                    {
                        cardName = "Nocturnal Spectre",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        manaCost = 4,
                        color = new List<string> { "Black", "Black" },
                        cardType = CardType.Creature,
                        power = 2,
                        toughness = 2,
                        subtypes = new List<string> { "Spectre" },
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.Flying
                        },
                        artwork = Resources.Load<Sprite>("Art/nocturnal_spectre"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnOpponentDiscard,
                                description = "this creature gets +2/+2 until end of turn.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is CreatureCard creature)
                                    {
                                        creature.AddTemporaryBuff(2, 2);
                                        var vis = GameManager.Instance.FindCardVisual(creature);
                                        if (vis != null)
                                            vis.UpdateVisual();
                                    }
                                }
                        }
                    }
                });
                Add(new CardData //Alchemist Renegade
                    {
                        cardName = "Alchemist Renegade",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        manaCost = 3,
                        color = new List<string> { "Black" },
                        cardType = CardType.Creature,
                        power = 2,
                        toughness = 2,
                        subtypes = new List<string> { "Human", "Wizard", "Rogue" },
                        artwork = Resources.Load<Sprite>("Art/alchemist_renegade"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                description = "return a random Potion card from your graveyard to the battlefield.",
                                effect = (Player owner, Card unused) =>
                                {
                                    GameManager.Instance.ReturnRandomPotionFromGraveyardToBattlefield(owner);
                                }
                            }
                        }
                    });
                Add(new CardData //Cursed Necromancer
                    {
                        cardName = "Cursed Necromancer",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        manaCost = 4,
                        color = new List<string> { "Black", "Black" },
                        cardType = CardType.Creature,
                        power = 2,
                        toughness = 2,
                        subtypes = new List<string> { "Zombie", "Wizard" },
                        artwork = Resources.Load<Sprite>("Art/cursed_necromancer"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                description = "return a random Zombie from your graveyard to the battlefield.",
                                effect = (Player owner, Card unused) =>
                                {
                                    GameManager.Instance.ReturnRandomZombieFromGraveyardToBattlefield(owner);
                                }
                            }
                        }
                    });
                Add(new CardData //The Worlds Evil
                    {
                        cardName = "The Worlds Evil",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 8,
                        color = new List<string> { "Black", "Black" },
                        cardType = CardType.Creature,
                        power = 8,
                        toughness = 8,
                        subtypes = new List<string> { "Demon" },
                        manaToPayToActivate = 8,
                        activatedAbilities = new List<ActivatedAbility>
                        {
                            ActivatedAbility.ReturnSelfFromGraveyard
                        },
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.Flying,
                            KeywordAbility.Trample
                        },
                        artwork = Resources.Load<Sprite>("Art/the_worlds_evil"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                description = "exile 8 random creature cards from your graveyard. If you can't, you lose 8 life.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    var creatures = owner.Graveyard.OfType<CreatureCard>().ToList();
                                    if (creatures.Count < 8)
                                    {
                                        owner.Life -= 8;
                                        GameObject ui = owner == GameManager.Instance.humanPlayer ?
                                            GameManager.Instance.playerLifeContainer :
                                            GameManager.Instance.enemyLifeContainer;
                                        GameManager.Instance.ShowFloatingDamage(8, ui);
                                        GameManager.Instance.UpdateUI();
                                        GameManager.Instance.CheckForGameEnd();
                                    }
                                    else
                                    {
                                        for (int i = 0; i < 8; i++)
                                        {
                                            int index = Random.Range(0, creatures.Count);
                                            Card chosen = creatures[index];
                                            creatures.RemoveAt(index);
                                            owner.Graveyard.Remove(chosen);

                                            CardVisual vis = GameManager.Instance.FindCardVisual(chosen);
                                            if (vis != null)
                                            {
                                                GameManager.Instance.activeCardVisuals.Remove(vis);
                                                GameObject.Destroy(vis.gameObject);
                                            }
                                        }
                                        GameManager.Instance.RefreshGraveyardVisuals(owner);
                                        GameManager.Instance.UpdateUI();
                                    }
                                }
                            }
                        }
                    });
                Add(new CardData // Demon Token
                    {
                        cardName = "Demon",
                        artist = "Sora AI",
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = new List<string> { "Black" },
                        cardType = CardType.Creature,
                        power = 5,
                        toughness = 5,
                        subtypes = new List<string> { "Demon" },
                        keywordAbilities = new List<KeywordAbility> { KeywordAbility.Flying },
                        artwork = Resources.Load<Sprite>("Art/demon_token")
                    });
            //RED
                Add (new CardData { //Firedancer
                    cardName = "Firedancer",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human", "Shaman" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.ProtectionFromRed
                    },
                    artwork = Resources.Load<Sprite>("Art/firedancer")
                    });
                Add(new CardData //Rabid dog
                    {
                    cardName = "Rabid Dog",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Dog" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Haste,
                        KeywordAbility.CantBlock
                    },
                    artwork = Resources.Load<Sprite>("Art/rabid_dog")
                    });
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
                Add(new CardData //Fireborn dragon
                    {
                    cardName = "Fireborn Dragon",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 6,
                    color = new List<string> { "Red", "Red" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
                    subtypes = new List<string> { "Dragon" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Haste,
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/fireborn_dragon")
                    });
                Add(new CardData //Dragon summoner
                    {
                    cardName = "Dragon Summoner",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 7,
                    color = new List<string> { "Red", "Red" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human", "Shaman" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/dragon_summoner"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnUpkeep,
                            description = "create a Dragon.",
                            effect = (Player owner, Card unused) =>
                            {
                                Card dragon = CardFactory.Create("Dragon");
                                if (dragon == null)
                                {
                                    Debug.LogError("Failed to spawn Dragon Token — check card database!");
                                    return;
                                }

                                GameManager.Instance.SummonToken(dragon, owner);
                            }
                        }
                        
                    }
                    });
                Add(new CardData //Great boulder
                    {
                    cardName = "Great Boulder",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 4,
                    subtypes = new List<string> { "Elemental" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                    },
                    flavorText = "I swear, the street was empty yesterday.",
                    artwork = Resources.Load<Sprite>("Art/great_boulder")
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
                Add(new CardData //Flying pig
                    {
                    cardName = "Flying Pig",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Beast" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/flying_pig")
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
                Add(new CardData //Goblin puncher
                    {
                    cardName = "Goblin Puncher",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Goblin", "Warrior" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlock
                    },
                    flavorText = "Goblins are raised as bullies from youth, trained to charge into battle without a single thought in their heads.",
                    artwork = Resources.Load<Sprite>("Art/goblin_puncher")
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
                Add(new CardData //Scarred Wildboar
                    {
                    cardName = "Scarred Wildboar",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 1,
                    subtypes = new List<string> { "Beast" },
                    flavorText = "Its fresh wounds do not stop it the charge.",
                    artwork = Resources.Load<Sprite>("Art/scarred_wildboar")
                    });
                Add(new CardData //Goblin Beastmaster
                    {
                    cardName = "Goblin Beastmaster",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Goblin", "Shaman" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.BeastCreatureSpellsCostOneLess
                    },
                    artwork = Resources.Load<Sprite>("Art/goblin_beastmaster")
                    });
                Add(new CardData //Goblin Invader
                    {
                    cardName = "Goblin Invader",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 5,
                    color = new List<string> { "Red", "Red" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Goblin", "Shaman" },
                    artwork = Resources.Load<Sprite>("Art/goblin_invader"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "gain control of target land until this creature leaves.",
                            requiresTarget = true,
                            requiredTargetType = SorceryCard.TargetType.Land,
                            effect = (Player owner, Card target) =>
                            {
                                if (target == null)
                                    return;
                                Card source = GameManager.Instance.lastAbilitySource;
                                if (source == null)
                                    return;
                                source.gainedControlCard = target;
                                source.gainedControlCardOriginalOwner = GameManager.Instance.GetOwnerOfCard(target);
                                GameManager.Instance.ChangeController(target, owner);
                            }
                        },
                        new CardAbility
                        {
                            timing = TriggerTiming.OnDeath,
                            effect = (Player owner, Card card) =>
                            {
                                if (card.gainedControlCard != null && card.gainedControlCardOriginalOwner != null)
                                {
                                    GameManager.Instance.ChangeController(card.gainedControlCard, card.gainedControlCardOriginalOwner);
                                    card.gainedControlCard = null;
                                    card.gainedControlCardOriginalOwner = null;
                                }
                            }
                        }
                    }
                    });
                Add(new CardData //Thundermare
                    {
                    cardName = "Thundermare",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "Red", "Red" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 3,
                    subtypes = new List<string> { "Elemental" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Haste,
                        KeywordAbility.Trample,
                    },
                    flavorText = "The storm is coming.",
                    artwork = Resources.Load<Sprite>("Art/thundermare")
                    });
                Add(new CardData //Village idiot
                    {
                    cardName = "Village Idiot",
                    artist = "Anna Camattari",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    requiresTarget = false,
                    subtypes = new List<string> { "Human" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/village_idiot"),
                    abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                description = "draw a card, then discard two cards at random.",
                                effect = (Player owner, Card unused) =>
                                {
                                    GameManager.Instance.DrawCard(owner);
                                    owner.DiscardRandomCard(2);
                                    Debug.Log("Village Idiot enters: draw a card an discard 2.");
                                }
                            }
                        }
                    });
                Add(new CardData //Wild ostrich
                    {
                    cardName = "Wild Ostrich",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 1,
                    subtypes = new List<string> { "Bird", "Beast" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Haste,
                        KeywordAbility.CantBlock
                    },
                    flavorText = "Meep meep",
                    artwork = Resources.Load<Sprite>("Art/wild_ostrich")
                    });
                Add(new CardData //Spitfire Cobrox
                    {
                    cardName = "Spitfire Cobrox",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "Red", "Red" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 4,
                    subtypes = new List<string> { "Beast", "Reptile", "Dog" },
                    manaToPayToActivate = 1,
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.PayToBuffSelf
                    },
                    artwork = Resources.Load<Sprite>("Art/spitfire_cobrox")
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
                Add(new CardData // Dragon Token
                    {
                        cardName = "Dragon",
                        artist = "Sora AI",
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = new List<string> { "Red" },
                        cardType = CardType.Creature,
                        power = 5,
                        toughness = 5,
                        subtypes = new List<string> { "Dragon" },
                        keywordAbilities = new List<KeywordAbility> { KeywordAbility.Flying },
                        artwork = Resources.Load<Sprite>("Art/dragon_token")
                    });
            //GREEN
                Add(new CardData //River crocodile
                        {
                        cardName = "River Crocodile",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        manaCost = 4,
                        color = new List<string> { "Green" },
                        cardType = CardType.Creature,
                        power = 2,
                        toughness = 3,
                        subtypes = new List<string> { "Reptile" },
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.Islandwalk,
                            KeywordAbility.Swampwalk,
                        },
                        artwork = Resources.Load<Sprite>("Art/river_crocodile")
                        });
                Add(new CardData //Living tree
                    {
                    cardName = "Living Tree",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 5,
                    subtypes = new List<string> { "Treefolk", "Druid" },
                    artwork = Resources.Load<Sprite>("Art/living_tree"),
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Vigilance
                    },
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.TapForMana
                    }
                    });
                Add(new CardData //Wall of roots
                    {
                    cardName = "Wall of Roots",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 2,
                    subtypes = new List<string> { "Plant" },
                    artwork = Resources.Load<Sprite>("Art/wall_of_roots"),
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender
                    },
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.TapForMana
                    }
                    });
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
                Add(new CardData //Spinewall Cactus
                    {
                    cardName = "Spinewall Cactus",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 2,
                    subtypes = new List<string> { "Plant" },
                    artwork = Resources.Load<Sprite>("Art/spinewall_cactus"),
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender
                    },
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnBlock,
                            description = "it gets +X/+0 until end of turn where X is the power of the blocked creature.",
                            effect = (Player owner, Card target) =>
                            {
                                var self = GameManager.Instance.lastAbilitySource as CreatureCard;
                                var attacker = target as CreatureCard;
                                if (self != null && attacker != null)
                                {
                                    self.AddTemporaryBuff(attacker.power, 0);
                                    var vis = GameManager.Instance.FindCardVisual(self);
                                    if (vis != null)
                                        vis.UpdateVisual();
                                    Debug.Log($"{self.cardName} blocks {attacker.cardName} and gets +{attacker.power}/+0 until end of turn.");
                                }
                            }
                        }
                    }
                    });
                Add(new CardData //Cactusaurus
                    {
                    cardName = "Cactusaurus",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
                    subtypes = new List<string> { "Plant", "Reptile" },
                    keywordAbilities = new List<KeywordAbility> {
                        KeywordAbility.Trample
                    },
                    flavorText = "One day, a mad wizard crossed ancient bones with a cactus. He was eaten by his own creation shortly after.",
                    artwork = Resources.Load<Sprite>("Art/cactusaurus")
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
                Add(new CardData //Realms crasher
                    {
                    cardName = "Realms Crasher",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 7,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 7,
                    toughness = 7,
                    subtypes = new List<string> { "Beast" },
                    keywordAbilities = new List<KeywordAbility> {
                        KeywordAbility.Trample
                    },
                    artwork = Resources.Load<Sprite>("Art/realms_crasher")
                    });
                Add(new CardData //Drumming elf
                    {
                    cardName = "Drumming Elf",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Elf" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/drumming_elf"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnUpkeep,
                            description = "create a Monkey.",
                            effect = (Player owner, Card unused) =>
                            {
                                Card monkey = CardFactory.Create("Monkey");
                                if (monkey == null)
                                {
                                    Debug.LogError("Failed to spawn Monkey Token — check card database!");
                                    return;
                                }

                                GameManager.Instance.SummonToken(monkey, owner);
                            }
                        }
                        
                    }
                    });
                Add(new CardData //Crazy cat lady
                    {
                    cardName = "Crazy Cat Lady",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Human" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/crazy_cat_lady"),
                    abilities = new List<CardAbility>
                    {
                    new CardAbility
                    {
                        timing = TriggerTiming.OnEnter,
                        description = "create two Cats.",
                        effect = (Player owner, Card unused) =>
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Card cat = CardFactory.Create("Cat");
                                if (cat == null)
                                {
                                    Debug.LogError("Failed to spawn Cat Token — check card database!");
                                    return;
                                }

                                GameManager.Instance.SummonToken(cat, owner);
                            }
                        }
                    }
                    }
                    });
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
                Add(new CardData //Domestic cat
                    {
                    cardName = "Domestic Cat",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Cat" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Reach
                    },
                    flavorText = "Cats are just tiny tigers living in your home.",
                    artwork = Resources.Load<Sprite>("Art/domestic_cat")
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
                Add(new CardData //Deepwood monkeys
                    {
                    cardName = "Deepwood Monkeys",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Monkey" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "Beneath the fur and teeth lies a mind that knows the forest better than any map ever could.",
                    artwork = Resources.Load<Sprite>("Art/deepwood_monkeys")
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
                Add(new CardData //Violent ape
                    {
                    cardName = "Violent Ape",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Monkey" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "They've learned which bones snap easiest. And they enjoy the sound.",
                    artwork = Resources.Load<Sprite>("Art/violent_ape")
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
                Add(new CardData //Gorilla Chief
                    {
                    cardName = "Gorilla Chief",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 3,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Monkey" },
                    artwork = Resources.Load<Sprite>("Art/gorilla_chief"),
                    rulesText = "Monkeys you control get +1/+1.",
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            usesStack = false,
                            description = "",
                            effect = (Player owner, Card selfCard) =>
                            {
                                foreach (var c in owner.Battlefield.OfType<CreatureCard>())
                                {
                                    if (c.subtypes.Contains("Monkey"))
                                    {
                                        c.AddAuraBuff(1,1);
                                        GameManager.Instance.FindCardVisual(c)?.UpdateVisual();
                                    }
                                }
                            }
                        },
                        new CardAbility
                        {
                            timing = TriggerTiming.OnCreatureEnter,
                            description = "",
                            effect = (Player owner, Card selfCard) =>
                            {
                                Card entering = GameManager.Instance.lastEnteredCreature;
                                if (entering is CreatureCard creature && entering != selfCard &&
                                    creature.subtypes.Contains("Monkey") &&
                                    GameManager.Instance.GetOwnerOfCard(entering) == owner)
                                {
                                    creature.AddAuraBuff(1,1);
                                    GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                                }
                            }
                        },
                        new CardAbility
                        {
                            timing = TriggerTiming.OnDeath,
                            usesStack = false,
                            description = "",
                            effect = (Player owner, Card selfCard) =>
                            {
                                foreach (var c in owner.Battlefield.OfType<CreatureCard>())
                                {
                                    if (c.subtypes.Contains("Monkey"))
                                    {
                                        c.RemoveAuraBuff(1,1);
                                        GameManager.Instance.FindCardVisual(c)?.UpdateVisual();
                                    }
                                }
                            }
                        }
                    }
                    });
                Add(new CardData //Veilbreaker Druid
                    {
                    cardName = "Veilbreaker Druid",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Human", "Druid" },
                    artwork = Resources.Load<Sprite>("Art/veilbreaker_druid"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = " you may destroy target enchantment.",
                            requiresTarget = true,
                            requiredTargetType = SorceryCard.TargetType.Enchantment,
                            effect = (Player owner, Card target) =>
                            {
                                Player controller = GameManager.Instance.GetOwnerOfCard(target);
                                if (!target.keywordAbilities.Contains(KeywordAbility.Indestructible))
                                    GameManager.Instance.SendToGraveyard(target, controller);
                            }
                        }
                    }
                    });
                Add(new CardData //Flying donkey
                    {
                    cardName = "Flying Donkey",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Beast" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/flying_donkey")
                    });
                Add(new CardData //Slack tungo
                    {
                    cardName = "Slack Tungo",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 8,
                    color = new List<string> { "Green", "Green" },
                    cardType = CardType.Creature,
                    power = 8,
                    toughness = 8,
                    subtypes = new List<string> { "Beast" },
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "To most, it's a slumbering monster. To the elves, it's a playground, a guardian, and the softest spot for a midsummer nap.",
                    artwork = Resources.Load<Sprite>("Art/slack_tungo")
                    });
                Add(new CardData // Cat Token
                    {
                        cardName = "Cat",
                        artist = "Sora AI",
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = new List<string> { "Green" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Cat" },
                        keywordAbilities = new List<KeywordAbility> { KeywordAbility.Reach },
                        artwork = Resources.Load<Sprite>("Art/cat_token")
                    });
                Add(new CardData // Human Token
                    {
                        cardName = "Human",
                        artist = "Sora AI",
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = new List<string> { "White" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Human" },
                        keywordAbilities = new List<KeywordAbility> { },
                        artwork = Resources.Load<Sprite>("Art/human_soldier_token")
                    });
                Add(new CardData // Monkey Token
                    {
                        cardName = "Monkey",
                        artist = "Sora AI",
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = new List<string> { "Green" },
                        cardType = CardType.Creature,
                        power = 2,
                        toughness = 2,
                        subtypes = new List<string> { "Monkey" },
                        keywordAbilities = new List<KeywordAbility> { },
                        artwork = Resources.Load<Sprite>("Art/monkey_token")
                    });
            //ARTIFACT
                Add(new CardData //Sphyx lynx
                    {
                    cardName = "Sphynx Lynx",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 6,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 4,
                    subtypes = new List<string> { "Cat", "Sphynx" },
                    manaToPayToActivate = 4,
                    abilityToGain = KeywordAbility.Flying,
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.PayToGainAbility
                    },
                    artwork = Resources.Load<Sprite>("Art/sphynx_lynx")
                    });
                Add(new CardData //Origin Golem
                    {
                    cardName = "Origin Golem",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 4,
                    subtypes = new List<string> { "Golem" },
                    entersTapped = true,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/origin_golem")
                    });
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
                Add(new CardData //Omega golemoid
                    {
                    cardName = "Omega Golemoid",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 7,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 7,
                    toughness = 5,
                    subtypes = new List<string> { "Golem" },
                    entersTapped = true,
                    keywordAbilities = new List<KeywordAbility> { },
                    flavorText = "In a field of ancient ruins, it stands as a silent, cold guardian: an echo of a long-forgotten war.",
                    artwork = Resources.Load<Sprite>("Art/omega_golemoid")
                    });
                Add(new CardData //Glassmole
                    {
                    cardName = "Glassmole",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 1,
                    subtypes = new List<string> { "Beast" },
                    entersTapped = true,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/glassmole")
                    });
                Add(new CardData //Obstacle
                    {
                    cardName = "Obstacle",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 0,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 1,
                    subtypes = new List<string> { "Wall" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                    },
                    flavorText = "Whether physical or psychological, obstacles are what allow people to act and overcome their fears.",
                    artwork = Resources.Load<Sprite>("Art/obstacle")
                    });
                Add(new CardData //Iron skyman
                    {
                    cardName = "Iron Skyman",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 4,
                    subtypes = new List<string> { "Golem" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                        KeywordAbility.Flying,
                    },
                    flavorText = "Intruder detected.",
                    artwork = Resources.Load<Sprite>("Art/iron_skyman")
                    });
                Add(new CardData //Autonomous miner
                    {
                    cardName = "Autonomous Miner",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    manaToPayToActivate = 3,
                    entersTapped = true,
                    subtypes = new List<string> { "Golem" },
                    tokenToCreate = "Autonomous Miner",
                    activatedAbilities = new List<ActivatedAbility> {
                        ActivatedAbility.TapToCreateToken
                    },
                    artwork = Resources.Load<Sprite>("Art/autonomous_miner")
                    });
                Add(new CardData //Stormcutter Galleon
                    {
                    cardName = "Stormcutter Galleon",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 6,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 4,
                    subtypes = new List<string> { "Ship" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/stormcutter_galleon"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "return a random instant or sorcery card from your graveyard to your hand.",
                            effect = (Player owner, Card selfCard) =>
                            {
                                GameManager.Instance.ReturnRandomInstantOrSorceryFromGraveyard(owner);
                            }
                        }
                    }
                    });
            //MULTI
                Add(new CardData //Blazefire angel
                    {
                    cardName = "Blazefire Angel",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 7,
                    color = new List<string> { "White", "Red" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
                    subtypes = new List<string> { "Angel", "Warrior" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying,
                        KeywordAbility.Haste,
                        KeywordAbility.Vigilance
                    },
                    artwork = Resources.Load<Sprite>("Art/blazefire_angel")
                    });
                Add(new CardData //Wild crusher
                        {
                        cardName = "Wild crusher",
                        artist = "Sora AI",
                        rarity = "rare",
                        manaCost = 4,
                        color = new List<string> { "Red", "Green" },
                        cardType = CardType.Creature,
                        power = 4,
                        toughness = 4,
                        manaToPayToActivate = 2,
                        subtypes = new List<string> { "Beast", "Monkey" },
                        abilityToGain = KeywordAbility.Flying,
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.Haste,
                        },
                        activatedAbilities = new List<ActivatedAbility>
                        {
                            ActivatedAbility.PayToGainAbility
                        },
                        artwork = Resources.Load<Sprite>("Art/wild_crusher")
                        });
                Add(new CardData //Wild dragon
                    {
                    cardName = "Wild Dragon",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 7,
                    color = new List<string> { "Red", "Green" },
                    cardType = CardType.Creature,
                    power = 6,
                    toughness = 6,
                    subtypes = new List<string> { "Dragon" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying,
                        KeywordAbility.Haste,
                        KeywordAbility.Trample,
                        KeywordAbility.CantBlock
                    },
                    artwork = Resources.Load<Sprite>("Art/wild_Dragon")
                    });
                Add(new CardData //Twin elves
                    {
                    cardName = "Twin Elves",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "White", "Green" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Elf", "Soldier" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Vigilance,
                    },
                    flavorText = "Bound by ancient blood, elves sometimes follow each other even into death.",
                    artwork = Resources.Load<Sprite>("Art/twin_elves")
                    });
                Add(new CardData //Night moth
                    {
                    cardName = "Night Moth",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = new List<string> { "Black", "Green" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Insect" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying,
                    },
                    activatedAbilities = new List<ActivatedAbility> {
                        ActivatedAbility.TapForMana
                    },
                    artwork = Resources.Load<Sprite>("Art/night_moth")
                    });
                Add(new CardData //Poison mushroom
                    {
                    cardName = "Poison Mushroom",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = new List<string> { "Black", "Green" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Plant" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                        KeywordAbility.Deathtouch
                    },
                    artwork = Resources.Load<Sprite>("Art/poison_mushroom"),
                    });
                Add(new CardData //Mindcat
                    {
                    cardName = "Mindcat",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Blue", "Black" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Cat", "Spirit" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying,
                    },
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "opponent discards a card at random.",
                            effect = (Player owner, Card unused) =>
                            {
                                Player opponent = GameManager.Instance.GetOpponentOf(owner);
                                opponent.DiscardRandomCard();
                            }
                        },
                        new CardAbility
                        {
                            timing = TriggerTiming.OnDeath,
                            description = "draw a card.",
                            effect = (Player owner, Card unused) =>
                            {
                                GameManager.Instance.DrawCard(owner);
                            }
                        },
                    },
                    artwork = Resources.Load<Sprite>("Art/mindcat"),
                    });
                Add(new CardData //Mutant gorilla
                    {
                    cardName = "Mutant Gorilla",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Blue", "Green" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Monkey", "Cephalopod" },
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.TapForMana
                    },
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "draw a card.",
                            effect = (Player owner, Card unused) =>
                            {
                                GameManager.Instance.DrawCard(owner);
                            }
                        },
                    },
                    artwork = Resources.Load<Sprite>("Art/mutant_gorilla"),
                    });
                Add(new CardData //Battle bear
                    {
                    cardName = "Battle Bear",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 3,
                    color = new List<string> { "Red", "Green" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Beast", "Warrior" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Haste,
                        KeywordAbility.Trample,
                        KeywordAbility.CantBlock
                    },
                    flavorText = "What's more deadly than an angry bear?",
                    artwork = Resources.Load<Sprite>("Art/battle_bear")
                    });
                Add(new CardData //Robin
                    {
                    cardName = "Robin",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Red", "Green" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Bird" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Haste,
                        KeywordAbility.Flying,
                    },
                    flavorText = "Though phoenixes are long extinct, some birds still seem to carry a flicker of their flame.",
                    artwork = Resources.Load<Sprite>("Art/robin")
                    });
                Add(new CardData //Crazed shaman
                    {
                    cardName = "Crazed Shaman",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = new List<string> { "Red", "Green" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human", "Shaman" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Haste,
                    },
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.TapForMana
                    },
                    artwork = Resources.Load<Sprite>("Art/crazed_shaman")
                    });
                Add(new CardData //Bloodmoon vampire
                    {
                    cardName = "Bloodmoon Vampire",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = new List<string> { "White", "Red" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Vampire", },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Haste,
                        KeywordAbility.Vigilance,
                        KeywordAbility.Lifelink
                    },
                    artwork = Resources.Load<Sprite>("Art/bloodmoon_vampire")
                    });
                Add(new CardData //Battle Mage
                    {
                    cardName = "Battle Mage",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Blue", "Red" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Human", "Wizard", "Warrior" },
                    artwork = Resources.Load<Sprite>("Art/battle_mage"),
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
                    }
                    });
                Add(new CardData //Cloudmane Leviathan
                    {
                    cardName = "Cloudmane Leviathan",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "White", "Blue" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 6,
                    subtypes = new List<string> { "Leviathan" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/cloudmane_leviathan")
                    });
                Add(new CardData //Lich Queen
                    {
                    cardName = "Lich Queen",
                    artist = "Sora AI",
                    rarity = "Mythic",
                    manaCost = 4,
                    color = new List<string> { "Black", "White" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 4,
                    subtypes = new List<string> { "Zombie" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Vigilance,
                        KeywordAbility.Indestructible,
                        KeywordAbility.Lifelink
                    },
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnUpkeep,
                            description = "sacrifice a creature.",
                            effect = (Player owner, Card source) =>
                            {
                                var creatures = owner.Battlefield.OfType<CreatureCard>().ToList();
                                if (creatures.Count == 0)
                                    return;

                                bool isLichQueen = source != null && source.cardName == "Lich Queen";
                                bool ownerIsAI = owner == GameManager.Instance.aiPlayer;

                                if (ownerIsAI && isLichQueen && creatures.Count > 1)
                                {
                                    creatures = creatures.Where(c => c != source).ToList();

                                    if (creatures.Count == 0)
                                        creatures = owner.Battlefield.OfType<CreatureCard>().ToList();
                                }

                                Card chosen = creatures
                                    .OrderBy(c => c.power + c.toughness)
                                    .ThenBy(c => c.power)
                                    .ThenBy(c => c.manaCost)
                                    .First();

                                GameManager.Instance.SendToGraveyard(chosen, owner);
                            }
                        }
                    },
                    artwork = Resources.Load<Sprite>("Art/lich_queen")
                    });
        // Sorceries
            //WHITE
                Add(new CardData { //Exorcism
                    cardName = "Exorcism",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "White", "White" },
                    requiresTarget = true,
                    destroyTargetIfTypeMatches = true,
                    requiredTargetType = SorceryCard.TargetType.Creature,
                    requiredTargetColor ="Black",
                    artwork = Resources.Load<Sprite>("Art/exorcism")
                    });
                Add(new CardData { //for glory
                    cardName = "For Glory",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    cardType = CardType.Sorcery,
                    manaCost = 5,
                    color = new List<string> { "White", "White" },
                    artwork = Resources.Load<Sprite>("Art/for_glory"),
                    abilities = new List<CardAbility>(),
                    tokenToCreate = "Human Soldier",       // Token name as defined in CardFactory
                    numberOfTokensMin = 4,
                    numberOfTokensMax = 4,
                    });
                Add(new CardData { //rolling army
                    cardName = "Rolling Army",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 4,
                    color = new List<string> { "White" },
                    artwork = Resources.Load<Sprite>("Art/rolling_army"),
                    abilities = new List<CardAbility>(),
                    tokenToCreate = "Human Soldier",       // Token name as defined in CardFactory
                    numberOfTokensMin = 1,
                    numberOfTokensMax = 6,
                    });
                Add(new CardData { //Sacred Horn Nectar
                    cardName = "Sacred Horn Nectar",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 1,
                    color = new List<string> { "White" },
                    lifeToGain = 4,
                    artwork = Resources.Load<Sprite>("Art/sacred_horn_nectar"),
                    abilities = new List<CardAbility>(),
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
                Add(new CardData { //Solid prayer
                    cardName = "Solid Prayer",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "White", "White" },
                    lifeToGain = 7,
                    artwork = Resources.Load<Sprite>("Art/solid_prayer"),
                    abilities = new List<CardAbility>(),
                    });
                Add(new CardData
                    {
                        cardName = "Deny the Afterlife",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        cardType = CardType.Sorcery,
                        manaCost = 1,
                        color = new List<string> { "White" },
                        artwork = Resources.Load<Sprite>("Art/deny_the_afterlife"),
                        exileAllCreaturesFromGraveyards = true
                    });
                Add(new CardData { //Bell Call
                    cardName = "Bell Call",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "White" },
                    revealUntilCreature = true,
                    artwork = Resources.Load<Sprite>("Art/bells_call"),
                    abilities = new List<CardAbility>(),
                    });
                Add(new CardData //Blinding light
                    {
                    cardName = "Blinding Light",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Instant,
                    manaCost = 1,
                    color = new List<string> { "White" },
                    requiresTarget = true,
                    requiredTargetType = SorceryCard.TargetType.Creature,
                    keywordToGrant = KeywordAbility.CantDealCombatDamage,
                    rulesText = "Prevent all combat damage that target creature would deal this turn.",
                    artwork = Resources.Load<Sprite>("Art/blinding_light"),
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
            //BLUE
                Add(new CardData { //Blast of knowledge
                    cardName = "Blast of Knowledge",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 5,
                    color = new List<string> { "Blue" },
                    cardsToDraw = 3,
                    artwork = Resources.Load<Sprite>("Art/blast_of_knowledge"),
                    });
                Add(new CardData { //Astral Plane
                    cardName = "Astral Plane",
                    artist = "Sora AI",
                    rarity = "Rare",
                    cardType = CardType.Sorcery,
                    manaCost = 5,
                    color = new List<string> { "Blue" },
                    swapGraveyardAndLibrary = true,
                    artwork = Resources.Load<Sprite>("Art/astral_plane"),
                    });
                Add(new CardData { //Rolling thoughts
                    cardName = "Rolling Thoughts",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    cardType = CardType.Sorcery,
                    manaCost = 6,
                    color = new List<string> { "Blue" },
                    cardsToDrawMin = 1,
                    cardsToDrawMax = 6,
                    artwork = Resources.Load<Sprite>("Art/rolling_thoughts"),
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
                Add(new CardData { //Blasphemy
                    cardName = "Blasphemy",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "Black", "Black" },
                    requiresTarget = true,
                    destroyTargetIfTypeMatches = true,
                    requiredTargetType = SorceryCard.TargetType.Creature,
                    requiredTargetColor = "White",
                    artwork = Resources.Load<Sprite>("Art/blasphemy")
                    });
                Add(new CardData //stain of rot
                    {
                        cardName = "Stain of Rot",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        color = new List<string> { "Black" },
                        manaCost = 4,
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Land,
                        destroyTargetIfTypeMatches = true,
                        lifeToLoseForOpponent = 2,
                        artwork = Resources.Load<Sprite>("Art/stain_of_rot")
                    });
                Add(new CardData //Forced mummification
                    {
                        cardName = "Forced Mummification",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        color = new List<string> { "Black", "Black" },
                        manaCost = 6,
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        destroyTargetIfTypeMatches = true,
                        excludeArtifactCreatures = true,
                        tokenToCreate = "Zombie",
                        numberOfTokensMin = 1,
                        numberOfTokensMax = 1,
                        artwork = Resources.Load<Sprite>("Art/forced_mummification")
                    });
                Add(new CardData //Lights out
                    {
                        cardName = "Lights Out",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Instant,
                        manaCost = 4,
                        color = new List<string> { "Black" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        destroyTargetIfTypeMatches = true,
                        artwork = Resources.Load<Sprite>("Art/lights_out"),
                    });
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
                Add(new CardData { //Witches rite
                    cardName = "Witches Rite",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 1,
                    color = new List<string> { "Black" },
                    lifeToLoseForOpponent = 3,
                    artwork = Resources.Load<Sprite>("Art/witches_rite"),
                    });
                Add(new CardData { //Communed rot
                    cardName = "Communed Rot",
                    artist = "Sora AI",
                    rarity = "Rare",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "Black", "Black" },
                    lifeLossForBothPlayers = 4,
                    artwork = Resources.Load<Sprite>("Art/communed_rot"),
                    });
                Add(new CardData //Forget
                    {
                        cardName = "Forget",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 1,
                        color = new List<string> { "Black" },
                        artwork = Resources.Load<Sprite>("Art/forget"),
                        cardsToDiscardorDraw = 1,
                        drawIfOpponentCantDiscard = true,
                    });
                Add(new CardData //Massacre
                    {
                        cardName = "Massacre",
                        artist = "Sora AI",
                        rarity = "Rare",
                        cardType = CardType.Sorcery,
                        manaCost = 4,
                        color = new List<string> { "Black", "Black" },
                        artwork = Resources.Load<Sprite>("Art/massacre"),
                        typeOfPermanentToDestroyAll = SorceryCard.PermanentTypeToDestroy.Creature
                    });
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
                Add(new CardData //Mirrorbreak
                    {
                        cardName = "Mirrorbreak",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        cardType = CardType.Sorcery,
                        manaCost = 3,
                        color = new List<string> { "Black", "Black" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        requireNonTokenTarget = true,
                        destroyAllWithSameName = true,
                        artwork = Resources.Load<Sprite>("Art/mirrorbreak"),
                    });
                Add(new CardData //Filth Discharge
                    {
                        cardName = "Filth Discharge",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 1,
                        hasXCost = true,
                        color = new List<string> { "Black" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        addXMinusOneCounters = true,
                        rulesText = "Put X -1/-1 counters on target creature.",
                    artwork = Resources.Load<Sprite>("Art/filth_discharge"),
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
                Add(new CardData //Sinister Murmurs
                    {
                        cardName = "Sinister Murmurs",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 2,
                        color = new List<string> { "Black", "Black" },
                        rulesText = "Return a random creature card from your graveyard to your hand.",
                        returnRandomCreatureFromGraveyard = true,
                        cardsToDraw = 1,
                        artwork = Resources.Load<Sprite>("Art/sinister_murmurs")
                    });
                Add(new CardData //Pact of Bones
                    {
                        cardName = "Pact of Bones",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 1,
                        color = new List<string> { "Black" },
                        rulesText = "Return a random creature card with mana value 1 or less from your graveyard to the battlefield.",
                        returnRandomCheapCreatureToBattlefield = true,
                        maxManaCostForReturn = 1,
                        artwork = Resources.Load<Sprite>("Art/pact_of_bones")
                    });
                Add(new CardData //Rolling Despair
                    {
                        cardName = "Rolling Despair",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        cardType = CardType.Sorcery,
                        manaCost = 5,
                        color = new List<string> { "Black", "Black" },
                        rulesText = "Each player rolls a six-sided die and sacrifices that many creatures.",
                        creaturesToSacrificeEachPlayerMin = 1,
                        creaturesToSacrificeEachPlayerMax = 6,
                        artwork = Resources.Load<Sprite>("Art/rolling_despair")
                    });
            // RED
                Add(new CardData //To dig a hole
                        {
                            cardName = "To Dig a Hole",
                            artist = "Sora AI",
                            rarity = "Common",
                            cardType = CardType.Sorcery,
                            manaCost = 3,
                            color = new List<string> { "Red" },
                            requiresTarget = true,
                            requiredTargetType = SorceryCard.TargetType.Land,
                            destroyTargetIfTypeMatches = true,
                            artwork = Resources.Load<Sprite>("Art/to_dig_a_hole"),
                        });
                Add(new CardData //Melt
                        {
                            cardName = "Melt",
                            artist = "Sora AI",
                            rarity = "Common",
                            cardType = CardType.Instant,
                            manaCost = 1,
                            color = new List<string> { "Red" },
                            requiresTarget = true,
                            requiredTargetType = SorceryCard.TargetType.Artifact,
                            destroyTargetIfTypeMatches = true,
                            artwork = Resources.Load<Sprite>("Art/melt"),
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
                Add(new CardData //Dash
                    {
                        cardName = "Dash",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 1,
                        color = new List<string> { "Red" },
                        rulesText = "Target creature gains haste until the end of turn.",
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        keywordToGrant = KeywordAbility.Haste,
                        artwork = Resources.Load<Sprite>("Art/dash"),
                    });
                Add(new CardData //thunderstrike
                    {
                        cardName = "Thunderstrike",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Instant,
                        manaCost = 6,
                        color = new List<string> { "Red", "Red" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        damageToTarget = 6,
                        artwork = Resources.Load<Sprite>("Art/thunderstrike"),
                    });
                Add(new CardData //fire hatchet
                    {
                        cardName = "Fire Hatchet",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 4,
                        color = new List<string> { "Red", "Red" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.CreatureOrPlayer,
                        damageToTarget = 4,
                        artwork = Resources.Load<Sprite>("Art/fire_hatchet"),
                    });
                Add(new CardData
                    {
                        cardName = "Explosion",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 1,
                        color = new List<string> { "Red" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.CreatureOrPlayer,
                        damageToTarget = 3,
                        artwork = Resources.Load<Sprite>("Art/explosion"),
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
                Add(new CardData //Moonfall
                    {
                        cardName = "Moonfall",
                        artist = "Sora AI",
                        rarity = "Rare",
                        cardType = CardType.Sorcery,
                        manaCost = 4,
                        color = new List<string> { "Red", "Red" },
                        artwork = Resources.Load<Sprite>("Art/moonfall"),
                        typeOfPermanentToDestroyAll = SorceryCard.PermanentTypeToDestroy.Land,
                    });
                Add(new CardData
                    {
                        cardName = "Fire Spirals",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        cardType = CardType.Sorcery,
                        manaCost = 3,
                        color = new List<string> { "Red", "Red" },
                        artwork = Resources.Load<Sprite>("Art/fire_spirals"),
                        damageToEachCreatureAndPlayer = 2
                    });
                Add(new CardData //Rolling Thunder
                    {
                        cardName = "Rolling Thunder",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        cardType = CardType.Sorcery,
                        manaCost = 4,
                        color = new List<string> { "Red" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.CreatureOrPlayer,
                        damageToTargetMin = 1,
                        damageToTargetMax = 6,
                        artwork = Resources.Load<Sprite>("Art/rolling_thunder"),
                    });
            // GREEN
                Add(new CardData //whip of thorns
                        {
                            cardName = "Whip of Thorns",
                            artist = "Sora AI",
                            rarity = "Common",
                            cardType = CardType.Sorcery,
                            manaCost = 1,
                            color = new List<string> { "Green" },
                            requiresTarget = true,
                            requiredTargetType = SorceryCard.TargetType.Creature,
                            damageToTarget = 2,
                            artwork = Resources.Load<Sprite>("Art/whip_of_thorns"),
                        });
                Add(new CardData //Natures Rebuke
                        {
                            cardName = "Natures Rebuke",
                            artist = "Sora AI",
                            rarity = "Rare",
                            cardType = CardType.Sorcery,
                            manaCost = 4,
                            color = new List<string> { "Green", "Green" },
                            artwork = Resources.Load<Sprite>("Art/natures_rebuke"),
                            typeOfPermanentToDestroyAll = SorceryCard.PermanentTypeToDestroy.Artifact
                        });
                Add(new CardData { //Feast
                    cardName = "Feast",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 1,
                    color = new List<string> { "Green" },
                    eachPlayerGainLifeEqualToLands = true,
                    artwork = Resources.Load<Sprite>("Art/feast"),
                    });
                Add(new CardData { //Touch Grass
                    cardName = "Touch Grass",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 1,
                    color = new List<string> { "Green" },
                    revealUntilLand = true,
                    artwork = Resources.Load<Sprite>("Art/touch_grass"),
                    abilities = new List<CardAbility>(),
                    });
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
                Add(new CardData { //Rolling Energy
                    cardName = "Rolling Energy",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "Green" },
                    manaToGainMin = 1,
                    manaToGainMax = 6,
                    artwork = Resources.Load<Sprite>("Art/rolling_energy"),
                    });
                Add(new CardData { //Muscle Blast
                    cardName = "Muscle Blast",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    cardType = CardType.Instant,
                    manaCost = 6,
                    color = new List<string> { "Green", "Green" },
                    requiresTarget = true,
                    rulesText = "Target creature gets +6/+6 until the end of turn.",
                    requiredTargetType = SorceryCard.TargetType.Creature,
                    powerBuff = 6,
                    toughnessBuff = 6,
                    artwork = Resources.Load<Sprite>("Art/muscle_blast"),
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
                Add(new CardData //Empowering Charge
                    {
                        cardName = "Empowering Charge",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 1,
                        hasXCost = true,
                        color = new List<string> { "Green" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        addXPlusOneCounters = true,
                        rulesText = "Put X +1/+1 counters on target creature.",
                        artwork = Resources.Load<Sprite>("Art/empowering_charge"),
                    });
            ///MULTI
                Add(new CardData //Drain mind
                        {
                        cardName = "Drain Mind",
                        artist = "Sora AI",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 2,
                        color = new List<string> { "Blue", "Black" },
                        cardsToDiscardorDraw = 1,
                        drawIfOpponentCantDiscard = false,
                        cardsToDraw = 1,
                        artwork = Resources.Load<Sprite>("Art/drain_mind")
                        });
                Add(new CardData //Burn mind
                    {
                    cardName = "Burn Mind",
                    artist = "Sora AI",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "Black", "Red" },
                    cardsToDiscardorDraw = 1,
                    drawIfOpponentCantDiscard = false,
                    lifeToLoseForOpponent = 2,
                    artwork = Resources.Load<Sprite>("Art/burn_mind")
                    });

                Add(new CardData //Fireborn Succubus
                    {
                    cardName = "Fireborn Succubus",
                    artist = "Sora AI",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Black", "Red" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Demon" },
                    artwork = Resources.Load<Sprite>("Art/fireborn_succubus"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "gain control of target creature with converted mana cost 2 or less until this creature leaves the battlefield.",
                            requiresTarget = true,
                            requiredTargetType = SorceryCard.TargetType.Creature,
                            effect = (Player owner, Card target) =>
                            {
                                if (target is CreatureCard creature && creature.manaCost <= 2)
                                {
                                    Card source = GameManager.Instance.lastAbilitySource;
                                    if (source == null)
                                        return;
                                    source.gainedControlCard = target;
                                    source.gainedControlCardOriginalOwner = GameManager.Instance.GetOwnerOfCard(target);
                                    GameManager.Instance.ChangeController(target, owner);
                                }
                            }
                        },
                        new CardAbility
                        {
                            timing = TriggerTiming.OnDeath,
                            effect = (Player owner, Card card) =>
                            {
                                if (card.gainedControlCard != null && card.gainedControlCardOriginalOwner != null)
                                {
                                    GameManager.Instance.ChangeController(card.gainedControlCard, card.gainedControlCardOriginalOwner);
                                    card.gainedControlCard = null;
                                    card.gainedControlCardOriginalOwner = null;
                                }
                            }
                        }
                    }
                    });

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
            Add(new CardData // Pressure Sphere
                {
                    cardName = "Pressure Sphere",
                    artist = "Sora AI",
                    rarity = "Rare",
                    manaCost = 2,
                    color = new List<string>(),
                    cardType = CardType.Artifact,
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.AllPermanentsEnterTapped
                    },
                    artwork = Resources.Load<Sprite>("Art/pressure_sphere")
                });
            Add(new CardData //Potion of lava
                {
                    cardName = "Potion of Lava",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string>(),
                    cardType = CardType.Artifact,
                    subtypes = new List<string> { "Potion" },
                    entersTapped = true,
                    manaToPayToActivate = 2,
                    damageToCreature = 2,
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.DealDamageToCreature
                    },
                    artwork = Resources.Load<Sprite>("Art/potion_of_lava")
                });
            Add(new CardData // Potion of knowledge
                {
                    cardName = "Potion of Knowledge",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string>(),
                    cardType = CardType.Artifact,
                    subtypes = new List<string> { "Potion" },
                    entersTapped = true,
                    cardsToDraw = 2,
                    manaToPayToActivate = 5,
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.SacrificeToDrawCards
                    },
                    artwork = Resources.Load<Sprite>("Art/potion_of_knowledge")
                });
            Add(new CardData // Potion of health
                {
                    cardName = "Potion of Health",
                    artist = "Sora AI",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string>(),
                    cardType = CardType.Artifact,
                    subtypes = new List<string> { "Potion" },
                    entersTapped = true,
                    lifeToGain = 3,
                    manaToPayToActivate = 2,
                    activatedAbilities = new List<ActivatedAbility>
                    {
                        ActivatedAbility.SacrificeForLife
                    },
                    artwork = Resources.Load<Sprite>("Art/potion_of_health")
                });
                  Add(new CardData // Potion of mana
                      {
                          cardName = "Potion of Mana",
                          artist = "Sora AI",
                          rarity = "Common",
                          manaCost = 1,
                          color = new List<string>(),
                          cardType = CardType.Artifact,
                          subtypes = new List<string> { "Potion" },
                          entersTapped = true,
                          manaToGain = 3,
                          manaToPayToActivate = 2,
                          activatedAbilities = new List<ActivatedAbility>
                          {
                              ActivatedAbility.SacrificeForMana
                          },
                          artwork = Resources.Load<Sprite>("Art/potion_of_mana")
                      });
                  Add(new CardData // Potion of strength
                      {
                          cardName = "Potion of Strength",
                          artist = "Sora AI",
                          rarity = "Common",
                          manaCost = 1,
                          color = new List<string>(),
                          cardType = CardType.Artifact,
                          subtypes = new List<string> { "Potion" },
                          entersTapped = true,
                          powerBuff = 2,
                          toughnessBuff = 2,
                          manaToPayToActivate = 2,
                          activatedAbilities = new List<ActivatedAbility>
                          {
                              ActivatedAbility.BuffTargetCreature
                          },
                          artwork = Resources.Load<Sprite>("Art/potion_of_strength")
                      });
                  Add(new CardData //Stone of plague
                      {
                          cardName = "Stone of Plague",
                          artist = "Sora AI",
                        rarity = "Uncommon",
                        manaCost = 3,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        plagueAmount = 1,
                        activatedAbilities = new List<ActivatedAbility>
                        {
                            ActivatedAbility.TapToPlague
                        },
                        artwork = Resources.Load<Sprite>("Art/stone_of_plague")
                    });

                Add(new CardData //Mana rock
                    {
                        cardName = "Mana Rock",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 3,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        entersTapped = true,
                        activatedAbilities = new List<ActivatedAbility> {
                            ActivatedAbility.TapForMana
                        },
                        artwork = Resources.Load<Sprite>("Art/mana_rock")
                    });

                Add(new CardData //Crystallium
                    {
                        cardName = "Crystallium",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        manaToGain = 1,
                        activatedAbilities = new List<ActivatedAbility> {
                            ActivatedAbility.TapAndSacrificeForMana
                        },
                        artwork = Resources.Load<Sprite>("Art/crystallium")
                    });
                    
                Add(new CardData //Bonfire
                    {
                        cardName = "Bonfire",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 2,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        activatedAbilities = new List<ActivatedAbility> {
                            ActivatedAbility.TapToGainLife
                        },
                        artwork = Resources.Load<Sprite>("Art/bonfire")
                    });

                Add(new CardData //Blood Grail
                    {
                        cardName = "Blood Grail",
                        artist = "Sora AI",
                        rarity = "Mythic",
                        manaCost = 1,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.NoLifeGain
                        },
                        artwork = Resources.Load<Sprite>("Art/blood_grail")
                    });

                Add(new CardData //Potionist grimoire
                    {
                        cardName = "Potionist Grimoire",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        manaToPayToActivate = 1,
                        activatedAbilities = new List<ActivatedAbility>
                        {
                            ActivatedAbility.TapToPlayRandomPotion
                        },
                        artwork = Resources.Load<Sprite>("Art/potionist_grimoire")
                    });

                Add(new CardData //Anti-Magic Grid
                    {
                        cardName = "Anti-Magic Grid",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 6,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.OnlyCastCreatureSpells
                        },
                        artwork = Resources.Load<Sprite>("Art/anti_magic_grid")
                    });

                Add(new CardData //Tablet of Creation
                    {
                        cardName = "Tablet of Creation",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 2,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.CreatureSpellsCostOneLess
                        },
                        artwork = Resources.Load<Sprite>("Art/tablet_of_creation")
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

                Add(new CardData // Morning Star
                    {
                        cardName = "Morning Star",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 3,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        subtypes = new List<string> { "Equipment" },
                        powerBuff = 2,
                        toughnessBuff = 0,
                        manaToPayToActivate = 1,
                        activatedAbilities = new List<ActivatedAbility> { ActivatedAbility.Equip },
                        rulesText = "Equipped creature gets +2/+0.",
                        artwork = Resources.Load<Sprite>("Art/morning_star")
                    });

                Add(new CardData // Battle Shield
                    {
                        cardName = "Battle Shield",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        subtypes = new List<string> { "Equipment" },
                        powerBuff = 0,
                        toughnessBuff = 2,
                        manaToPayToActivate = 1,
                        activatedAbilities = new List<ActivatedAbility> { ActivatedAbility.Equip },
                        rulesText = "Equipped creature gets +0/+2.",
                        artwork = Resources.Load<Sprite>("Art/battle_shield")
                    });

                // Avatar cycle gaining +1/+1 counters
                Add(new CardData //Progress Incarnate
                    {
                        cardName = "Progress Incarnate",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Artifact" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Avatar" },
                        artwork = Resources.Load<Sprite>("Art/progress_incarnate"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnArtifactEnter,
                                description = "put a +1/+1 counter on this creature.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is CreatureCard creature)
                                    {
                                        creature.AddPlusOneCounter();
                                        GameManager.Instance.UpdateUI();
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData //Faith Incarnate
                    {
                        cardName = "Faith Incarnate",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "White", "White" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Avatar" },
                        keywordAbilities = new List<KeywordAbility> { KeywordAbility.Vigilance },
                        artwork = Resources.Load<Sprite>("Art/faith_incarnate"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnLifeGain,
                                description = "put that many +1/+1 counters on this creature.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is CreatureCard creature)
                                    {
                                        for (int i = 0; i < GameManager.Instance.lastLifeGainedAmount; i++)
                                            creature.AddPlusOneCounter();
                                        GameManager.Instance.UpdateUI();
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData //Wisdom Incarnate
                    {
                        cardName = "Wisdom Incarnate",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Blue", "Blue" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Avatar" },
                        keywordAbilities = new List<KeywordAbility> { KeywordAbility.Flying },
                        artwork = Resources.Load<Sprite>("Art/wisdom_incarnate"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnCardDraw,
                                description = "put that many +1/+1 counters on this creature.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is CreatureCard creature)
                                    {
                                        for (int i = 0; i < GameManager.Instance.lastCardsDrawnAmount; i++)
                                            creature.AddPlusOneCounter();
                                        GameManager.Instance.UpdateUI();
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData //Death Incarnate
                    {
                        cardName = "Death Incarnate",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Black", "Black" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Avatar" },
                        keywordAbilities = new List<KeywordAbility> { KeywordAbility.Lifelink },
                        artwork = Resources.Load<Sprite>("Art/death_incarnate"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnCreatureDies,
                                description = "put a +1/+1 counter on this creature.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is CreatureCard creature)
                                    {
                                        creature.AddPlusOneCounter();
                                        GameManager.Instance.UpdateUI();
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData //War Incarnate
                    {
                        cardName = "War Incarnate",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Red", "Red" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Avatar" },
                        keywordAbilities = new List<KeywordAbility> { KeywordAbility.Haste },
                        artwork = Resources.Load<Sprite>("Art/war_incarnate"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnCombatDamageToPlayer,
                                description = "put a +1/+1 counter on this creature.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is CreatureCard creature)
                                    {
                                        creature.AddPlusOneCounter();
                                        GameManager.Instance.UpdateUI();
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData //Nature Incarnate
                    {
                        cardName = "Nature Incarnate",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Green", "Green" },
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        subtypes = new List<string> { "Avatar" },
                        keywordAbilities = new List<KeywordAbility> { KeywordAbility.Trample },
                        artwork = Resources.Load<Sprite>("Art/nature_incarnate"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnLandEnter,
                                description = "put a +1/+1 counter on this creature.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is CreatureCard creature)
                                    {
                                        creature.AddPlusOneCounter();
                                        GameManager.Instance.UpdateUI();
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData //Pyramid of Pain
                    {
                        cardName = "Pyramid of Pain",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Black", "Black" },
                        cardType = CardType.Enchantment,
                        artwork = Resources.Load<Sprite>("Art/pyramid_of_pain"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnOpponentDraw,
                                description = "that player loses 1 life.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    Player opp = GameManager.Instance.GetOpponentOf(owner);
                                    opp.Life -= 1;
                                    GameObject ui = opp == GameManager.Instance.humanPlayer ?
                                        GameManager.Instance.playerLifeContainer :
                                        GameManager.Instance.enemyLifeContainer;
                                    GameManager.Instance.ShowFloatingDamage(1, ui);
                                    GameManager.Instance.UpdateUI();
                                    GameManager.Instance.CheckForGameEnd();
                                }
                            },
                            new CardAbility
                            {
                                timing = TriggerTiming.OnOpponentDiscard,
                                description = "that player loses 1 life.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    Player opp = GameManager.Instance.GetOpponentOf(owner);
                                    opp.Life -= 1;
                                    GameObject ui = opp == GameManager.Instance.humanPlayer ?
                                        GameManager.Instance.playerLifeContainer :
                                        GameManager.Instance.enemyLifeContainer;
                                    GameManager.Instance.ShowFloatingDamage(1, ui);
                                    GameManager.Instance.UpdateUI();
                                    GameManager.Instance.CheckForGameEnd();
                                }
                            }
                        }
                    });

                Add(new CardData //Demonic Corrosion
                    {
                        cardName = "Demonic Corrosion",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Black", "Black" },
                        cardType = CardType.Enchantment,
                        artwork = Resources.Load<Sprite>("Art/demonic_corrosion"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnUpkeep,
                                description = "you lose a life and draw a card for each pain counter on this enchantment. Then put a pain counter on this enchantment.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is EnchantmentCard enchantment)
                                    {
                                        int pain = enchantment.minusOneCounters;
                                        if (pain > 0)
                                        {
                                            owner.Life -= pain;
                                            GameObject ui = owner == GameManager.Instance.humanPlayer ?
                                                GameManager.Instance.playerLifeContainer :
                                                GameManager.Instance.enemyLifeContainer;
                                            GameManager.Instance.ShowFloatingDamage(pain, ui);
                                            for (int i = 0; i < pain; i++)
                                                GameManager.Instance.DrawCard(owner);
                                            GameManager.Instance.CheckForGameEnd();
                                        }
                                        enchantment.AddMinusOneCounter();
                                        GameManager.Instance.UpdateUI();
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData //Heavy Taxation
                    {
                        cardName = "Heavy Taxation",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 4,
                        color = new List<string> { "White", "White" },
                        cardType = CardType.Enchantment,
                        artwork = Resources.Load<Sprite>("Art/heavy_taxation"),
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.OpponentSpellsCostOneMore
                        }
                    });

                Add(new CardData //Headeache
                    {
                        cardName = "Headache",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        manaCost = 2,
                        color = new List<string> { "Red" },
                        cardType = CardType.Enchantment,
                        artwork = Resources.Load<Sprite>("Art/headache"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnPlayerDiscard,
                                description = "that player takes 1 damage.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    Player target = GameManager.Instance.lastDiscardingPlayer;
                                    if (target != null)
                                    {
                                        target.Life -= 1;
                                        GameObject ui = target == GameManager.Instance.humanPlayer ?
                                            GameManager.Instance.playerLifeContainer :
                                            GameManager.Instance.enemyLifeContainer;
                                        GameManager.Instance.ShowFloatingDamage(1, ui);
                                        GameManager.Instance.UpdateUI();
                                        GameManager.Instance.CheckForGameEnd();
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData //Like a Thunder
                    {
                        cardName = "Like a Thunder",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Red", "Red" },
                        cardType = CardType.Enchantment,
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.HasteCreaturesOnlyBlockedByHaste
                        },
                        artwork = Resources.Load<Sprite>("Art/like_a_thunder"),
                        rulesText = "Creatures with haste you control can only be blocked by creatures with haste.",
                    });

                Add(new CardData //Shrine of rot
                    {
                        cardName = "Shrine of Rot",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        manaCost = 3,
                        color = new List<string> { "Black", "Green" },
                        cardType = CardType.Enchantment,
                        artwork = Resources.Load<Sprite>("Art/shrine_of_rot"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnUpkeep,
                                description = "return a random land card from your graveyard to your hand.",
                                effect = (Player owner, Card selfCard) =>
                                {
                                    GameManager.Instance.ReturnRandomLandFromGraveyard(owner);
                                }
                            }
                        }
                    });

                Add(new CardData //Afterlife jinx lantern
                    {
                        cardName = "Afterlife Jinx Lantern",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 4,
                        color = new List<string> { "White", "Black" },
                        cardType = CardType.Enchantment,
                        artwork = Resources.Load<Sprite>("Art/afterlife_jinx_lantern"),
                        rulesText = "Whenever a non-token creature dies, create a Spirit.",
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnCreatureDies,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    Card dead = GameManager.Instance.lastDeadCreature;
                                    if (dead != null && dead.isToken)
                                        return;

                                    Card spirit = CardFactory.Create("Spirit");
                                    if (spirit == null)
                                    {
                                        Debug.LogError("Failed to spawn Spirit Token — check card database!");
                                        return;
                                    }
                                    GameManager.Instance.SummonToken(spirit, owner);
                                }
                            }
                        }
                    });

                Add(new CardData // Brotherhood
                    {
                        cardName = "Brotherhood",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "White", "Green" },
                        cardType = CardType.Enchantment,
                        artwork = Resources.Load<Sprite>("Art/brotherhood"),
                        rulesText = "Creatures you control gets +1/+1 for each other creature you control with the same name.",
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is EnchantmentCard enchantment)
                                    {
                                        enchantment.brotherhoodBuffs.Clear();
                                        var groups = owner.Battlefield.OfType<CreatureCard>().GroupBy(c => c.cardName);
                                        foreach (var group in groups)
                                        {
                                            int buff = group.Count() - 1;
                                            foreach (var creature in group)
                                            {
                                                if (buff > 0)
                                                {
                                                    creature.AddAuraBuff(buff, buff);
                                                    enchantment.brotherhoodBuffs[creature] = buff;
                                                }
                                                GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                                            }
                                        }
                                    }
                                }
                            },
                            new CardAbility
                            {
                                timing = TriggerTiming.OnCreatureEnter,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is EnchantmentCard enchantment)
                                    {
                                        Card entering = GameManager.Instance.lastEnteredCreature;
                                        if (entering is CreatureCard creature && GameManager.Instance.GetOwnerOfCard(entering) == owner)
                                        {
                                            string name = creature.cardName;
                                            var group = owner.Battlefield.OfType<CreatureCard>().Where(c => c.cardName == name).ToList();
                                            int buff = group.Count - 1;
                                            foreach (var c in group)
                                            {
                                                if (enchantment.brotherhoodBuffs.TryGetValue(c, out int prev))
                                                {
                                                    c.RemoveAuraBuff(prev, prev);
                                                    enchantment.brotherhoodBuffs.Remove(c);
                                                }
                                            }
                                            foreach (var c in group)
                                            {
                                                if (buff > 0)
                                                {
                                                    c.AddAuraBuff(buff, buff);
                                                    enchantment.brotherhoodBuffs[c] = buff;
                                                }
                                                GameManager.Instance.FindCardVisual(c)?.UpdateVisual();
                                            }
                                        }
                                    }
                                }
                            },
                            new CardAbility
                            {
                                timing = TriggerTiming.OnCreatureDies,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is EnchantmentCard enchantment)
                                    {
                                        Card dead = GameManager.Instance.lastDeadCreature;
                                        if (dead is CreatureCard creature && GameManager.Instance.GetOwnerOfCard(dead) == owner)
                                        {
                                            enchantment.brotherhoodBuffs.Remove(creature);
                                            string name = creature.cardName;
                                            var group = owner.Battlefield.OfType<CreatureCard>().Where(c => c.cardName == name).ToList();
                                            int buff = group.Count - 1;
                                            foreach (var c in group)
                                            {
                                                if (enchantment.brotherhoodBuffs.TryGetValue(c, out int prev))
                                                {
                                                    c.RemoveAuraBuff(prev, prev);
                                                    enchantment.brotherhoodBuffs.Remove(c);
                                                }
                                            }
                                            foreach (var c in group)
                                            {
                                                if (buff > 0)
                                                {
                                                    c.AddAuraBuff(buff, buff);
                                                    enchantment.brotherhoodBuffs[c] = buff;
                                                }
                                                GameManager.Instance.FindCardVisual(c)?.UpdateVisual();
                                            }
                                        }
                                    }
                                }
                            },
                            new CardAbility
                            {
                                timing = TriggerTiming.OnDeath,
                                description = string.Empty,
                                effect = (Player owner, Card selfCard) =>
                                {
                                    if (selfCard is EnchantmentCard enchantment)
                                    {
                                        foreach (var kv in enchantment.brotherhoodBuffs)
                                        {
                                            kv.Key.RemoveAuraBuff(kv.Value, kv.Value);
                                            GameManager.Instance.FindCardVisual(kv.Key)?.UpdateVisual();
                                        }
                                        enchantment.brotherhoodBuffs.Clear();
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData // Faith protection
                    {
                        cardName = "Faith Protection",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string> { "White" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = 0,
                        toughnessBuff = 4,
                        artwork = Resources.Load<Sprite>("Art/faith_protection"),
                        rulesText = "Enchanted creature gets +0/+4.",
                    });

                Add(new CardData // Glorious Anthem
                    {
                        cardName = "Glorious Anthem",
                        artist = "Greg Staples",
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "White", "White" },
                        cardType = CardType.Enchantment,
                        artwork = Resources.Load<Sprite>("Art/glorious_anthem"),
                        rulesText = "Creatures you control get +1/+1.",
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

                Add(new CardData // Sacred Horn
                    {
                        cardName = "Sacred Horn",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 2,
                        color = new List<string> { "White", "White" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = 1,
                        toughnessBuff = 1,
                        keywordBuff = KeywordAbility.Lifelink,
                        artwork = Resources.Load<Sprite>("Art/sacred_horn"),
                        rulesText = "Enchanted creature gets +1/+1 and has lifelink.",
                    });

                Add(new CardData // Tame
                    {
                        cardName = "Tame",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string> { "White" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        keywordBuff = KeywordAbility.Defender,
                        artwork = Resources.Load<Sprite>("Art/tame"),
                        rulesText = "Enchanted creature has defender.",
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

                Add(new CardData // Cut off hands
                    {
                        cardName = "Cut Off Hands",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string> { "Blue" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = -4,
                        toughnessBuff = 0,
                        artwork = Resources.Load<Sprite>("Art/cut_off_hands"),
                        rulesText = "Enchanted creature gets -4/-0",
                    });

                Add(new CardData // Sickness
                    {
                        cardName = "Sickness",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string> { "Black" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = -2,
                        toughnessBuff = -2,
                        artwork = Resources.Load<Sprite>("Art/sickness"),
                        rulesText = "Enchanted creature gets -2/-2",
                    });
                
                Add(new CardData // Faith protection
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

                Add(new CardData // Bog Pest
                    {
                        cardName = "Bog Pest",
                        artist = "Sora AI",
                        rarity = "Rare",
                        manaCost = 1,
                        color = new List<string> { "Black" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = -1,
                        toughnessBuff = -1,
                        artwork = Resources.Load<Sprite>("Art/bog_pest"),
                        rulesText = "Enchanted creature gets -1/-1 and gains \"At the beginning of your upkeep, put a copy of this enchantment on target random creature.\"",
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnUpkeep,
                                description = "put a copy of this enchantment on target random creature.",
                                effect = (Player owner, Card source) =>
                                {
                                    var creatures = GameManager.Instance.humanPlayer.Battlefield
                                        .OfType<CreatureCard>()
                                        .Concat(GameManager.Instance.aiPlayer.Battlefield.OfType<CreatureCard>())
                                        .ToList();
                                    if (creatures.Count == 0)
                                        return;
                                    int index = Random.Range(0, creatures.Count);
                                    CreatureCard target = creatures[index];
                                    Card copy = CardFactory.Create(source.cardName);
                                    if (copy is AuraCard auraCopy)
                                    {
                                        auraCopy.attachedTo = target;
                                        GameManager.Instance.SummonToken(auraCopy, owner);
                                    }
                                }
                            }
                        }
                    });

                Add(new CardData // Devouring Fury
                    {
                        cardName = "Devouring Fury",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string> { "Red" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = 4,
                        toughnessBuff = -2,
                        targetMustBeControlledCreature = true,
                        artwork = Resources.Load<Sprite>("Art/devouring_fury"),
                        rulesText = "Enchanted creature gets +4/-2.",
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

                Add(new CardData // Woodskin
                    {
                        cardName = "Woodskin",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 1,
                        color = new List<string> { "Green" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        powerBuff = 2,
                        toughnessBuff = 2,
                        artwork = Resources.Load<Sprite>("Art/woodskin"),
                        rulesText = "Enchanted creature gets +2/+2.",
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

                Add(new CardData // Sleep
                    {
                        cardName = "Sleep",
                        artist = "Sora AI",
                        rarity = "Common",
                        manaCost = 3,
                        color = new List<string> { "Blue" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        requiredTargetType = SorceryCard.TargetType.TappedCreature,
                        keywordBuff = KeywordAbility.CantUntap,
                        artwork = Resources.Load<Sprite>("Art/sleep"),
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

                Add(new CardData // Fascinate
                    {
                        cardName = "Fascinate",
                        artist = "Sora AI",
                        rarity = "Uncommon",
                        manaCost = 4,
                        color = new List<string> { "Blue", "Blue" },
                        cardType = CardType.Enchantment,
                        subtypes = new List<string> { "Aura" },
                        gainControlOfCreature = true,
                        artwork = Resources.Load<Sprite>("Art/fascinate"),
                        //rulesText = "You control enchanted creature.",
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
