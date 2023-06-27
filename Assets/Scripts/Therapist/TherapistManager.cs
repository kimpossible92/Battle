using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TherapistManager: MonoBehaviour
{
    public static TherapistManager instance;

    public int therapistMaxAP = 5;
    public int therapistCurrentAP = 5;

    public List<Card> deck;
    public List<Card> hand;
    public List<Card> discard;

    [SerializeField]
    private TMP_Text cardsCountInDeck;
    [SerializeField]
    private TMP_Text actionPointsText;

    private void Awake()
    {
        instance = this;
    }

    public void InitializeTherapist()
    {
        if (deck == null)
            deck = new List<Card>();
        deck.Clear();

        if (hand == null)
            hand = new List<Card>();
        hand.Clear();

        if (discard == null)
            discard = new List<Card>();
        discard.Clear();

        //deck = new List<Card>(Cards.TherapistStandartCards());

        therapistMaxAP = 5;
        therapistCurrentAP = 5;
    }

    public void AddToDeck(List<Card> cards)
    {
        deck.AddRange(cards);
    }

    public void AddToDeck(Card card)
    {
        deck.Add(card);
    }

    public void PrepareNewTurn()
    {
        SetActionPoint(therapistMaxAP, therapistMaxAP);

        Discard();
        PullCard(4);
        Debug.Log("PrepareNewTurn");
        CardManager.instance.UpdateCards(true);
    }

    public void PullCard(int count)
    {
        if (deck.Count >= count)
        {
            for (int i = 0; i < count; i++)
            {
                PullACard();
            }
        }
        else
        {
            int deckCount = deck.Count;
            for (int i = 0; i < deckCount; i++)
            {
                PullACard();
            }
            CardManager.instance.SortDiscards(true);
            for (int i = 0; i < count - deckCount; i++)
            {
                PullACard();
            }
        }

    }

    public void Discard()
    {
        discard.AddRange(hand);
        hand.Clear();
        CardManager.instance.Discard(CardUIType.TherapistCard);
    }

    public void RemoveCardFromHand(Card card)
    {
        if (!hand.Contains(card))
        {
            Debug.Log($"<color=red>Can't</color> remove card({card.name}) from therapist in hand cards cause it doesnt contain that");
            return;
        }
        hand.Remove(card);
    }

    public void AddCardToHand(int index, Card card)
    {
        if (index < 0 || index >= hand.Count + 1)
        {
            Debug.Log($"<color=red>Can't</color> add card ({card.name}) to therapist in hand cards by index {index}");
        }
        hand.Insert(index, card);
    }

    public void SetActionPoint(int current,int max)
    {
        therapistCurrentAP = current;
        therapistMaxAP = max;
        actionPointsText.text = current.ToString() + "/" + max.ToString();
    }

    private void PullACard()
    {
        int index = Random.Range(0, deck.Count);
        ShuffleDeck();
        //Debug.Log(index);
        CardManager.instance.PullCardForTherapist(deck[index]);
        deck.RemoveAt(index);
        cardsCountInDeck.text = deck.Count.ToString();
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

    }
}
