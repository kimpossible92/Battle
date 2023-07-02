using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Spine;
using Spine.Unity;
using Spine.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class PatientManager : MonoBehaviour
{
    public static PatientManager instance;

    public Patient patient;


    public List<Card> deck;
    public List<Card> Hand;
    public List<Card> discard;

    [SerializeField] [NonReorderable] private List<Affect> affects;

    [SerializeField] private PatientAnimationController patientAnimationController;

    [SerializeField] private UISpline effectSplinePath;

    [SerializeField] private TMP_Text cardsCountInDeck;
    [SerializeField] private TMP_Text actionPointsText;

    [SerializeField] private TMP_Text healthTxtp;
    [SerializeField] private TMP_Text spikesTmp;
    [SerializeField] private TMP_Text armorTmp;
    [SerializeField] private TMP_Text blockTmp;

    [SerializeField] private int nextEffectCount = 1;

    [SerializeField] private float armor;

    [SerializeField] private float block;
    [SerializeField] private bool saveBlock;

    [SerializeField] private bool blockTheDamage;
    [SerializeField] private bool attackWhenDamaged;
    [SerializeField] private bool giveEnemyWeakness;

    private bool removeCurrentCardFromDeck = false;
    private int patientMaximumAPHolder;
    [SerializeField] bool UseCutInNewBattleSys=false;
    private void Awake()
    {
        instance = this;
        patientMaximumAPHolder = patient.patientMaximumActionPoints;
    }
    public void InitPatient()
    {
        if (affects == null)
            affects = new List<Affect>();
        affects.Clear();

        patient.InitializeNewCutScene();

        if (Hand == null)
            Hand = new List<Card>();
        Hand.Clear();
        if (discard == null)
            discard = new List<Card>();
        discard.Clear(); 
        deck = new List<Card>();
        if (UseCutInNewBattleSys)
        {
            UpdateSpikesTMP();//шипы

            UpdateArmorTMP();//броня

            UpdateBlockTMP();//блок
        }
        patient.patientMaximumActionPoints = patientMaximumAPHolder;
        patientAnimationController.SetPatientSitting();
        SetActionPoint();
    }

    public void InitializePatient()
    {
        if (affects == null)
            affects = new List<Affect>();
        affects.Clear();

        patient.Initialize();

        if (Hand == null)
            Hand = new List<Card>();
        Hand.Clear();

        if (discard == null)
            discard = new List<Card>();
        discard.Clear();

        deck = new List<Card>(Cards.instance.PatientStandartCards);
        UpdateHealthBar();

        UpdateSpikesTMP();

        UpdateArmorTMP();

        UpdateBlockTMP();

        patient.patientMaximumActionPoints = patientMaximumAPHolder;

        saveBlock = false;
        giveEnemyWeakness = false;

        patientAnimationController.SetPatientSitting();
        SetActionPoint();
    }

    public void PrepareNewTurn()
    {
        patient.patientActionPoints = patient.patientMaximumActionPoints;
        SetActionPoint();

        Discard();
        PullCard(3);

        CardManager.instance.UpdateCards(true);
    }

    public void StartTurn()
    {
        UIManager.instance.SetCanvasGroupActive(false);
        CardManager.instance.SetHandCardsInteractable(false);

        if (IStartTurnHelper == null)
        {
            IStartTurnHelper = StartCoroutine(IStartTurn());
        }
    }

    public void AddHealth(float health)
    {
        patient.health += health;
        UpdateHealthBar();
    }

    public void AddPower(float power)
    {
        patient.power += power;
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
            CardManager.instance.SortDiscards(false);
            for (int i = 0; i < count - deckCount; i++)
            {
                PullACard();
            }
        }

    }

    public void Discard()
    {
        discard.AddRange(Hand);
        Hand.Clear();
        CardManager.instance.Discard(CardUIType.PatientCard);
    }


    public void DiscardAndAddBlockForEach(int blockCount)
    {
        int handCardsCount = Hand.Count;
        Discard();
        AddBlock(blockCount * handCardsCount);
    }

    public void AddAffects(List<Affect> affects)
    {
        this.affects.AddRange(affects);
    }

    public void SaveBlock() 
    {
        Debug.Log($"<color=cyan>Block will be saved </color>");
        saveBlock = true;
    }

    public void ActivateGiveEnemyWeaknessOnHit()
    {
        giveEnemyWeakness = true;
    }
    public void ActivateAttackWhenDamaged()
    {
        attackWhenDamaged = true;
    }

    public void DoubleNextEffect()
    {
        nextEffectCount *= 2;
    }

    private void Attack()
    {
        if (giveEnemyWeakness)
        {
            PhobiaManager.instance.AddWeakness(1);
        }
        PhobiaManager.instance.MakeTheDamage(Mathf.RoundToInt(patient.attackForce + patient.power));
    }

    public void AddBlock(float block)
    {
        Debug.Log($"<color=cyan>Block added </color>value = {block} ");
        this.block += block;
        UpdateBlockTMP();
    }

    public float GetBlock()
    {
        Debug.Log($"<color=cyan>Block </color>value = {block}");
        return block;
    }

    public void BlockTheDamage()
    {
        Debug.Log($"<color=cyan>The damage will be blocked</color>");
        blockTheDamage = true;
    }

    public void AddArmor(float armor)
    {
        Debug.Log($"<color=cyan>Armor added </color>value = {armor} ");
        this.armor += armor;
        if (this.armor < 0)
            this.armor = 0;
        UpdateArmorTMP();
    }

    public void RemoveCardFromHand(Card card)
    {
        if (!Hand.Contains(card))
        {
            Debug.Log($"<color=red>Can't</color> remove card({card.name}) from patient in hand cards cause it doesnt contain that");
            return;
        }
        Hand.Remove(card);   
    }

    public void AddCardToHand(int index, Card card)
    {
        if (index < 0 || index >= Hand.Count+1)
        {
            Debug.Log($"<color=red>Can't</color> add card ({card.name}) to patient in hand cards by index {index}");
        }
        Hand.Insert(index,card);
    }

    public void RemoveCardFromDeck()//callonly after played
    {
        removeCurrentCardFromDeck = true;
    }

    public void AddSpikes(float spikesCount)
    {
        Debug.Log($"<color=cyan>spikess added </color>added spikes count " +
            $"= {spikesCount}, current spikes count = {patient.spikes} ");
        patient.spikes += Mathf.FloorToInt(spikesCount);
        UpdateSpikesTMP();
    }

    public void AddActionPoint(int toAP,int toMaxAP)
    {
        Debug.Log($"<color=cyan>AP added </color>added AP count " +
            $"= {toAP}, maxAP count = {toMaxAP} ");

        patient.patientActionPoints += toAP;
        patient.patientMaximumActionPoints += toMaxAP;

        SetActionPoint();
    }

    public void SetActionPoint()
    {
        actionPointsText.text = patient.patientActionPoints.ToString() + "/" + patient.patientMaximumActionPoints.ToString();
    }

    public void SetAttackForce(float force, int attackCount)
    {
        patient.attackForce = force;
        patient.attackCount += attackCount;
        //Debug.Log($"<color=teal>NPC:</color> attackForce =  {force}");
    }

    public void MakeTheDamage(float damage)
    {
        foreach (Affect affect in affects)
        {
            affect.Invoke(InPhobiaEventType.OnDefense);
        }

        if (blockTheDamage)
        {
            blockTheDamage = false;
            return;
        }

        float damageHolder = damage;
        damage -= block;
        block -= damageHolder;
        if (block < 0)
            block = 0;
        UpdateBlockTMP();

        damage -= armor;
        if (armor > 0)
            armor--;
        UpdateArmorTMP();

        if (damage > 0)
        {
            patientAnimationController.SetPatientGetHit();
            patient.health -= damage;
            UIElementFlow uIElementFlow = Instantiate(CardManager.instance.effectElement, effectSplinePath.transform.parent).GetComponent<UIElementFlow>();
            uIElementFlow.FlowElement(effectSplinePath, $"-{damage}");
            if (attackWhenDamaged)
            {
                Attack();
            }

            if (patient.spikes > 0)
            {
                SetAttackForce(patient.spikes, 1);
                Attack();

                patient.attackForce = 0f;
                patient.attackCount = 0;
                patient.spikes--;
            }
        }
        if (patient.health <= 0)
        {
            patient.health = 0;
            //GameManager.instance.LevelFailed();
        }
        UpdateSpikesTMP();
        UpdateHealthBar();
    }




    private void PullACard()
    {
        int index = Random.Range(0, deck.Count);

        CardManager.instance.PullCardForPatient(deck[index]);
        deck.RemoveAt(index);
        cardsCountInDeck.text = deck.Count.ToString();
    }

    private void UpdateHealthBar()
    {
        //healthBarImage.fillAmount = Health / maxHealth;
        healthTxtp.text = $"{Mathf.RoundToInt(patient.health)}/{Mathf.RoundToInt(patient.maximumHealth)}";
    }



    private Coroutine IStartTurnHelper;
    private IEnumerator IStartTurn()
    {
        if (!saveBlock)
            block = 0;
        else
            Debug.Log($"<color=cyan>Block saved</color>");


        Debug.Log($"<color=cyan>Turn Started</color>");
        patientAnimationController.SetPatientInteractAnimations();

        if (patient.phobia.poison > 0)
        {
            SetAttackForce(patient.phobia.poison, 1);
            Attack();
            patient.attackForce = 0f;
            patient.attackCount = 0;
            patient.phobia.poison--;
        }
        PhobiaManager.instance.UpdatePoisonTMP();

        foreach (Affect affect in affects)
        {
            affect.Invoke(InPhobiaEventType.OnTurnStart);
        }

        affects = Affects.UpdateAffects(affects);

        Debug.Log("foreach (Card card in Hand)");
        int cardCountInHand = Hand.Count;
        //while()
        for (int i = 0; i < cardCountInHand; i++)
        {
            if (patient.patientActionPoints < Hand[i].actionPoint)//card.actionPoint)
                break;

            string currentCardID = Hand[i].name;

            Debug.Log($"Current card is {currentCardID}");
            if (i + 1 < cardCountInHand)
                Debug.Log($"Next card will be {Hand[i + 1].name}");
            else
                Debug.Log($"And itsthe last card in Hand");

            for (int k = 0; k < nextEffectCount; k++)
            {
                affects.AddRange(Hand[i].GetAffects());
            }
            nextEffectCount = 1;

            //Debug.Break();
            ///////////////////////////////////////////////////////       affects.Update();
            yield return new WaitForFixedUpdate();
            //Debug.Break();

            Debug.Log($"<color=cyan>Step Started </color>with card -> {currentCardID}");// card.cardID}");

            patient.patientActionPoints -= Hand[i].actionPoint;
            SetActionPoint();

            bool patientCardPlayed = false;
            CardManager.instance.PlayPatientTopCard(() => 
            { 
                patientCardPlayed = true; 
            });


            yield return new WaitUntil(()=>patientCardPlayed);
            yield return new WaitForFixedUpdate();


            foreach (Affect affect in affects)
            {
                affect.Invoke(InPhobiaEventType.OnStepStart);
            }


            if (Hand.Count > 0)
            {
                if (Hand[i].cardType == CardTypes.Attack)
                {
                    foreach (Affect affect in affects)
                    {
                        affect.Invoke(InPhobiaEventType.OnAttack);
                    }

                    for (int j = 0; j < patient.attackCount; j++)
                    {
                        Attack();
                        patientAnimationController.SetPatientAttack();
                        Debug.Log($"<color=cyan>Attacked</color>_{currentCardID}_");
                        yield return new WaitForSeconds(1.5f);
                    }
                    patientAnimationController.SetPatientInteractAnimations();

                    patient.attackForce = 0f;
                    patient.attackCount = 0;
                }
            }
            foreach (Affect affect in affects)
            {
                affect.Invoke(InPhobiaEventType.OnStepEnd);
            }

            Debug.Log($"<color=cyan>Step Ended</color>_{currentCardID}_");
            if (!removeCurrentCardFromDeck && Hand.Count > 0)
            {
                Debug.Log($"<color=cyan>Patient: </color> The card <color=green>({currentCardID})</color> discarded");
                discard.Add(Hand[i]);
            }
            else
            {
                Debug.Log($"<color=cyan>Patient: </color> The card <color=red>({currentCardID})</color> removed from deck(not discard)");
                
                removeCurrentCardFromDeck = false;
            }
            Hand.Remove(Hand[i]);
            cardCountInHand = Hand.Count;
            i--;
        }

        Debug.Log($"<color=cyan>Turn Ended</color>");

        //RemovePlayedCards();

        PhobiaManager.instance.StartTurn(()=>
        {
            foreach (Affect affect in affects)
            {
                affect.Invoke(InPhobiaEventType.OnTurnEnd);
            }

            UIManager.instance.SetCanvasGroupActive(true);
            CardManager.instance.SetHandCardsInteractable(true);

            GameManager.instance.PlayNextTurn();
            patientAnimationController.SetPatientSitting();
        });


        IStartTurnHelper = null;
    }

    private void UpdateBlockTMP()
    {
        blockTmp.text = block.ToString();
    }

    private void UpdateArmorTMP()
    {
        armorTmp.text = armor.ToString();
    }

    private void UpdateSpikesTMP()
    {
        spikesTmp.text = patient.spikes.ToString();
    }

    private void RemovePlayedCards()
    {
        foreach (var playedCard in discard)
        {
            foreach (var cardInHand in Hand)
            {
                if (playedCard == cardInHand)
                {
                    Hand.Remove(playedCard);
                    break;
                }
            }
        }
    }
}
