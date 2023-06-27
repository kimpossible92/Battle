using System;
using UnityEngine.Events;

[Serializable]
public class InPhobiaAction
{
    public UnityAction action;
    //public UnityAction OnAction { get { return onAction; } }

    public bool saveAction;
    //public bool SaveAction { get { return saveAction; } }

    public string id;
    //public string ID { get { return id; } }

    public bool invoked = false;
    //public bool Invoked { get { return invoked; } }

    public int invokedCount = 0;


    public InPhobiaAction(string id, UnityAction action, bool saveAction)
    {
        invoked = false;
        invokedCount = 0;
        this.id = id;
        this.action = action;
        this.saveAction = saveAction;
    }


    public void Invoke()
    {
        if (!invoked)
        {
            action?.Invoke();
            invokedCount++;
        }
        if (!saveAction)
            invoked = true;

    }



    private bool EqualsP(InPhobiaAction obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (this.GetHashCode() != obj.GetHashCode())
        {
            return false;
        }

        System.Diagnostics.Debug.Assert(
            base.GetType() != typeof(object));

        if (!base.Equals(obj))
        {
            return false;
        }

        return (this.id.Equals(obj.id));
    }
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (this.GetType() != obj.GetType())
        {
            return false;
        }
        return EqualsP((InPhobiaAction)obj);
    }
    public static bool operator == (InPhobiaAction a, InPhobiaAction b)
    {
        if (a is null || b is null)
            return false;
        return a.id == b.id;
    }
    public static bool operator !=(InPhobiaAction a, InPhobiaAction b)
    {
        if (a is null || b is null)
            return false;
        return a.id != b.id;
    }
    public override int GetHashCode()
    {
        char[] idc = id.ToCharArray();
        int intId = 0;
        for (int i = 0; i < idc.Length; i++)
        {
            intId += idc[i];
        }
        return intId;
    }
}

