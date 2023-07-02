using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentFire : MonoBehaviour
{
    //private Vector3 DestroyPosition;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetEnumerator());
    }
    IEnumerator GetEnumerator()
    {
        yield return new WaitForSeconds(4.0f);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
