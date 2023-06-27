using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private GameObject bgOutline;
    [SerializeField] private List<CanvasGroup> bgs;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text ap;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text description;

    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private Vector3 cardMaxScale = Vector3.one * 0.8f;
    [SerializeField] private Vector3 cardMinScale = Vector3.one * 0.5f;

    [SerializeField] private float cardAnimationSpeed;

    [SerializeField] private float animDuration = 2f;

    #endregion

    public Card card;
    public CardUIType cardCurrentType;
    public int index;//first card is 0

    public UISpline handSpline;


    private RectTransform cardRect;
    private List<CardController> otherCardsUI;
    private Vector2 startDragPos;
    private float draggedTime = 0;
    private bool rightButtonClicked;
    private bool interactable = true;



    public void SetInteractable(bool value)
    {
        interactable = value;
    }

    public void OnPointerDown()
    {
        //Debug.Log($"card OnPointerDown {card.name}");
        if (!interactable)
            return;

        draggedTime = Time.time;
        startDragPos = cardRect.anchoredPosition;
        if (Input.GetMouseButton(1))
            rightButtonClicked = true;
    }

    public void OnPointerUP()
    {
        //Debug.Log($"card OnPointerUp {card.name}");
        if (!interactable)
            return;

        float deltaT = Mathf.Abs(Time.time - draggedTime);
        if (deltaT < 0.3f && Vector2.Distance(startDragPos, cardRect.anchoredPosition) < 18f)
            OnClicked(rightButtonClicked);
        else
        {
            if (CardManager.instance.firstSelectedCard != null)
            {
                CardManager.instance.firstSelectedCard.transform.GetChild(0).gameObject.SetActive(false);
                CardManager.instance.firstSelectedCard = null;
            }
            if (CardManager.instance.secondSelectedCard != null)
            {
                CardManager.instance.secondSelectedCard.transform.GetChild(0).gameObject.SetActive(false);
                CardManager.instance.secondSelectedCard = null;
            }
        }
        draggedTime = 0f;
    }

    public void OnDrag()
    {
        if (cardCurrentType != CardUIType.TherapistCard || !interactable)
            return;

        //Debug.Log($"{card.cardID} Draging");
        StopCardMoving();

        //cardRect.position = Vector3.Lerp(cardRect.position, Input.mousePosition, Time.fixedDeltaTime * 20f);

        if(CardManager.instance.GetScreenPart() == ScreenPart.PatientHand)
        {
            Vector2 ap = Input.mousePosition;
            //Debug.Log($"ap before = { ap }");
            ap.y *=  1080f / (float)Screen.height;
            ap.y -= 1080f / 2f;
            ap.x *= 1920f / (float)Screen.width;
            ap.x -= 1920f / 2f;
            //Debug.Log($"ap after = {ap }");

            CardManager.instance.AnimatePatientCardsBeforeDrop(ap,this);
        }
    }

    public void OnDragBegin()
    {
        if (cardCurrentType != CardUIType.TherapistCard || !interactable)
            return;

        CardManager.instance.SetScreenPartsActive(true);

        //Debug.Log($"{card.cardID} Drag Begin");
        StopCardMoving();

        StartFollowMouse();
    }

    public void OnDragEnd()
    {
        if (cardCurrentType != CardUIType.TherapistCard || !interactable)
            return;

        StopFollowMouse();
        CardManager.instance.SetScreenPartsActive(false);

        if (card.cardType != CardTypes.Equipment)
        {
            switch (CardManager.instance.GetScreenPart())
            {
                case ScreenPart.PatientHand:
                    CardManager.instance.DropCardToPatientHand(this);
                    break;
                //case ScreenPart.Middle:
                //    CardManager.instance.PutCardInRandomPlace(this);
                //    break;
                case ScreenPart.Therapist:
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (CardManager.instance.GetScreenPart())
            {
                case ScreenPart.PatientHand:
                case ScreenPart.Middle:
                    CardManager.instance.PlayEquipment(this);
                    break;
                default:
                    break;
            }
        }
    }

    public void AnimateZoomIn()
    {
        if (!interactable)
            return;

        foreach (var item in otherCardsUI)
        {
            item.AnimateZoomOut();
        }

        ScaleCardIn(animDuration);
        SplinePoint sp = handSpline.GetSplinePoint(GetCardT());
        sp.Position += sp.Normal * 40f;
        sp.Normal = Vector3.up;

        MoveCardTo(animDuration, sp);
    }

    public void AnimateZoomOut()
    {
        if (!interactable)
            return;

        ScaleCardOut(animDuration);
        MoveCardToPlace(animDuration);
    }




    public void SetCardParametersToGameObject(Card card)
    {
        SetInteractable(true);
        gameObject.name = card.name;
        this.card = card;

        if (card.cardImageSprite == null)
        {
            cardImage.enabled = false;
        }
        else
        {
            cardImage.sprite = card.cardImageSprite;
            cardImage.enabled = true;
        }

        if (card.cardType == CardTypes.Equipment)
        {
            ap.text = "";
        }
        else
        {
            ap.text = $"{card.actionPoint}";
        }
        cardName.text = card.name;
        //2
        foreach (CanvasGroup cg in bgs)
        {
            cg.alpha = 0f;
        }
        if (card.cardType == CardTypes.Equipment)
            bgs[2].alpha = 1f;
        else
            bgs[0].alpha = 1f;

        description.text = card.affectDescription;
    }

    public void SetCardCurrentType(CardUIType cardUIType)
    {
        cardCurrentType = cardUIType;
    }

    public void SetCardType(CardUIType cardUIType)
    {
        if (card.cardType == CardTypes.Equipment)
            return;

        //CardUIType cardTypeHolder = card.cardBelonging;
        //card.cardBelonging = cardUIType;

        CanvasGroup turnOffcg = null;
        CanvasGroup turnOncg = null;
        if (card.cardBelonging == CardUIType.TherapistCard)
        {
            turnOffcg = bgs[0];
            turnOncg = bgs[3];

            ChangeCardColor(turnOncg, turnOffcg);
        }
        else if(card.cardBelonging == CardUIType.PatientCard)
        {
            turnOncg = bgs[0];
            turnOffcg = bgs[3];

            ChangeCardColor(turnOncg, turnOffcg);
        }
        else
        {
            foreach (CanvasGroup cg in bgs)
            {
                cg.alpha = 0f;
            }
            if (cardUIType == CardUIType.PatientCard)
                bgs[3].alpha = 1f;
            else
                bgs[0].alpha = 1f;
        }

        card.cardBelonging = cardUIType;
    }

    public void SetCardMetrics(CardController newCardUI)
    {
        SetCardMetrics(newCardUI.index, newCardUI.handSpline);
    }

    public void SetCardMetrics(int index, UISpline handSpline)
    {
        cardRect = GetComponent<RectTransform>();
        cardRect.SetSiblingIndex(index);
        //enableActions = true;
        this.index = index;
        this.handSpline = handSpline;
    }
   
    public void DestroyCard()
    {
        Destroy(this.gameObject);
    }

    public void UpdateCard(bool smoothly)
    {
        if(otherCardsUI==null)
            otherCardsUI = new List<CardController>();
        otherCardsUI.Clear();

        List<CardController> currentCards = cardCurrentType == CardUIType.PatientCard ? CardManager.instance.patientCardsInHand : CardManager.instance.therapistCardsInHand;
        foreach (CardController cardC in currentCards)
        {
            if (cardC.index != index)
                otherCardsUI.Add(cardC);
        }

        UpdateCardPosition(smoothly);
        UpdateCardScale(smoothly);
    }

    public void UpdateCardPosition(bool smoothly)
    {
        if (smoothly)
        {
            MoveCardToPlace(animDuration);
        }
        else
        {
            MoveCardToPlace(0);
        }
    }

    public void UpdateCardScale(bool smoothly)
    {
        if (smoothly)
        {
            ScaleCardOut(animDuration);
        }
        else
        {
            ScaleCardOut(0);
        }
    }

    public void MoveCardToPlace(float duration, UnityAction onDone = null)
    {
        MoveCardTo(duration, handSpline.GetSplinePoint(GetCardT()), onDone);
    }

    public void MoveCardToPlace(float duration,float t, UnityAction onDone = null)
    {
        MoveCardTo(duration, handSpline.GetSplinePoint(t), onDone);
    }

    public void MoveCardToCenter(UnityAction onDone)
    {
        SetInteractable(false);

        SplinePoint point = new SplinePoint(Vector3.zero, Vector3.up);

        MoveCardTo(animDuration, point, ()=> 
        {
            FadeCardIn(onDone);
        });
    }
    public void MoveCardToDiscard(SplinePoint point)
    {
        SetInteractable(false);
        MoveCardTo(animDuration, point, () =>
        {
            FadeCardIn();
        });
    }

    public void FadeCardIn(UnityAction onDone = null)
    {
        isCardFading = true;
        if (IFadeCardHelper == null)
            IFadeCardHelper = StartCoroutine(IFadeCard(0f, animDuration, () =>
            {
                isCardFading = false;
                onDone?.Invoke();
                DestroyCard();
            }));
    }

    public void SetSelect()
    {
        TrySelect();
    }

    public bool TrySelect()
    {
        bool active = !transform.GetChild(0).gameObject.activeSelf;
        transform.GetChild(0).gameObject.SetActive(active);
        return active;
    }

    public void SetSelect(bool active)
    {
        transform.GetChild(0).gameObject.SetActive(active);
    }

    #region Private Methods

    private void OnClicked(bool isRightButton)
    {
        if(!isRightButton)
        {
            if (cardCurrentType == CardUIType.TherapistCard)
            {
                foreach (var cardUI in otherCardsUI)
                {
                    cardUI.SetSelect(false);
                }
            }

            if (TrySelect())
            {
                SetAsSelectedCardUI(this, index);
                CardManager.instance.CheckSelectedCardUIs();
            }
            else
            {
                SetAsSelectedCardUI(null, index);
            }
        }
        else
        {
            CardManager.instance.bigCardUI.SetActive(true);
            CardManager.instance.bigCardUI.transform.GetChild(0).GetComponent<CardController>().SetCardParametersToGameObject(card);
        }
        rightButtonClicked = false;
    }

    private void SetAsSelectedCardUI(CardController cardUI, int index)
    {
        if (cardUI == null)
        {
            if (cardCurrentType == CardUIType.PatientCard)
            {
                if (CardManager.instance.firstSelectedCard != null)
                {
                    if (CardManager.instance.firstSelectedCard.index == index)
                    {
                        CardManager.instance.firstSelectedCard = null;
                    }
                }
                
                if(CardManager.instance.secondSelectedCard != null)
                {
                    if (CardManager.instance.secondSelectedCard.index == index)
                    {
                        CardManager.instance.secondSelectedCard = null;
                    }
                }
            }
            else
            {
                if (CardManager.instance.secondSelectedCard != null)
                {
                    if (CardManager.instance.secondSelectedCard.cardCurrentType == CardUIType.TherapistCard)
                    {
                        CardManager.instance.secondSelectedCard = null;
                    }
                }
            }
        }
        else
        {
            if (cardCurrentType == CardUIType.PatientCard)
            {
                if (CardManager.instance.firstSelectedCard == null)
                {
                    CardManager.instance.firstSelectedCard = cardUI;
                }
                else
                {
                    CardManager.instance.secondSelectedCard = cardUI;
                }
            }
            else
            {
                CardManager.instance.secondSelectedCard = cardUI;
            }
        }
    }

    private float GetCardT()
    {
        float step, t;
        int currentCardsCount = cardCurrentType == CardUIType.PatientCard ? CardManager.instance.patientCardsInHand.Count : CardManager.instance.therapistCardsInHand.Count;
        if (currentCardsCount > 1)
        {
            step = 1f / (currentCardsCount - 1f);
            t = index * step;
        }
        else
            t = 0.5f;
        return t;
    }

    private void MoveCardTo(float duration, SplinePoint point, UnityAction OnDone=null)
    {
        StopCardMoving();
        isCardMoving = true;

        if (gameObject.activeInHierarchy)
            IMoveCardToHelper = StartCoroutine(IMoveCardTo(point, duration, OnDone));
        else
        {
            cardRect.anchoredPosition = point.Position;
            cardRect.rotation = Quaternion.LookRotation(Vector3.forward, point.Normal);
        }
    }

    private void StopCardMoving()
    {
        isCardMoving = false;
        if (IMoveCardToHelper != null)
            StopCoroutine(IMoveCardToHelper);
    }

    private void ScaleCardIn(float duration, UnityAction onDone = null)
    {
        StopCardScaling();
        isCardScaling = true;

        cardRect.SetAsLastSibling();

        IScaleCardToHelper = StartCoroutine(IScaleCardTo(cardMaxScale, duration, onDone));
    }

    private void ScaleCardOut(float duration, UnityAction onDone = null)
    {
        StopCardScaling();
        isCardScaling = true;

        foreach (CardController cardUI in otherCardsUI)
        {
            cardUI.cardRect.SetSiblingIndex(cardUI.index);
        }
        cardRect.SetSiblingIndex(index);

        if (gameObject.activeInHierarchy)
            IScaleCardToHelper = StartCoroutine(IScaleCardTo(cardMinScale, duration, onDone));
        else
            cardRect.localScale = cardMinScale;
    }

    private void StopCardScaling()
    {
        isCardScaling = false;
        if (IScaleCardToHelper != null)
            StopCoroutine(IScaleCardToHelper);
    } 

    private void StartFollowMouse()
    {
        followMouse = true;
        if (IFollowMouseHelper == null)
            IFollowMouseHelper = StartCoroutine(IFollowMouse());
    }

    private void StopFollowMouse()
    {
        followMouse = false;
        if (IFollowMouseHelper != null)
            StopCoroutine(IFollowMouseHelper);
        IFollowMouseHelper = null;
    }

    private Vector3 MousePosToCanvasPos(Vector3 mousePosition)
    {
        Vector3 card_sCanvasSizes = UIManager.instance.GetCard_sCanvas().sizeDelta;

        float x_T = Mathf.InverseLerp(0f, Screen.width, mousePosition.x);
        float y_T = Mathf.InverseLerp(0f, Screen.height, mousePosition.y);

        Vector3 canvasPos = Vector3.zero;
        canvasPos.x = Mathf.Lerp(-card_sCanvasSizes.x / 2f, card_sCanvasSizes.x / 2f, x_T);
        canvasPos.y = Mathf.Lerp(-card_sCanvasSizes.y / 2f, card_sCanvasSizes.y / 2f, y_T);

        return canvasPos;
    }

    private void ChangeCardColor(CanvasGroup turnOn, CanvasGroup turnOff)
    {
        if (IChangeColorHelper != null)
            StopCoroutine(IChangeColorHelper);

        if (gameObject.activeInHierarchy)
        {
            changeColor = true;
            IChangeColorHelper = StartCoroutine(IChangeColor(turnOn, turnOff));
        }
        else
        {
            for (int i = 1; i < 3; i++)
            {
                bgs[i].alpha = 0f;
            }
            turnOff.alpha = 0f;
            turnOn.alpha = 1f;
        }
    }

    #endregion

    #region Coroutine

    private bool changeColor = false;
    private Coroutine IChangeColorHelper;
    private IEnumerator IChangeColor(CanvasGroup turnOn, CanvasGroup turnOff)
    {
        for (int i = 1; i < 3; i++)
        {
            bgs[i].alpha = 0f;
        }

        float t = 0f;
        while (changeColor)
        {
            turnOff.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
            turnOn.alpha = Mathf.Lerp(0f, 1f, t / 0.5f);

            t += Time.fixedDeltaTime;
            if (t > 0.5f)
                changeColor = false;
            yield return new WaitForFixedUpdate();
        }

        turnOff.alpha = 0f;
        turnOn.alpha = 1f;

        IChangeColorHelper = null;
    }

    private bool followMouse = false;
    private Coroutine IFollowMouseHelper;
    private IEnumerator IFollowMouse()
    {
        while (followMouse)
        {
            //Debug.Log(Input.mousePosition);
            cardRect.anchoredPosition = Vector3.Lerp(cardRect.anchoredPosition, MousePosToCanvasPos(Input.mousePosition), Time.fixedDeltaTime * 20f);
            cardRect.rotation = Quaternion.Lerp(cardRect.rotation, Quaternion.LookRotation(Vector3.forward, Vector3.up), Time.fixedDeltaTime * 20f);

            yield return new WaitForFixedUpdate();
        }
    }

    //cardRect.position = Vector3.Lerp(cardRect.position, Input.mousePosition, Time.fixedDeltaTime* 20f);
    private bool isCardScaling = false;
    private Coroutine IScaleCardToHelper;
    private IEnumerator IScaleCardTo(Vector3 scale, float duration, UnityAction onDone = null)
    {
        if (duration <= 0f)
        {
            //Debug.Log($"<color=yellow>Card {card.cardID}'s MoveCardTo animation duration = {duration}</color>");
            duration = 0.1f;
        }

        float t = 0f;

        Vector3 startScale = cardRect.localScale;

        while (isCardScaling)
        {
            //yield return new WaitUntil(() => !IsBeingDrag);
            cardRect.localScale = Vector2.Lerp(startScale, scale, t / duration);

            t += Time.fixedDeltaTime;

            if (t / duration >= 1f)
                isCardScaling = false;

            yield return new WaitForFixedUpdate();
        }

        cardRect.localScale = scale;

        if (onDone != null)
            onDone();
        yield return null;
        IScaleCardToHelper = null;
    }


    private bool isCardMoving = false;
    private Coroutine IMoveCardToHelper;
    private IEnumerator IMoveCardTo(SplinePoint point, float duration, UnityAction onDone = null)
    {
        if (duration <= 0f)
        {
            //Debug.Log($"<color=yellow>Card {card.cardID}'s MoveCardTo animation duration = {duration}</color>");
            duration = 0.1f;
        }

        float t = 0f;
        Vector2 startPos = cardRect.anchoredPosition;
        Quaternion startRot = cardRect.rotation;

        while (isCardMoving)
        {
            //yield return new WaitUntil(() => !IsBeingDrag);
            cardRect.anchoredPosition = Vector2.Lerp(startPos, point.Position, t / duration);
            cardRect.rotation = Quaternion.Lerp(startRot, Quaternion.LookRotation(Vector3.forward, point.Normal), t / duration);

            t += Time.fixedDeltaTime;

            if (t / duration >= 1f)
                isCardMoving = false;
            yield return new WaitForFixedUpdate();
        }

        cardRect.anchoredPosition = point.Position;
        cardRect.rotation = Quaternion.LookRotation(Vector3.forward, point.Normal);

        onDone?.Invoke();

        yield return null;
        IMoveCardToHelper = null;
    }


    private bool isCardFading = false;
    private Coroutine IFadeCardHelper;
    private IEnumerator IFadeCard(float value, float duration, UnityAction onDone = null)
    {
        if (duration <= 0f)
        {
            //Debug.Log($"<color=yellow>Card {card.cardID}'s IFadeCard animation duration = {duration}</color>");
            duration = 0.1f;
        }

        float t = 0f;
        float startFadeValue = canvasGroup.alpha;

        while (isCardFading)
        {
            canvasGroup.alpha = Mathf.Lerp(startFadeValue, value, t / duration);

            t += Time.fixedDeltaTime;

            if (t / duration >= 1f)
                isCardFading = false;

            yield return new WaitForFixedUpdate();
        }

        canvasGroup.alpha = value;

        if (onDone != null)
            onDone();
        yield return null;
        IFadeCardHelper = null;
    }

    #endregion
}
