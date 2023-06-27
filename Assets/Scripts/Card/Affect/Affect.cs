//using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Affect
{
    public string name;
    public InPhobiaAction OnTurnStart;//The act before patient play first card
    public InPhobiaAction OnTurnEnd;//The act after patient play the last card
    public InPhobiaAction OnStepStart;//The act before Patient play card
    public InPhobiaAction OnStepEnd;//The act after Patient play card
    public InPhobiaAction OnAttack;//The act before Patient attack
    public InPhobiaAction OnDefense;//The act before patient attacted by enamy
    //public InPhobiaAction OnPlayed;//added only for equipment cards

    private int index;

    public Affect(string name)
    {
        this.name = name;
    }
    //public void Update()
    //{
    //    //Debug.Log($"<color=green>Affect: </color>Affect Update Started");

    //    name = "";
    //    index = 0;

    //    UpdateActionList(OnTurnStart);
    //    UpdateActionList(OnTurnEnd);
    //    UpdateActionList(OnStepStart);
    //    UpdateActionList(OnStepEnd);
    //    UpdateActionList(OnAttack);
    //    UpdateActionList(OnDefense);


    //    //Debug.Log($"<color=green>Affect: </color>Affect Updated {name}");
    //}

    //public void Clear()
    //{
    //    OnTurnStart.Clear();
    //    OnTurnEnd.Clear();
    //    OnStepStart.Clear();
    //    OnStepEnd.Clear();
    //    OnAttack.Clear();
    //    OnDefense.Clear();
    //}

    public bool IsInvoked()
    {
        bool invoked = true;
        if (OnTurnStart != null)
        {
            if (!OnTurnStart.invoked)
                invoked = false;
            else
                OnTurnStart = null;
        }

        if (OnTurnEnd != null)
        {
            if (!OnTurnEnd.invoked)
                invoked = false;
            else
                OnTurnEnd = null;
        }

        if (OnStepStart != null)
        {
            if (!OnStepStart.invoked)
                invoked = false;
            else
                OnStepStart = null;
        }

        if (OnStepEnd != null)
        {
            if (!OnStepEnd.invoked)
                invoked = false;
            else
                OnStepEnd = null;
        }

        if (OnAttack != null)
        {
            if (!OnAttack.invoked)
                invoked = false;
            else
                OnAttack = null;
        }

        if (OnDefense != null)
        {
            if (!OnDefense.invoked)
                invoked = false;
            else
                OnDefense = null;
        }

        //if (OnPlayed != null)
        //{
        //    if (!OnPlayed.invoked)
        //        invoked = false;
        //    else
        //        OnPlayed = null;
        //}

        return invoked;
    }


    public void InvokeAll()
    {
        OnTurnStart?.Invoke();
        OnTurnEnd?.Invoke();
        OnStepStart?.Invoke();
        OnStepEnd?.Invoke();
        OnAttack?.Invoke();
        OnDefense?.Invoke();
    }

    public void Invoke(InPhobiaEventType type)
    {
        switch (type)
        {
            case InPhobiaEventType.OnTurnStart:
                OnTurnStart?.Invoke();
                break;
            case InPhobiaEventType.OnTurnEnd:
                OnTurnEnd?.Invoke();
                break;
            case InPhobiaEventType.OnStepStart:
                OnStepStart?.Invoke();
                break;
            case InPhobiaEventType.OnStepEnd:
                OnStepEnd?.Invoke();
                break;
            case InPhobiaEventType.OnAttack:
                OnAttack?.Invoke();
                break;
            case InPhobiaEventType.OnDefense:
                OnDefense?.Invoke();
                break;
            //case InPhobiaEventType.OnPlayed:
            //    OnPlayed?.Invoke();
            //    break;
            default:
                break;
        }
    }

    private void UpdateActionList(List<InPhobiaAction> inPhobiaActions)
    {
        int length = inPhobiaActions.Count;

        //remove nulls
        for (int i = 0; i < length; i++)
        {
            if (string.IsNullOrEmpty(inPhobiaActions[i].id))
            {
                inPhobiaActions.RemoveAt(i);
                i--;
                length--;
            }
        }

        //remove invoked actions that not set to save
        for (int i = 0; i < length; i++)
        {
            //if (inPhobiaActions[i].Invoked)
            if (inPhobiaActions[i].invoked)
            {
                inPhobiaActions.RemoveAt(i);
                i--;
                length--;
            }
        }
        //remove similars
        for (int i = 0; i < length - 1; i++)
        {
            for (int j = i + 1; j < length; j++)
            {
                if (inPhobiaActions[i] == inPhobiaActions[j])
                {
                    inPhobiaActions.RemoveAt(i);
                    i--;
                    j--;
                    length--;
                }
            }
        }


        //Debug.Log($"<color=green>Affect: </color>{GetActionsListName(index)} Updated. Actions count {inPhobiaActions.Count}");
        index++;
        foreach (InPhobiaAction action in inPhobiaActions)//this is only for debug
        {
            //name += action.ID + " ";
            name += action.id + " ";
        }
    }


    private string GetActionsListName(int index)
    {
        return index switch
        {
            0 => "OnTurnStart",
            1 => "OnTurnEnd",
            2 => "OnStepStart",
            3 => "OnStepEnd",
            4 => "OnAttack",
            5 => "OnDefense",
            6 => "OnPlayed",
            _ => "Wrong index",
        };
    }
}
