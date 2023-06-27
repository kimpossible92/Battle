using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TherapistDeckCollecter : InPhobiaScrollView
{
    #region Serialized Fields

    [SerializeField] private IdeaController ideaController;
    [SerializeField] private ProgressBarController progressBarController;

    [Space(20f)]
    [Header("Card Collecter Part")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<Card> therapistCardsToSelect;
    [SerializeField] private List<Card> rareCards;
    [SerializeField] private List<Card> equipmentCards;
    [SerializeField] private List<Card> commonCards;


    [Space(20f)]
    [Header("Card Collecter Part")]
    [SerializeField] private Button removeCardButton;
    [SerializeField] private Button removeCardConfirmButton;
    [SerializeField] private CardController removeCardController;
    [SerializeField] private Button addCardButton;
    [SerializeField] private TMPro.TMP_Text addCardInfo;
    [SerializeField] private Transform threeCardsParent;
    [SerializeField] private GameObject deckBuildingBegin;
    [SerializeField] private GameObject deckBuildingMain;
    [SerializeField] private GameObject deckBuildingRemoveCard;
    [SerializeField] private TMPro.TMP_Text removeCardInfo;
    [SerializeField] private GameObject deckBuildingAddCard;

    #endregion

    #region Private Fields

    private List<Card> selectedCards;
    private List<Card> therapistDeck;
    private List<CardController> deckCardsControllers;
    private CardController selectedCard;
    private int ideaCountNeeded;
    private int pointCountNeeded;

    #endregion

    #region Public Methods

    public void SetAsSelected(CardController cardController)
    {
        if(selectedCard==cardController)
        {
            removeCardButton.interactable = false;
            selectedCard = null;
        }
        else
        {
            selectedCard = cardController;
            removeCardButton.interactable = true;
        }

        foreach (CardController cc in deckCardsControllers)
        {
            if (cc != selectedCard)
                cc.SetSelect(false);
        }
    }

    public void InitializeCollecter()
    {
        ideaCountNeeded = 1;
        pointCountNeeded = 1;

        if (deckCardsControllers == null)
            deckCardsControllers = new List<CardController>();
        foreach (CardController card in deckCardsControllers)
        {
            card.DestroyCard();
        }
        deckCardsControllers.Clear();

        if (therapistDeck == null)
            therapistDeck = new List<Card>();
        therapistDeck = new List<Card>(Cards.instance.TherapistStandartCards);

        therapistCardsToSelect = new List<Card>(Cards.instance.TherapistCardsToSelect);
        removeCardButton.interactable = false;
        print("InitializeCollecter");
        PrepareCardsToSelect();
        ActivateDeckBuildingBegin();
    }

    public void ActivateDeckBuildingBegin()
    {
        //RandomizeThreeCards(threeCardsParent);

        deckBuildingBegin.SetActive(true);
        deckBuildingMain.SetActive(false);
        deckBuildingRemoveCard.SetActive(false);
        deckBuildingAddCard.SetActive(false);
    }

    public void ActivateDeckBuildingMain()
    {
        foreach (CardController cardC in deckCardsControllers)
        {
            cardC.DestroyCard();
        }
        deckCardsControllers.Clear();

        foreach (Card card in therapistDeck)
        {
            AddCard(card);
        }

        addCardButton.interactable = ideaController.CheckIdeaCount(ideaCountNeeded);
        removeCardButton.interactable = false;

        deckBuildingBegin.SetActive(false);
        deckBuildingMain.SetActive(true);
        deckBuildingRemoveCard.SetActive(false);
        deckBuildingAddCard.SetActive(false);
    }

    public void ActivateDeckBuildingRemoveCard()
    {
        removeCardController.SetCardParametersToGameObject(selectedCard.card);

        removeCardConfirmButton.interactable = progressBarController.HasNeededAmoutOfPointsToUse(pointCountNeeded);
        removeCardInfo.text = $"-{pointCountNeeded}";
        deckBuildingBegin.SetActive(false);
        deckBuildingMain.SetActive(false);
        deckBuildingRemoveCard.SetActive(true);
        deckBuildingAddCard.SetActive(false);
    }

    public void ActivateDeckBuildingAddCard()
    {
        addCardInfo.text = $"Выбрать карту [-{ideaCountNeeded}    ]";

        deckBuildingBegin.SetActive(false);
        deckBuildingMain.SetActive(false);
        deckBuildingRemoveCard.SetActive(false);
        deckBuildingAddCard.SetActive(true);
    }

    public void PrepareCardsToSelect()
    {
        if (rareCards == null)
            rareCards = new List<Card>();
        if (equipmentCards == null)
            equipmentCards = new List<Card>();
        if (commonCards == null)
            commonCards = new List<Card>();

        rareCards.Clear();
        equipmentCards.Clear();
        commonCards.Clear();

        foreach (Card card in therapistCardsToSelect)
        {
            if (card.rarity == Rarity.Rare)
            {
                rareCards.Add(new Card(card));
            }
            else if (card.rarity == Rarity.Equipment)
            {
                equipmentCards.Add(new Card(card));
            }
            else
            {
                commonCards.Add(new Card(card));
            }
        }
    }

    public void RandomizeThreeCards(Transform cardsParent)
    {
        List<Card> rareAndEquipment = new List<Card>();
        rareAndEquipment.AddRange(rareCards);
        rareAndEquipment.AddRange(equipmentCards);

        Card leftCard = rareAndEquipment[Random.Range(0, rareAndEquipment.Count)];
        if (leftCard.rarity == Rarity.Rare)
        {
            rareCards.Remove(leftCard);
        }
        else if(leftCard.rarity==Rarity.Equipment)
        {
            equipmentCards.Remove(leftCard);
        }
        cardsParent.GetChild(0).GetComponent<CardController>().SetCardParametersToGameObject(leftCard);

        Card medianCard = commonCards[Random.Range(0, commonCards.Count)];
        commonCards.Remove(medianCard);
        cardsParent.GetChild(1).GetComponent<CardController>().SetCardParametersToGameObject(medianCard);

        Card rightCard;
        if (rareCards.Count > 0)
        {
            int ff = Random.Range(0, 2);
            if (ff == 0)
            {
                rightCard = rareCards[Random.Range(0, rareCards.Count)];
                rareCards.Remove(rightCard);
            }
            else
            {
                rightCard = commonCards[Random.Range(0, commonCards.Count)];
                commonCards.Remove(rightCard);
            }
        }
        else
        {
            rightCard = commonCards[Random.Range(0, commonCards.Count)];
            commonCards.Remove(rightCard);
        }
        cardsParent.GetChild(2).GetComponent<CardController>().SetCardParametersToGameObject(rightCard);
    }

    public void AddCardToTherapistDeckByIdeas(CardController cardGameObject)
    {
        Card selectedCard = cardGameObject.card;
        

        ideaController.AddIdea(-ideaCountNeeded);
        ideaCountNeeded++;

        therapistDeck.Add(selectedCard);

        PrepareCardsToSelect();
    }

    public void AddCardToTherapistDeck(CardController cardGameObject)
    {
        Card selectedCard = cardGameObject.card;


        if (selectedCard.cardType == CardTypes.Equipment)
        {
            foreach (Card card in therapistCardsToSelect)
            {
                if (card.name == selectedCard.name)
                {
                    therapistCardsToSelect.Remove(card);
                    break;
                }
            }
        }

        therapistDeck.Add(selectedCard);

        PrepareCardsToSelect();
    }

    public void RemoveCardFromTherapistDeck(CardController cardGameObject)
    {
        Card selectedCard = cardGameObject.card;

        therapistDeck.Remove(selectedCard);

        progressBarController.AddPoint(-pointCountNeeded);
        pointCountNeeded++;

        PrepareCardsToSelect();
    }


    public void SendBuiltDeckToTherapist()
    {
        TherapistManager.instance.AddToDeck(therapistDeck);
    }

    #endregion

    #region Private Methods

    private void AddCard(Card card)
    {
        CardController newCC = Instantiate(cardPrefab, content).GetComponent<CardController>();
        newCC.GetComponent<Button>().onClick.AddListener(() => SetAsSelected(newCC));
        newCC.SetCardParametersToGameObject(card);
        deckCardsControllers.Add(newCC);
    }

    #endregion
}
