using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class DialogController : MonoBehaviour
{
    [SerializeField] private ProgressBarController progressBarController;
    [SerializeField] private FactController factController;

    public TMP_Text persionTmp;
    public TMP_Text textTMP;
    public Button skipButton;
    public Button nextButton;
    public List<QuestionObject> questions;
    [Header("DIOLOGS")]
    public UnityEvent OnDialogsStarts;
    public UnityEvent OnDialogsEnd;


    [SerializeField]private List<Dialog> dialog;

    private bool onDialogue = false;
    private bool currentDiologHasQuestion = false;
    private int selectedQuestionIndex = -1;
    private int dialogIndex = 0;
    private int passedDiologIndex = 0;

    private void Update()
    {
        if (!onDialogue)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!currentDiologHasQuestion)
                ShowPreviousDialog();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!currentDiologHasQuestion)
                ShowNextDialog();
        }

        if (currentDiologHasQuestion)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SelectQuestion(selectedQuestionIndex - 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SelectQuestion(selectedQuestionIndex + 1);
            }
            else if(Input.GetKeyDown(KeyCode.Space))
            {
                AskTheSelectedQuestion();
            }
        }
    }

    #region Public Methods

    public void InitializeDiolog()
    {
        if (dialog == null)
            dialog = new List<Dialog>();
        dialog.Clear();
        OnDialogsStarts?.Invoke();

        ShowDiolog(0);

        onDialogue = true;
    }

    public void ShowPreviousDialog()
    {
        if (dialogIndex - 1 < 0)
            return;

        ShowDiolog(dialogIndex - 1);
    }

    public void ShowNextDialog()
    {
        nextButton.onClick?.Invoke();
    }

    public void ShowDiolog(int index)
    {
        bool isNext = false;
        dialogIndex = index;
        if (dialogIndex > passedDiologIndex)
        {
            passedDiologIndex = dialogIndex;
            isNext = true;
        }

        if (dialog.Count <= dialogIndex)
        {
            onDialogue = false;
            OnDialogsEnd?.Invoke();
            return;
        }

        if (isNext)
            dialog[dialogIndex].onThisDiologStart?.Invoke();

        //Prepare Next Button
        nextButton.onClick.RemoveAllListeners();

        Debug.Log($"dialog index = {dialogIndex}, isNext = {isNext}");

        bool newIsNext = isNext;
        nextButton.onClick.AddListener(() =>
        {
            Debug.Log($"dialogIndex = {dialogIndex}, newIsNext = {newIsNext} OnClick");

            if (newIsNext)
                dialog[dialogIndex].onNext?.Invoke();

            ShowDiolog(dialogIndex + 1);
        });

        //Reset Diolog parametres
        selectedQuestionIndex = -1;
        currentDiologHasQuestion = false;
        foreach (QuestionObject qo in questions)
        {
            qo.questionButton.gameObject.SetActive(false);
            qo.questionButton.interactable = true;
            qo.outline.enabled = false;

        }
        skipButton.interactable = true;
        nextButton.interactable = true;

        //Assign diolog parametrs
        persionTmp.text = dialog[dialogIndex].Persion;
        textTMP.text = dialog[dialogIndex].text;

        if (dialog[dialogIndex].questions.Count > 0)
        {
            skipButton.interactable = false;
            nextButton.interactable = false;

            for(int i = 0; i < dialog[dialogIndex].questions.Count; i++)
            {
                questions[i].questionButton.gameObject.SetActive(true);
                questions[i].questionText.text = dialog[dialogIndex].questions[i].question;

                questions[i].questionButton.onClick.RemoveAllListeners();
                int questionIndex = i;
                int j = dialogIndex;
                questions[i].questionButton.onClick.AddListener(()=>
                {
                    dialog[j].onNext?.Invoke();
                    dialog[j].questions[questionIndex].onQuestion?.Invoke();

                    ShowDiolog(dialogIndex + 1);
                    //nextButton.onClick?.Invoke();
                });
            }
        }

        if (dialog[dialogIndex].checkTrustPoints && isNext)
        {
            if (progressBarController.PointsHavePassedTheLimit(dialog[dialogIndex].trustPointsLimit))
            {
                dialog[dialogIndex].onTrustPointPassedTheLimit?.Invoke();
            }
        }

        if (dialog[dialogIndex].factCheckers.Count>0 && isNext)
        {
            for (int i = 0; i < dialog[dialogIndex].factCheckers.Count; i++)
            {
                if (!factController.HasFact(dialog[dialogIndex].factCheckers[i].fact))
                {
                    dialog[dialogIndex].factCheckers[i].onHasNotFact?.Invoke();
                }
            }
        }

        currentDiologHasQuestion = dialog[dialogIndex].questions.Count > 0;
    }

    public void SetFalseQuestionInteractable(int index)
    {
        questions[index].questionButton.interactable = false;
    }

    public void SetActiveQuestionFalse(int index)
    {
        questions[index].questionButton.gameObject.SetActive(false);
    }

    public void PrepareNextDialogWithQuestion()
    {
        if (ISearchNextDiologWithQuestionHelper == null)
            ISearchNextDiologWithQuestionHelper = StartCoroutine(ISearchNextDiologWithQuestion());

    }

    public void SetBlock(DialogBlock dialogBlock)
    {
        Debug.Log($"SetBlock ---> {dialogBlock.gameObject.name}");
        dialog = new List<Dialog>(dialogBlock.dialogBlock);
        dialogIndex = -1;
        passedDiologIndex = -1;
    }

    #endregion

    #region Private Methods

    List<QuestionObject> activeQuestionObjects = new List<QuestionObject>();
    private void SelectQuestion(int index)
    {
        selectedQuestionIndex = index;

        activeQuestionObjects.Clear();
        foreach (QuestionObject qo in questions)
        {
            if (qo.questionButton.interactable == true &&
                  qo.questionButton.gameObject.activeSelf)
                activeQuestionObjects.Add(qo);
        }

        if (selectedQuestionIndex == -1)
            selectedQuestionIndex = activeQuestionObjects.Count - 1;
        else if (selectedQuestionIndex == activeQuestionObjects.Count)
            selectedQuestionIndex = 0;

        for (int i = 0; i < activeQuestionObjects.Count; i++)
        {
            if (i == selectedQuestionIndex)
                activeQuestionObjects[i].outline.enabled = true;
            else
                activeQuestionObjects[i].outline.enabled = false;
        }
    }

    private void AskTheSelectedQuestion()
    {
        if (selectedQuestionIndex > -1)
            activeQuestionObjects[selectedQuestionIndex].questionButton.onClick?.Invoke();
    }

    #endregion

    #region Coroutines

    private Coroutine ISearchNextDiologWithQuestionHelper;
    private IEnumerator ISearchNextDiologWithQuestion()
    {
        int counter = 0;
        while (!currentDiologHasQuestion && onDialogue)
        {
            Debug.Log($"Searching diolog with question - {counter++}");

            ShowNextDialog();
            yield return new WaitForEndOfFrame();
        }
        ISearchNextDiologWithQuestionHelper = null;
    }

    #endregion
}

[Serializable]
public class QuestionObject
{
    public Button questionButton;
    public Outline outline;
    public TMP_Text questionText;
}
