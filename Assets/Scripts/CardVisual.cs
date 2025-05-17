using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CardVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Card linkedCard; // which card it represents
    public GameManager gameManager; // which manager it talks to
    public CreatureCard blockingThisAttacker; // If blocking, who am I blocking
    public CreatureCard blockedByThisBlocker; // If attacker, who blocks me
    public LineRenderer lineRenderer;
    public Image artImage;
    public Image backgroundImage;
    
    public GameObject costBackground;
    public GameObject statsBackground;
    public GameObject swordIcon;
    public GameObject shieldIcon;

    public TMP_Text titleText;
    public TMP_Text sicknessText;
    public TMP_Text costText;
    public TMP_Text statsText;
    public TMP_Text keywordText;

    private readonly Vector2 battlefieldStatsPosition = new Vector2(0, -18); // Adjust as needed
    private readonly Vector2 defaultStatsPosition = new Vector2(32, -56); // whatever your default was

    public bool isInBattlefield = false;
    public bool isInGraveyard = false;

    void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

    public void OnPointerEnter(PointerEventData eventData)
        {
            if (isInGraveyard || linkedCard == null || linkedCard.artwork == null)
                return;

            CardHoverPreview.Instance.ShowCard(linkedCard);

            if (SceneManager.GetActiveScene().name == "DeckBuilderScene")
                return;

            transform.localScale = Vector3.one * 1.1f;
        }

    public void OnPointerExit(PointerEventData eventData)
        {
            if (isInGraveyard)
                return;

            CardHoverPreview.Instance.HidePreview();

            if (SceneManager.GetActiveScene().name == "DeckBuilderScene")
                return;

            transform.localScale = Vector3.one;
        }

    public void UpdateVisual()
        {
            // Apply background color (if not already done in Setup)
            if (backgroundImage != null)
            {
                CardData data = CardDatabase.GetCardData(linkedCard.cardName);

                if (data != null)
                {
                    string color = data.color;
                    Color bgColor = Color.white;

                    switch (color)
                    {
                        case "White": bgColor = HexToColor("F8F6D8"); break;
                        case "Blue":  bgColor = HexToColor("C1D7E9"); break;
                        case "Black": bgColor = HexToColor("BAB1AB"); break;
                        case "Red":   bgColor = HexToColor("E49977"); break;
                        case "Green": bgColor = HexToColor("A3C095"); break;
                        case "Artifact": bgColor = HexToColor("4B413F"); break;
                        case "None":
                            if (data.cardType == CardType.Artifact)
                                bgColor = HexToColor("4B413F");
                            break;
                    }

                    if (data.cardType == CardType.Land)
                    {
                        string name = data.cardName.ToLower();
                        if (name.Contains("plains"))   bgColor = HexToColor("F8F6D8");
                        if (name.Contains("island"))   bgColor = HexToColor("C1D7E9");
                        if (name.Contains("swamp"))    bgColor = HexToColor("BAB1AB");
                        if (name.Contains("mountain")) bgColor = HexToColor("E49977");
                        if (name.Contains("forest"))   bgColor = HexToColor("A3C095");
                    }

                    backgroundImage.color = bgColor;
                }
            }

            // Tapped rotation
            transform.rotation = linkedCard.isTapped
                ? Quaternion.Euler(0, 0, -90)
                : Quaternion.identity;

            if (swordIcon != null && linkedCard is CreatureCard)
                {
                    bool showSword =
                        GameManager.Instance.currentAttackers.Contains(linkedCard) ||
                        GameManager.Instance.selectedAttackers.Contains(linkedCard);
                    swordIcon.SetActive(showSword);
                    swordIcon.transform.rotation = Quaternion.identity;
                }
            
            if (shieldIcon != null && linkedCard is CreatureCard)
            {
                CreatureCard cc = (CreatureCard)linkedCard;

                bool showShield =
                    cc.blockingThisAttacker != null &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(cc);

                shieldIcon.SetActive(showShield);
                shieldIcon.transform.rotation = Quaternion.identity;
            }

            // Hide all UI except artwork (and stats if it's a creature) on battlefield
            if (isInBattlefield)
            {
                if (backgroundImage != null) backgroundImage.enabled = false;
                costBackground.SetActive(false);
                titleText.text = "";
                costText.text = "";
                sicknessText.text = "";
                keywordText.text = "";

                if (linkedCard is CreatureCard battlefieldCreature)
                {
                    statsText.text = $"{battlefieldCreature.power}/{battlefieldCreature.toughness}";
                    sicknessText.text = battlefieldCreature.hasSummoningSickness ? "(@)" : "";
                    statsBackground.SetActive(true);
                    RectTransform statsRect = statsBackground.GetComponent<RectTransform>();
                    if (statsRect != null)
                        statsRect.anchoredPosition = battlefieldStatsPosition;

                    // Only check and draw line if it's a creature
                    if (battlefieldCreature.blockingThisAttacker != null)
                    {
                        lineRenderer.enabled = true;
                        lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0));
                        
                        var attackerVisual = GameManager.Instance.FindCardVisual(battlefieldCreature.blockingThisAttacker);
                        if (attackerVisual != null)
                            lineRenderer.SetPosition(1, new Vector3(attackerVisual.transform.position.x, attackerVisual.transform.position.y, 0));
                    }
                    else
                    {
                        lineRenderer.enabled = false;
                    }
                }
                else
                {
                    lineRenderer.enabled = false;
                }

                return;
            }

            // Disable line if not in battlefield
            if (!isInBattlefield)
            {
                lineRenderer.enabled = false;
                return;
            }

            // Reset default display
            costText.text = "";
            statsText.text = "";
            keywordText.text = "";
            sicknessText.text = "";

            if (linkedCard is CreatureCard creature)
            {
                costText.text = creature.manaCost.ToString();
                statsText.text = $"{creature.power}/{creature.toughness}";
                keywordText.text = linkedCard.GetCardText();
                sicknessText.text = creature.hasSummoningSickness ? "(@)" : "";

                costBackground.SetActive(true);
                statsBackground.SetActive(true);
            }
            else if (linkedCard is SorceryCard sorcery)
            {
                costText.text = sorcery.manaCost.ToString();

                string rules = "";
                if (sorcery.lifeToGain > 0)
                    rules += $"Gain {sorcery.lifeToGain} life.\n";
                if (sorcery.lifeToLoseForOpponent > 0)
                    rules += $"Opponent loses {sorcery.lifeToLoseForOpponent} life.\n";
                if (sorcery.lifeLossForBothPlayers > 0)
                    rules += $"Each player loses {sorcery.lifeLossForBothPlayers} life.\n";
                if (sorcery.cardsToDraw > 0)
                    rules += $"Draw {sorcery.cardsToDraw} card(s).\n";
                if (sorcery.cardsToDiscardorDraw > 0)
                    rules += $"Opponent discards {sorcery.cardsToDiscardorDraw} card(s) at random. If can't, you draw a card.\n";
                if (sorcery.eachPlayerGainLifeEqualToLands)
                    rules += $"Each player gains life equal to the number of lands they control.\n";
                if (sorcery.typeOfPermanentToDestroyAll != SorceryCard.PermanentTypeToDestroy.None)
                {
                    string typeStr = sorcery.typeOfPermanentToDestroyAll == SorceryCard.PermanentTypeToDestroy.Land
                        ? "lands" : "creatures";
                    rules += $"Destroy all {typeStr}.\n";
                }
                if (sorcery.exileAllCreaturesFromGraveyards)
                    rules += "Exile all creature cards from all graveyards.\n";
                if (sorcery.damageToEachCreatureAndPlayer > 0)
                    rules += $"Deal {sorcery.damageToEachCreatureAndPlayer} damage to each creature and each player.\n";

                keywordText.text = rules.Trim();

                costBackground.SetActive(true);
                statsBackground.SetActive(false);
            }
            else if (linkedCard is ArtifactCard artifact)
            {
                costText.text = artifact.manaCost.ToString();
                keywordText.text = linkedCard.GetCardText();

                costBackground.SetActive(true);
                statsBackground.SetActive(false);
            }
            else if (linkedCard is LandCard)
            {
                costText.text = "";
                statsText.text = "";
                keywordText.text = "";

                costBackground.SetActive(false);
                statsBackground.SetActive(false);
            }
            else
            {
                // Fallback
                costText.text = "";
                statsText.text = "";
                keywordText.text = "";

                costBackground.SetActive(false);
                statsBackground.SetActive(false);
            }
        }

    public void Setup(Card card, GameManager manager, CardData sourceData = null)
        {
            // Re-enable everything when card is set up (e.g., for hand, stack, etc.)
            if (backgroundImage != null) backgroundImage.enabled = true;
            if (titleText != null) titleText.enabled = true;
            if (sicknessText != null) sicknessText.enabled = true;
            if (costText != null) costText.enabled = true;
            if (statsText != null) statsText.enabled = true;
            if (keywordText != null) keywordText.enabled = true;

            linkedCard = card;
            gameManager = manager;
            titleText.text = card.cardName;
            lineRenderer = GetComponent<LineRenderer>();
            artImage.sprite = linkedCard.artwork;

            float scale = isInGraveyard ? 0.5f : 1f;
            //transform.localScale = Vector3.one * scale;

            Color bgColor = Color.black;
            string color = sourceData != null ? sourceData.color : card.color;

            switch (color)
            {
                case "White": bgColor = HexToColor("F8F6D8"); break;
                case "Blue":  bgColor = HexToColor("C1D7E9"); break;
                case "Black": bgColor = HexToColor("BAB1AB"); break;
                case "Red":   bgColor = HexToColor("E49977"); break;
                case "Green": bgColor = HexToColor("A3C095"); break;
                case "Artifact": bgColor = HexToColor("4B413F"); break;
                case "None":
                    if (sourceData != null && sourceData.cardType == CardType.Artifact)
                        bgColor = HexToColor("4B413F");
                    break;
            }
            if (sourceData != null && sourceData.cardType == CardType.Land)
            {
                string name = sourceData.cardName.ToLower();
                if (name.Contains("plains"))   bgColor = HexToColor("F8F6D8");
                if (name.Contains("island"))   bgColor = HexToColor("C1D7E9");
                if (name.Contains("swamp"))    bgColor = HexToColor("BAB1AB");
                if (name.Contains("mountain")) bgColor = HexToColor("E49977");
                if (name.Contains("forest"))   bgColor = HexToColor("A3C095");
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = bgColor;
            }

            sicknessText.text = ""; // Clear at start

            if (linkedCard is CreatureCard creature)
            {
                costText.text = creature.manaCost.ToString();
                statsText.text = $"{creature.power}/{creature.toughness}";
                keywordText.text = linkedCard.GetCardText();

                costBackground.SetActive(true);
                statsBackground.SetActive(true);
            }
            else if (linkedCard is SorceryCard sorcery)
            {
                costText.text = sorcery.manaCost.ToString();
                statsText.text = "";
                sicknessText.text = "";

                string rules = "";

                if (sorcery.lifeToGain > 0)
                    rules += $"Gain {sorcery.lifeToGain} life.\n";
                if (sorcery.lifeToLoseForOpponent > 0)
                    rules += $"Opponent loses {sorcery.lifeToLoseForOpponent} life.\n";
                if (sorcery.lifeLossForBothPlayers > 0)
                    rules += $"Each player loses {sorcery.lifeLossForBothPlayers} life.\n";
                if (sorcery.cardsToDraw > 0)
                    rules += $"Draw {sorcery.cardsToDraw} card(s).\n";
                if (sorcery.cardsToDiscardorDraw > 0)
                    rules += $"Opponent discards {sorcery.cardsToDiscardorDraw} card(s) at random. If can't, you draw a card.\n";
                if (sorcery.eachPlayerGainLifeEqualToLands)
                    rules += $"Each player gains life equal to the number of lands they control.\n";
                if (sorcery.typeOfPermanentToDestroyAll != SorceryCard.PermanentTypeToDestroy.None)
                {
                    string typeStr = sorcery.typeOfPermanentToDestroyAll == SorceryCard.PermanentTypeToDestroy.Land
                        ? "lands" : "creatures";
                    rules += $"Destroy all {typeStr}.\n";
                }
                if (sorcery.exileAllCreaturesFromGraveyards)
                    rules += "Exile all creature cards from all graveyards.\n";
                if (sorcery.damageToEachCreatureAndPlayer > 0)
                    rules += $"Deal {sorcery.damageToEachCreatureAndPlayer} damage to each creature and each player.\n";

                keywordText.text = rules.Trim();

                costBackground.SetActive(true);
                statsBackground.SetActive(false);
            }
            else if (linkedCard is ArtifactCard artifact)
            {
                costText.text = artifact.manaCost.ToString();
                statsText.text = "";
                keywordText.text = artifact.GetCardText();

                costBackground.SetActive(true);
                statsBackground.SetActive(false);
            }
            else if (linkedCard is LandCard)
            {
                costText.text = "";
                statsText.text = "";
                keywordText.text = "";

                costBackground.SetActive(false);
                statsBackground.SetActive(false);
            }
            else
            {
                costText.text = "";
                statsText.text = "";
                keywordText.text = "";

                costBackground.SetActive(false);
                statsBackground.SetActive(false);
            }
        }

    private Color HexToColor(string hex)
        {
            Color color;
            ColorUtility.TryParseHtmlString("#" + hex, out color);
            return color;
        }

    public void OnClick()
        {

            if (!isInBattlefield && GameManager.Instance.humanPlayer.Hand.Contains(linkedCard) == false)
            {
                Debug.Log("Cannot play or interact with cards in the graveyard.");
                return;
            }

            if (linkedCard.activatedAbilities != null &&
            linkedCard.activatedAbilities.Contains(ActivatedAbility.TapForMana) &&
            !linkedCard.isTapped &&
            GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
            TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
            (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2) &&
            (!(linkedCard is CreatureCard cc) || !cc.hasSummoningSickness || cc.keywordAbilities.Contains(KeywordAbility.Haste)))
        {
            linkedCard.isTapped = true;
            GameManager.Instance.humanPlayer.ManaPool++;
            GameManager.Instance.UpdateUI();
            SoundManager.Instance.PlaySound(SoundManager.Instance.tap_for_mana);
            UpdateVisual();
            return;
        }

            // PAY-TO-GAIN-ABILITY during Main Phase
                if (linkedCard is CreatureCard abilityCreature &&
                GameManager.Instance.humanPlayer.Battlefield.Contains(abilityCreature) &&
                TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2) &&
                abilityCreature.activatedAbilities != null &&
                abilityCreature.activatedAbilities.Contains(ActivatedAbility.PayToGainAbility))
            {
                if (GameManager.Instance.humanPlayer.ManaPool >= abilityCreature.manaToPayToActivate)
                {
                    GameManager.Instance.PayToGainAbility(abilityCreature);
                    UpdateVisual();
                }
                else
                {
                    Debug.Log($"Not enough mana to activate {abilityCreature.cardName}'s ability.");
                }
                return;
            }

                
            // TAP-TO-CREATE-TOKEN generic ability
                if (linkedCard.activatedAbilities.Contains(ActivatedAbility.TapToCreateToken) &&
                    linkedCard is CreatureCard tokenSpawner &&
                    !linkedCard.isTapped &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2) &&
                    (!tokenSpawner.hasSummoningSickness || tokenSpawner.keywordAbilities.Contains(KeywordAbility.Haste)))
                {
                    int cost = linkedCard.manaToPayToActivate;
                    if (GameManager.Instance.humanPlayer.ManaPool >= cost)
                    {
                        GameManager.Instance.humanPlayer.ManaPool -= cost;
                        linkedCard.isTapped = true;

                        string tokenName = linkedCard.tokenToCreate;
                        Card token = CardFactory.Create(tokenName);
                        if (token != null)
                        {
                            GameManager.Instance.SummonToken(token, GameManager.Instance.humanPlayer);
                            Debug.Log($"{linkedCard.cardName} created a {tokenName} token.");
                        }
                        else
                        {
                            Debug.LogError($"Failed to create token: {tokenName}");
                        }

                        GameManager.Instance.UpdateUI();
                        UpdateVisual();
                    }
                    else
                    {
                        Debug.Log("Not enough mana to activate token creation.");
                    }

                    return;
                }
            // TAP-TO-LOSE-LIFE ability during Main Phase
                if (linkedCard is CreatureCard creatureForDrain &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(creatureForDrain) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2) &&
                    creatureForDrain.activatedAbilities != null &&
                    creatureForDrain.activatedAbilities.Contains(ActivatedAbility.TapToLoseLife) &&
                    !creatureForDrain.isTapped &&
                    (!creatureForDrain.hasSummoningSickness || creatureForDrain.keywordAbilities.Contains(KeywordAbility.Haste)))
                {
                    GameManager.Instance.TapToLoseLife(creatureForDrain);
                    SoundManager.Instance.PlaySound(SoundManager.Instance.plague);
                    UpdateVisual();
                    return;
                }

            // TAP-TO-PLAGUE ability during Main Phase
                if (linkedCard.activatedAbilities.Contains(ActivatedAbility.TapToPlague) &&
                !linkedCard.isTapped &&
                GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2))
                {
                    linkedCard.isTapped = true;
                    GameManager.Instance.humanPlayer.Life -= linkedCard.plagueAmount;
                    GameManager.Instance.aiPlayer.Life -= linkedCard.plagueAmount;
                    GameManager.Instance.UpdateUI();
                    SoundManager.Instance.PlaySound(SoundManager.Instance.plague);
                    UpdateVisual();
                    Debug.Log($"{linkedCard.cardName} tapped: Both players lose {linkedCard.plagueAmount} life.");
                    return;
                }
            
            // TAP-AND-SACRIFICE-FOR-MANA or SACRIFICE-FOR-MANA during Main Phase
                if (linkedCard.activatedAbilities != null &&
                    (linkedCard.activatedAbilities.Contains(ActivatedAbility.TapAndSacrificeForMana) ||
                    linkedCard.activatedAbilities.Contains(ActivatedAbility.SacrificeForMana)) &&
                    !linkedCard.isTapped &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2))
                {
                    ArtifactCard artifact = linkedCard as ArtifactCard;

                    // Check for SacrificeForMana with cost
                    if (linkedCard.activatedAbilities.Contains(ActivatedAbility.SacrificeForMana))
                    {
                        if (GameManager.Instance.humanPlayer.ManaPool >= artifact.manaToPayToActivate)
                        {
                            GameManager.Instance.humanPlayer.ManaPool -= artifact.manaToPayToActivate;
                            GameManager.Instance.humanPlayer.ManaPool += artifact.manaToGain;
                            linkedCard.isTapped = true;
                            GameManager.Instance.SendToGraveyard(linkedCard, GameManager.Instance.humanPlayer);
                            GameManager.Instance.UpdateUI();
                            SoundManager.Instance.PlaySound(SoundManager.Instance.break_artifact);
                            UpdateVisual();
                            Debug.Log($"{linkedCard.cardName} activated: +{artifact.manaToGain} mana.");
                        }
                        else
                        {
                            Debug.Log("Not enough mana for ability.");
                        }
                    }
                    else // TapAndSacrificeForMana (no cost, gain 1 mana)
                    {
                        linkedCard.isTapped = true;
                        GameManager.Instance.humanPlayer.ManaPool++;
                        GameManager.Instance.SendToGraveyard(linkedCard, GameManager.Instance.humanPlayer);
                        GameManager.Instance.UpdateUI();
                        SoundManager.Instance.PlaySound(SoundManager.Instance.break_artifact);
                        UpdateVisual();
                        Debug.Log($"{linkedCard.cardName} sacrificed: +1 mana.");
                    }

                    return;
                }

            
            // TAP-TO-GAIN-LIFE ability during Main Phase
                if (linkedCard.activatedAbilities != null &&
                    linkedCard.activatedAbilities.Contains(ActivatedAbility.TapToGainLife) &&
                    !linkedCard.isTapped &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2))
                {
                    linkedCard.isTapped = true;
                    GameManager.Instance.humanPlayer.Life += 1;
                    GameManager.Instance.UpdateUI();
                    UpdateVisual();
                    return;
                }

            // SACRIFICE-FOR-LIFE during Main Phase
                if (linkedCard.activatedAbilities != null &&
                    linkedCard.activatedAbilities.Contains(ActivatedAbility.SacrificeForLife) &&
                    !linkedCard.isTapped &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2))
                {
                    ArtifactCard artifact = linkedCard as ArtifactCard;

                    if (GameManager.Instance.humanPlayer.ManaPool >= artifact.manaToPayToActivate)
                    {
                        GameManager.Instance.humanPlayer.ManaPool -= artifact.manaToPayToActivate;
                        GameManager.Instance.humanPlayer.Life += artifact.lifeToGain;
                        linkedCard.isTapped = true;
                        GameManager.Instance.SendToGraveyard(linkedCard, GameManager.Instance.humanPlayer);
                        GameManager.Instance.UpdateUI();
                        UpdateVisual();
                        Debug.Log($"{linkedCard.cardName} activated: Gain {artifact.lifeToGain} life.");
                    }
                    else
                    {
                        Debug.Log("Not enough mana for ability.");
                    }

                    return;
                }
            
            // SACRIFICE-TO-DRAW-CARDS during Main Phase
                if (linkedCard.activatedAbilities != null &&
                    linkedCard.activatedAbilities.Contains(ActivatedAbility.SacrificeToDrawCards) &&
                    !linkedCard.isTapped &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2))
                {
                    ArtifactCard artifact = linkedCard as ArtifactCard;

                    if (GameManager.Instance.humanPlayer.ManaPool >= artifact.manaToPayToActivate)
                    {
                        GameManager.Instance.humanPlayer.ManaPool -= artifact.manaToPayToActivate;

                        for (int i = 0; i < artifact.cardsToDraw; i++)
                        {
                            GameManager.Instance.DrawCard(GameManager.Instance.humanPlayer);
                        }
                        
                        linkedCard.isTapped = true;
                        GameManager.Instance.SendToGraveyard(linkedCard, GameManager.Instance.humanPlayer);
                        GameManager.Instance.UpdateUI();
                        UpdateVisual();
                        Debug.Log($"{linkedCard.cardName} activated: Draw {artifact.cardsToDraw} cards.");
                    }
                    else
                    {
                        Debug.Log("Not enough mana for ability.");
                    }

                    return;
                }

            // Blocking phase
                if (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.ChooseBlockers &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.AI)

                if (linkedCard is CreatureCard clickedCreature)
                {
                    Player you = GameManager.Instance.humanPlayer;
                    Player enemy = GameManager.Instance.aiPlayer;

                    // If you click an attacker first
                    if (enemy.Battlefield.Contains(clickedCreature) &&
                        GameManager.Instance.currentAttackers.Contains(clickedCreature))
                    {
                        GameManager.Instance.selectedAttackerForBlocking = clickedCreature;
                        Debug.Log($"Selected attacker to block: {clickedCreature.cardName}");
                        return;
                    }

                    // If you click your own untapped creature next
                    if (you.Battlefield.Contains(clickedCreature) &&
                        !clickedCreature.isTapped &&
                        !clickedCreature.keywordAbilities.Contains(KeywordAbility.CantBlock) &&
                        !(
                            clickedCreature.keywordAbilities.Contains(KeywordAbility.CantBlockWithoutForest) &&
                            !you.Battlefield.Exists(card => card is LandCard land && land.cardName.ToLower().Contains("forest"))
                        ))
                    {
                        var attacker = GameManager.Instance.selectedAttackerForBlocking;

                        if (clickedCreature.blockingThisAttacker != null)
                        {
                            // Already blocking → remove block
                            Debug.Log($"{clickedCreature.cardName} stops blocking.");
                            clickedCreature.blockingThisAttacker.blockedByThisBlocker = null;
                            clickedCreature.blockingThisAttacker = null;
                            GameManager.Instance.UpdateUI();
                            return;
                        }

                        if (attacker == null)
                        {
                            Debug.Log("Click an attacking enemy creature first to block.");
                            return;
                        }

                        // Remove previous block from this attacker, if any
                        if (attacker.blockedByThisBlocker != null)
                        {
                            CreatureCard oldBlocker = attacker.blockedByThisBlocker;
                            oldBlocker.blockingThisAttacker = null;
                            attacker.blockedByThisBlocker = null;

                            // Force line to disappear on old blocker
                            var oldVisual = GameManager.Instance.FindCardVisual(oldBlocker);
                            if (oldVisual != null)
                                oldVisual.UpdateVisual();
                        }

                        // Check if this blocker is allowed to block the attacker (Flying rule)
                        if (attacker.keywordAbilities.Contains(KeywordAbility.Flying) &&
                            !clickedCreature.keywordAbilities.Contains(KeywordAbility.Flying) &&
                            !clickedCreature.keywordAbilities.Contains(KeywordAbility.Reach))
                        {
                            Debug.Log($"{clickedCreature.cardName} can't block {attacker.cardName} because it lacks Flying or Reach.");
                            return;
                        }
                    
                        // Check if blocker is restricted to flying-only
                        if (clickedCreature.keywordAbilities.Contains(KeywordAbility.CanOnlyBlockFlying) &&
                            !attacker.keywordAbilities.Contains(KeywordAbility.Flying))
                        {
                            Debug.Log($"{clickedCreature.cardName} can only block flying creatures.");
                            return;
                        }
                        // Check if attacker is unblockable due to landwalk
                        if (IsLandwalkPreventingBlock(attacker, you))
                        {
                            Debug.Log($"{clickedCreature.cardName} can't block {attacker.cardName} due to landwalk.");
                            return;
                        }
                        
                        // Assign the block
                        clickedCreature.blockingThisAttacker = attacker;
                        attacker.blockedByThisBlocker = clickedCreature;
                        GameManager.Instance.selectedAttackerForBlocking = null;

                        Debug.Log($"{clickedCreature.cardName} is blocking {attacker.cardName}");
                        SoundManager.Instance.PlaySound(SoundManager.Instance.declareBlock);
                        GameManager.Instance.UpdateUI();
                        return;
                    }
                }

            // Declare attacker
                if (linkedCard is CreatureCard creature)
                {
                    if (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.ChooseAttackers &&
                        TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human)
                    {
                        if (GameManager.Instance.selectedAttackers.Contains(creature))
                        {
                            // Removing from combat
                            GameManager.Instance.selectedAttackers.Remove(creature);
                            if (!creature.keywordAbilities.Contains(KeywordAbility.Vigilance))
                                creature.isTapped = false;

                            Debug.Log($"{creature.cardName} removed from attackers.");
                        }
                        else if (!creature.hasSummoningSickness && !creature.keywordAbilities.Contains(KeywordAbility.Defender))
                        {
                            // Adding to combat
                            GameManager.Instance.selectedAttackers.Add(creature);
                            SoundManager.Instance.PlaySound(SoundManager.Instance.declareAttack);
                            if (!creature.keywordAbilities.Contains(KeywordAbility.Vigilance))
                                creature.isTapped = true;

                            Debug.Log($"{creature.cardName} declared as attacker.");
                        }

                        UpdateVisual();
                        return;
                    }
                }

            // Land usage
                if (isInBattlefield && linkedCard is LandCard land)
                {
                    GameManager.Instance.TapLandForMana(land, GameManager.Instance.humanPlayer);
                    SoundManager.Instance.PlaySound(SoundManager.Instance.tap_for_mana);
                    UpdateVisual();
                    return;
                }

            // Playing card from hand
                if (!isInBattlefield)
                    {
                        if (TurnSystem.Instance.currentPlayer != TurnSystem.PlayerType.Human ||
                            (TurnSystem.Instance.currentPhase != TurnSystem.TurnPhase.Main1 &&
                            TurnSystem.Instance.currentPhase != TurnSystem.TurnPhase.Main2))
                        {
                            Debug.Log("You can only play cards during your own Main Phase.");
                            return;
                        }

                        GameManager.Instance.PlayCard(GameManager.Instance.humanPlayer, this);
                        UpdateVisual();
                        return;
                    }
        }

    private bool IsLandwalkPreventingBlock(CreatureCard attacker, Player defender)
        {
                foreach (var ability in attacker.keywordAbilities)
                {
                    if (ability == KeywordAbility.Plainswalk &&
                        defender.Battlefield.Any(card => card is LandCard land && land.cardName.ToLower().Contains("plains")))
                        return true;
                    if (ability == KeywordAbility.Islandwalk &&
                        defender.Battlefield.Any(card => card is LandCard land && land.cardName.ToLower().Contains("island")))
                        return true;
                    if (ability == KeywordAbility.Swampwalk &&
                        defender.Battlefield.Any(card => card is LandCard land && land.cardName.ToLower().Contains("swamp")))
                        return true;
                    if (ability == KeywordAbility.Mountainwalk &&
                        defender.Battlefield.Any(card => card is LandCard land && land.cardName.ToLower().Contains("mountain")))
                        return true;
                    if (ability == KeywordAbility.Forestwalk &&
                        defender.Battlefield.Any(card => card is LandCard land && land.cardName.ToLower().Contains("forest")))
                        return true;
                }
                return false;
        }

    public void ResetVisualForGraveyard()
        {
            isInBattlefield = false;
            isInGraveyard = true;

            if (lineRenderer != null)
                lineRenderer.enabled = false;

            transform.localScale = Vector3.one * 0.5f;
            transform.rotation = Quaternion.identity;

            costBackground.SetActive(false);
            statsBackground.SetActive(false);

            sicknessText.text = "";
            keywordText.text = "";
            statsText.text = "";
        }

    public void PrepareForGraveyard()
        {
            isInBattlefield = false;
            isInGraveyard = true;

            // Re-enable UI components in case they were disabled on battlefield
            if (backgroundImage != null) backgroundImage.enabled = true;
            if (titleText != null) titleText.enabled = true;
            if (sicknessText != null) sicknessText.enabled = true;
            if (costText != null) costText.enabled = true;
            if (statsText != null) statsText.enabled = true;
            if (keywordText != null) keywordText.enabled = true;

            costBackground.SetActive(true);
            statsBackground.SetActive(true);

            if (lineRenderer != null) lineRenderer.enabled = false;

            // Reset rotation & scale
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one * 0.5f;
        }

    public void UpdateGraveyardVisual()
        {
            // General setup
            isInBattlefield = false;
            isInGraveyard = true;

            transform.localScale = Vector3.one * 0.5f;
            transform.rotation = Quaternion.identity;

            // Reset stats position (in case it was moved on battlefield)
            if (statsBackground != null)
            {
                RectTransform statsRect = statsBackground.GetComponent<RectTransform>();
                if (statsRect != null)
                    statsRect.anchoredPosition = defaultStatsPosition;
            }

            if (lineRenderer != null)
                lineRenderer.enabled = false;

            // Enable all UI elements
            if (backgroundImage != null) backgroundImage.enabled = true;
            if (titleText != null) titleText.enabled = true;
            if (sicknessText != null) sicknessText.enabled = true;
            if (costText != null) costText.enabled = true;
            if (statsText != null) statsText.enabled = true;
            if (keywordText != null) keywordText.enabled = true;

            titleText.text = linkedCard.cardName;
            sicknessText.text = "";
            lineRenderer.enabled = false;

            // Load card data
            CardData sourceData = CardDatabase.GetCardData(linkedCard.cardName);

            if (sourceData != null && backgroundImage != null)
            {
                string color = sourceData.color;
                Color bgColor = Color.white;

                switch (color)
                {
                    case "White": bgColor = HexToColor("F8F6D8"); break;
                    case "Blue":  bgColor = HexToColor("C1D7E9"); break;
                    case "Black": bgColor = HexToColor("BAB1AB"); break;
                    case "Red":   bgColor = HexToColor("E49977"); break;
                    case "Green": bgColor = HexToColor("A3C095"); break;
                    case "Artifact": bgColor = HexToColor("4B413F"); break;
                }

                backgroundImage.color = bgColor;
            }

            if (artImage != null && linkedCard.artwork != null)
                artImage.sprite = linkedCard.artwork;

            // Show correct info by card type
            if (linkedCard is CreatureCard creature)
            {
                costText.text = creature.manaCost.ToString();
                statsText.text = $"{creature.power}/{creature.toughness}";
                keywordText.text = linkedCard.GetCardText();

                costBackground.SetActive(true);
                statsBackground.SetActive(true);
            }
            else if (linkedCard is ArtifactCard artifact)
            {
                costText.text = artifact.manaCost.ToString();
                statsText.text = "";
                keywordText.text = linkedCard.GetCardText();

                costBackground.SetActive(true);
                statsBackground.SetActive(false);
            }
            else if (linkedCard is SorceryCard sorcery)
            {
                costText.text = sorcery.manaCost.ToString();
                statsText.text = "";

                string rules = "";
                if (sorcery.lifeToGain > 0)
                    rules += $"Gain {sorcery.lifeToGain} life.\n";
                if (sorcery.lifeToLoseForOpponent > 0)
                    rules += $"Opponent loses {sorcery.lifeToLoseForOpponent} life.\n";
                if (sorcery.lifeLossForBothPlayers > 0)
                    rules += $"Each player loses {sorcery.lifeLossForBothPlayers} life.\n";
                if (sorcery.cardsToDraw > 0)
                    rules += $"Draw {sorcery.cardsToDraw} card(s).\n";
                if (sorcery.cardsToDiscardorDraw > 0)
                    rules += $"Opponent discards {sorcery.cardsToDiscardorDraw} card(s) at random. If can't, you draw a card.\n";
                if (sorcery.eachPlayerGainLifeEqualToLands)
                    rules += $"Each player gains life equal to the number of lands they control.\n";
                if (sorcery.typeOfPermanentToDestroyAll != SorceryCard.PermanentTypeToDestroy.None)
                {
                    string typeStr = sorcery.typeOfPermanentToDestroyAll == SorceryCard.PermanentTypeToDestroy.Land
                        ? "lands" : "creatures";
                    rules += $"Destroy all {typeStr}.\n";
                }
                if (sorcery.exileAllCreaturesFromGraveyards)
                    rules += "Exile all creature cards from all graveyards.\n";
                if (sorcery.damageToEachCreatureAndPlayer > 0)
                    rules += $"Deal {sorcery.damageToEachCreatureAndPlayer} damage to each creature and each player.\n";

                keywordText.text = rules.Trim();

                costBackground.SetActive(true);
                statsBackground.SetActive(false);
            }
            else if (linkedCard is LandCard)
            {
                costText.text = "";
                statsText.text = "";
                keywordText.text = "";

                costBackground.SetActive(false);
                statsBackground.SetActive(false);
            }
            else
            {
                // Unknown type — hide everything
                costText.text = "";
                statsText.text = "";
                keywordText.text = "";

                costBackground.SetActive(false);
                statsBackground.SetActive(false);
            }
        }
}