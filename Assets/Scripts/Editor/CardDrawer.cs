using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Card))]
public class CardDrawer : Editor
{
    private Card T;

    private void OnEnable()
    {
        T = target as Card;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (EditorUtility.IsDirty(T))
        {

            EditorUtility.SetDirty(T);
            EditorSceneManager.MarkAllScenesDirty();
        }
    }
}
