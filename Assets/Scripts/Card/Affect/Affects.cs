using System;
using System.Collections.Generic;
using UnityEngine;

public static class Affects
{
    /// <summary>
    /// ---Существа с уязвимостью получают +50% урона․
    /// ---Примичение.---В конце хода снижается на 1․
    /// </summary>
    public static Affect Vulnerablity(int count)
    {
        Affect affect = new Affect($"Vulnerablity_{count}_{DateTime.UtcNow}".ToLower());
        Debug.Log($"Effect: Vulnerable creatures take +50% damage\nNote: Decrease by 1 at end of turn");
        affect.OnStepStart = new InPhobiaAction($"Vulnerablity_{count}".ToLower(),
            () => { PhobiaManager.instance.AddVulnerablity(count); }, false);
        return affect;
    }


    /// <summary>
    /// ---Существа со слабостью наносят каждой атакой - x урона․
    /// ---Примичение.---Уменьшается на 1 каждый раз когда наносишь урон․ 
    /// </summary>
    public static Affect Weakness(int weaknesStack)
    {
        Affect affect = new Affect($"Weakness_{weaknesStack}".ToLower());
        affect.OnStepStart = new InPhobiaAction($"Weakness_{weaknesStack}".ToLower(),
            () => PhobiaManager.instance.AddWeakness(weaknesStack), false);
        return affect;
    }


    /// <summary>
    /// ---Снижает урон на свое количество.
    /// ---Примичение.---Полностью пропадает в начале хода.
    /// </summary>
    public static Affect AddBlock(float block)//??
    {
        Affect affect = new Affect($"Add Block_{block}".ToLower());
        affect.OnStepStart = new InPhobiaAction($"Add Block_{block}".ToLower(),
            () => PatientManager.instance.AddBlock(block), false);
        ////affect.OnTurnEnd.Add(new InPhobiaAction($"Block_{-block}".ToLower(), () => PatientManager.instance.AddBlock(-block), false));
        return affect;
    }

    public static Affect MultiplyBlock(float multiplier)
    {
        Affect affect = new Affect($"Multiply Block_{multiplier}".ToLower());
        affect.OnStepStart = new InPhobiaAction($"Multiply Block_{multiplier}".ToLower(),
            () => PatientManager.instance.AddBlock(PatientManager.instance.GetBlock() *
            (multiplier - 1)), false);
        ////affect.OnTurnEnd.Add(new InPhobiaAction($"Block_{-block}".ToLower(), () => PatientManager.instance.AddBlock(-block), false));
        return affect;
    }

    /// <summary>
    /// ---Снижает урон на свое количество.
    /// ---Примичение.---Снижается на 1 каждый раз, когда получаешь урон по хп.
    /// </summary>
    public static Affect Armor(float armor)
    {
        Affect affect = new Affect($"Armor_{armor}".ToLower());
        affect.OnTurnEnd = new InPhobiaAction($"Armor_{armor}".ToLower(),
            () => PatientManager.instance.AddArmor(armor), false);
        return affect;
    }


    /// <summary>
    /// ---Карта пропадает из игры до конца боя.
    /// </summary>
    public static Affect Exhaust()
    {
        Affect affect = new Affect($"Exhaust".ToLower());
        affect.OnStepEnd = new InPhobiaAction($"Exhaust".ToLower(),
            () => PatientManager.instance.RemoveCardFromDeck(), false);
        return affect;
    }


    /// <summary>
    /// ---Увеличивает урон на x, где x - значение силы.
    /// </summary>
    //public static Affect Power(float damage)
    //{
    //    Affect affect = new Affect($"Power_{damage}".ToLower());
    //    affect.OnStepStart = new InPhobiaAction($"Power_{damage}".ToLower(),
    //        () => PatientManager.instance.patient.attackForce += damage, false);
    //    return affect;
    //}

    public static Affect AddWeaknessOnDamage(float weaknessStack)
    {
        Affect affect = new Affect($"AddWeaknessOnDmage_{weaknessStack}".ToLower());
        affect.OnDefense = new InPhobiaAction($"AddWeaknessOnDmage_{weaknessStack}".ToLower(),
            () => PhobiaManager.instance.AddWeakness(Mathf.FloorToInt(weaknessStack)), true);

        return affect;
    }

    public static Affect SaveBlock()
    {
        Affect affect = new Affect($"SaveBlock".ToLower());
        affect.OnStepStart = new InPhobiaAction($"SaveBlock".ToLower(),
            () => PatientManager.instance.SaveBlock(), false);
        return affect;
    }

    public static Affect AddHealth(float healthAmount)
    {
        Affect affect = new Affect($"Addhealth_{healthAmount}".ToLower());
        affect.OnStepStart = new InPhobiaAction($"Addhealth_{healthAmount}".ToLower(),
            () => PatientManager.instance.AddHealth(healthAmount), false);
        return affect;
    }

    public static Affect AddPoison(float poisons)
    {
        Affect affect = new Affect($"AddPoison_{poisons}".ToLower());
        affect.OnStepStart = new InPhobiaAction($"AddPoison_{poisons}".ToLower(),
            () => PhobiaManager.instance.AddPoison(poisons), false);
        return affect;
    }

    public static Affect AddPower(float power)
    {
        Affect affect = new Affect($"AddPower_{power}".ToLower());
        affect.OnStepStart = new InPhobiaAction($"AddPower_{power}".ToLower(),
            () => PatientManager.instance.AddPower(power), false);
        return affect;
    }

