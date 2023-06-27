using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Phobia", menuName = "ScriptableObjects/Phobia", order = 1)]
[Serializable]
public class Phobia : ScriptableObject
{
    public string Name;
    public Sprite image;
    public float maximumHealth;
    public float health;
    public float attackForce;
    public float block;
    public int attackCountInAStep;
    public int vulnerablityCount;
    public int weaknessStack;
    public int poison;
    public int power = -1;


    public void Initialize()
    {
        poison = 0;
        power = -1;
        vulnerablityCount = 0;
        weaknessStack = 0;
        maximumHealth = 160;
        health = maximumHealth;
    }

    public void PrepareAttack()
    {
        power++;

        if (power > 10)
        {
            float percent = Random.Range(0f, 100f);

            if (percent <= 35f)
            {
                block = 0;
                attackCountInAStep = 5;
                attackForce = 1f + power;

                power = Mathf.FloorToInt(power / 2f);

                return;
            }
        }

        PrepareMainAttack();
    }

    private void PrepareMainAttack()
    {
        float percent = Random.Range(0f, 100f);

        if (percent <= 30f)
        {
            block = 0;
            attackCountInAStep = 1;
            attackForce = 17f + power;
        }
        else if (percent <= 55)
        {
            block = 0;
            attackCountInAStep = 3;
            attackForce = 1f + power;
        }
        else if (percent <= 70)
        {
            block = 4 * power;

            //CardManager.instance.AddPsychosisToPatient();
        }
        else if (percent <= 90)
        {
            block = 0;
            attackCountInAStep = 2;
            attackForce = 15f + power;
        }
        else if (percent <= 100)
        {
            block = 0;
            attackCountInAStep = 2;
            attackForce = 1f + power;

            //CardManager.instance.AddPsychosisToPatient();
        }
    }
}
