using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                color = "White",
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/plains")
            });
            Add(new CardData //Island
            {
                cardName = "Island",
                rarity = "Common",
                manaCost = 0,
                color = "Blue",
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/island")
            });
            Add(new CardData //Swamp
            {
                cardName = "Swamp",
                rarity = "Common",
                manaCost = 0,
                color = "Black",
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/swamp")
            });
            Add(new CardData //Mountain
            {
                cardName = "Mountain",
                rarity = "Common",
                manaCost = 0,
                color = "Red",
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/mountain")
            });
            Add(new CardData //Forest
            {
                cardName = "Forest",
                rarity = "Common",
                manaCost = 0,
                color = "Green",
                cardType = CardType.Land,
                artwork = Resources.Load<Sprite>("Art/forest")
            });

        // Creatures
            //WHITE
                Add(new CardData //Angry farmer
                    {
                    cardName = "Angry Farmer",
                    rarity = "Common",
                    manaCost = 1,
                    color = "White",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/angry_farmer")
                    });
                Add(new CardData //Gallant lord
                    {
                    cardName = "Gallant Lord",
                    rarity = "Common",
                    manaCost = 3,
                    color = "White",
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 3,
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
                    color = "White",
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 4,
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
                    color = "White",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 3,
                    keywordAbilities = new List<KeywordAbility> {},
                    artwork = Resources.Load<Sprite>("Art/waterbearer"),
                    abilities = new List<CardAbility>
                    {
                        new CardAbility
                        {
                            timing = TriggerTiming.OnEnter,
                            description = "gain 1 life.",
                            effect = (Player owner) =>
                            {
                                owner.Life += 1;
                                GameManager.Instance.UpdateUI();
                                //Debug.Log("Waterbearer enters: gain 1 life.");
                            }
                        },
                        new CardAbility
                        {
                            timing = TriggerTiming.OnDeath,
                            description = "gain 1 life.",
                            effect = (Player owner) =>
                            {
                                owner.Life += 1;
                                GameManager.Instance.UpdateUI();
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
                    color = "White",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 4,
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
                            effect = (Player owner) =>
                            {
                                owner.Life += 4;
                                GameManager.Instance.UpdateUI();
                            }
                        },
                    }
                    });
                Add(new CardData //Realm protector
                    {
                    cardName = "Realm Protector",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = "White",
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 7,
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
                    color = "White",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/hamlet_recruiter")
                    });
                Add(new CardData //Skyhunter unicorn
                    {
                    cardName = "Skyhunter Unicorn",
                    rarity = "Common",
                    manaCost = 4,
                    color = "White",
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
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
                    color = "White",
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/pure_angel")
                    });
            //BLUE
                Add(new CardData //Skyward whale
                    {
                    cardName = "Skyward Whale",
                    rarity = "Common",
                    manaCost = 6,
                    color = "Blue",
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
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
                    color = "Blue",
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 6,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/wandering_squid")
                    });
                Add(new CardData //Giant crab
                    {
                    cardName = "Giant Crab",
                    rarity = "Common",
                    manaCost = 4,
                    color = "Blue",
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 4,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/giant_crab")
                    });
                Add(new CardData //Wandering cloud
                    {
                    cardName = "Wandering Cloud",
                    rarity = "Common",
                    manaCost = 4,
                    color = "Blue",
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/wandering_cloud")
                    });
                Add(new CardData //Lucky fisherman
                    {
                        cardName = "Lucky Fisherman",
                        rarity = "Common",
                        manaCost = 2,
                        color = "Blue",
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        keywordAbilities = new List<KeywordAbility> { },
                        artwork = Resources.Load<Sprite>("Art/lucky_fisherman"),
                        abilities = new List<CardAbility>
                        {
                            new CardAbility
                            {
                                timing = TriggerTiming.OnEnter,
                                description = "draw a card.",
                                effect = (Player owner) =>
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
                    color = "Blue",
                    cardType = CardType.Creature,
                    power = 8,
                    toughness = 8,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/colossal_octopus")
                    });
                Add(new CardData //Replicator
                    {
                    cardName = "Replicator",
                    rarity = "Rare",
                    manaCost = 3,
                    color = "Blue",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/replicator")
                    });
                Add(new CardData //Sharkmen tribe
                    {
                    cardName = "Sharkmen Tribe",
                    rarity = "Common",
                    manaCost = 5,
                    color = "Blue",
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 3,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/sharkmen_tribe")
                    });
            //BLACK
                Add(new CardData //Rotting whale
                    {
                    cardName = "Rotting Whale",
                    rarity = "Common",
                    manaCost = 6,
                    color = "Black",
                    cardType = CardType.Creature,
                    power = 6,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/rotting_whale")
                    });
                Add(new CardData //Limping corpse
                    {
                    cardName = "Limping Corpse",
                    rarity = "Common",
                    manaCost = 2,
                    color = "Black",
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    entersTapped = true,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/limping_corpse")
                    });
                Add(new CardData //Famished crow
                    {
                    cardName = "Famished Crow",
                    rarity = "Common",
                    manaCost = 1,
                    color = "Black",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
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
                    color = "Black",
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlock,
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/giant_crow")
                    });
                Add(new CardData //Possessed innocent
                    {
                    cardName = "Possessed Innocent",
                    rarity = "Rare",
                    manaCost = 5,
                    color = "Black",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/possessed_innocent"),
                    abilities = new List<CardAbility>
                    {
                    new CardAbility
                        {
                            timing = TriggerTiming.OnDeath,
                            description = "create a Demon.",
                            effect = (Player owner) =>
                            {
                                Card demon = CardFactory.Create("Demon Token");
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
                    color = "Black",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/lunatic_necromancer")
                    });
                Add(new CardData //Sad clown
                    {
                    cardName = "Sad Clown",
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = "Black",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/sad_clown")
                    });
                Add(new CardData //Ratbat
                    {
                    cardName = "Ratbat",
                    rarity = "Common",
                    manaCost = 2,
                    color = "Black",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlock,
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/ratbat")
                    });
                Add(new CardData //Giant rat
                    {
                    cardName = "Giant Rat",
                    rarity = "Common",
                    manaCost = 3,
                    color = "Black",
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/giant_rat")
                    });
                Add(new CardData //Bog mosquito
                    {
                    cardName = "Bog Mosquito",
                    rarity = "Common",
                    manaCost = 4,
                    color = "Black",
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlock,
                        KeywordAbility.Flying
                    },
                    artwork = Resources.Load<Sprite>("Art/bog_mosquito")
                    });
                Add(new CardData // Demon Token
                    {
                        cardName = "Demon Token",
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = "Black",
                        cardType = CardType.Creature,
                        power = 5,
                        toughness = 5,
                        keywordAbilities = new List<KeywordAbility> { KeywordAbility.Flying },
                        artwork = Resources.Load<Sprite>("Art/demon_token"),
                        abilities = new List<CardAbility>()
                    });
            //RED
                Add(new CardData //Rabid dog
                    {
                    cardName = "Rabid Dog",
                    rarity = "Common",
                    manaCost = 1,
                    color = "Red",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
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
                    color = "Red",
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
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
                    color = "Red",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/dragon_summoner")
                    });
                Add(new CardData //Great boulder
                    {
                    cardName = "Great Boulder",
                    rarity = "Common",
                    manaCost = 2,
                    color = "Red",
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 4,
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
                    color = "Red",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
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
                    color = "Red",
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.CantBlock
                    },
                    artwork = Resources.Load<Sprite>("Art/goblin_puncher")
                    });
                Add(new CardData //Thundermare
                    {
                    cardName = "Thundermare",
                    rarity = "Uncommon",
                    manaCost = 4,
                    color = "Red",
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 3,
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Haste,
                    },
                    artwork = Resources.Load<Sprite>("Art/thundermare")
                    });
                Add(new CardData //Village idiot
                    {
                    cardName = "Village Idiot",
                    rarity = "Common",
                    manaCost = 1,
                    color = "Red",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/village_idiot")
                    });
                Add(new CardData //Wild ostrich
                    {
                    cardName = "Wild Ostrich",
                    rarity = "Uncommon",
                    manaCost = 2,
                    color = "Red",
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Haste,
                        KeywordAbility.CantBlock
                    },
                    artwork = Resources.Load<Sprite>("Art/wild_ostrich")
                    });
            //GREEN
                Add(new CardData //Cactusaurus
                    {
                    cardName = "Cactusaurus",
                    rarity = "Common",
                    manaCost = 5,
                    color = "Green",
                    cardType = CardType.Creature,
                    power = 5,
                    toughness = 5,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/cactusaurus")
                    });
                Add(new CardData //Realms crasher
                    {
                    cardName = "Realms Crasher",
                    rarity = "Uncommon",
                    manaCost = 7,
                    color = "Green",
                    cardType = CardType.Creature,
                    power = 7,
                    toughness = 7,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/realm_crasher")
                    });
                Add(new CardData //Drumming elf
                    {
                    cardName = "Drumming elf",
                    rarity = "Uncommon",
                    manaCost = 5,
                    color = "Green",
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/drumming_elf")
                    });
                Add(new CardData //Crazy cat lady
                    {
                    cardName = "Crazy Cat Lady",
                    rarity = "Common",
                    manaCost = 4,
                    color = "Green",
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/crazy_cat_lady"),
                    abilities = new List<CardAbility>
                    {
                    new CardAbility
                    {
                        timing = TriggerTiming.OnEnter,
                        description = "create two Cats.",
                        effect = (Player owner) =>
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Card cat = CardFactory.Create("Cat Token");
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
                    color = "Green",
                    cardType = CardType.Creature,
                    power = 1,
                    toughness = 1,
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
                    color = "Green",
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/deep_forest_monkeys")
                    });
                Add(new CardData //Violent ape
                    {
                    cardName = "Violent Ape",
                    rarity = "Common",
                    manaCost = 3,
                    color = "Green",
                    cardType = CardType.Creature,
                    power = 3,
                    toughness = 3,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/violent_ape")
                    });
                Add(new CardData //Flying donkey
                    {
                    cardName = "Flying Donkey",
                    rarity = "Common",
                    manaCost = 4,
                    color = "Green",
                    cardType = CardType.Creature,
                    power = 2,
                    toughness = 2,
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
                    color = "Green",
                    cardType = CardType.Creature,
                    power = 8,
                    toughness = 8,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/slack_tungo")
                    });
                Add(new CardData // Cat Token
                    {
                        cardName = "Cat Token",
                        rarity = "Token",
                        manaCost = 0,
                        isToken = true,
                        color = "Green",
                        cardType = CardType.Creature,
                        power = 1,
                        toughness = 1,
                        keywordAbilities = new List<KeywordAbility> { KeywordAbility.Reach },
                        artwork = Resources.Load<Sprite>("Art/cat_token"),
                        abilities = new List<CardAbility>()
                    });
            //ARTIFACT
                Add(new CardData //Origin Golem
                    {
                    cardName = "Origin Golem",
                    rarity = "Common",
                    manaCost = 5,
                    color = "None",
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 4,
                    entersTapped = true,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/origin_golem")
                    });
                Add(new CardData //Omega golemoid
                    {
                    cardName = "Omega Golemoid",
                    rarity = "Uncommon",
                    manaCost = 7,
                    color = "None",
                    cardType = CardType.Creature,
                    power = 7,
                    toughness = 5,
                    entersTapped = true,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/omega_golemoid")
                    });
                Add(new CardData //Glassmole
                    {
                    cardName = "Glassmole",
                    rarity = "Common",
                    manaCost = 4,
                    color = "None",
                    cardType = CardType.Creature,
                    power = 4,
                    toughness = 1,
                    entersTapped = true,
                    keywordAbilities = new List<KeywordAbility> { },
                    artwork = Resources.Load<Sprite>("Art/glassmole")
                    });
                Add(new CardData //Obstacle
                    {
                    cardName = "Obstacle",
                    rarity = "Common",
                    manaCost = 0,
                    color = "None",
                    cardType = CardType.Creature,
                    power = 0,
                    toughness = 1,
                    keywordAbilities = new List<KeywordAbility>
                    {
                        KeywordAbility.Defender,
                    },
                    artwork = Resources.Load<Sprite>("Art/obstacle")
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
}