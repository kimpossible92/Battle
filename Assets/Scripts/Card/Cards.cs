using System.Collections.Generic;
using UnityEngine;

public class Cards : MonoBehaviour
{
    public static Cards instance;

    public List<Card> TherapistCardsToSelect;

    public List<Card> TherapistStandartCards;

    public List<Card> PatientStandartCards;

    public Card Psychosis;

    private void Awake()
    {
        instance = this;
    }
}
