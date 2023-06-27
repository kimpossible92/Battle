using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhobiaManager : MonoBehaviour
{
    public static PhobiaManager instance;

    public Phobia phobia;

    [SerializeField] private TMP_Text healthTmp;
    [SerializeField] private TMP_Text poisonTmp;
    [SerializeField] private TMP_Text blockTmp;
    [SerializeField] private TMP_Text weakTmp;


    [SerializeField] private TMP_Text phobiaNextAction;
    [SerializeField] UISpline effectSplinePath;

    private void Awake()
    {
        instance = this;
    }

    public void InitializePhobia()
    {
        phobia.Initialize();

        UpdateHealthBar();

        UpdateWeakTMP();

        UpdatePoisonTMP();

        UpdateBlockTMP();

        PrepareAttack();
    }

    public void StartTurn(UnityAction onDone =null)
    {
        if (IStartTurnHelper == null)
        {
            IStartTurnHelper = StartCoroutine(IStartTurn(onDone));
        }
    }

    public void MakeTheDamage(float damage)
    {
        if (phobia.vulnerablityCount > 0)
        {
            damage += Mathf.FloorToInt(damage * 0.5f);
        }
        Debug.Log($"<color=#ffa500ff>phobia's</color> gotten damage is {damage},  vulnerablityCount = {phobia.vulnerablityCount}, block = {phobia.block}");

        float damageHolder = damage;
        damage -= phobia.block;
        if (damage < 0f)
            damage = 0f;
        phobia.block -= damageHolder;
        if (phobia.block < 0)
            phobia.block = 0;

        UpdateBlockTMP();

        phobia.health -= damage;

        UIElementFlow uIElementFlow = Instantiate(CardManager.instance.effectElement, effectSplinePath.transform.parent).GetComponent<UIElementFlow>();
        uIElementFlow.FlowElement(effectSplinePath, $"-{damage}");

        if (phobia.health <= 0)
        {
            phobia.health = 0;
            if (IStartTurnHelper != null)
            {
                StopCoroutine(IStartTurnHelper);
            }

            GameManager.instance.LevelCompleted();
        }

        UpdateHealthBar();
    }

    public void AddVulnerablity(int value)
    {
        phobia.vulnerablityCount += value;
        if (phobia.vulnerablityCount < 0)
        {
            Debug.Log($"<color=#ffa500ff>phobia's</color> vulnerablity count is less or equal to 0 ");
            phobia.vulnerablityCount = 0;
        }
    }

    public bool IsPhobiaHaveVulnerablity()
    {
        if (phobia.vulnerablityCount > 0)
            return true;
        else
            return false;
    }

    public void AddWeakness(int value)
    {
        Debug.Log($"<color=#ffa500ff>phobia: </color> AddWeakness called. Value = {value}");
        phobia.weaknessStack += value;
        if (phobia.weaknessStack < 0)
        {
            Debug.Log($"<color=#ffa500ff>phobia's</color> weakness stack is less or equal to 0 ");
            phobia.weaknessStack = 0;
        }
        UpdateWeakTMP();
    }

    public void AddPoison(float poisonCount)
    {
        Debug.Log($"<color=cyan>poison added </color>added poison count " +
            $"= {poisonCount}, current spikes count = {phobia.poison} ");
        phobia.poison += Mathf.FloorToInt(poisonCount);
        UpdatePoisonTMP();
    }

    public void TurnWeaknessIntoPoison()
    {
        Debug.Log($"<color=#ffa500ff>The weakness turned into poison </color>");
        phobia.poison = phobia.weaknessStack;
        phobia.weaknessStack = 0;
        UpdateWeakTMP();
        UpdatePoisonTMP();
    }



    private void UpdateHealthBar()
    {
        //healthBarImage.fillAmount = Health / maxHealth;
        healthTmp.text = $"{Mathf.RoundToInt(phobia.health)}/{Mathf.RoundToInt(phobia.maximumHealth)}";
    }

    private void UpdateBlockTMP()
    {
        blockTmp.text = phobia.block.ToString();
    }

    public void UpdatePoisonTMP()
    {
        poisonTmp.text = phobia.poison.ToString();
    }

    private void UpdateWeakTMP()
    {
        weakTmp.text = phobia.weaknessStack.ToString();
    }

    private void PrepareAttack()
    {
        phobia.PrepareAttack();

        UpdateBlockTMP();
        phobiaNextAction.text = $"{phobia.attackCountInAStep}<color=#6b61fe>X</color>{phobia.attackForce}";
    }

    private void AttackATime()
    {
        Debug.Log($"<color=orange>PHOBIA: </color>Attackpatient with {phobia.attackForce} attack force, {phobia.weaknessStack} weaknessStack aaand {phobia.power} power");
        PatientManager.instance.MakeTheDamage(phobia.attackForce - phobia.weaknessStack);
        if (phobia.weaknessStack > 0)
            phobia.weaknessStack--;
        UpdateWeakTMP();
    }


    private Coroutine IStartTurnHelper;
    private IEnumerator IStartTurn(UnityAction onDone)
    {
        Debug.Log($"<color=orange>Turn Started</color>");

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < phobia.attackCountInAStep; i++)
        {
            AttackATime();
            yield return new WaitForSeconds(2.2f);
        }

        if (phobia.vulnerablityCount > 0)
            phobia.vulnerablityCount--;

        onDone?.Invoke();

        phobia.block = 0;
        PrepareAttack();
        IStartTurnHelper = null;

        Debug.Log($"<color=orange>Turn Ended</color>");
    }
}
