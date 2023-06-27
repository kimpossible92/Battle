using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] private IdeaController ideaController;
    [SerializeField] private List<Transform> pointsGO;
    [SerializeField] private int startPointCount = 2;
    [SerializeField] private int pointsMinimumLimit = 1;

    private int currentPointsCount;
    private int maxPointCount;

    public void InitializeProgressBar()
    {
        ideaController.Initialize();

        pointsMinimumLimit = 1;

        maxPointCount = pointsGO.Count;
        currentPointsCount = startPointCount;

        UpdatePointsBar();
    }

    public void SetPointsMinimumLimit(int limit)
    {
        pointsMinimumLimit = limit;
    }

    public bool PointsHavePassedTheLimit(int limit)
    {
        if (currentPointsCount <= limit)
            return true;
        else
            return false;
    }

    public bool HasNeededAmoutOfPointsToUse(int amount)
    {
        if (currentPointsCount - amount < 0)
            return false;
        return true;
    }

    public void AddPoint(int value)
    {
        currentPointsCount += value;

        if (currentPointsCount > maxPointCount)
        {
            currentPointsCount = maxPointCount;
            return;
        }
        if (currentPointsCount < 0)
        {
            currentPointsCount = 0;
            return;
        }

        UpdatePointsBar();
    }

    private void UpdatePointsBar()
    {
        for (int i = 0; i < pointsGO.Count; i++)
        {
            if (i < currentPointsCount)
                pointsGO[i].gameObject.SetActive(true);
            else
                pointsGO[i].gameObject.SetActive(false);
        }
    }
}
