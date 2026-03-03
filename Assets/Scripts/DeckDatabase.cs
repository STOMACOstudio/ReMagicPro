using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeckDatabase
{
    private struct DeckEntry
    {
        public string CardName;
        public int Count;

        public DeckEntry(string cardName, int count)
        {
            CardName = cardName;
            Count = count;
        }
    }

    private static void AddCards(Player player, string cardName, int count)
    {
        if (player == null)
        {
            Debug.LogError($"Cannot add '{cardName}' cards because target player is null.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Card createdCard = CardFactory.Create(cardName);
            if (createdCard == null)
            {
                Debug.LogError($"Skipping invalid card '{cardName}' while building deck.");
                continue;
            }

            player.Deck.Add(createdCard);
        }
    }

    private static void AddCardData(List<CardData> deck, string cardName, int count)
    {
        CardData data = CardDatabase.GetCardData(cardName);
        if (data == null)
        {
            Debug.LogWarning($"Starter deck card '{cardName}' does not exist in CardDatabase.");
            return;
        }

        for (int i = 0; i < count; i++)
            deck.Add(data);
    }

    private static void AddDeckEntries(List<CardData> deck, DeckEntry[] entries)
    {
        foreach (DeckEntry entry in entries)
            AddCardData(deck, entry.CardName, entry.Count);
    }

    private static DeckEntry[] GetBeginnerDeckEntries(string color)
    {
        switch (color.ToLowerInvariant())
        {
            case "white":
                return new[]
                {
                    new DeckEntry("Plains", 16), new DeckEntry("Eager Cadet", 3), new DeckEntry("Glory Seeker", 3),
                    new DeckEntry("Angelic Wall", 3), new DeckEntry("Yotian Soldier", 3), new DeckEntry("Abbey Griffin", 2),
                    new DeckEntry("Foot Soldiers", 2), new DeckEntry("Serra Angel", 1), new DeckEntry("Charge", 3),
                    new DeckEntry("Sacred Nectar", 2), new DeckEntry("Pacifism", 1), new DeckEntry("Demystify", 1),
                };
            case "blue":
                return new[]
                {
                    new DeckEntry("Island", 16), new DeckEntry("Fugitive Wizard", 3), new DeckEntry("Coral Eel", 3),
                    new DeckEntry("Sea Eagle", 3), new DeckEntry("Wind Drake", 3), new DeckEntry("Fighting Drake", 2),
                    new DeckEntry("Giant Octopus", 2), new DeckEntry("Unsummon", 3), new DeckEntry("Mahamoti Djinn", 1),
                    new DeckEntry("Inertia Bubble", 2), new DeckEntry("Icy Manipulator", 1), new DeckEntry("Flight", 1),
                };
            case "black":
                return new[]
                {
                    new DeckEntry("Swamp", 16), new DeckEntry("Maggot Carrier", 3), new DeckEntry("Bog Imp", 3),
                    new DeckEntry("Cyclopean Mummy", 3), new DeckEntry("Frozen Shade", 3), new DeckEntry("Giant Cockroach", 2),
                    new DeckEntry("Scavenging Scarab", 2), new DeckEntry("Nightmare", 1), new DeckEntry("Phyrexian Hulk", 1),
                    new DeckEntry("Terror", 2), new DeckEntry("Feast of the Unicorn", 1), new DeckEntry("Raise Dead", 3),
                };
            case "red":
                return new[]
                {
                    new DeckEntry("Mountain", 16), new DeckEntry("Goblin Sky Raider", 3), new DeckEntry("Crazed Goblin", 3),
                    new DeckEntry("Wall of Earth", 3), new DeckEntry("Shock", 3), new DeckEntry("Goblin Rider", 3),
                    new DeckEntry("Shatter", 2), new DeckEntry("Shivan Dragon", 1), new DeckEntry("Anaba Shaman", 2),
                    new DeckEntry("Hill Giant", 2), new DeckEntry("Granite Grip", 1), new DeckEntry("Jayamdae Tome", 1)
                };
            case "green":
                return new[]
                {
                    new DeckEntry("Forest", 17), new DeckEntry("Cactusaurus", 2), new DeckEntry("Domestic Cat", 3),
                    new DeckEntry("Deepwood Monkeys", 3), new DeckEntry("Violent Ape", 3), new DeckEntry("Crazy Cat Lady", 1),
                    new DeckEntry("Flying Donkey", 2), new DeckEntry("Realms Crasher", 1), new DeckEntry("Mana Rock", 1),
                    new DeckEntry("Gorilla Chief", 1), new DeckEntry("Touch Grass", 2), new DeckEntry("Whip of Thorns", 1),
                    new DeckEntry("Woodskin", 1), new DeckEntry("Muscle Blast", 1), new DeckEntry("Potion of Strength", 1)
                };
            default:
                Debug.LogWarning($"Unknown starter color '{color}'. Falling back to Red beginner deck.");
                return GetBeginnerDeckEntries("red");
        }
    }
    public static void BuildStartingDeck(Player player)
    {
        if (player == null)
        {
            Debug.LogError("Cannot build starting deck because player is null.");
            return;
        }

        player.Deck.Clear();

        if (DeckHolder.SelectedDeck != null && DeckHolder.SelectedDeck.Count > 0)
        {
            foreach (CardData data in DeckHolder.SelectedDeck)
            {
                if (data == null)
                {
                    Debug.LogWarning("Skipping null deck entry in selected deck.");
                    continue;
                }

                AddCards(player, data.cardName, 1);
            }
        }
        else
        {
            Debug.LogWarning("No deck found in DeckHolder. Using fallback test deck.");
            // fallback to your hardcoded test deck if needed
            AddCards(player, "Plains", 1);
            AddCards(player, "Angry Farmer", 1);
            // etc.
        }
    }

    public static List<CardData> BuildPlayerStarterDeck(string color)
    {
        string normalized = string.IsNullOrWhiteSpace(color) ? "Red" : color.Trim();
        List<CardData> starterDeck = new List<CardData>();
        AddDeckEntries(starterDeck, GetBeginnerDeckEntries(normalized));

        return starterDeck;
    }

    public static void BuildStarterDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Plains", 16);
            AddCards(ai, "Obstacle", 3);
            AddCards(ai, "Angry Farmer", 3);
            AddCards(ai, "Waterbearer", 3);
            AddCards(ai, "For Glory", 2);
            AddCards(ai, "Sacred Horn Nectar", 3);
            AddCards(ai, "Virgins Procession", 3);
            AddCards(ai, "Skyhunter Unicorn", 2);
            AddCards(ai, "Crystallium", 3);
            AddCards(ai, "Mana Rock", 2);
        }

    public static void BuildWhiteBeginnerDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Plains", 16);
            AddCards(ai, "Angry Farmer", 3);
            AddCards(ai, "Waterbearer", 3);
            AddCards(ai, "Iconoclast Monk", 2);
            AddCards(ai, "Gallant Lord", 2);
            AddCards(ai, "Gentle Giant", 2);
            AddCards(ai, "Hamlet Recruiter", 2);
            AddCards(ai, "Skyhunter Unicorn", 2);
            AddCards(ai, "Solid Prayer", 2);
            AddCards(ai, "Beasthunter", 2);
            AddCards(ai, "Sacred Horn Nectar", 2);
            AddCards(ai, "Bonfire", 2);
        }

    public static void BuildWhiteAdvancedDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Plains", 16);
            AddCards(ai, "Waterbearer", 3);
            AddCards(ai, "Faith Incarnate", 1);
            AddCards(ai, "Iconoclast Monk", 3);
            AddCards(ai, "Solid Prayer", 2);
            AddCards(ai, "Gallant Lord", 2);
            AddCards(ai, "Realm Protector", 2);
            AddCards(ai, "Untamed Unicorn", 1);
            AddCards(ai, "Hamlet Recruiter", 2);
            AddCards(ai, "Skyhunter Unicorn", 2);
            AddCards(ai, "Pure Angel", 1);
            AddCards(ai, "Bonfire", 2);
            AddCards(ai, "Virgins Procession", 3);
        }

    public static void BuildBlueBeginnerDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Island", 16);
            AddCards(ai, "Lucky Fisherman", 4);
            AddCards(ai, "Giant Crab", 3);
            AddCards(ai, "Wandering Squid", 3);
            AddCards(ai, "Wandering Cloud", 3);
            AddCards(ai, "Sharkmen Tribe", 3);
            AddCards(ai, "Colossal Octopus", 2);
            AddCards(ai, "Mana Rock", 2);
            AddCards(ai, "Crystallium", 2);
            AddCards(ai, "Blast of Knowledge", 2);
        }

    public static void BuildBlueAdvancedDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Island", 17);
            AddCards(ai, "Lucky Fisherman", 4);
            AddCards(ai, "Wisdom Incarnate", 1);
            AddCards(ai, "Tide Spirit", 3);
            AddCards(ai, "Wandering Squid", 2);
            AddCards(ai, "Giant Crab", 1);
            AddCards(ai, "Wandering Cloud", 2);
            AddCards(ai, "Sharkmen Tribe", 3);
            AddCards(ai, "Colossal Octopus", 2);
            AddCards(ai, "Skyward Whale", 2);
            AddCards(ai, "Blast of Knowledge", 3);
            AddCards(ai, "Replicator", 1);
        }

    public static void BuildBlackBeginnerDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Swamp", 16);
            AddCards(ai, "Limping Corpse", 3);
            AddCards(ai, "Famished Crow", 3);
            AddCards(ai, "Ratbat", 3);
            AddCards(ai, "Forced Mummification", 2);
            AddCards(ai, "Giant Rat", 3);
            AddCards(ai, "Bog Mosquito", 2);
            AddCards(ai, "Forget", 3);
            AddCards(ai, "Rotting Whale", 2);
            AddCards(ai, "Flayed Deer", 3);
        }

    public static void BuildBlackAdvancedDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Swamp", 16);
            AddCards(ai, "Ratbat", 3);
            AddCards(ai, "Bog Mosquito", 3);
            AddCards(ai, "Wicked Witch", 3);
            AddCards(ai, "Witches Rite", 3);
            AddCards(ai, "Communed Rot", 3);
            AddCards(ai, "Stone of Plague", 3);
            AddCards(ai, "Lights Out", 2);
            AddCards(ai, "Death Incarnate", 1);
            AddCards(ai, "Massacre", 1);
            AddCards(ai, "Possessed Innocent", 1);
            AddCards(ai, "Rotting Dragon", 1);
        }

    public static void BuildRedBeginnerDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Mountain", 16);
            AddCards(ai, "Village Idiot", 3);
            AddCards(ai, "Rabid Dog", 3);
            AddCards(ai, "Fire Hatchet", 2);
            AddCards(ai, "Great Boulder", 2);
            AddCards(ai, "Explosion", 2);
            AddCards(ai, "Goblin Puncher", 2);
            AddCards(ai, "Melt", 2);
            AddCards(ai, "Flying Pig", 3);
            AddCards(ai, "To Dig a Hole", 1);
            AddCards(ai, "Crystallium", 2);
            AddCards(ai, "Wild Ostrich", 2);
        }

    public static void BuildRedAdvancedDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Mountain", 16);
            AddCards(ai, "Rabid Dog", 3);
            AddCards(ai, "Wild Ostrich", 3);
            AddCards(ai, "Fire Hatchet", 2);
            AddCards(ai, "Explosion", 3);
            AddCards(ai, "Thundermare", 3);
            AddCards(ai, "Fire Spirals", 2);
            AddCards(ai, "Fireborn Dragon", 2);
            AddCards(ai, "Dragon Summoner", 2);
            AddCards(ai, "Potion of Knowledge", 1);
            AddCards(ai, "Thunderstrike", 1);
            AddCards(ai, "War Incarnate", 1);
            AddCards(ai, "Melt", 1);
        }

    public static void BuildGreenBeginnerDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Forest", 16);
            AddCards(ai, "Wall of Roots", 3);
            AddCards(ai, "Domestic Cat", 3);
            AddCards(ai, "Deepwood Monkeys", 3);
            AddCards(ai, "Violent Ape", 3);
            AddCards(ai, "Living Tree", 3);
            AddCards(ai, "Flying Donkey", 3);
            AddCards(ai, "Feast", 3);
            AddCards(ai, "Mana Rock", 3);
        }

    public static void BuildGreenAdvancedDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Forest", 16);
            AddCards(ai, "Wall of Roots", 3);
            AddCards(ai, "Deepwood Monkeys", 3);
            AddCards(ai, "Violent Ape", 3);
            AddCards(ai, "Crazy Cat Lady", 3);
            AddCards(ai, "Drumming Elf", 2);
            AddCards(ai, "Cactusaurus", 2);
            AddCards(ai, "Realms Crasher", 2);
            AddCards(ai, "Slack Tungo", 2);
            AddCards(ai, "Nature Incarnate", 1);
            AddCards(ai, "River Crocodile", 2);
            AddCards(ai, "Muscle Blast", 1);
        }

    public static void BuildBossDeck(Player ai)
    {
        ai.Deck.Clear();
        AddCards(ai, "Swamp", 8);
        AddCards(ai, "Plains", 8);
        AddCards(ai, "Waterbearer", 4);
        AddCards(ai, "Ratbat", 3);
        AddCards(ai, "Death Incarnate", 2);
        AddCards(ai, "Faith Incarnate", 2);
        AddCards(ai, "Lights Out", 2);
        AddCards(ai, "Possessed Innocent", 2);
        AddCards(ai, "Pure Angel", 1);
        AddCards(ai, "The Worlds Evil", 1);
        AddCards(ai, "Massacre", 1);
        AddCards(ai, "Afterlife Jinx Lantern", 1);
        AddCards(ai, "Giant Bat", 2);
        ai.StartingPermanents.Add(CardFactory.Create("Lich Queen"));
        ai.StartingPermanents.Add(CardFactory.Create("Dump People"));
        ai.StartingPermanents.Add(CardFactory.Create("Dump People"));
        }

    public static void BuildRuinsDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Island", 8);
            AddCards(ai, "Plains", 8);
            AddCards(ai, "Obstacle", 4);
            AddCards(ai, "Sphynx Lynx", 3);
            AddCards(ai, "Glassmole", 3);
            AddCards(ai, "Origin Golem", 3);
            AddCards(ai, "Omega Golemoid", 2);
            AddCards(ai, "Mana Rock", 2);
            AddCards(ai, "Progress Incarnate", 1);
            AddCards(ai, "Potion of Mana", 1);
            AddCards(ai, "Potion of Knowledge", 1);
            AddCards(ai, "Potion of Lava", 1);
            AddCards(ai, "Trinkets Collector", 1);
            AddCards(ai, "Blast of Knowledge", 1);
            ai.StartingPermanents.Add(CardFactory.Create("Pressure Sphere"));
        }

        /*public static void BuildPlayerStarterWhite(Player player)
        {
            player.Deck.Clear();
            AddCards(player, "Plains", 17);

            //Creature
            AddCards(player, "Angry Farmer", 3);
            AddCards(player, "Waterbearer", 3);
            AddCards(player, "Gallant Lord", 3);
            AddCards(player, "Skyhunter Unicorn", 2);
            AddCards(player, "Virgins Procession", 2);
            AddCards(player, "Beasthunter", 1);
            AddCards(player, "Realm Protector", 1);
            AddCards(player, "Untamed Unicorn", 1);

            AddCards(player, "Sacred Horn Nectar", 2);
            AddCards(player, "Blinding Light", 1);
            AddCards(player, "Faith Protection", 1);
            AddCards(player, "Sacred Horn", 1);
            AddCards(player, "For Glory", 1);

            AddCards(player, "Potion of Knowledge", 1);
        }

        public static void BuildPlayerStarterBlue(Player player)
        {
            player.Deck.Clear();
            AddCards(player, "Island", 17);

            //Creature
            AddCards(player, "Arcane Barrier", 3);
            AddCards(player, "Lucky Fisherman", 3);
            AddCards(player, "Deepwood Owl", 3);
            AddCards(player, "Giant Crab", 2);
            AddCards(player, "Sharkmen Tribe", 2);
            AddCards(player, "Tide Spirit", 1);
            AddCards(player, "Autonomous Miner", 1);
            AddCards(player, "Cosmic Whale", 1);

            //Spells
            AddCards(player, "Blast of Knowledge", 2);
            AddCards(player, "Cut Off Hands", 1);
            AddCards(player, "Sleep", 1);
            AddCards(player, "Starpowder", 1);
            AddCards(player, "Fascinate", 1);

            //Artifacts
            AddCards(player, "Potion of Lava", 1);
        }

        public static void BuildPlayerStarterBlack(Player player)
        {
            player.Deck.Clear();
            AddCards(player, "Swamp", 17);

            //Creature
            AddCards(player, "Famished Crow", 3);
            AddCards(player, "Limping Corpse", 3);
            AddCards(player, "Giant Rat", 3);
            AddCards(player, "Bog Mosquito", 2);
            AddCards(player, "Giant Bat", 2);
            AddCards(player, "Rotting Dragon", 1);
            AddCards(player, "Undead Army", 1);
            AddCards(player, "Possessed Innocent", 1);

            //Spells
            AddCards(player, "Filth Discharge", 2);
            AddCards(player, "Sickness", 1);
            AddCards(player, "Forget", 1);
            AddCards(player, "Lights Out", 1);
            AddCards(player, "Mirror Break", 1);

            //Artifacts
            AddCards(player, "Potion of Health", 1);
        }

        public static void BuildPlayerStarterRed(Player player)
        {
            player.Deck.Clear();
            AddCards(player, "Mountain", 17);

            //Creature
            AddCards(player, "Rabid Dog", 3);
            AddCards(player, "Great Boulder", 3);
            AddCards(player, "Goblin Puncher", 3);
            AddCards(player, "Scarred Wildboar", 2);
            AddCards(player, "Iron Skyman", 2);
            AddCards(player, "Spitfire Cobrox", 1);
            AddCards(player, "Wild Ostrich", 1);
            AddCards(player, "Fireborn Dragon", 1);

            //Spells
            AddCards(player, "Explosion", 2);
            AddCards(player, "Devouring Fury", 1);
            AddCards(player, "Dash", 1);
            AddCards(player, "Thunderstrike", 1);
            AddCards(player, "Fire Spirals", 1);

            //Artifacts
            AddCards(player, "Potion of Mana", 1);
        }

        public static void BuildPlayerStarterGreen(Player player)
        {
            player.Deck.Clear();
            AddCards(player, "Forest", 17);

            //Creature
            AddCards(player, "Domestic Cat", 3);
            AddCards(player, "Deepwood Monkeys", 3);
            AddCards(player, "Violent Ape", 3);
            AddCards(player, "Flying Donkey", 2);
            AddCards(player, "Cactusaurus", 2);
            AddCards(player, "Crazy Cat Lady", 1);
            AddCards(player, "Realms Crasher", 1);
            AddCards(player, "Gorilla Chief", 1);

            //Spells
            AddCards(player, "Touch Grass", 2);
            AddCards(player, "Whip of Thorns", 1);
            AddCards(player, "Woodskin", 1);
            AddCards(player, "Mana Rock", 1);
            AddCards(player, "Muscleblast", 1);

            //Artifacts
            AddCards(player, "Potion of Might", 1);
        }*/
}
