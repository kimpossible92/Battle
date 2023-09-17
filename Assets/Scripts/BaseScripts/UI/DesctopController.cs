using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DesctopController : InPhobiaScrollView
{
    #region Serialized Fields

    [Header("Desctop Info Properties")]
    [SerializeField] private List<Patient> patients;
    [SerializeField] private TMPro.TMP_Text infoText;
    [SerializeField] private Image patientImage;
    [SerializeField] private Image phobiaImage;

    #endregion

    #region Private Fields

    private int patientIndexHolder;

    #endregion

    #region Unity Behaviour

    #endregion

    #region Public Methods

    public void ShowNextPatient(bool fromFirst)
    {
        if (fromFirst)
            patientIndexHolder = 0;

        InitializeScrollView();

        Patient patient = patients[patientIndexHolder];
        infoText.text = patient.info;
        patientImage.sprite = patient.image;
        phobiaImage.sprite = patient.phobia.image;

        patientIndexHolder++;
        if (patientIndexHolder == patients.Count)
            patientIndexHolder = 0;
    }

    public void LeafThrough(int value)
    {
        patientIndexHolder += value;
        if (patientIndexHolder == patients.Count)
            patientIndexHolder = 0;
        if (patientIndexHolder < 0)
            patientIndexHolder = patients.Count - 1;

        InitializeScrollView();

        Patient patient = patients[patientIndexHolder];
        infoText.text = patient.info;
        patientImage.sprite = patient.image;
        phobiaImage.sprite = patient.phobia.image;
    }

    public void OpenGlossary()
    {
        Patient patient = patients[patientIndexHolder];
        infoText.text = patient.glossary;
    }

    public void OpenInfo()
    {
        Patient patient = patients[patientIndexHolder];
        infoText.text = patient.info;
    }

    #endregion
}
