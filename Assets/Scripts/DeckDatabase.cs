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
                    new DeckEntry("Wall of Earth", 3), new DeckEntry("Shock", 3), new DeckEntry("Goblin Raider", 3),
                    new DeckEntry("Shatter", 2), new DeckEntry("Shivan Dragon", 1), new DeckEntry("Anaba Shaman", 2),
                    new DeckEntry("Hill Giant", 2), new DeckEntry("Granite Grip", 1), new DeckEntry("Jayemdae Tome", 1)
                };
            case "green":
                return new[]
                {
                    new DeckEntry("Forest", 16), new DeckEntry("Argothian Swine", 2), new DeckEntry("Wall of Wood", 3),
                    new DeckEntry("Canopy Spider", 3), new DeckEntry("Trained Armodon", 3), new DeckEntry("Grizzly Bears", 3),
                    new DeckEntry("Rampant Growth", 2), new DeckEntry("Oakenform", 1), new DeckEntry("Might of Oaks", 1),
                    new DeckEntry("Fountain of Youth", 1), new DeckEntry("Giant Spider", 2), new DeckEntry("Giant Growth", 3)
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


    private static void BuildDeckFromEntries(Player player, DeckEntry[] entries)
    {
        if (player == null)
        {
            Debug.LogError("Cannot build deck because player is null.");
            return;
        }

        player.Deck.Clear();

        foreach (DeckEntry entry in entries)
            AddCards(player, entry.CardName, entry.Count);
    }

    public static void BuildBeginnerDeck(Player player, string color)
    {
        string normalized = string.IsNullOrWhiteSpace(color) ? "red" : color.Trim();
        BuildDeckFromEntries(player, GetBeginnerDeckEntries(normalized));
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
            AddCards(ai, "Plains", 20);
            AddCards(ai, "Eager Cadet", 40);
        }

    public static void BuildFarmerDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Plains", 17);
            AddCards(ai, "Eager Cadet", 3);
            AddCards(ai, "Charge", 4);
            AddCards(ai, "Resupply", 4);
            AddCards(ai, "Sacred Nectar", 4);
            AddCards(ai, "Glory Seeker", 4);
            AddCards(ai, "Voice of the Provinces", 3);
            ai.StartingPermanents.Add(CardFactory.Create("Eager Cadet"));
        }
    
    public static void BuildGuardDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Plains", 17);
            AddCards(ai, "Eager Cadet", 3);
            AddCards(ai, "Charge", 3);
            AddCards(ai, "Foot Soldiers", 3);
            AddCards(ai, "Short Sword", 3);
            AddCards(ai, "Glory Seeker", 3);
            AddCards(ai, "Alaborn Trooper", 3);
            AddCards(ai, "Capashen Templar", 3);
            AddCards(ai, "Wall of Swords", 1);
            ai.StartingPermanents.Add(CardFactory.Create("Short Sword"));
        }
    
    public static void BuildMonkDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Plains", 17);
            AddCards(ai, "Venerable Monk", 3);
            AddCards(ai, "Pacifism", 4);
            AddCards(ai, "Angelic Wall", 4);
            AddCards(ai, "Sacred Nectar", 4);
            AddCards(ai, "Holy Day", 4);
            AddCards(ai, "Angel of Mercy", 3);
            ai.StartingPermanents.Add(CardFactory.Create("Venerable Monk"));
        }

    public static void BuildCorpseDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Swamp", 17);
            AddCards(ai, "Maggot Carrier", 4);
            AddCards(ai, "Scathe Zombies", 3);
            AddCards(ai, "Scavenging Scarab", 4);
            AddCards(ai, "Giant Cockroach", 4);
            AddCards(ai, "Raise Dead", 4);
            AddCards(ai, "Unholy Strength", 3);
            ai.StartingPermanents.Add(CardFactory.Create("Scathe Zombies"));
        }
    
    public static void BuildFisherDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Island", 17);
            AddCards(ai, "Coral Eel", 4);
            AddCards(ai, "Sea Eagle", 4);
            AddCards(ai, "Giant Octopus", 4);
            AddCards(ai, "Ancient Carp", 4);
            AddCards(ai, "Killer Whale", 4);
            AddCards(ai, "Sea Monster", 3);
        }
    
    public static void BuildGipsyDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Island", 17);
            AddCards(ai, "Fugitive Wizard", 4);
            AddCards(ai, "Merchant of Secrets", 4);
            AddCards(ai, "Wind Drake", 4);
            AddCards(ai, "Inspiration", 3);
            AddCards(ai, "Unsummon", 4);
            AddCards(ai, "Flight", 3);
        }

    public static void BuildBarbariansDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Mountain", 17);
            AddCards(ai, "Balduvian Barbarians", 3);
            AddCards(ai, "Wall of Earth", 4);
            AddCards(ai, "Granite Grip", 4);
            AddCards(ai, "Conquer", 4);
            AddCards(ai, "Accelerate", 4);
            AddCards(ai, "Marauder's Axe", 3);
            ai.StartingPermanents.Add(CardFactory.Create("Balduvian Barbarians"));
        }

    public static void BuildScimitarDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Plains", 17);
            AddCards(ai, "Dancing Scimitar", 3);
            AddCards(ai, "Short Sword", 4);
            AddCards(ai, "Ogre's Cleaver", 4);
            AddCards(ai, "Greatsword", 4);
            AddCards(ai, "Wall of Swords", 4);
            AddCards(ai, "Kite Shield", 3);
            ai.StartingPermanents.Add(CardFactory.Create("Dancing Scimitar"));
        }

    public static void BuildOldWomanDruidDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Forest", 17);
            AddCards(ai, "Grizzly Bears", 3);
            AddCards(ai, "Canopy Spider", 3);
            AddCards(ai, "Wall of Ice", 2);
            AddCards(ai, "Argothian Swine", 3);
            AddCards(ai, "Dosan's Oldest Chant", 3);
            AddCards(ai, "Rampant Growth", 4);
            AddCards(ai, "Oakenform", 3);
            AddCards(ai, "Craw Wurm", 2);
        }
    public static void BuildPhantomWarriorDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Island", 17);
            AddCards(ai, "Phantom Warrior", 3);
            AddCards(ai, "Wall of Air", 4);
            AddCards(ai, "Unsummon", 4);
            AddCards(ai, "Dehydratation", 4);
            AddCards(ai, "Spirit Away", 1);
            AddCards(ai, "Air Elemental", 4);
            AddCards(ai, "Greatsword", 2);
            ai.StartingPermanents.Add(CardFactory.Create("Phantom Warrior"));
        }
    
    public static void BuildArchivistDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Island", 21);
            AddCards(ai, "Yotian Soldier", 4);
            AddCards(ai, "Fugitive Wizard", 4);
            AddCards(ai, "Merchant of Secrets", 4);
            AddCards(ai, "Inertia Bubble", 3);
            AddCards(ai, "Juggernaut", 4);
            AddCards(ai, "Glass Golem", 4);
            AddCards(ai, "Ancient Carp", 3);
            AddCards(ai, "Meteorite", 4);
            AddCards(ai, "Jayemdae Tome", 2);
            AddCards(ai, "Inspiration", 4);
            AddCards(ai, "Wind Drake", 2);
            ai.StartingPermanents.Add(CardFactory.Create("Archivist"));
        }
    
    public static void BuildRootsWallDeck(Player ai)
        {
            ai.Deck.Clear();
            AddCards(ai, "Forest", 21);
            AddCards(ai, "Wall of Wood", 22);
            AddCards(ai, "Wall of Blossoms", 4);
            AddCards(ai, "Squall", 4);
            AddCards(ai, "Battlegrowth", 4);
            AddCards(ai, "Oakenform", 4);
            ai.StartingPermanents.Add(CardFactory.Create("Wall of Wood"));
        }

    public static void BuildSoldiersAndGloryDeck(Player player)
    {
        ai.Deck.Clear();
        AddCards(ai, "Plains", 20);
        AddCards(ai, "Eager Cadet", 4);
        AddCards(ai, "Glory Seeker", 4);
        AddCards(ai, "Yotian Soldier", 3);
        AddCards(ai, "Alaborn Trooper", 4);
        AddCards(ai, "Capashen Templar", 3);
        AddCards(ai, "Intrepid Hero", 2);
        AddCards(ai, "Abbey Griffin", 2);
        AddCards(ai, "Serra Angel", 2);
        AddCards(ai, "Charge", 4);
        AddCards(ai, "Holy Day", 2);
        AddCards(ai, "Sacred Nectar", 2);
        AddCards(ai, "Pacifism", 3);
        AddCards(ai, "Demystify", 2);
        AddCards(ai, "Glorious Anthem", 2);
        AddCards(ai, "Holy Strength", 1);
    }

    public static void BuildGreenMachineDeck(Player player)
    {
        ai.Deck.Clear();
        AddCards(ai, "Forest", 22);
        AddCards(ai, "Wall of Wood", 3);
        AddCards(ai, "Canopy Spider", 3);
        AddCards(ai, "Grizzly Bears", 4);
        AddCards(ai, "Trained Armodon", 3);
        AddCards(ai, "Argothian Swine", 3);
        AddCards(ai, "Giant Spider", 2);
        AddCards(ai, "Moss Monster", 2);
        AddCards(ai, "Craw Wurm", 1);
        AddCards(ai, "Rampant Growth", 4);
        AddCards(ai, "Giant Growth", 4);
        AddCards(ai, "Battlegrowth", 3);
        AddCards(ai, "Oakenform", 3);
        AddCards(ai, "Squall", 2);
        AddCards(ai, "Might of Oaks", 1);
    }

    public static void BuildRedBlackAggroDeck(Player player)
    {
        ai.Deck.Clear();
        AddCards(ai, "Swamp", 13);
        AddCards(ai, "Mountain", 9);
        AddCards(ai, "Crazed Goblin", 4);
        AddCards(ai, "Bog Imp", 3);
        AddCards(ai, "Goblin Raider", 3);
        AddCards(ai, "Frozen Shade", 3);
        AddCards(ai, "Scavenging Scarab", 3);
        AddCards(ai, "Hill Giant", 2);
        AddCards(ai, "Nightmare", 2);
        AddCards(ai, "Anaba Shaman", 1);
        AddCards(ai, "Shock", 4);
        AddCards(ai, "Terror", 3);
        AddCards(ai, "Raise Dead", 3);
        AddCards(ai, "Unholy Strength", 3);
        AddCards(ai, "Feast of the Unicorn", 2);
        AddCards(ai, "Lava Axe", 2);
    }

    // Blue skies control deck. Bounces threats with Unsummon, locks permanents with Dehydration and
    // Icy Manipulator, then wins through evasive flyers. Merchant of Secrets and Inspiration keep
    // cards flowing. Air Elemental and Mahamoti Djinn close the game in the air.
    public static void BuildBlueSkiesDeck(Player ai)
    {
        ai.Deck.Clear();
        AddCards(ai, "Island", 22);
        AddCards(ai, "Fugitive Wizard", 3);
        AddCards(ai, "Sea Eagle", 3);
        AddCards(ai, "Merchant of Secrets", 3);
        AddCards(ai, "Wind Drake", 4);
        AddCards(ai, "Phantom Warrior", 2);
        AddCards(ai, "Fighting Drake", 3);
        AddCards(ai, "Air Elemental", 2);
        AddCards(ai, "Mahamoti Djinn", 1);
        AddCards(ai, "Unsummon", 4);
        AddCards(ai, "Inspiration", 3);
        AddCards(ai, "Flight", 2);
        AddCards(ai, "Dehydratation", 3);
        AddCards(ai, "Inertia Bubble", 2);
        AddCards(ai, "Icy Manipulator", 1);
    }

    // White life gain / token engine. Voice of the Provinces and Luminous Angel flood the board with
    // Spirit and Human tokens. Blessed Reversal punishes wide attacks. Wrath of God acts as a reset
    // when behind; Archangel's Light refills life and reshuffles the graveyard for a late-game engine.
    public static void BuildAngelicHostDeck(Player ai)
    {
        ai.Deck.Clear();
        AddCards(ai, "Plains", 22);
        AddCards(ai, "Venerable Monk", 3);
        AddCards(ai, "Angelic Wall", 3);
        AddCards(ai, "Angel of Mercy", 3);
        AddCards(ai, "Voice of the Provinces", 3);
        AddCards(ai, "Serra Angel", 2);
        AddCards(ai, "Luminous Angel", 1);
        AddCards(ai, "Archangel", 1);
        AddCards(ai, "Sacred Nectar", 3);
        AddCards(ai, "Resupply", 2);
        AddCards(ai, "Holy Day", 3);
        AddCards(ai, "Blessed Reversal", 3);
        AddCards(ai, "Pacifism", 3);
        AddCards(ai, "Wrath of God", 2);
        AddCards(ai, "Archangel's Light", 1);
        AddCards(ai, "Divine Transformation", 2);
        AddCards(ai, "Purify", 1);
    }

    // Black zombie horde with recursion. Maggot Carrier chips life on entry. Cyclopean Mummy and
    // Scathe Zombies apply constant pressure. Terror and Unholy Strength handle threats and pump.
    // Raise Dead recycles fallen threats repeatedly. Nightmare is the late-game bomb fed by Swamps.
    public static void BuildZombieHordeDeck(Player ai)
    {
        ai.Deck.Clear();
        AddCards(ai, "Swamp", 22);
        AddCards(ai, "Maggot Carrier", 4);
        AddCards(ai, "Cyclopean Mummy", 4);
        AddCards(ai, "Bog Imp", 3);
        AddCards(ai, "Scathe Zombies", 4);
        AddCards(ai, "Frozen Shade", 3);
        AddCards(ai, "Giant Cockroach", 3);
        AddCards(ai, "Phyrexian Hulk", 1);
        AddCards(ai, "Nightmare", 2);
        AddCards(ai, "Terror", 3);
        AddCards(ai, "Raise Dead", 4);
        AddCards(ai, "Unholy Strength", 3);
        AddCards(ai, "Feast of the Unicorn", 2);
        AddCards(ai, "Purify", 1);
    }

    // Red burn and land destruction. Shock, Lava Axe, and Inferno handle both creatures and the
    // player's life total. Stone Rain denies mana. Goblin Raider and Balduvian Barbarians apply
    // pressure while removal clears the way. Granite Grip rewards a Mountain-heavy mana base.
    // Shivan Dragon is the top-end finisher that also benefits from spare mana each turn.
    public static void BuildRedBurnDeck(Player ai)
    {
        ai.Deck.Clear();
        AddCards(ai, "Mountain", 22);
        AddCards(ai, "Crazed Goblin", 4);
        AddCards(ai, "Goblin Raider", 4);
        AddCards(ai, "Balduvian Barbarians", 3);
        AddCards(ai, "Goblin Sky Raider", 3);
        AddCards(ai, "Hill Giant", 2);
        AddCards(ai, "Anaba Shaman", 2);
        AddCards(ai, "Shivan Dragon", 1);
        AddCards(ai, "Shock", 4);
        AddCards(ai, "Accelerate", 3);
        AddCards(ai, "Stone Rain", 3);
        AddCards(ai, "Shatter", 2);
        AddCards(ai, "Granite Grip", 3);
        AddCards(ai, "Lava Axe", 2);
        AddCards(ai, "Inferno", 1);
    }

    // Green-White ramp into big creatures buffed by auras and equipment. Rampant Growth accelerates
    // into Moss Monster, Iron Tusk Elephant, and Staunch Defenders. Short Sword and Greatsword give
    // Trample creatures extra punch. Giant Growth and Might of Oaks are combat blowouts. Wrath of God
    // and Pacifism provide the removal White contributes to the pairing.
    public static void BuildGreenWhiteRampDeck(Player ai)
    {
        ai.Deck.Clear();
        AddCards(ai, "Forest", 12);
        AddCards(ai, "Plains", 10);
        AddCards(ai, "Grizzly Bears", 3);
        AddCards(ai, "Canopy Spider", 2);
        AddCards(ai, "Trained Armodon", 3);
        AddCards(ai, "Argothian Swine", 3);
        AddCards(ai, "Iron Tusk Elephant", 2);
        AddCards(ai, "Moss Monster", 2);
        AddCards(ai, "Staunch Defenders", 2);
        AddCards(ai, "Craw Wurm", 1);
        AddCards(ai, "Rampant Growth", 4);
        AddCards(ai, "Giant Growth", 3);
        AddCards(ai, "Might of Oaks", 1);
        AddCards(ai, "Oakenform", 2);
        AddCards(ai, "Pacifism", 2);
        AddCards(ai, "Holy Strength", 2);
        AddCards(ai, "Wrath of God", 1);
        AddCards(ai, "Short Sword", 2);
        AddCards(ai, "Greatsword", 1);
    }
}
