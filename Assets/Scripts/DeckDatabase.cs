using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeckDatabase
{
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
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Obstacle"));
            ai.Deck.Add(CardFactory.Create("Obstacle"));
            ai.Deck.Add(CardFactory.Create("Obstacle"));
            ai.Deck.Add(CardFactory.Create("Angry Farmer"));
            ai.Deck.Add(CardFactory.Create("Angry Farmer"));
            ai.Deck.Add(CardFactory.Create("Angry Farmer"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("For Glory"));
            ai.Deck.Add(CardFactory.Create("For Glory"));
            ai.Deck.Add(CardFactory.Create("Candlelight"));
            ai.Deck.Add(CardFactory.Create("Candlelight"));
            ai.Deck.Add(CardFactory.Create("Candlelight"));
            ai.Deck.Add(CardFactory.Create("Virgins Procession"));
            ai.Deck.Add(CardFactory.Create("Virgins Procession"));
            ai.Deck.Add(CardFactory.Create("Virgins Procession"));
            ai.Deck.Add(CardFactory.Create("Skyhunter Unicorn"));
            ai.Deck.Add(CardFactory.Create("Skyhunter Unicorn"));
            ai.Deck.Add(CardFactory.Create("Crystallium"));
            ai.Deck.Add(CardFactory.Create("Crystallium"));
            ai.Deck.Add(CardFactory.Create("Crystallium"));
            ai.Deck.Add(CardFactory.Create("Mana Rock"));
            ai.Deck.Add(CardFactory.Create("Mana Rock"));
        }

    public static void BuildWhiteBeginnerDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Angry Farmer"));
            ai.Deck.Add(CardFactory.Create("Angry Farmer"));
            ai.Deck.Add(CardFactory.Create("Angry Farmer"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("Iconoclast Monk"));
            ai.Deck.Add(CardFactory.Create("Iconoclast Monk"));
            ai.Deck.Add(CardFactory.Create("Gallant Lord"));
            ai.Deck.Add(CardFactory.Create("Gallant Lord"));
            ai.Deck.Add(CardFactory.Create("Gentle Giant"));
            ai.Deck.Add(CardFactory.Create("Gentle Giant"));
            ai.Deck.Add(CardFactory.Create("Hamlet Recruiter"));
            ai.Deck.Add(CardFactory.Create("Hamlet Recruiter"));
            ai.Deck.Add(CardFactory.Create("Skyhunter Unicorn"));
            ai.Deck.Add(CardFactory.Create("Skyhunter Unicorn"));
            ai.Deck.Add(CardFactory.Create("Solid Prayer"));
            ai.Deck.Add(CardFactory.Create("Solid Prayer"));
            ai.Deck.Add(CardFactory.Create("Beasthunter"));
            ai.Deck.Add(CardFactory.Create("Beasthunter"));
            ai.Deck.Add(CardFactory.Create("Candlelight"));
            ai.Deck.Add(CardFactory.Create("Candlelight"));
            ai.Deck.Add(CardFactory.Create("Bonfire"));
            ai.Deck.Add(CardFactory.Create("Bonfire"));
        }

    public static void BuildWhiteAdvancedDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("Faith Incarnate"));
            ai.Deck.Add(CardFactory.Create("Iconoclast Monk"));
            ai.Deck.Add(CardFactory.Create("Iconoclast Monk"));
            ai.Deck.Add(CardFactory.Create("Iconoclast Monk"));
            ai.Deck.Add(CardFactory.Create("Solid Prayer"));
            ai.Deck.Add(CardFactory.Create("Solid Prayer"));
            ai.Deck.Add(CardFactory.Create("Gallant Lord"));
            ai.Deck.Add(CardFactory.Create("Gallant Lord"));
            ai.Deck.Add(CardFactory.Create("Realm Protector"));
            ai.Deck.Add(CardFactory.Create("Realm Protector"));
            ai.Deck.Add(CardFactory.Create("Untamed Unicorn"));
            ai.Deck.Add(CardFactory.Create("Hamlet Recruiter"));
            ai.Deck.Add(CardFactory.Create("Hamlet Recruiter"));
            ai.Deck.Add(CardFactory.Create("Skyhunter Unicorn"));
            ai.Deck.Add(CardFactory.Create("Skyhunter Unicorn"));
            ai.Deck.Add(CardFactory.Create("Pure Angel"));
            ai.Deck.Add(CardFactory.Create("Bonfire"));
            ai.Deck.Add(CardFactory.Create("Bonfire"));
            ai.Deck.Add(CardFactory.Create("Virgins Procession"));
            ai.Deck.Add(CardFactory.Create("Virgins Procession"));
            ai.Deck.Add(CardFactory.Create("Virgins Procession"));
        }

    public static void BuildBlueBeginnerDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Lucky Fisherman"));
            ai.Deck.Add(CardFactory.Create("Lucky Fisherman"));
            ai.Deck.Add(CardFactory.Create("Lucky Fisherman"));
            ai.Deck.Add(CardFactory.Create("Lucky Fisherman"));
            ai.Deck.Add(CardFactory.Create("Giant Crab"));
            ai.Deck.Add(CardFactory.Create("Giant Crab"));
            ai.Deck.Add(CardFactory.Create("Giant Crab"));
            ai.Deck.Add(CardFactory.Create("Wandering Squid"));
            ai.Deck.Add(CardFactory.Create("Wandering Squid"));
            ai.Deck.Add(CardFactory.Create("Wandering Squid"));
            ai.Deck.Add(CardFactory.Create("Wandering Cloud"));
            ai.Deck.Add(CardFactory.Create("Wandering Cloud"));
            ai.Deck.Add(CardFactory.Create("Wandering Cloud"));
            ai.Deck.Add(CardFactory.Create("Sharkmen Tribe"));
            ai.Deck.Add(CardFactory.Create("Sharkmen Tribe"));
            ai.Deck.Add(CardFactory.Create("Sharkmen Tribe"));
            ai.Deck.Add(CardFactory.Create("Colossal Octopus"));
            ai.Deck.Add(CardFactory.Create("Colossal Octopus"));
            ai.Deck.Add(CardFactory.Create("Mana Rock"));
            ai.Deck.Add(CardFactory.Create("Mana Rock"));
            ai.Deck.Add(CardFactory.Create("Crystallium"));
            ai.Deck.Add(CardFactory.Create("Crystallium"));
            ai.Deck.Add(CardFactory.Create("Blast of Knowledge"));
            ai.Deck.Add(CardFactory.Create("Blast of Knowledge"));
        }

    public static void BuildBlueAdvancedDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Lucky Fisherman"));
            ai.Deck.Add(CardFactory.Create("Lucky Fisherman"));
            ai.Deck.Add(CardFactory.Create("Lucky Fisherman"));
            ai.Deck.Add(CardFactory.Create("Lucky Fisherman"));
            ai.Deck.Add(CardFactory.Create("Wisdom Incarnate"));
            ai.Deck.Add(CardFactory.Create("Tide Spirit"));
            ai.Deck.Add(CardFactory.Create("Tide Spirit"));
            ai.Deck.Add(CardFactory.Create("Tide Spirit"));
            ai.Deck.Add(CardFactory.Create("Wandering Squid"));
            ai.Deck.Add(CardFactory.Create("Wandering Squid"));
            ai.Deck.Add(CardFactory.Create("Giant Crab"));
            ai.Deck.Add(CardFactory.Create("Wandering Cloud"));
            ai.Deck.Add(CardFactory.Create("Wandering Cloud"));
            ai.Deck.Add(CardFactory.Create("Sharkmen Tribe"));
            ai.Deck.Add(CardFactory.Create("Sharkmen Tribe"));
            ai.Deck.Add(CardFactory.Create("Sharkmen Tribe"));
            ai.Deck.Add(CardFactory.Create("Colossal Octopus"));
            ai.Deck.Add(CardFactory.Create("Colossal Octopus"));
            ai.Deck.Add(CardFactory.Create("Skyward Whale"));
            ai.Deck.Add(CardFactory.Create("Skyward Whale"));
            ai.Deck.Add(CardFactory.Create("Blast of Knowledge"));
            ai.Deck.Add(CardFactory.Create("Blast of Knowledge"));
            ai.Deck.Add(CardFactory.Create("Blast of Knowledge"));
            ai.Deck.Add(CardFactory.Create("Replicator"));
        }

    public static void BuildBlackBeginnerDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Limping Corpse"));
            ai.Deck.Add(CardFactory.Create("Limping Corpse"));
            ai.Deck.Add(CardFactory.Create("Limping Corpse"));
            ai.Deck.Add(CardFactory.Create("Famished Crow"));
            ai.Deck.Add(CardFactory.Create("Famished Crow"));
            ai.Deck.Add(CardFactory.Create("Famished Crow"));
            ai.Deck.Add(CardFactory.Create("Ratbat"));
            ai.Deck.Add(CardFactory.Create("Ratbat"));
            ai.Deck.Add(CardFactory.Create("Ratbat"));
            ai.Deck.Add(CardFactory.Create("Forced Mummification"));
            ai.Deck.Add(CardFactory.Create("Forced Mummification"));
            ai.Deck.Add(CardFactory.Create("Giant Rat"));
            ai.Deck.Add(CardFactory.Create("Giant Rat"));
            ai.Deck.Add(CardFactory.Create("Giant Rat"));
            ai.Deck.Add(CardFactory.Create("Bog Mosquito"));
            ai.Deck.Add(CardFactory.Create("Bog Mosquito"));
            ai.Deck.Add(CardFactory.Create("Forget"));
            ai.Deck.Add(CardFactory.Create("Forget"));
            ai.Deck.Add(CardFactory.Create("Forget"));
            ai.Deck.Add(CardFactory.Create("Rotting Whale"));
            ai.Deck.Add(CardFactory.Create("Rotting Whale"));
            ai.Deck.Add(CardFactory.Create("Flayed Deer"));
            ai.Deck.Add(CardFactory.Create("Flayed Deer"));
            ai.Deck.Add(CardFactory.Create("Flayed Deer"));
        }

    public static void BuildBlackAdvancedDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Ratbat"));
            ai.Deck.Add(CardFactory.Create("Ratbat"));
            ai.Deck.Add(CardFactory.Create("Ratbat"));
            ai.Deck.Add(CardFactory.Create("Bog Mosquito"));
            ai.Deck.Add(CardFactory.Create("Bog Mosquito"));
            ai.Deck.Add(CardFactory.Create("Bog Mosquito"));
            ai.Deck.Add(CardFactory.Create("Wicked Witch"));
            ai.Deck.Add(CardFactory.Create("Wicked Witch"));
            ai.Deck.Add(CardFactory.Create("Wicked Witch"));
            ai.Deck.Add(CardFactory.Create("Witches Rite"));
            ai.Deck.Add(CardFactory.Create("Witches Rite"));
            ai.Deck.Add(CardFactory.Create("Witches Rite"));
            ai.Deck.Add(CardFactory.Create("Communed Rot"));
            ai.Deck.Add(CardFactory.Create("Communed Rot"));
            ai.Deck.Add(CardFactory.Create("Communed Rot"));
            ai.Deck.Add(CardFactory.Create("Stone of Plague"));
            ai.Deck.Add(CardFactory.Create("Stone of Plague"));
            ai.Deck.Add(CardFactory.Create("Stone of Plague"));
            ai.Deck.Add(CardFactory.Create("Lights Out"));
            ai.Deck.Add(CardFactory.Create("Lights Out"));
            ai.Deck.Add(CardFactory.Create("Death Incarnate"));
            ai.Deck.Add(CardFactory.Create("Massacre"));
            ai.Deck.Add(CardFactory.Create("Possessed Innocent"));
            ai.Deck.Add(CardFactory.Create("Rotting Dragon"));
        }

    public static void BuildRedBeginnerDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Village Idiot"));
            ai.Deck.Add(CardFactory.Create("Village Idiot"));
            ai.Deck.Add(CardFactory.Create("Village Idiot"));
            ai.Deck.Add(CardFactory.Create("Rabid Dog"));
            ai.Deck.Add(CardFactory.Create("Rabid Dog"));
            ai.Deck.Add(CardFactory.Create("Rabid Dog"));
            ai.Deck.Add(CardFactory.Create("Fire Hatchet"));
            ai.Deck.Add(CardFactory.Create("Fire Hatchet"));
            ai.Deck.Add(CardFactory.Create("Great Boulder"));
            ai.Deck.Add(CardFactory.Create("Great Boulder"));
            ai.Deck.Add(CardFactory.Create("Explosion"));
            ai.Deck.Add(CardFactory.Create("Explosion"));
            ai.Deck.Add(CardFactory.Create("Goblin Puncher"));
            ai.Deck.Add(CardFactory.Create("Goblin Puncher"));
            ai.Deck.Add(CardFactory.Create("Melt"));
            ai.Deck.Add(CardFactory.Create("Melt"));
            ai.Deck.Add(CardFactory.Create("Flying Pig"));
            ai.Deck.Add(CardFactory.Create("Flying Pig"));
            ai.Deck.Add(CardFactory.Create("Flying Pig"));
            ai.Deck.Add(CardFactory.Create("To Dig a Hole"));
            ai.Deck.Add(CardFactory.Create("Crystallium"));
            ai.Deck.Add(CardFactory.Create("Crystallium"));
            ai.Deck.Add(CardFactory.Create("Wild Ostrich"));
            ai.Deck.Add(CardFactory.Create("Wild Ostrich"));
        }

    public static void BuildRedAdvancedDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Mountain"));
            ai.Deck.Add(CardFactory.Create("Rabid Dog"));
            ai.Deck.Add(CardFactory.Create("Rabid Dog"));
            ai.Deck.Add(CardFactory.Create("Rabid Dog"));
            ai.Deck.Add(CardFactory.Create("Wild Ostrich"));
            ai.Deck.Add(CardFactory.Create("Wild Ostrich"));
            ai.Deck.Add(CardFactory.Create("Wild Ostrich"));
            ai.Deck.Add(CardFactory.Create("Fire Hatchet"));
            ai.Deck.Add(CardFactory.Create("Fire Hatchet"));
            ai.Deck.Add(CardFactory.Create("Explosion"));
            ai.Deck.Add(CardFactory.Create("Explosion"));
            ai.Deck.Add(CardFactory.Create("Explosion"));
            ai.Deck.Add(CardFactory.Create("Thundermare"));
            ai.Deck.Add(CardFactory.Create("Thundermare"));
            ai.Deck.Add(CardFactory.Create("Thundermare"));
            ai.Deck.Add(CardFactory.Create("Fire Spirals"));
            ai.Deck.Add(CardFactory.Create("Fireborn Dragon"));
            ai.Deck.Add(CardFactory.Create("Fireborn Dragon"));
            ai.Deck.Add(CardFactory.Create("Dragon Summoner"));
            ai.Deck.Add(CardFactory.Create("Dragon Summoner"));
            ai.Deck.Add(CardFactory.Create("Potion of Knowledge"));
            ai.Deck.Add(CardFactory.Create("Fire Spirals"));
            ai.Deck.Add(CardFactory.Create("Thunderstrike"));
            ai.Deck.Add(CardFactory.Create("War Incarnate"));
            ai.Deck.Add(CardFactory.Create("Melt"));
        }

    public static void BuildGreenBeginnerDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Wall of Roots"));
            ai.Deck.Add(CardFactory.Create("Wall of Roots"));
            ai.Deck.Add(CardFactory.Create("Wall of Roots"));
            ai.Deck.Add(CardFactory.Create("Domestic Cat"));
            ai.Deck.Add(CardFactory.Create("Domestic Cat"));
            ai.Deck.Add(CardFactory.Create("Domestic Cat"));
            ai.Deck.Add(CardFactory.Create("Deep Forest Monkeys"));
            ai.Deck.Add(CardFactory.Create("Deep Forest Monkeys"));
            ai.Deck.Add(CardFactory.Create("Deep Forest Monkeys"));
            ai.Deck.Add(CardFactory.Create("Violent Ape"));
            ai.Deck.Add(CardFactory.Create("Violent Ape"));
            ai.Deck.Add(CardFactory.Create("Violent Ape"));
            ai.Deck.Add(CardFactory.Create("Living Tree"));
            ai.Deck.Add(CardFactory.Create("Living Tree"));
            ai.Deck.Add(CardFactory.Create("Living Tree"));
            ai.Deck.Add(CardFactory.Create("Flying Donkey"));
            ai.Deck.Add(CardFactory.Create("Flying Donkey"));
            ai.Deck.Add(CardFactory.Create("Flying Donkey"));
            ai.Deck.Add(CardFactory.Create("Feast"));
            ai.Deck.Add(CardFactory.Create("Feast"));
            ai.Deck.Add(CardFactory.Create("Feast"));
            ai.Deck.Add(CardFactory.Create("Mana Rock"));
            ai.Deck.Add(CardFactory.Create("Mana Rock"));
            ai.Deck.Add(CardFactory.Create("Mana Rock"));
        }

    public static void BuildGreenAdvancedDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Forest"));
            ai.Deck.Add(CardFactory.Create("Wall of Roots"));
            ai.Deck.Add(CardFactory.Create("Wall of Roots"));
            ai.Deck.Add(CardFactory.Create("Wall of Roots"));
            ai.Deck.Add(CardFactory.Create("Deep Forest Monkeys"));
            ai.Deck.Add(CardFactory.Create("Deep Forest Monkeys"));
            ai.Deck.Add(CardFactory.Create("Deep Forest Monkeys"));
            ai.Deck.Add(CardFactory.Create("Violent Ape"));
            ai.Deck.Add(CardFactory.Create("Violent Ape"));
            ai.Deck.Add(CardFactory.Create("Violent Ape"));
            ai.Deck.Add(CardFactory.Create("Crazy Cat Lady"));
            ai.Deck.Add(CardFactory.Create("Crazy Cat Lady"));
            ai.Deck.Add(CardFactory.Create("Crazy Cat Lady"));
            ai.Deck.Add(CardFactory.Create("Drumming Elf"));
            ai.Deck.Add(CardFactory.Create("Drumming Elf"));
            ai.Deck.Add(CardFactory.Create("Cactusaurus"));
            ai.Deck.Add(CardFactory.Create("Cactusaurus"));
            ai.Deck.Add(CardFactory.Create("Realms Crasher"));
            ai.Deck.Add(CardFactory.Create("Realms Crasher"));
            ai.Deck.Add(CardFactory.Create("Slack Tungo"));
            ai.Deck.Add(CardFactory.Create("Slack Tungo"));
            ai.Deck.Add(CardFactory.Create("Nature Incarnate"));
            ai.Deck.Add(CardFactory.Create("River Crocodile"));
            ai.Deck.Add(CardFactory.Create("River Crocodile"));
            ai.Deck.Add(CardFactory.Create("Muscle Blast"));
        }

    public static void BuildBossDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Swamp"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("Waterbearer"));
            ai.Deck.Add(CardFactory.Create("Ratbat"));
            ai.Deck.Add(CardFactory.Create("Ratbat"));
            ai.Deck.Add(CardFactory.Create("Ratbat"));
            ai.Deck.Add(CardFactory.Create("Death Incarnate"));
            ai.Deck.Add(CardFactory.Create("Death Incarnate"));
            ai.Deck.Add(CardFactory.Create("Faith Incarnate"));
            ai.Deck.Add(CardFactory.Create("Faith Incarnate"));
            ai.Deck.Add(CardFactory.Create("Lights Out"));
            ai.Deck.Add(CardFactory.Create("Lights Out"));
            ai.Deck.Add(CardFactory.Create("Possessed Innocent"));
            ai.Deck.Add(CardFactory.Create("Possessed Innocent"));
            ai.Deck.Add(CardFactory.Create("Pure Angel"));
            ai.Deck.Add(CardFactory.Create("The Worlds Evil"));
            ai.Deck.Add(CardFactory.Create("Massacre"));
            ai.Deck.Add(CardFactory.Create("Rotting Dragon"));
            ai.Deck.Add(CardFactory.Create("Afterlife Jinx Lantern"));
            ai.Deck.Add(CardFactory.Create("Giant Bat"));
            ai.Deck.Add(CardFactory.Create("Giant Bat"));
            ai.Deck.Add(CardFactory.Create("Bog Mosquito"));
            ai.Deck.Add(CardFactory.Create("Bog Mosquito"));
            ai.Deck.Add(CardFactory.Create("Pure Angel"));

        }

    public static void BuildRuinsDeck(Player ai)
        {
            ai.Deck.Clear();
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Island"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Plains"));
            ai.Deck.Add(CardFactory.Create("Obstacle"));
            ai.Deck.Add(CardFactory.Create("Obstacle"));
            ai.Deck.Add(CardFactory.Create("Obstacle"));
            ai.Deck.Add(CardFactory.Create("Obstacle"));
            ai.Deck.Add(CardFactory.Create("Sphynx Lynx"));
            ai.Deck.Add(CardFactory.Create("Sphynx Lynx"));
            ai.Deck.Add(CardFactory.Create("Sphynx Lynx"));
            ai.Deck.Add(CardFactory.Create("Glassmole"));
            ai.Deck.Add(CardFactory.Create("Glassmole"));
            ai.Deck.Add(CardFactory.Create("Glassmole"));
            ai.Deck.Add(CardFactory.Create("Origin Golem"));
            ai.Deck.Add(CardFactory.Create("Origin Golem"));
            ai.Deck.Add(CardFactory.Create("Origin Golem"));
            ai.Deck.Add(CardFactory.Create("Omega Golemoid"));
            ai.Deck.Add(CardFactory.Create("Omega Golemoid"));
            ai.Deck.Add(CardFactory.Create("Mana Rock"));
            ai.Deck.Add(CardFactory.Create("Mana Rock"));
            ai.Deck.Add(CardFactory.Create("Progress Incarnate"));
            ai.Deck.Add(CardFactory.Create("Potion of Mana"));
            ai.Deck.Add(CardFactory.Create("Potion of Knowledge"));
            ai.Deck.Add(CardFactory.Create("Potion of Lava"));
            ai.Deck.Add(CardFactory.Create("Pressure Sphere"));
            ai.Deck.Add(CardFactory.Create("Trinkets Collector"));
            ai.Deck.Add(CardFactory.Create("Blast of Knowledge"));
        }
}
