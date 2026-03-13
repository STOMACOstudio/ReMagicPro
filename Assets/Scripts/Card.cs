using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Card
{
    private static readonly Dictionary<TriggerTiming, string> TriggerTextPrefixByTiming =
        new Dictionary<TriggerTiming, string>
        {
            { TriggerTiming.OnUpkeep, "At the beginning of your upkeep," },
            { TriggerTiming.OnArtifactEnter, "Whenever an artifact enters the battlefield," },
            { TriggerTiming.OnEnchantmentEnter, "Whenever an enchantment enters the battlefield," },
            { TriggerTiming.OnLandEnter, "Whenever a land enters the battlefield," },
            { TriggerTiming.OnCreatureEnter, "Whenever a creature enters the battlefield," },
            { TriggerTiming.OnLandLeave, "Whenever a land leaves the battlefield," },
            { TriggerTiming.OnLifeGain, "Whenever you gain life," },
            { TriggerTiming.OnCardDraw, "Whenever you draw a card," },
            { TriggerTiming.OnOpponentDraw, "Whenever an opponent draws a card," },
            { TriggerTiming.OnCreatureDiesOrDiscarded, "Whenever a creature dies or is discarded," },
            { TriggerTiming.OnPlayerDiscard, "Whenever a player discards a card," },
            { TriggerTiming.OnCombatDamageToPlayer, "Whenever a creature deals combat damage to a player," },
            { TriggerTiming.OnOpponentDiscard, "Whenever an opponent discards a card," },
        };

    public string cardName;
    public string rarity;
    public int manaCost;
    public bool isToken = false;
    public bool isTapped = false;
    public bool entersTapped = false;
    public bool exileSelfOnDeath = false;

    // If true, this card has an additional variable mana cost "X".
    public bool hasXCost = false;

    // Holds the value paid for X when the card was cast.
    public int xValue = 0;

    public List<string> color = new List<string>();
    public string PrimaryColor => color.Count > 0 ? color[0] : "None";

    // e.g. "Human", "Wizard"
    public List<string> subtypes = new List<string>();

    public string rulesText;
    public string flavorText;

    // Artist credited for the card's artwork
    public string artist;

    public int plagueAmount;
    public int manaToGain;
    public int lifeToGain;
    public int manaToPayToActivate;
    public int cardsToDraw;
    public int damageToCreature;
    public int buffPower;
    public int buffToughness;
    public KeywordAbility keywordBuff = KeywordAbility.None;

    public string tokenToCreate;

    public Sprite artwork;

    public Player owner;

    // Stores a card temporarily controlled by this card and its original owner.
    public Card gainedControlCard;
    public Player gainedControlCardOriginalOwner;

    public List<CardAbility> abilities = new List<CardAbility>();
    public List<ActivatedAbility> activatedAbilities = new List<ActivatedAbility>();
    public List<KeywordAbility> keywordAbilities = new List<KeywordAbility>();

    public string GetPrimaryManaColor()
    {
        if (color == null || color.Count == 0)
            return "Colorless";

        string firstColored = color.FirstOrDefault(c => c != "Artifact");
        if (string.IsNullOrEmpty(firstColored))
            firstColored = color.FirstOrDefault();

        return ManaColorUtility.NormalizeColor(firstColored);
    }

    public virtual string GetActivationColor()
    {
        return GetPrimaryManaColor();
    }

    protected string FormatColoredManaNumber(int amount, string colorName)
    {
        return ManaColorUtility.FormatColoredManaNumber(amount, colorName);
    }

    protected string FormatColoredManaWithLabel(int amount, string colorName)
    {
        return ManaColorUtility.FormatColoredManaNumber(amount, colorName);
    }

    public virtual void Play(Player player)
        {
            if (entersTapped)
            {
                isTapped = true;
                Debug.Log($"{cardName} enters tapped.");
            }

            player.PlayCard(this);
        }

    public virtual void OnEnterPlay(Player owner)
        {
            foreach (var ability in abilities)
            {
                if (ability.timing != TriggerTiming.OnEnter)
                    continue;

                if (ability.requiresTarget)
                {
                    // Only initiate optional targeting for the human player's cards.
                    // AI-controlled cards handle their ETB targeting automatically.
                    if (owner == GameManager.Instance.humanPlayer)
                        GameManager.Instance.BeginOptionalTargetSelectionAfterEntry(this, owner, ability);
                }
                else
                {
                    if (ability.effect != null)
                    {
                        int oldLife = owner.Life;
                        ability.effect.Invoke(owner, this);
                        int gained = owner.Life - oldLife;

                        if (gained > 0)
                        {
                            GameManager.Instance.ShowFloatingHeal(gained,
                                owner == GameManager.Instance.humanPlayer
                                    ? GameManager.Instance.playerLifeContainer
                                    : GameManager.Instance.enemyLifeContainer);
                        }
                    }
                }
            }
        }

    public virtual void OnLeavePlay(Player owner)
    {
        foreach (var ability in abilities)
        {
            if (ability.timing != TriggerTiming.OnDeath || ability.effect == null)
                continue;

            if (ability.usesStack)
            {
                GameManager.Instance.QueueTriggeredAbility(ability, owner, this, this);
            }
            else
            {
                int oldLife = owner.Life;
                ability.effect.Invoke(owner, this);
                int gained = owner.Life - oldLife;

                if (gained > 0)
                {
                    GameManager.Instance.ShowFloatingHeal(gained,
                        owner == GameManager.Instance.humanPlayer
                            ? GameManager.Instance.playerLifeContainer
                            : GameManager.Instance.enemyLifeContainer);
                }
            }
        }
    }

    public virtual string GetCardText()
        {
            List<string> lines = new List<string>();

        if (this is AuraCard aura)
        {
            string enchantText = aura.requiredTargetType switch
            {
                SorceryCard.TargetType.TappedCreature => "Enchant tapped creature",
                SorceryCard.TargetType.Artifact => "Enchant artifact",
                _ => "Enchant creature"
            };
            if (aura.targetMustBeControlledCreature)
                enchantText += " you control";
            lines.Add(enchantText);
            if (aura.gainControlOfCreature)
                lines.Add("You control enchanted creature.");
        }

            // Keyword abilities — only for creatures
            if (this is CreatureCard creature)
                {
                foreach (var keyword in creature.keywordAbilities)
                {
                    if (keyword == KeywordAbility.CantBlock ||
                        keyword == KeywordAbility.CanOnlyBlockFlying ||
                        keyword == KeywordAbility.CantBlockWithoutForest ||
                        keyword == KeywordAbility.CantDealCombatDamage ||
                        keyword == KeywordAbility.MustAttackEachTurnIfAble ||
                        keyword == KeywordAbility.CantBeBlocked ||
                        keyword == KeywordAbility.BeastCreatureSpellsCostOneLess ||
                        keyword == KeywordAbility.PotionSpellsCostOneLess ||
                        keyword.ToString().StartsWith("ProtectionFrom"))
                        continue;

                    lines.Add(keyword.ToString());
                }

                if (creature.keywordAbilities.Contains(KeywordAbility.CanOnlyBlockFlying))
                    lines.Add("This creature can only block creatures with flying.");
                if (creature.keywordAbilities.Contains(KeywordAbility.CantBlock))
                    lines.Add("This creature can't block.");
                if (creature.keywordAbilities.Contains(KeywordAbility.CantBlockWithoutForest))
                    lines.Add("This creature can't block if you don't control a forest.");
                if (entersTapped)
                    lines.Add("This creature enters the battlefield tapped.");
                if (creature.keywordAbilities.Contains(KeywordAbility.CantUntap))
                    lines.Add("This creature doesn't untap during its controller's untap step.");
                if (creature.keywordAbilities.Contains(KeywordAbility.CantDealCombatDamage))
                    lines.Add("This creature can't deal combat damage.");
                if (creature.keywordAbilities.Contains(KeywordAbility.MustAttackEachTurnIfAble))
                    lines.Add("This creature attacks each turn if able.");
                if (creature.keywordAbilities.Contains(KeywordAbility.CantBeBlocked))
                    lines.Add("This creature can't be blocked.");
                if (creature.keywordAbilities.Contains(KeywordAbility.ProtectionFromWhite))
                    lines.Add("Protection from White");
                if (creature.keywordAbilities.Contains(KeywordAbility.ProtectionFromBlue))
                    lines.Add("Protection from Blue");
                if (creature.keywordAbilities.Contains(KeywordAbility.ProtectionFromBlack))
                    lines.Add("Protection from Black");
                if (creature.keywordAbilities.Contains(KeywordAbility.ProtectionFromRed))
                    lines.Add("Protection from Red");
                if (creature.keywordAbilities.Contains(KeywordAbility.ProtectionFromGreen))
                    lines.Add("Protection from Green");

                // Activated abilities
                if (creature.activatedAbilities != null)
                    {
                        Debug.Log($"{creature.cardName} has {creature.activatedAbilities.Count} activated abilities.");

                        foreach (var activated in creature.activatedAbilities)
                        {
                            Debug.Log($"Activated ability: {activated}");

                            switch (activated)
                            {
                                case ActivatedAbility.TapForMana:
                                    lines.Add($"Tap: Add {FormatColoredManaWithLabel(1, creature.GetActivationColor())}.");
                                    break;
                                case ActivatedAbility.TapToLoseLife:
                                    lines.Add($"Tap: Your opponent loses {creature.tapLifeLossAmount} life.");
                                    break;
                                case ActivatedAbility.TapToCreateToken:
                                    lines.Add($"{FormatColoredManaNumber(creature.manaToPayToActivate, creature.GetActivationColor())}TAP: Create a {tokenToCreate} token.");
                                    break;
                                case ActivatedAbility.TapToDealDamageAnyTarget:
                                    lines.Add($"{FormatColoredManaNumber(creature.manaToPayToActivate, creature.GetActivationColor())}, TAP: Deal {creature.damageToCreature} damage to any target.");
                                    break;
                                case ActivatedAbility.TapToDestroyPower4OrGreater:
                                    lines.Add("Tap: Destroy target creature with power 4 or greater.");
                                    break;
                                case ActivatedAbility.TapToDrawCards:
                                    if (creature.manaToPayToActivate > 0)
                                        lines.Add($"{FormatColoredManaNumber(creature.manaToPayToActivate, creature.GetActivationColor())}, TAP: Draw {creature.cardsToDraw} card(s).");
                                    else
                                        lines.Add($"Tap: Draw {creature.cardsToDraw} card(s).");
                                    break;
                                case ActivatedAbility.PayToGainAbility:
                                    lines.Add($"{FormatColoredManaNumber(creature.manaToPayToActivate, creature.GetActivationColor())}: Gains {creature.abilityToGain} until end of turn.");
                                    break;
                                case ActivatedAbility.PayToBuffSelf:
                                    int powerBuff = creature.buffPower == 0 && creature.buffToughness == 0 ? 1 : creature.buffPower;
                                    int toughnessBuff = creature.buffPower == 0 && creature.buffToughness == 0 ? 0 : creature.buffToughness;
                                    lines.Add($"{FormatColoredManaNumber(creature.manaToPayToActivate, creature.GetActivationColor())}: +{powerBuff}/+{toughnessBuff} until end of turn.");
                                    break;
                                case ActivatedAbility.ReturnSelfFromGraveyard:
                                    lines.Add($"{FormatColoredManaNumber(creature.manaToPayToActivate, creature.GetActivationColor())}: Return this card from your graveyard to the battlefield.");
                                    break;
                                case ActivatedAbility.ReturnSelfFromGraveyardToHand:
                                    lines.Add($"{FormatColoredManaNumber(creature.manaToPayToActivate, creature.GetActivationColor())}: Return this card from your graveyard to your hand.");
                                    break;
                            }
                        }
                    }
                }

            if (!string.IsNullOrEmpty(rulesText))
            {
                lines.Add(rulesText);
            }

            if (this is ArtifactCard artifact)
            {
                if (entersTapped)
                    lines.Add("This card enters the battlefield tapped.");

                if (activatedAbilities != null)
                {
                    foreach (var activated in activatedAbilities)
                    {
                        switch (activated)
                        {
                            case ActivatedAbility.TapForMana:
                                lines.Add($"Tap: Add {FormatColoredManaWithLabel(1, GetActivationColor())}.");
                                break;
                            case ActivatedAbility.TapToGainLife:
                                lines.Add("Tap: Gain 1 life.");
                                //lines.Add($"Tap: Gain {plagueAmount} life.");
                                break;
                            case ActivatedAbility.TapAndSacrificeForMana:
                                lines.Add($"Tap, sacrifice: Add {FormatColoredManaWithLabel(1, GetActivationColor())}.");
                                break;
                            case ActivatedAbility.TapToPlague:
                                lines.Add($"Tap: Each player loses {plagueAmount} life.");
                                break;
                            case ActivatedAbility.SacrificeForMana:
                                lines.Add($"{FormatColoredManaNumber(manaToPayToActivate, GetActivationColor())}TAP, sacrifice: Add {FormatColoredManaWithLabel(manaToGain, GetActivationColor())}.");
                                break;
                            case ActivatedAbility.SacrificeForLife:
                                lines.Add($"{manaToPayToActivate}TAP, sacrifice: Gain {lifeToGain} life.");
                                break;
                            case ActivatedAbility.SacrificeToDrawCards:
                                lines.Add($"{manaToPayToActivate}TAP, sacrifice: Draw {cardsToDraw} card(s).");
                                break;
                            case ActivatedAbility.DealDamageToCreature:
                                    lines.Add($"{manaToPayToActivate}TAP, sacrifice: Deal {damageToCreature} damage to target creature.");
                                    break;
                            case ActivatedAbility.BuffTargetCreature:
                                lines.Add($"{manaToPayToActivate}TAP, sacrifice: Target creature gets +{buffPower}/+{buffToughness} until end of turn.");
                                break;
                            case ActivatedAbility.TapToPlayRandomPotion:
                                lines.Add($"{FormatColoredManaNumber(manaToPayToActivate, GetActivationColor())}TAP: Search your library for a random Potion and put it onto the battlefield, then shuffle.");
                                break;
                            case ActivatedAbility.TapToDrawCards:
                                lines.Add($"{manaToPayToActivate}, TAP: Draw {cardsToDraw} card(s).");
                                break;
                            case ActivatedAbility.Equip:
                                lines.Add($"Equip {FormatColoredManaNumber(manaToPayToActivate, GetActivationColor())}");
                                break;
                        }
                    }
                }
            }
            // Triggered abilities — shared across all cards
            foreach (var ability in abilities)
            {
                if (string.IsNullOrWhiteSpace(ability.description))
                    continue;

                string line = BuildTriggeredAbilityLine(ability);
                if (!string.IsNullOrEmpty(line))
                    lines.Add(line);
            }

            if (keywordAbilities != null)
            {
                if (keywordAbilities.Contains(KeywordAbility.AllPermanentsEnterTapped))
                    lines.Add("All permanents enter the battlefield tapped.");
                if (keywordAbilities.Contains(KeywordAbility.NoLifeGain))
                    lines.Add("Players can't gain life.");
                if (keywordAbilities.Contains(KeywordAbility.OnlyCastCreatureSpells))
                    lines.Add("Players can only cast creature spells.");
                if (keywordAbilities.Contains(KeywordAbility.CreatureSpellsCostOneLess))
                    lines.Add("Creature spells you cast cost 1 less.");
                if (keywordAbilities.Contains(KeywordAbility.BeastCreatureSpellsCostOneLess))
                    lines.Add("Beast creature spells you cast cost 1 less.");
                if (keywordAbilities.Contains(KeywordAbility.PotionSpellsCostOneLess))
                    lines.Add("Potion spells you cast cost 1 less to cast.");
                if (keywordAbilities.Contains(KeywordAbility.OpponentSpellsCostOneMore))
                    lines.Add("Spells cast by your opponent cost 1 more.");
            }

            if (!string.IsNullOrEmpty(flavorText))
                lines.Add($"<i>{flavorText}</i>");

            return string.Join("\n", lines);
        }

    private string BuildTriggeredAbilityLine(CardAbility ability)
    {
        string trimmedDescription = ability.description.TrimStart();

        if (ability.timing == TriggerTiming.OnEnter)
            return $"When {GetThisObjectLabel()} enters, {trimmedDescription}";

        if (ability.timing == TriggerTiming.OnDeath)
            return $"When {GetThisObjectLabel()} dies, {trimmedDescription}";

        if (ability.timing == TriggerTiming.OnCreatureDies)
        {
            if (ability.triggerOnlyOnAttachedCreatureDeath && this is AuraCard)
                return $"When enchanted creature dies, {trimmedDescription}";

            return $"Whenever a creature dies, {trimmedDescription}";
        }

        if (ability.timing == TriggerTiming.OnBlock)
            return $"When {GetThisObjectLabel()} blocks, {trimmedDescription}";

        if (TriggerTextPrefixByTiming.TryGetValue(ability.timing, out string prefix))
            return $"{prefix} {trimmedDescription}";

        return trimmedDescription;
    }

    private string GetThisObjectLabel()
    {
        return this is CreatureCard ? "this creature" : "this permanent";
    }
}
