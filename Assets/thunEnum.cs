using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thunEnum : MonoBehaviour
{
    [SerializeField] GameObject[] GetObjects = new GameObject[4];
    int ind = 0;
    private void OnEnable()
    {
        InvokeRepeating("enumThun", 0.5f, 0.5f);
        
    }
    private void enumThun()
    {
        if (ind > 3) { ind = 0; }
        for(int i = 0; i < GetObjects.Length; i++)
        {
            if (i == ind) { GetObjects[i].SetActive(true); }
            else { GetObjects[i].SetActive(false); }
        }
        ind++;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
