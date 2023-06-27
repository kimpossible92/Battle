using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity : MonoBehaviour, IComparable<Entity>
{
    public bool isCharacter;
    //BASE STATS
    public int maxHp;
    public float dodge;
    public float prot;
    public int speed;


    //BASE RESIST
    public float stunResist;
    public float moveResist;
    public float blightResist;
    public float bleedResist;
    public float debufResist;

    //public List<Effect> currentEffects;


    public int currentHp;
    public int initiative;
    public bool canPlay;
    public bool hasPlayed;


    public GameObject entityGameObject;
    public int positionInGroup;
    Vector3 worldPosition;
    void Start()
    {
        entityGameObject = gameObject; 
        worldPosition = transform.position;
    }

    public int CompareTo(Entity other)
    {

        if (initiative < other.initiative)
        {
            return 1;
        }
        if (initiative > other.initiative)
        {
            return -1;
        }
        return 0;
    }
    Vector3 oldPos = Vector3.zero;
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            print(UIManager.instance.camera2.transform.localPosition.x);
            worldPosition = UIManager.instance.camera2.ScreenToWorldPoint(Input.mousePosition);
        }
        if (worldPosition.x > -42.2f && worldPosition.x < -3f)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(worldPosition.x, transform.position.y, transform.position.z), Time.deltaTime);
            UIManager.instance.camera2.transform.position = new Vector3(transform.position.x, UIManager.instance.camera2.transform.position.y, UIManager.instance.camera2.transform.position.z);
            oldPos = worldPosition;
        }
        else
        {
        }
    }

}
