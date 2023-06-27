using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UISpline))]
public class UISplineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UISpline cardSpline = (UISpline)target;
        if (GUILayout.Button("Add Point"))
        {
            cardSpline.AddPoint();
        }

        if (GUILayout.Button("Remove Point"))
        {
            cardSpline.RemovePoint();
        }
    }
}
