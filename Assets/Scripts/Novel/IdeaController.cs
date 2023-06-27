using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IdeaController : MonoBehaviour
{
    [SerializeField] private TMP_Text ideaCounter;
    [SerializeField] private Animator ideaAnimator;
    [SerializeField] private List<GameObject> cardsToSelect;


    private int ideaCount = 0;

    public void Initialize()
    {
        ideaCount = 0;
        ideaCounter.text = ideaCount.ToString();
        UpdateCardsToSelect();
    }

    public bool CheckIdeaCount(int count)
    {
        if (ideaCount < count)
            return false;
        else
            return true;
    }

    public void AddIdea(int value)
    {
        ideaCount += value;
        if(ideaCount<0)
            ideaCount = 0;

        ideaCounter.text = ideaCount.ToString();
        AnimateIdeaIcon();
        UpdateCardsToSelect();
    }

    private void AnimateIdeaIcon()
    {
        ideaAnimator.Play("Idea");
    }

    private void UpdateCardsToSelect()
    {
        for (int i = 0; i < cardsToSelect.Count; i++)
        {
            if (i < ideaCount)
                cardsToSelect[i].gameObject.SetActive(true);
            else
                cardsToSelect[i].gameObject.SetActive(false);
        }
    }
}
