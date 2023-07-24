using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Public Fields

    public static UIManager instance;

    #endregion

    #region Serialized Fields

    [Header("Main scenes")]

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject cutscene_1;
    [SerializeField] private GameObject cutscene_2;
    [SerializeField] private GameObject cutscene_3;
    [SerializeField] private GameObject Game2;
    [SerializeField] private GameObject desctop;
    [SerializeField] private GameObject novel;
    [SerializeField] private GameObject cardCollecter;
    [SerializeField] private GameObject fight,fight2;
    [SerializeField] private GameObject cardsCanvas;
    [SerializeField] private GameObject gameEnd;
    [SerializeField] private GameObject Bar;
    [SerializeField] private bool ViewNewGame2 = false;
    [SerializeField] private BattleSystem battleSystem1;
    [SerializeField] private Camera cam1, cam2,cam3;
    [SerializeField] private PersistentObjectSpawner persistentObjectSpawner;
    [HideInInspector] public Camera camera1 => cam1;
    [HideInInspector] public Camera camera2 => cam2;
    [Header("Main Menu")]
    [SerializeField] private GameObject continueButton;

    [Header("Desctop")]
    [SerializeField] private DesctopController desctopController;

    [Header("Dialogs")]
    [SerializeField] private DialogController dialogController;

    [Header("Progress Bar")]
    [SerializeField] private ProgressBarController progressBarController;

    [Header("Therapist Deck Collecter")]
    [SerializeField] private TherapistDeckCollecter therapistDeckCollecter;

    [Header("Canvas Settings")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Fight Scene")]
    [SerializeField] private RectTransform card_sCanvasRT;

    [Header("Test settings")]
    [SerializeField] private IdeaController ideaController;

    [Header("Game end settings")]
    [SerializeField] private GameObject gameComplete;
    [SerializeField] private GameObject gameFailed;
    [Header("Get Player Controll")]
    [SerializeField] PlayerControll GetPlayerControll;
    [SerializeField] GameObject gamePlayer;
    [SerializeField] GameObject gamePlayer2,gamePlayer3;
    #endregion

    #region Unity Behaviour

    private void Awake()
    {
        instance = this;
        Game2.SetActive(false);
        persistentObjectSpawner.gameObject.SetActive(false);
        cam1.gameObject.SetActive(true);
        cam2.gameObject.SetActive(false);
    }

    private void Update()
    {
        
    }

    #endregion

    #region Public Methods

    public RectTransform GetCard_sCanvas()
    {
        return card_sCanvasRT;
    }

    public void Initialize()
    {
        OpenMainMenu(false);
    }
    public void NewCutScene()
    {
        //GetPlayerControll.gameObject.SetActive(true);
        ViewNewGame2 = true;
        AudioManager.instance.StopAll();
        mainMenu.SetActive(false);
        cutscene_1.SetActive(false);
        cutscene_2.SetActive(false); 
        cutscene_3.SetActive(true); 
        desctop.SetActive(false);
        novel.SetActive(false);
        cardCollecter.SetActive(false);
        fight.SetActive(false);
        fight2.SetActive(false);
        cardsCanvas.SetActive(false);
        gameEnd.SetActive(false);
        Bar.SetActive(false);
    }

    public void OpenMainMenu(bool withPause)
    {
        AudioManager.instance.PlayMainMenuPhaseAudios();
        mainMenu.SetActive(true);
        if (withPause)
        {
            continueButton.SetActive(true);
        }
        else
        {
            cutscene_1.SetActive(false);
            cutscene_2.SetActive(false); 
            cutscene_3.SetActive(false);
            desctop.SetActive(false);
            novel.SetActive(false);
            cardCollecter.SetActive(false);
            fight.SetActive(false);
            fight2.SetActive(false);
            cardsCanvas.SetActive(false);
            gameEnd.SetActive(false);
            Bar.SetActive(false);
        }
    }

    public void ContinueGame()
    {
        continueButton.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void OpenCutSceneOne()
    {
        AudioManager.instance.StopAll();
        mainMenu.SetActive(false);
        cutscene_1.SetActive(true);
        cutscene_2.SetActive(false); 
        cutscene_3.SetActive(false);
        desctop.SetActive(false);
        novel.SetActive(false);
        cardCollecter.SetActive(false);
        fight.SetActive(false);
        fight2.SetActive(false);
        cardsCanvas.SetActive(false);
        gameEnd.SetActive(false);
        Bar.SetActive(false);
    }

    public void OpenCutSceneTwo()
    {
        AudioManager.instance.StopAll();
        mainMenu.SetActive(false);
        cutscene_1.SetActive(false);
        cutscene_2.SetActive(true); 
        cutscene_3.SetActive(false);
        desctop.SetActive(false);
        novel.SetActive(false);
        cardCollecter.SetActive(false);
        fight.SetActive(false);
        fight2.SetActive(false);
        cardsCanvas.SetActive(false);
        gameEnd.SetActive(false);
        Bar.SetActive(false);
    }

public void OpenDesctop()
    {
        AudioManager.instance.PlayMainMenuPhaseAudios();
        desctopController.ShowNextPatient(true);

        mainMenu.SetActive(false);
        cutscene_1.SetActive(false);
        cutscene_2.SetActive(false); 
        cutscene_3.SetActive(false);
        desctop.SetActive(true);
        novel.SetActive(false);
        cardCollecter.SetActive(false);
        fight.SetActive(false);
        fight2.SetActive(false);
        cardsCanvas.SetActive(false);
        gameEnd.SetActive(false);
        Bar.SetActive(false);
    }

    public void OpenNovel()
    {
        AudioManager.instance.PlayNovelPhaseAudios();
        progressBarController.InitializeProgressBar();
        dialogController.InitializeDiolog();

        mainMenu.SetActive(false);
        cutscene_1.SetActive(false);
        cutscene_2.SetActive(false); 
        cutscene_3.SetActive(false);
        desctop.SetActive(false);
        novel.SetActive(true);
        cardCollecter.SetActive(false);
        fight.SetActive(false);
        fight2.SetActive(false);
        cardsCanvas.SetActive(false);
        gameEnd.SetActive(false);
        Bar.SetActive(true);
    }

    [ContextMenu("Open Card collector (5trust, 9 idea)")]
    public void OpenCardCollecterImmidiatly()
    {
        print("OpenCardCollecterImmidiatly");
        GameManager.instance.InitialiseGame();

        progressBarController.InitializeProgressBar();
        ideaController.Initialize();

        progressBarController.AddPoint(3);
        ideaController.AddIdea(9);

        OpenCardCollecter();
    }
    public void NewGame3()
    {
        Game2.SetActive(false); persistentObjectSpawner.gameObject.SetActive(true);
        //FindObjectOfType<GameCC>().SetNewState(GameState.Battle);
        cam1.gameObject.SetActive(false);
        cam2.gameObject.SetActive(false);
        cam3.gameObject.SetActive(true);
    }
    public void NewGame2()
    {
        Game2.SetActive(true);
        FindObjectOfType<GameCC>().SetNewState(GameState.Battle);
        cam1.gameObject.SetActive(false);
        cam3.gameObject.SetActive(false);
        cam2.gameObject.SetActive(true);
    }
    public void OpenCardCollecter2()
    {

        if (ViewNewGame2)
        {
            cutscene_3.SetActive(false);
            NewGame2();
            battleSystem1.gameObject.SetActive(true); 
            return;
        }
        AudioManager.instance.PlayDeckBuildingPhaseAudios();
        therapistDeckCollecter.InitializeCollecter();

        mainMenu.SetActive(false);
        cutscene_1.SetActive(false);
        cutscene_2.SetActive(false);
        cutscene_3.SetActive(false);
        desctop.SetActive(false);
        novel.SetActive(false);
        cardCollecter.SetActive(true);
        fight.SetActive(false);
        fight2.SetActive(false);
        cardsCanvas.SetActive(false);
        gameEnd.SetActive(false);
        Bar.SetActive(true);
    }
    public void OpenCardCollecter()
    {
        if (ViewNewGame2)
        {
            cutscene_3.SetActive(false);
            NewGame3();
            
            //battleSystem1.gameObject.SetActive(true);
            gamePlayer.gameObject.SetActive(true);
            gamePlayer2.gameObject.SetActive(true);
            gamePlayer3.gameObject.SetActive(true);
            return;
        }
        AudioManager.instance.PlayDeckBuildingPhaseAudios();
        therapistDeckCollecter.InitializeCollecter();

        mainMenu.SetActive(false);
        cutscene_1.SetActive(false);
        cutscene_2.SetActive(false); 
        cutscene_3.SetActive(false);
        desctop.SetActive(false);
        novel.SetActive(false);
        cardCollecter.SetActive(true);
        fight.SetActive(false);
        fight2.SetActive(false);
        cardsCanvas.SetActive(false);
        gameEnd.SetActive(false);
        Bar.SetActive(true);
    }

    public void OpenFightScene()
    {
        AudioManager.instance.PlayBattlePhaseAudios();
        mainMenu.SetActive(false);
        cutscene_1.SetActive(false);
        cutscene_2.SetActive(false); cutscene_3.SetActive(false);
        desctop.SetActive(false);
        novel.SetActive(false);
        cardCollecter.SetActive(false);
        fight.SetActive(true);
        fight2.SetActive(true);
        cardsCanvas.SetActive(true);
        gameEnd.SetActive(false);
        Bar.SetActive(false);
    }

    public void OpenGameEndScene()
    {
        mainMenu.SetActive(false);
        cutscene_1.SetActive(false);
        cutscene_2.SetActive(false); cutscene_3.SetActive(false);
        desctop.SetActive(false);
        novel.SetActive(false);
        cardCollecter.SetActive(false);
        fight.SetActive(false);
        fight2.SetActive(false);
        cardsCanvas.SetActive(false);
        gameEnd.SetActive(false);
        Bar.SetActive(false);
    }

    public void SetCanvasGroupActive(bool value)
    {
        canvasGroup.interactable = value;
        canvasGroup.interactable = value;
    }

    public void OpenGameEndPanel(bool completed)
    {
        if(completed)
            AudioManager.instance.PlayGameEndAudios();

        gameFailed.SetActive(!completed);
        gameComplete.SetActive(completed);

        gameEnd.SetActive(true);
    }

    #endregion

    #region Private Fields

    #endregion

    #region Private Methods

    #endregion

    #region Coroutines
    #endregion
}
