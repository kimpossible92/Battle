using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SplineControllPoint
{
    public RectTransform controllPoint;
    public RectTransform firstTanget;
    public RectTransform secondTanget;

    private Vector2 controllPointPos = new Vector2(0f, 0f);
    private Vector2 firstTangetPos = new Vector2(-100f, 0f);
    private Vector2 secondTangetPos = new Vector2(100f, 0f);

    private Vector2 prevFirstTangetPos;
    private Vector2 prevSecondTangetPos;


    public SplineControllPoint(Transform parent)
    {
        controllPoint = new GameObject($"controllPoint({parent.childCount + 1})", typeof(RectTransform)).GetComponent<RectTransform>();
        controllPoint.SetParent(parent);
        controllPoint.anchoredPosition = controllPointPos;
        controllPoint.sizeDelta = new Vector2(10f, 10f);
        controllPoint.anchorMin = new Vector2(0.5f, 0.5f);
        controllPoint.anchorMax = new Vector2(0.5f, 0.5f);
        controllPoint.localScale = Vector3.one;
        //controllPoint.hideFlags = HideFlags.HideInHierarchy;

        firstTanget = new GameObject($"firstTanget", typeof(RectTransform)).GetComponent<RectTransform>();
        firstTanget.SetParent(controllPoint);
        firstTanget.anchoredPosition = firstTangetPos;
        prevFirstTangetPos = firstTangetPos;
        firstTanget.sizeDelta = new Vector2(10f, 10f);
        firstTanget.anchorMin = new Vector2(0.5f, 0.5f);
        firstTanget.anchorMax = new Vector2(0.5f, 0.5f);
        firstTanget.localScale = Vector3.one;
        //firstTanget.hideFlags = HideFlags.HideInHierarchy;

        secondTanget = new GameObject($"secondTanget", typeof(RectTransform)).GetComponent<RectTransform>();
        secondTanget.SetParent(controllPoint);
        secondTanget.anchoredPosition = secondTangetPos;
        prevSecondTangetPos = secondTangetPos;
        secondTanget.sizeDelta = new Vector2(10f, 10f);
        secondTanget.anchorMin = new Vector2(0.5f, 0.5f);
        secondTanget.anchorMax = new Vector2(0.5f, 0.5f);
        secondTanget.localScale = Vector3.one;
        //secondTanget.hideFlags = HideFlags.HideInHierarchy;
    }

    public void UpdatePoint()
    {
       // bool updated = false;
        controllPointPos = controllPoint.anchoredPosition;
        firstTangetPos = firstTanget.anchoredPosition;
        secondTangetPos = secondTanget.anchoredPosition;


        if (prevFirstTangetPos != firstTangetPos)
        {
            secondTanget.anchoredPosition = -firstTangetPos.normalized * secondTangetPos.magnitude;

            prevFirstTangetPos = firstTangetPos;
            prevSecondTangetPos = secondTangetPos = secondTanget.anchoredPosition;

            //updated = true;
        }

        if (prevSecondTangetPos != secondTangetPos)
        {
            firstTanget.anchoredPosition = - secondTangetPos.normalized * firstTangetPos.magnitude;

            prevSecondTangetPos = secondTangetPos;
            prevFirstTangetPos = firstTangetPos = firstTanget.anchoredPosition;

            //updated = true;
        }

        //return updated;
    }
}
