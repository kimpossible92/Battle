using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum LoadEventTypes
{
    LevelStart,
    LevelComplete,
    LevelEnd,
    Pause,
    UnPause,
    PlayerDeath,
    Respawn,
    StarPicked,
    GameOver,
    CharacterSwitch,
    CharacterSwap
}
public struct LoadEvent
{
    public LoadEventTypes EventType;
    public LoadEvent(LoadEventTypes eventType)
    {
        EventType = eventType;
    }

    static LoadEvent e;
    public static void Trigger(LoadEventTypes eventType)
    {
        e.EventType = eventType;
        MMEventManager.TriggerEvent(e);
    }
}
public enum PointsMethods
{
    Add,
    Set
}
public struct PointsEvent
{
    public PointsMethods PointsMethod;
    public int Points;
    public PointsEvent(PointsMethods pointsMethod, int points)
    {
        PointsMethod = pointsMethod;
        Points = points;
    }

    static PointsEvent e;
    public static void Trigger(PointsMethods pointsMethod, int points)
    {
        e.PointsMethod = pointsMethod;
        e.Points = points;
        MMEventManager.TriggerEvent(e);
    }
}

public enum PauseMethods
{
    PauseMenu,
    NoPauseMenu
}
[System.Serializable]
public class PointsOfEStorage
{
    public string LevelName;
    public int PointOfEntryIndex;
    public int EnemiesCount, buttonLampsCount;
    public CharacterMotor3D.FacingDirections FacingDirection;

    public PointsOfEStorage(string levelName, int pointOfEntryIndex, CharacterMotor3D.FacingDirections facingDirection)
    {
        LevelName = levelName;
        FacingDirection = facingDirection;
        PointOfEntryIndex = pointOfEntryIndex;
    }
}
public class GameMode : MonoBehaviour
{
    public static GameMode THIS;
    protected bool isgameover;
    protected Vector3 startPosition1;
    protected List<Vector3> startPositionsEnemy = new List<Vector3>();
    protected List<GameObject> Enemies = new List<GameObject>();
    protected List<EnemyCharmander> buttonLamps = new List<EnemyCharmander>();
    protected PointsOfEStorage Points1;
    private string leveln = "level1";
    private int pointIndex=0;
    public void loadisgameover(bool over)
    {
        isgameover = over;
    }
    public void AddButtonLamps(EnemyCharmander lamp) { buttonLamps.Add(lamp); }
    public void loadStartPosition()
    {
        //startPosition1 = FindObjectOfType<InputManager>().transform.position;
    }
    private void Start()
    {

    }
    private void Awake()
    {
        THIS = this;
        loadStartPosition();
    }
    IEnumerator GetEnumerator()
    {
        Points1 = new PointsOfEStorage(leveln, pointIndex, CharacterMotor3D.FacingDirections.Right);
        Points1.EnemiesCount = Enemies.Count;Points1.buttonLampsCount = buttonLamps.Count;
        while (isgameover)
        {
            yield return new WaitForSeconds(2.0f);
            for(int i = 0; i < Enemies.Count; i++) { Enemies[i].transform.position = startPositionsEnemy[i]; }
            for(int k = 0; k < buttonLamps.Count; k++) { buttonLamps[k].ButtonOnOff(false); }
            loadisgameover(false);
            yield return null;
        }
    }
    public void AddPositionsEnemy(Vector3 pos) { startPositionsEnemy.Add(pos); }
    public void AddEnemy(GameObject obj) { Enemies.Add(obj); }
    private void Update()
    {
        if (isgameover)
        {
            //StartCoroutine(GetEnumerator());
            GameObject.Find("Canvas").transform.Find("isGameOver").gameObject.SetActive(true);
            FindObjectOfType<InputManager>().transform.position = startPosition1;
        }
        else
        {
            //StopCoroutine(GetEnumerator());
            GameObject.Find("Canvas").transform.Find("isGameOver").gameObject.SetActive(false);
        }
    }
}
