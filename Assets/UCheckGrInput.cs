using UnityEngine;
using System.Collections;

public class UCheckGrInput : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        CharacterGravity characterGravity = collider.gameObject.GetComponentNoAlloc<CharacterGravity>();
        if (characterGravity == null)
        {
            return;
        }
        else
        {
            characterGravity.ReverseHorizontalInputWhenUpsideDown = false;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
