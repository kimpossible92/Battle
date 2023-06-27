using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBordPlayer : MonoBehaviour
{
    //[SerializeField] GameObject Player1Act, Player2Act;
    //public Texture2D lifeIconTexture;
    //[SerializeField] Vector3 old1PlayerPos, Old2PlayerPos;
    //[SerializeField]Cinemachine.CinemachineBrain cinemachine;
    //[SerializeField] Cinemachine.CinemachineVirtualCamera VirtualCamera;
    //[SerializeField] Cinemachine.CinemachineConfiner confiner;
    //public int currentSwitch = 0;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    currentSwitch = 0;
    //    Player1Act.SetActive(true);
    //    Player2Act.SetActive(false);
    //    old1PlayerPos = Player1Act.transform.position;
    //    Old2PlayerPos = Player2Act.transform.position;
    //    //FindObjectOfType<GameMode>().loadStartPos(Player1Act);
    //    cinemachine.enabled = true;
    //    VirtualCamera.enabled = true;
    //    confiner.enabled = true;
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    //if (collision.gameObject.tag == "Player")
    //    //{
    //    //    Switched();
    //    //}
    //}
    //public GameObject ReturnCurrentGo()
    //{
    //    if (currentSwitch == 0) { return Player1Act.gameObject; }
    //    else { return Player2Act.gameObject; }
    //}
    //private void OnGUI()
    //{
    //    Rect lifeIconRect = new Rect(10, 150, 32, 32);
    //    GUI.DrawTexture(lifeIconRect, lifeIconTexture);

    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 30;
    //    style.fontStyle = FontStyle.Bold;
    //    style.normal.textColor = Color.yellow;

    //    Rect labelRect = new Rect(lifeIconRect.xMax + 10, lifeIconRect.y, 60, 32);
    //    if(GUI.Button(labelRect, "switch", style))
    //    {
    //        Switched();
    //    }
    //}
    //public void Switched()
    //{
    //    currentSwitch = 1;
    //    Camera.main.transform.position = new Vector3(Player2Act.transform.position.x, Player2Act.transform.position.y, -10);
    //    Player1Act.SetActive(false);
    //    Player2Act.SetActive(true);
    //    FindObjectOfType<GameMode>().loadStartPos(Player2Act);
    //    Player1Act.transform.position = old1PlayerPos;
    //    Player2Act.transform.position = Old2PlayerPos;
    //    cinemachine.enabled = false;
    //    VirtualCamera.enabled = false;
    //    confiner.enabled = false;
    //}
}
