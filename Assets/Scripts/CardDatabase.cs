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
                artwork = Resources.Load<Sprite>("Art/plains")
            });
            Add(new CardData //Island
            {
                cardName = "Island",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Blue" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/island")
            });
            Add(new CardData //Swamp
            {
                cardName = "Swamp",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Black" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/swamp")
            });
            Add(new CardData //Mountain
            {
                cardName = "Mountain",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Red" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/mountain")
            });
            Add(new CardData //Forest
            {
                cardName = "Forest",
                rarity = "Common",
                manaCost = 0,
                color = new List<string> { "Green" },
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/forest")
            });

        // Creatures
            //WHITE
                Add(new CardData //Iconoclast monk
                    {
                        cardName = "Iconoclast Monk",
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
                                description = ", you may destroy target non-creature artifact.",
                                requiresTarget = true,
                                requiredTargetType = SorceryCard.TargetType.Artifact,
                                effect = (Player owner, Card target) =>
                                {
                                    Player controller = GameManager.Instance.GetOwnerOfCard(target);
                                    GameManager.Instance.SendToGraveyard(target, controller);
                                }
                            }
                        }
                    });
                Add (new CardData { // Beasthunter
                    cardName = "Beasthunter",
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Human", "Warrior" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.ProtectionFromRed,
                        KeywordAbility.ProtectionFromGreen,
                    },
                    artwork = Resources.Load<Sprite>("Art/beasthunter")
                    });
                Add(new CardData // Angry farmer
                    {
                    cardName = "Angry Farmer",
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Human" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/angry_farmer")
                    });
                Add(new CardData //Trinkets Collector
                    {
                    cardName = "Trinkets Collector",
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
                                GameManager.Instance.TryGainLife(owner, 1);
                            }
                        }
                    }
                    });
                Add(new CardData //Gallant lord
                    {
                    cardName = "Gallant Lord",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "White" },
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
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = new List<string> { "White" },
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
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 3,
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
                                GameManager.Instance.TryGainLife(owner, 1);
                                //Debug.Log("Waterbearer enters: gain 1 life.");
                            }
                        },
                        new CardAbility
                        {
                            timing = TriggerTiming.OnDeath,
                            description = "gain 1 life.",
                            effect = (Player owner, Card unused) =>
                            {
                                GameManager.Instance.TryGainLife(owner, 1);
                                //Debug.Log("Waterbearer dies: gain 1 life.");
                            }
                        }
                    }
                    });
                Add(new CardData //Virgins procession
                    {
                    cardName = "Virgins Procession",
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "White" },
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
                                GameManager.Instance.TryGainLife(owner, 4);
                            }
                        },
                    }
                    });
                Add(new CardData //Realm protector
                    {
                    cardName = "Realm Protector",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "White" },
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
                Add(new CardData //Hamlet Recruiter
                    {
                    cardName = "Hamlet Recruiter",
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
                Add(new CardData //Skyhunter unicorn
                    {
                    cardName = "Skyhunter Unicorn",
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "White" },
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
                    rarity = "Rare",
                    manaCost = 7,
                    color = new List<string> { "White" },
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
                                GameManager.Instance.TryGainLife(owner, 5);
                                Debug.Log("Gain 5 life at upkeep.");
                            }
                        }
                    }
                    });
                Add(new CardData //Untamed Unicorn
                    {
                    cardName = "Untamed Unicorn",
                    rarity = "Rare",
                    manaCost = 6,
                    color = new List<string> { "White" },
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 0,
                    subtypes = new List<string> { "Horse" },
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
                                }
                            }
                        }
                    }
                    });
                Add(new CardData // Human Soldier Token
                    {
                        cardName = "Human Soldier",
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
            
            //BLUE
                Add(new CardData //Skyward whale
                    {
                    cardName = "Skyward Whale",
                    rarity = "Common",
                    manaCost = 6,
                    color = new List<string> { "Blue" },
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
                Add(new CardData //Wandering squid
                    {
                    cardName = "Wandering Squid",
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
                    rarity = "Common",
                    manaCost = 9,
                    color = new List<string> { "Blue" },
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
                    rarity = "Rare",
                    manaCost = 3,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    subtypes = new List<string> { "Wizard" },
                    keywordAbilities = new List<KeywordAbility> { },
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
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 3,
                    subtypes = new List<string> { "Merfolk", "Warrior" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/sharkmen_tribe")
                    });
                Add(new CardData //Cosmic Whale
                    {
                    cardName = "Cosmic Whale",
                    rarity = "Rare",
                    manaCost = 8,
                    color = new List<string> { "Blue" },
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
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Blue" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Spirit" },
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
                            description = "this creature gets +2/+2 until end of turn.",
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
            //BLACK
                Add(new CardData { //Hired assassin
                    cardName = "Hired Assassin",
                    rarity = "Uncommon",
                    manaCost = 6,
                    color = new List<string> { "Black" },
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
                                if (target is CreatureCard creature && !creature.color.Contains("Artifact"))
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
                Add(new CardData //Bog crocodile
                        {
                        cardName = "Bog Crocodile",
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
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Black" },
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
                    rarity = "Common",
                    manaCost = 6,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 6,
                    toughness = 1,
                    subtypes = new List<string> { "Zombie", "Leviathan" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/rotting_whale")
                    });
                Add(new CardData //Rotting Dragon
                    {
                    cardName = "Rotting Dragon",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "Black" },
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
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    entersTapped = true,
                    subtypes = new List<string> { "Zombie" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/limping_corpse")
                    });
                Add(new CardData //Famished crow
                    {
                    cardName = "Famished Crow",
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
                Add(new CardData //Giant crow
                    {
                    cardName = "Giant Crow",
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
                    rarity = "Rare",
                    manaCost = 5,
                    color = new List<string> { "Black" },
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
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = new List<string> { "Black" },
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
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Black" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 1,
                    subtypes = new List<string> { "Rat" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/giant_rat")
                    });
                Add(new CardData //Bog mosquito
                    {
                    cardName = "Bog Mosquito",
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
                    rarity = "Uncommon",
                    manaCost = 8,
                    color = new List<string> { "Black" },
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
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = new List<string> { "Black" },
                        cardType = CardType.Creature,
                        power = 2,
                        toughness = 2,
                        entersTapped = true,
                        subtypes = new List<string> { "Zombie" },
                        keywordAbilities = new List<KeywordAbility> { },
                        artwork = Resources.Load<Sprite>("Art/zombie_token")
                    });

                Add(new CardData //Nocturnal Spectre
                    {
                        cardName = "Nocturnal Spectre",
                        rarity = "Uncommon",
                        manaCost = 4,
                        color = new List<string> { "Black" },
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
                Add(new CardData // Demon Token
                    {
                        cardName = "Demon",
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
                Add(new CardData //Fireborn dragon
                    {
                    cardName = "Fireborn Dragon",
                    rarity = "Rare",
                    manaCost = 6,
                    color = new List<string> { "Red" },
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
                    rarity = "Rare",
                    manaCost = 7,
                    color = new List<string> { "Red" },
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
                    artwork = Resources.Load<Sprite>("Art/great_boulder")
                    });
                Add(new CardData //Flying pig
                    {
                    cardName = "Flying Pig",
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
                Add(new CardData //Goblin puncher
                    {
                    cardName = "Goblin Puncher",
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
                    artwork = Resources.Load<Sprite>("Art/goblin_puncher")
                    });
                Add(new CardData //Goblin Beastmaster
                    {
                    cardName = "Goblin Beastmaster",
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
                Add(new CardData //Thundermare
                    {
                    cardName = "Thundermare",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = new List<string> { "Red" },
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 3,
                    subtypes = new List<string> { "Elemental" },
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Haste,
                        KeywordAbility.Trample,
                    },
                    artwork = Resources.Load<Sprite>("Art/thundermare")
                    });
                Add(new CardData //Village idiot
                    {
                    cardName = "Village Idiot",
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
                    artwork = Resources.Load<Sprite>("Art/wild_ostrich")
                    });
                Add(new CardData // Dragon Token
                    {
                        cardName = "Dragon",
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
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "Green" },
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
                Add(new CardData //Cactusaurus
                    {
                    cardName = "Cactusaurus",
                    rarity = "Common",
                    manaCost = 5,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
                    subtypes = new List<string> { "Plant", "Reptile" },
                    keywordAbilities = new List<KeywordAbility> {
                        KeywordAbility.Trample
                    },
                    artwork = Resources.Load<Sprite>("Art/cactusaurus")
                    });
                Add(new CardData //Realms crasher
                    {
                    cardName = "Realms Crasher",
                    rarity = "Uncommon",
                    manaCost = 7,
                    color = new List<string> { "Green" },
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
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = new List<string> { "Green" },
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
                    rarity = "Common",
                    manaCost = 4,
                    color = new List<string> { "Green" },
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
                Add(new CardData //Domestic cat
                    {
                    cardName = "Domestic Cat",
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
                    artwork = Resources.Load<Sprite>("Art/domestic_cat")
                    });
                Add(new CardData //Deep forest monkeys
                    {
                    cardName = "Deep Forest Monkeys",
                    rarity = "Common",
                    manaCost = 2,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    subtypes = new List<string> { "Monkey" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/deep_forest_monkeys")
                    });
                Add(new CardData //Violent Monkey
                    {
                    cardName = "Violent Ape",
                    rarity = "Common",
                    manaCost = 3,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    subtypes = new List<string> { "Monkey" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/violent_ape")
                    });
                Add(new CardData //Flying donkey
                    {
                    cardName = "Flying Donkey",
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
                    rarity = "Common",
                    manaCost = 8,
                    color = new List<string> { "Green" },
                    cardType = CardType.Creature,
                    power = 8,
                    toughness = 8,
                    subtypes = new List<string> { "Beast" },
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/slack_tungo")
                    });
                Add(new CardData // Cat Token
                    {
                        cardName = "Cat",
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
                Add(new CardData // Monkey Token
                    {
                        cardName = "Monkey",
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
                Add(new CardData //Omega golemoid
                    {
                    cardName = "Omega Golemoid",
                    rarity = "Uncommon",
                    manaCost = 7,
                    color = new List<string> { "Artifact" },
                    cardType = CardType.Creature,
                    power = 7,
                    toughness = 5,
                    subtypes = new List<string> { "Golem" },
                    entersTapped = true,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/omega_golemoid")
                    });
                Add(new CardData //Glassmole
                    {
                    cardName = "Glassmole",
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
                    artwork = Resources.Load<Sprite>("Art/obstacle")
                    });
                Add(new CardData //Autonomous miner
                    { 
                    cardName = "Autonomous Miner",
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
            //MULTI
                Add(new CardData //Blazefire angel
                    {
                    cardName = "Blazefire Angel",
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
                    artwork = Resources.Load<Sprite>("Art/twin_elves")
                    });
                Add(new CardData //Night moth
                    {
                    cardName = "Night Moth",
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
                    artwork = Resources.Load<Sprite>("Art/battle_bear")
                    });
                Add(new CardData //Robin
                    {
                    cardName = "Robin",
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
                    artwork = Resources.Load<Sprite>("Art/robin")
                    });
                Add(new CardData //Crazed shaman
                    {
                    cardName = "Crazed Shaman",
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
        // Sorceries
            //WHITE
                Add(new CardData { //Shattering light
                    cardName = "Shattering Light",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "White" },
                    requiresTarget = true,
                    destroyTargetIfTypeMatches = true,
                    requiredTargetType = SorceryCard.TargetType.Creature,
                    requiredTargetColor ="Black",
                    artwork = Resources.Load<Sprite>("Art/shattering_light")
                    });
                Add(new CardData { //for glory
                    cardName = "For Glory",
                    rarity = "Uncommon",
                    cardType = CardType.Sorcery,
                    manaCost = 5,
                    color = new List<string> { "White" },
                    artwork = Resources.Load<Sprite>("Art/for_glory"),
                    abilities = new List<CardAbility>(),
                    tokenToCreate = "Human Soldier",       // Token name as defined in CardFactory
                    numberOfTokensMin = 4,
                    numberOfTokensMax = 4,
                    });
                Add(new CardData { //rolling army
                    cardName = "Rolling Army",
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
                Add(new CardData { //Candlelight
                    cardName = "Candlelight",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 1,
                    color = new List<string> { "White" },
                    lifeToGain = 4,
                    artwork = Resources.Load<Sprite>("Art/Candlelight"),
                    abilities = new List<CardAbility>(),
                    });
                Add(new CardData { //Solid prayer
                    cardName = "Solid Prayer",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "White" },
                    lifeToGain = 7,
                    artwork = Resources.Load<Sprite>("Art/solid_prayer"),
                    abilities = new List<CardAbility>(),
                    });
                Add(new CardData
                    {
                        cardName = "Deny the Afterlife",
                        rarity = "Uncommon",
                        cardType = CardType.Sorcery,
                        manaCost = 1,
                        color = new List<string> { "White" },
                        artwork = Resources.Load<Sprite>("Art/deny_the_afterlife"),
                        exileAllCreaturesFromGraveyards = true
                    });
            //BLUE
                Add(new CardData { //Blast of knowledge
                    cardName = "Blast of Knowledge",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 5,
                    color = new List<string> { "Blue" },
                    cardsToDraw = 3,
                    artwork = Resources.Load<Sprite>("Art/blast_of_knowledge"),
                    });
                Add(new CardData { //Astral Plane
                    cardName = "Astral Plane",
                    rarity = "Rare",
                    cardType = CardType.Sorcery,
                    manaCost = 5,
                    color = new List<string> { "Blue" },
                    swapGraveyardAndLibrary = true,
                    artwork = Resources.Load<Sprite>("Art/astral_plane"),
                    });
            //BLACK
                Add(new CardData { //Devouring shadows
                    cardName = "Devouring Shadow",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "Black" },
                    requiresTarget = true,
                    destroyTargetIfTypeMatches = true,
                    requiredTargetType = SorceryCard.TargetType.Creature,
                    requiredTargetColor = "White",
                    artwork = Resources.Load<Sprite>("Art/devouring_shadows")
                    });
                Add(new CardData //stain of rot
                    {
                        cardName = "Stain of Rot",
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
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        color = new List<string> { "Black" },
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
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 4,
                        color = new List<string> { "Black" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        destroyTargetIfTypeMatches = true,
                        artwork = Resources.Load<Sprite>("Art/lights_out"),
                    });
                Add(new CardData { //Witches rite
                    cardName = "Witches Rite",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 1,
                    color = new List<string> { "Black" },
                    lifeToLoseForOpponent = 3,
                    artwork = Resources.Load<Sprite>("Art/witches_rite"),
                    });
                Add(new CardData { //Communed rot
                    cardName = "Communed Rot",
                    rarity = "Rare",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "Black" },
                    lifeLossForBothPlayers = 4,
                    artwork = Resources.Load<Sprite>("Art/communed_rot"),
                    });
                Add(new CardData //Forget
                    {
                        cardName = "Forget",
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
                        rarity = "Rare",
                        cardType = CardType.Sorcery,
                        manaCost = 4,
                        color = new List<string> { "Black" },
                        artwork = Resources.Load<Sprite>("Art/massacre"),
                        typeOfPermanentToDestroyAll = SorceryCard.PermanentTypeToDestroy.Creature
                    });
                Add(new CardData //Mirrorbreak
                    {
                        cardName = "Mirrorbreak",
                        rarity = "Uncommon",
                        cardType = CardType.Sorcery,
                        manaCost = 3,
                        color = new List<string> { "Black" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        destroyAllWithSameName = true,
                        artwork = Resources.Load<Sprite>("Art/mirrorbreak"),
                    });
            // RED
                Add(new CardData //To dig a hole
                        {
                            cardName = "To Dig a Hole",
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
                            rarity = "Common",
                            cardType = CardType.Sorcery,
                            manaCost = 1,
                            color = new List<string> { "Red" },
                            requiresTarget = true,
                            requiredTargetType = SorceryCard.TargetType.Artifact,
                            destroyTargetIfTypeMatches = true,
                            artwork = Resources.Load<Sprite>("Art/melt"),
                        });
                Add(new CardData //Dash
                    {
                        cardName = "Dash",
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
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 6,
                        color = new List<string> { "Red" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.Creature,
                        damageToTarget = 6,
                        artwork = Resources.Load<Sprite>("Art/thunderstrike"),
                    });
                Add(new CardData //fire hatchet
                    {
                        cardName = "Fire Hatchet",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 4,
                        color = new List<string> { "Red" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.CreatureOrPlayer,
                        damageToTarget = 4,
                        artwork = Resources.Load<Sprite>("Art/fire_hatchet"),
                    });
                Add(new CardData
                    {
                        cardName = "Explosion",
                        rarity = "Common",
                        cardType = CardType.Sorcery,
                        manaCost = 1,
                        color = new List<string> { "Red" },
                        requiresTarget = true,
                        requiredTargetType = SorceryCard.TargetType.CreatureOrPlayer,
                        damageToTarget = 3,
                        artwork = Resources.Load<Sprite>("Art/explosion"),
                    });
                Add(new CardData //Moonfall
                    {
                        cardName = "Moonfall",
                        rarity = "Rare",
                        cardType = CardType.Sorcery,
                        manaCost = 4,
                        color = new List<string> { "Red" },
                        artwork = Resources.Load<Sprite>("Art/moonfall"),
                        typeOfPermanentToDestroyAll = SorceryCard.PermanentTypeToDestroy.Land,
                    });
                Add(new CardData
                    {
                        cardName = "Fire Spirals",
                        rarity = "Uncommon",
                        cardType = CardType.Sorcery,
                        manaCost = 3,
                        color = new List<string> { "Red" },
                        artwork = Resources.Load<Sprite>("Art/fire_spirals"),
                        damageToEachCreatureAndPlayer = 2
                    });
            // GREEN
                Add(new CardData //whip of thorns
                        {
                            cardName = "Whip of Thorns",
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
                            rarity = "Rare",
                            cardType = CardType.Sorcery,
                            manaCost = 4,
                            color = new List<string> { "Green" },
                            artwork = Resources.Load<Sprite>("Art/natures_rebuke"),
                            typeOfPermanentToDestroyAll = SorceryCard.PermanentTypeToDestroy.Artifact
                        });
                Add(new CardData { //Feast
                    cardName = "Feast",
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 1,
                    color = new List<string> { "Green" },
                    eachPlayerGainLifeEqualToLands = true,
                    artwork = Resources.Load<Sprite>("Art/feast"),
                    });
                Add(new CardData { //Rolling Energy
                    cardName = "Rolling Energy",
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
                    rarity = "Uncommon",
                    cardType = CardType.Sorcery,
                    manaCost = 6,
                    color = new List<string> { "Green" },
                    requiresTarget = true,
                    rulesText = "Target creature gets +6/+6 until the end of turn.",
                    requiredTargetType = SorceryCard.TargetType.Creature,
                    powerBuff = 6,
                    toughnessBuff = 6,
                    artwork = Resources.Load<Sprite>("Art/muscle_blast"),
                    });
            ///MULTI
                Add(new CardData //Drain mind
                        {
                        cardName = "Drain Mind",
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
                    rarity = "Common",
                    cardType = CardType.Sorcery,
                    manaCost = 2,
                    color = new List<string> { "Black", "Red" },
                    cardsToDiscardorDraw = 1,
                    drawIfOpponentCantDiscard = false,
                    lifeToLoseForOpponent = 2,
                    artwork = Resources.Load<Sprite>("Art/burn_mind")
                    });

        // Artifacts
            Add(new CardData // Pressure Sphere
                {
                    cardName = "Pressure Sphere",
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
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string>(),
                    cardType = CardType.Artifact,
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
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string>(),
                    cardType = CardType.Artifact,
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
                    rarity = "Common",
                    manaCost = 1,
                    color = new List<string>(),
                    cardType = CardType.Artifact,
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
                          rarity = "Common",
                          manaCost = 1,
                          color = new List<string>(),
                          cardType = CardType.Artifact,
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
                          rarity = "Common",
                          manaCost = 1,
                          color = new List<string>(),
                          cardType = CardType.Artifact,
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
                        rarity = "Uncommon",
                        manaCost = 4,
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
                        rarity = "Rare",
                        manaCost = 1,
                        color = new List<string>(),
                        cardType = CardType.Artifact,
                        keywordAbilities = new List<KeywordAbility>
                        {
                            KeywordAbility.NoLifeGain
                        },
                        artwork = Resources.Load<Sprite>("Art/blood_grail")
                    });

                Add(new CardData //Anti-Magic Grid
                    {
                        cardName = "Anti-Magic Grid",
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

                // Avatar cycle gaining +1/+1 counters
                Add(new CardData //Progress Incarnate
                    {
                        cardName = "Progress Incarnate",
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
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "White" },
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
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Blue" },
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
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Black" },
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
                                timing = TriggerTiming.OnCreatureDiesOrDiscarded,
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
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Red" },
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
                        rarity = "Rare",
                        manaCost = 3,
                        color = new List<string> { "Green" },
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