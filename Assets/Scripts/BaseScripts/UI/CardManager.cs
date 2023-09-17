using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [Header("Card Collections")]
    public List<Card> therapistCardsToSelect;
    public List<Card> therapistStandartCards;
    public List<Card> patientStandartCards;
    public Card psychosis;

    [Space]
    [Header("Common Fields")]
    public List<CardController> therapistCardsInHand;
    public List<CardController> patientCardsInHand;

    public CardController firstSelectedCard;
    public CardController secondSelectedCard;

    public GameObject bigCardUI;
    public GameObject effectElement;

    #region Serialized Fields 
    [Header("The Fight Game UIs")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RectTransform therapistCardsParent;
    [SerializeField] private RectTransform patientCardsParent;
    [SerializeField] private UISpline therapistHandSpline;
    [SerializeField] private UISpline patientHandSpline;

    [Space(40f)]
    [Header("Card Movement settings")]
    [SerializeField] private GameObject screeenParts;
    [SerializeField] private RectTransform patientDiscardRect;
    [SerializeField] private RectTransform therapistDiscardRect;
    [SerializeField] private float patientAnimationDuration = 1f;

    #endregion

    #region Private Fields

    [SerializeField] private ScreenPart screenPart;

    #endregion

    #region Unity Behaviour

    private void Awake()
    {
        instance = this;
    }

    #endregion

    #region Public Methods

    public void InitializeCardManager()
    {
        if (therapistCardsInHand == null)
            therapistCardsInHand = new List<CardController>();
        if (patientCardsInHand == null)
            patientCardsInHand = new List<CardController>();

        foreach (CardController card in therapistCardsInHand)
        {
            card.DestroyCard();
        }
        therapistCardsInHand.Clear();

        foreach (CardController card in patientCardsInHand)
        {
            card.DestroyCard();
        }
        patientCardsInHand.Clear();

        SetScreenPartsActive(false);
    }

    public void SetHandCardsInteractable(bool value)
    {
        foreach (CardController cct in therapistCardsInHand)
        {
            cct.SetInteractable(value);
        }
        foreach (CardController ccp in patientCardsInHand)
        {
            ccp.SetInteractable(value);
        }
    }

    public void SetScreenPartsActive(bool value)
    {
        screeenParts.SetActive(value);
    }

    public void SetScreenPart(int value)//0 - PatientHand, 1 - Middle, 2 - Therapist
    {
        switch (value)
        {
            case 0:
                screenPart = ScreenPart.PatientHand;
                break;
            case 1:
                screenPart = ScreenPart.Middle;
                break;
            case 2:
                screenPart = ScreenPart.Therapist;
                break;
            default:
                break;
        }
    }

    public ScreenPart GetScreenPart()
    {
        return screenPart;
    }

    public void PlayPatientTopCard(UnityAction onDone)
    {
        if (IPlayPatientTopCardHelper == null)
            IPlayPatientTopCardHelper = StartCoroutine(IPlayPatientTopCard(onDone)); Debug.Log("PlayPatientTopCard");
    }

    public void UpdateCards(bool smoothly)
    {
        List<CardController> sortedPatientCardsList = patientCardsInHand.OrderBy(o => o.index).ToList();
        patientCardsInHand = sortedPatientCardsList;

        List<CardController> sortedTherapistCardsList = therapistCardsInHand.OrderBy(o => o.index).ToList();
        therapistCardsInHand = sortedTherapistCardsList;

        foreach (CardController cardT in therapistCardsInHand)
        {
            cardT.UpdateCard(smoothly);
        }
        foreach (CardController cardP in patientCardsInHand)
        {
            cardP.UpdateCard(smoothly);
        }
        Debug.Log("UpdateCards");
    }

    public void UpdateCardsPosition(bool smoothly)
    {
        foreach (CardController cardT in therapistCardsInHand)
        {
            cardT.UpdateCardPosition(smoothly);
        }
        foreach (CardController cardP in patientCardsInHand)
        {
            cardP.UpdateCardPosition(smoothly);
        }
        Debug.Log("UpdateCardsPosition");
    }

    public void UpdateCardsScale(bool smoothly)
    {
        foreach (CardController cardT in therapistCardsInHand)
        {
            cardT.UpdateCardScale(smoothly);
        }
        foreach (CardController cardP in patientCardsInHand)
        {
            cardP.UpdateCardScale(smoothly);
        }
        Debug.Log("UpdateCardsScale");
    }

    public void PullCardForTherapist(Card card)
    {
        // int rIndex = Random.Range(0, therapistCardsInHand.Count + 1);
        int rIndex = therapistCardsInHand.Count;

        GameObject newCard = Instantiate(cardPrefab, therapistCardsParent);
        CardController newCardController = newCard.GetComponent<CardController>();


        foreach (CardController cardT in therapistCardsInHand)
        {
            if (cardT.index >= rIndex)
                cardT.index++;
        }

        //therapistCardsInHand.Insert(rIndex, newCardController);
        therapistCardsInHand.Add(newCardController);


        newCardController.SetCardParametersToGameObject(card);
        newCardController.SetCardCurrentType(CardUIType.TherapistCard);
        newCardController.SetCardType(CardUIType.TherapistCard);
        newCardController.SetCardMetrics(rIndex, therapistHandSpline);
        TherapistManager.instance.AddCardToHand(rIndex, newCardController.card);
        Debug.Log("PullCardForTherapist");
        //UpdateCards(true);
    }

    public void PullCardForPatient(Card card)
    {
        int rIndex = Random.Range(0, patientCardsInHand.Count + 1);
        //int rIndex = patientCardsInHand.Count;
        Debug.Log("PullCardForPatient");
        GameObject newCard = Instantiate(cardPrefab, patientCardsParent);
        CardController newCardController = newCard.GetComponent<CardController>();


        foreach (CardController cardP in patientCardsInHand)
        {
            if (cardP.index >= rIndex)
                cardP.index++;
        }

        // patientCardsInHand.Insert(rIndex, newCardController);
        patientCardsInHand.Add(newCardController);

        newCardController.SetCardParametersToGameObject(card);
        newCardController.SetCardCurrentType(CardUIType.PatientCard);
        newCardController.SetCardType(CardUIType.PatientCard);
        newCardController.SetCardMetrics(rIndex, patientHandSpline);
        PatientManager.instance.AddCardToHand(rIndex, newCardController.card);

        //UpdateCards(true);
    }

    //Поставить свою карту в случайное место в руке пациента
    public void PutCardInRandomPlace(CardController cardController)
    {
        if (TherapistManager.instance.therapistCurrentAP - 1 < 0)
        {
            cardController.UpdateCard(true);
            return;
        }

        int tookCardIndex = cardController.index;

        therapistCardsInHand.Remove(cardController);
        TherapistManager.instance.RemoveCardFromHand(cardController.card);

        foreach (CardController cardT in therapistCardsInHand)
        {
            if (cardT.index > tookCardIndex)
                cardT.index--;
        }

        int rIndex = Random.Range(0, patientCardsInHand.Count + 1);

        foreach (CardController cardP in patientCardsInHand)
        {
            if (cardP.index >= rIndex)
                cardP.index++;
        }

        patientCardsInHand.Insert(rIndex, cardController);

        cardController.transform.SetParent(patientCardsParent);
        cardController.SetCardCurrentType(CardUIType.PatientCard);
        cardController.SetCardMetrics(rIndex, patientHandSpline);
        PatientManager.instance.AddCardToHand(rIndex, cardController.card);

        UpdateCards(true);

        //Update TherapistActionPoints
        if (cardController.card.cardType == CardTypes.Equipment)
        {
            TherapistManager.instance.SetActionPoint(TherapistManager.instance.therapistCurrentAP - 1, TherapistManager.instance.therapistMaxAP - 1);
        }
        else
        {
            TherapistManager.instance.SetActionPoint(TherapistManager.instance.therapistCurrentAP - 1, TherapistManager.instance.therapistMaxAP);
        }
        Debug.Log("PutCardInRandomPlace");
    }

    //Поменять местами 2 карты (в руке пациента или один из руки пациентаа другой из руки игрока,
    //firstSelectedCard всегда находится в руке пациента, secondSelectedCard может быть или из руки пацента, или из руки теропевта)
    public void CheckSelectedCardUIs()
    {
        Debug.Log("CheckSelectedCardUIs");
        if (firstSelectedCard != null && secondSelectedCard != null)
        {
            if (firstSelectedCard.cardCurrentType != secondSelectedCard.cardCurrentType)
            {
                if (TherapistManager.instance.therapistCurrentAP - 3 < 0)
                {
                    ResetSelectedes();
                    return;
                }
            }
            else
            {
                if (TherapistManager.instance.therapistCurrentAP - 1 < 0)
                {
                    ResetSelectedes();
                    return;
                }
            }

            patientCardsInHand.Remove(firstSelectedCard);
            PatientManager.instance.RemoveCardFromHand(firstSelectedCard.card);

            if (secondSelectedCard.cardCurrentType == CardUIType.PatientCard)
            {
                patientCardsInHand.Insert(secondSelectedCard.index, firstSelectedCard);
                PatientManager.instance.AddCardToHand(secondSelectedCard.index, firstSelectedCard.card);

                patientCardsInHand.Remove(secondSelectedCard);
                PatientManager.instance.RemoveCardFromHand(secondSelectedCard.card);
            }
            else
            {
                therapistCardsInHand.Insert(secondSelectedCard.index, firstSelectedCard);
                TherapistManager.instance.AddCardToHand(secondSelectedCard.index, firstSelectedCard.card);

                therapistCardsInHand.Remove(secondSelectedCard);
                TherapistManager.instance.RemoveCardFromHand(secondSelectedCard.card);
            }

            patientCardsInHand.Insert(firstSelectedCard.index, secondSelectedCard);
            PatientManager.instance.AddCardToHand(firstSelectedCard.index, secondSelectedCard.card);

            int indexHolder = firstSelectedCard.index;
            UISpline handSpline = firstSelectedCard.handSpline;

            /////Set Patient card settings as Therapist
            firstSelectedCard.SetCardParametersToGameObject(firstSelectedCard.card);

            if (firstSelectedCard.cardCurrentType != secondSelectedCard.cardCurrentType &&
                firstSelectedCard.card.cardType != CardTypes.Equipment)
                firstSelectedCard.SetCardType(secondSelectedCard.cardCurrentType);

            firstSelectedCard.SetCardCurrentType(secondSelectedCard.cardCurrentType);
            firstSelectedCard.SetCardMetrics(secondSelectedCard.index, secondSelectedCard.handSpline);

            //Debug.Log($"secondSelectedCard.transform.parent name is {transformHolder.parent.gameObject.name}");
            firstSelectedCard.transform.SetParent(secondSelectedCard.transform.parent);


            /////Set Therapist card settings as Patient
            secondSelectedCard.SetCardParametersToGameObject(secondSelectedCard.card);

            if (CardUIType.PatientCard != secondSelectedCard.cardCurrentType &&
                secondSelectedCard.card.cardType != CardTypes.Equipment)
                secondSelectedCard.SetCardType(CardUIType.PatientCard);

            secondSelectedCard.SetCardCurrentType(CardUIType.PatientCard);
            secondSelectedCard.SetCardMetrics(indexHolder, handSpline);

            //Debug.Log($"transformHolder.parent name is {transformHolder.parent.gameObject.name}");
            secondSelectedCard.transform.SetParent(patientCardsParent);

            if (firstSelectedCard.cardCurrentType != CardUIType.PatientCard)
            {
                TherapistManager.instance.SetActionPoint(TherapistManager.instance.therapistCurrentAP - 3, TherapistManager.instance.therapistMaxAP);
                //NEED TO change deck belonging
            }
            else
            {
                TherapistManager.instance.SetActionPoint(TherapistManager.instance.therapistCurrentAP - 1, TherapistManager.instance.therapistMaxAP);
            }

            UpdateCards(true);
            //reset selectedes
            ResetSelectedes();
        }
    }

    public void AnimatePatientCardsBeforeDrop(Vector2 anchoredPosition, CardController cardController)
    {
        Debug.Log("AnimatePatientCardsBeforeDrop");
        if (cardController.card.cardType == CardTypes.Equipment)
            return;

        if (screenPart == ScreenPart.PatientHand)
        {
            float t = patientHandSpline.GetClosestT(anchoredPosition, patientCardsInHand.Count + 3);
            //float indexByT = t * patientCards.Count;

            float step = 1f / patientCardsInHand.Count;

            for (int i = 0; i < patientCardsInHand.Count; i++)
            {
                float tForCard = step * i;
                if (tForCard >= t)
                    tForCard += step;
                patientCardsInHand[i].MoveCardToPlace(0.5f, tForCard, () => { });
            }
        }
        else
        {
            float step = 1f / patientCardsInHand.Count - 1;
            for (int i = 0; i < patientCardsInHand.Count; i++)
            {
                float tForCard = step * i;
                //tForCard += step;
                patientCardsInHand[i].MoveCardToPlace(0.5f, tForCard, () => { });
            }
        }
    }

    //Добавить свою карту в руку пациента в любое место, по желанию игрока
    public void DropCardToPatientHand(CardController cardController)
    {
        Debug.Log("DropCardToPatientHand");
        if (cardController.card.cardType == CardTypes.Equipment)
        {
            cardController.UpdateCard(true);
            return;
        }
        else
        {
            if (TherapistManager.instance.therapistCurrentAP - 2 < 0)
            {
                cardController.UpdateCard(true);
                return;
            }
        }

        therapistCardsInHand.Remove(cardController);
        TherapistManager.instance.RemoveCardFromHand(cardController.card);
        foreach (CardController cardT in therapistCardsInHand)
        {
            if (cardT.index > cardController.index)
                cardT.index--;
        }

        //detect index
        float t = patientHandSpline.GetClosestT(cardController.GetComponent<RectTransform>().anchoredPosition, patientCardsInHand.Count + 3);
        //Debug.Log($"<color=maroon>UIController: </color> t of current card = {t} ");
        float step = 1f / patientCardsInHand.Count;
        int index = 0;
        for (int i = 0; i < patientCardsInHand.Count + 1; i++)
        {
            float tForCard = step * i;
            Debug.Log($"<color=maroon>UIController: </color> t For {i}-th Card = {tForCard} ");
            if (t > tForCard && t <= tForCard + step)
            {
                index = i + 1;
                Debug.Log($"<color=maroon>UIController: </color> index of current card = {index} ");
            }
        }

        cardController.SetCardParametersToGameObject(cardController.card);
        cardController.SetCardCurrentType(CardUIType.PatientCard);
        cardController.SetCardMetrics(index, patientHandSpline);

        //

        foreach (CardController cardP in patientCardsInHand)
        {
            if (cardP.index >= index)
                cardP.index++;
        }

        patientCardsInHand.Insert(index, cardController);
        PatientManager.instance.AddCardToHand(index, cardController.card);

        cardController.transform.SetParent(patientCardsParent);

        UpdateCards(true);

        TherapistManager.instance.SetActionPoint(TherapistManager.instance.therapistCurrentAP - 2, TherapistManager.instance.therapistMaxAP);
    }

    public void AddPsychosisToPatient()
    {
        //UpdateCardsUI(false);
        Debug.Log("AddPsychosisToPatient");
        int index = Random.Range(0, patientCardsInHand.Count + 1);

        GameObject newCard = Instantiate(cardPrefab, patientCardsParent);
        newCard.GetComponent<RectTransform>().localScale = 0.4f * Vector3.one;
        CardController newCardController = newCard.GetComponent<CardController>();
        newCardController.SetCardParametersToGameObject(Cards.instance.Psychosis);
        newCardController.SetCardCurrentType(CardUIType.PatientCard);
        newCardController.SetCardMetrics(index, patientHandSpline);

        //Debug.Log($"index = {index}");
        foreach (CardController cardP in patientCardsInHand)
        {
            if (cardP.index >= index)
                cardP.index++;
        }

        patientCardsInHand.Insert(index, newCardController);
        PatientManager.instance.AddCardToHand(index, newCardController.card);

        //Need to Insert and remove card in nessary abstract holders

        UpdateCards(true);
    }

    public void PlayEquipment(CardController cardController)
    {
        if (TherapistManager.instance.therapistCurrentAP - 1 < 0)
        {
            cardController.UpdateCard(true);
            return;
        }

        int tookCardIndex = cardController.index;

        therapistCardsInHand.Remove(cardController);
        TherapistManager.instance.RemoveCardFromHand(cardController.card);

        foreach (CardController cardT in therapistCardsInHand)
        {
            if (cardT.index > tookCardIndex)
                cardT.index--;
        }

        UpdateCards(true);

        //Update TherapistActionPoints
        TherapistManager.instance.SetActionPoint(TherapistManager.instance.therapistCurrentAP - 1, TherapistManager.instance.therapistMaxAP - 1);

        cardController.MoveCardToCenter(() =>
        {
            Debug.Log($"<color=cyan>Equimpent card {cardController.card.name} added</color>");
            //Initialize Equipment card effect
            List<Affect> equipmentAffects = new List<Affect>(cardController.card.GetAffects());

            foreach (Affect affect in equipmentAffects)
            {
                affect.InvokeAll();
            }
            IPlayPatientTopCardHelper = null;
        });
    }

    public void Discard(CardUIType cardUIType)
    {
        if (cardUIType == CardUIType.PatientCard)
        {
            SplinePoint sp = new SplinePoint(patientDiscardRect.anchoredPosition, Vector3.up);
            foreach (CardController cardC in patientCardsInHand)
            {
                cardC.MoveCardToDiscard(sp);
            }
            patientCardsInHand.Clear();
        }
        if (cardUIType == CardUIType.TherapistCard)
        {
            SplinePoint sp = new SplinePoint(therapistDiscardRect.anchoredPosition, Vector3.up);
            foreach (CardController cardC in therapistCardsInHand)
            {
                cardC.MoveCardToDiscard(sp);
            }
            therapistCardsInHand.Clear();
        }
    }

    public void SortDiscards(bool forTherapist)
    {
        string forWho = forTherapist ? "Therapist" : "Patient";
        Debug.Log($"<color=lightblue>SortDiscardCalled for {forWho}</color>");
        List<Card> patientDiscard = new List<Card>(PatientManager.instance.discard);
        foreach (Card card in patientDiscard)
        {
            if (card.cardBelonging == CardUIType.PatientCard && !forTherapist)
            {
                PatientManager.instance.deck.Add(card);
                PatientManager.instance.discard.Remove(card);
            }
            else if (card.cardBelonging == CardUIType.TherapistCard && forTherapist)
            {
                TherapistManager.instance.deck.Add(card);
                PatientManager.instance.discard.Remove(card);
            }
        }

        List<Card> therapistDiscard = new List<Card>(TherapistManager.instance.discard);
        foreach (Card card in therapistDiscard)
        {
            if (card.cardBelonging == CardUIType.PatientCard && !forTherapist)
            {
                PatientManager.instance.deck.Add(card);
                TherapistManager.instance.discard.Remove(card);
            }
            else if (card.cardBelonging == CardUIType.TherapistCard && forTherapist)
            {
                TherapistManager.instance.deck.Add(card);
                TherapistManager.instance.discard.Remove(card);
            }
        }
    }

    #endregion


    #region Private Methods

    private void ResetSelectedes()
    {
        foreach (CardController card in patientCardsInHand)
        {
            card.transform.GetChild(0).gameObject.SetActive(false);
        }
        foreach (CardController card in therapistCardsInHand)
        {
            card.transform.GetChild(0).gameObject.SetActive(false);
        }

        secondSelectedCard = null;
        firstSelectedCard = null;
    }

    private Coroutine IPlayPatientTopCardHelper;
    private IEnumerator IPlayPatientTopCard(UnityAction onDone)
    {
        if (patientCardsInHand.Count <= 0)
            yield break;
        CardController patientTopCard = patientCardsInHand[0];
        patientTopCard.transform.SetParent(patientCardsParent.parent);
        patientCardsInHand.RemoveAt(0);

        foreach (CardController card in patientCardsInHand)
        {
            card.index--;
        }

        UpdateCards(true);

        patientTopCard.MoveCardToCenter(() =>
        {
            onDone();
            IPlayPatientTopCardHelper = null;
        });
    }

    //private Coroutine IFadeMainGameUIHelper;
    //private IEnumerator IFadeMainGameUI()
    //{
    //    CanvasGroup cg = mainGameUI.GetComponent<CanvasGroup>();
    //    cg.alpha = 1f;

    //    float t = 0;
    //    float duration = 0.5f;

    //    while (t / duration < 1f)
    //    {
    //        cg.alpha = Mathf.Lerp(1f, 0f, t / duration);
    //        t += Time.fixedDeltaTime;
    //        yield return new WaitForFixedUpdate();
    //    }

    //    cg.alpha = 0f;
    //    IFadeMainGameUIHelper = null;
    //}

    #endregion
}
