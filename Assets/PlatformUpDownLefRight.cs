using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformUpDownLefRight : MonoBehaviour
{
    enum platformNapravlenie { Up,Down,Left,Right };
    [SerializeField] platformNapravlenie napravlenie;
    Vector3 pointStart;
    private void Awake()
    {
        pointStart = transform.position;
        StartCoroutine(GetEnumerator());
    }
    IEnumerator GetEnumerator()
    {
        while (true)
        {
            transform.position += Vector3.up*2f;
            yield return new WaitForSeconds(1.8f);
            transform.position += Vector3.down*2f;
            yield return new WaitForSeconds(1.8f);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < (pointStart + Vector3.up * 4).y) { napravlenie = platformNapravlenie.Up; }//
        if (transform.position.y > (pointStart - (Vector3.up * 4)).y) { napravlenie = platformNapravlenie.Down; }
    }
}
