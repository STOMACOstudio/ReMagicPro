using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeckDatabase
{
    private static void AddCards(Player player, string cardName, int count)
        {
            for (int i = 0; i < count; i++)
            {
                player.Deck.Add(CardFactory.Create(cardName));
            }
        }
    public static void BuildStartingDeck(Player player)
        {
            if (DeckHolder.SelectedDeck != null && DeckHolder.SelectedDeck.Count > 0)
            {
                foreach (var data in DeckHolder.SelectedDeck)
                {
                    player.Deck.Add(CardFactory.Create(data.cardName));
                }
            }
            else
            {
                Debug.LogWarning("No deck found in DeckHolder. Using fallback test deck.");
                // fallback to your hardcoded test deck if needed
                player.Deck.Add(CardFactory.Create("Plains"));
                player.Deck.Add(CardFactory.Create("Angry Farmer"));
                // etc.
            }
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
            AddCards(ai, "Deep Forest Monkeys", 3);
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
            AddCards(ai, "Deep Forest Monkeys", 3);
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
            //AddCards(ai, "Pressure Sphere", 1);
            AddCards(ai, "Trinkets Collector", 1);
            AddCards(ai, "Blast of Knowledge", 1);
            ai.StartingPermanents.Add(CardFactory.Create("Pressure Sphere"));
        }
}
