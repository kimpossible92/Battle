using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New card", menuName = "ScriptableObjects/Card", order = 1)]
[Serializable]
public class Card : ScriptableObject
{
    public Sprite cardImageSprite;

    public CardTypes cardType;

    public string affectDescription;

    public int actionPoint;

    public Rarity rarity;

    public CardUIType cardBelonging = CardUIType.defaultCard;

    //[HideInInspector] public string cardID;


    [NonReorderable] public List<AffectHolder> affects;


    //private static int id;

    private List<Affect> _affects;

    public Card(Card card)
    {
        //cardID = card.cardID;//Guid.NewGuid().ToString("N");
        name = card.name;
        cardImageSprite = card.cardImageSprite;
        cardType = card.cardType;
        affects = card.affects;
        affectDescription = card.affectDescription;
        actionPoint = card.actionPoint;
        rarity = card.rarity;
        cardBelonging = card.cardBelonging;
    }

    public Card(string cardName, Sprite cardImageSprite, CardTypes cardType, List<AffectHolder> affects, string affectDescription, int actionPoint, Rarity rarity, CardUIType cardBelonging)
    {
        //id++;
        //cardID = cardName.ToLower() + $"_{id}";//Guid.NewGuid().ToString("N");
        //Debug.Log(cardID);
        name = cardName;
        this.cardImageSprite = cardImageSprite;
        this.cardType = cardType;
        this.affects = affects;
        this.affectDescription = affectDescription;
        this.actionPoint = actionPoint;
        this.rarity = rarity;
        this.cardBelonging = cardBelonging;
    }

    public List<Affect> GetAffects()
    {
        if (_affects == null)
            _affects = new List<Affect>();
        _affects.Clear();

        foreach (AffectHolder ah in affects)
        {
            switch (ah.affectType)
            {
                case AffectType.AddActionPoints:
                    ah.affect = Affects.AddActionPoints(Mathf.FloorToInt(ah.firstValue), Mathf.FloorToInt(ah.secondValue));
                    continue;
                case AffectType.AddBlock:
                    ah.affect = Affects.AddBlock(ah.firstValue);
                    continue;
                case AffectType.AddHealth:
                    ah.affect = Affects.AddHealth(ah.firstValue);
                    continue;
                case AffectType.AddPoison:
                    ah.affect = Affects.AddPoison(ah.firstValue);
                    continue;
                case AffectType.AddPower:
                    ah.affect = Affects.AddPower(ah.firstValue);
                    continue;
                case AffectType.AddSpikes:
                    ah.affect = Affects.AddSpikes(ah.firstValue);
                    continue;
                case AffectType.AddWeaknessOnDefense:
                    ah.affect = Affects.AddWeaknessOnDamage(ah.firstValue);
                    continue;
                case AffectType.Armor:
                    ah.affect = Affects.Armor(ah.firstValue);
                    continue;
                case AffectType.Attack:
                    ah.affect = Affects.Attack(ah.firstValue, Mathf.FloorToInt(ah.secondValue));
                    continue;
                case AffectType.AttackOnDefense:
                    ah.affect = Affects.AttackOnDefense(ah.firstValue);
                    continue;
                case AffectType.BlockTheDamage:
                    ah.affect = Affects.BlockTheDamage();
                    continue;
                case AffectType.Discard:
                    ah.affect = Affects.Discard();
                    continue;
                case AffectType.DiscardAndAddBlockForEach:
                    ah.affect = Affects.DiscardAndAddBlockForEach(Mathf.FloorToInt(ah.firstValue));
                    continue;
                case AffectType.DoubleNextAffect:
                    ah.affect = Affects.DoubleNextAffect();
                    continue;
                case AffectType.DoubleBlock:
                    ah.affect = Affects.DoubleTheBlock();
                    continue;
                case AffectType.DropKickWithoutAttack://skaaaaaaaaaa ya zabil
                    ah.affect = Affects.DropKickWithouAttack();
                    continue;
                case AffectType.Exhaust:
                    ah.affect = Affects.Exhaust();
                    continue;
                case AffectType.GiveEnemyWeaknessOnHit:
                    ah.affect = Affects.GiveEnemyWeaknessOnHit();
                    continue;
                case AffectType.MultiplyBlock:
                    ah.affect = Affects.MultiplyBlock(ah.firstValue);
                    continue;
                case AffectType.Power:
                    ah.affect = Affects.AddPower(ah.firstValue);
                    continue;
                case AffectType.PullCard:
                    ah.affect = Affects.PullCard(Mathf.FloorToInt(ah.firstValue));
                    continue;
                case AffectType.SaveBlock:
                    ah.affect = Affects.SaveBlock();
                    continue;
                case AffectType.SteelBlock:
                    ah.affect = Affects.SteelBlock(ah.firstValue);
                    continue;
                case AffectType.TurnWeaknessIntoPoison:
                    ah.affect = Affects.TurnWeaknessIntoPoison();
                    continue;
                case AffectType.Vulnerability:
                    ah.affect = Affects.Vulnerablity(Mathf.FloorToInt(ah.firstValue));
                    continue;
                case AffectType.Weakness:
                    ah.affect = Affects.Weakness(Mathf.FloorToInt(ah.firstValue));
                    continue;
                default:
                    continue;
            }
        }
        for (int i = 0; i < affects.Count; i++)
        {
            _affects.Add(affects[i].affect);
        }
        return _affects;
    }
}
