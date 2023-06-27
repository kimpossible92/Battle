using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactController : MonoBehaviour
{
    [SerializeField] private Animator factAnimator;
    [SerializeField] private GameObject factUIPrefab;
    [SerializeField] private RectTransform factUIParnet;

    private List<Fact> collectedFacts;


    public void AddFact(Fact fact)
    {
        if (collectedFacts == null)
            collectedFacts = new List<Fact>();

        if (HasFact(fact))
            return;

        Instantiate(factUIPrefab, factUIParnet);
        //Assign fact to Game Object here

        collectedFacts.Add(fact);
        AnimateFactIcon();
    }

    public void RemoveFact(Fact fact)
    {
        if (collectedFacts != null)
        {
            if (collectedFacts.Count > 0)
                collectedFacts.Remove(fact);
        }
    }

    public bool HasFact(Fact fact)
    {
        if(collectedFacts==null)
            return false;
        foreach (Fact f in collectedFacts)
        {
            if (f.factID == fact.factID)
                return true;
        }
        return false;
    }

    private void AnimateFactIcon()
    {
        factAnimator.Play("fact");
    }
}