    public static Affect AddSpikes(float spikes)
    {
        Affect affect = new Affect($"AddSpikes_{spikes}".ToLower());
        affect.OnStepStart = new InPhobiaAction($"AddSpikes_{spikes}".ToLower(),
            () => PatientManager.instance.AddSpikes(spikes), false);
        return affect;
    }

    public static Affect BlockTheDamage()
    {
        Affect affect = new Affect($"BlockTheDamage".ToLower());
        affect.OnStepStart = new InPhobiaAction($"BlockTheDamage".ToLower(),
            () => PatientManager.instance.BlockTheDamage(), false);
        return affect;
    }

    public static Affect GiveEnemyWeaknessOnHit()
    {
        Affect affect = new Affect($"AttackWhenGetBlock".ToLower());
        affect.OnStepStart = new InPhobiaAction($"AttackWhenGetBlock".ToLower(),
            () => PatientManager.instance.ActivateGiveEnemyWeaknessOnHit(), false);
        return affect;
    }

    public static Affect SteelBlock(float value)
    {
        Affect affect = new Affect($"SteelBlock_{value}".ToLower());
        affect.OnTurnEnd = new InPhobiaAction($"SteelBlock_{value}".ToLower(),
            () => PatientManager.instance.AddBlock(value), true);
        return affect;
    }
    public static Affect Discard()
    {
        Affect affect = new Affect($"Discard".ToLower());
        affect.OnStepStart = new InPhobiaAction($"Discard".ToLower(),
            () => PatientManager.instance.Discard(), false);
        return affect;
    }

    public static Affect DiscardAndAddBlockForEach(int blockCount)
    {
        Affect affect = new Affect($"DiscardAndAddBlockForEach_{blockCount}".ToLower());
        affect.OnStepStart = new InPhobiaAction($"DiscardAndAddBlockForEach_{blockCount}".ToLower(),
            () => PatientManager.instance.DiscardAndAddBlockForEach(blockCount), false);
        return affect;
    }

    public static Affect PullCard(int count)
    {
        Affect affect = new Affect($"PullCard_{count}".ToLower());
        affect.OnStepStart = new InPhobiaAction($"PullCard_{count}".ToLower(),
            () => PatientManager.instance.PullCard(count), false);
        return affect;
    }

    public static Affect AddActionPoints(int _APvalue, int maxAPvalue)
    {
        Affect affect = new Affect($"AddActionPoint_{_APvalue}_{maxAPvalue}".ToLower());
        affect.OnStepStart = new InPhobiaAction($"AddActionPoint_{_APvalue}_{maxAPvalue}".ToLower(),
            () => PatientManager.instance.AddActionPoint(_APvalue, maxAPvalue), false);
        return affect;
    }

    public static Affect Attack(float attackForce, int attackCount)
    {
        Affect affect = new Affect($"Attack_{attackForce}_{attackCount}".ToLower());
        affect.OnAttack = new InPhobiaAction($"Attack_{attackForce}_{attackCount}".ToLower(),
            () => PatientManager.instance.SetAttackForce(attackForce, attackCount), false);
        return affect;
    }

    public static Affect DropKickWithouAttack()
    {
        Affect affect = new Affect($"DropKickWithoutAttack".ToLower());
        affect.OnStepStart = new InPhobiaAction($"DropKickWithoutAttack".ToLower(),
            () => {
                if (PhobiaManager.instance.IsPhobiaHaveVulnerablity())
                {
                    PatientManager.instance.AddActionPoint(1, 0);
                    PatientManager.instance.PullCard(1);
                }
            }, false);
        return affect;
    }

    public static Affect AttackOnDefense(float attackForce)
    {
        Affect affect = new Affect($"AttackOnDefense_{attackForce}".ToLower());
        affect.OnDefense = new InPhobiaAction($"AttackOnDefense_{attackForce}".ToLower(),
            () =>
            {
                PatientManager.instance.ActivateAttackWhenDamaged();
                PatientManager.instance.SetAttackForce(attackForce, 1);
            }, false);
        return affect;
    }

    public static Affect DoubleNextAffect()
    {
        Affect affect = new Affect($"DoubleNextAttack".ToLower())
        {
            OnStepStart = new InPhobiaAction($"DoubleNextAttack".ToLower(),
            () => PatientManager.instance.DoubleNextEffect(), false)
        };
        return affect;
    }

    public static Affect DoubleTheBlock()
    {
        Affect affect = new Affect($"DoubleTheBlock".ToLower());
        affect.OnStepStart = new InPhobiaAction($"DoubleTheBlock".ToLower(),
            () => PatientManager.instance.AddBlock(PatientManager.instance.GetBlock()), false);

        return affect;
    }

    public static Affect TurnWeaknessIntoPoison()
    {
        Affect affect = new Affect("TurnWeaknessIntoPoison".ToLower());
        affect.OnStepStart = new InPhobiaAction($"TurnWeaknessIntoPoison".ToLower(),
            () => PhobiaManager.instance.TurnWeaknessIntoPoison(), false);

        return affect;
    }

    public static List<Affect> UpdateAffects(List<Affect> affects)
    {
        for (int i = affects.Count - 1; i >= 0; i--)
        {
            if (affects[i].IsInvoked())
                affects.RemoveAt(i);
        }
        return affects;
    }
}
