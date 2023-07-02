using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ocupator : MonoBehaviour
{
    public bool isOccupied = false;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("damageFrame", 2, 2);
    }

    private void damageFrame()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
