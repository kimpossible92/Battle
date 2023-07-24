using System.Collections;
using RPG.Combat;
using RPG.Controller;
using UnityEngine;

public class PickableWeapon3 : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private float respawnTime = 5f;
    
    private void OnTriggerEnter(Collider other)
    {
        Fighter3 fighter = other.GetComponent<Fighter3>();
        if (fighter && other.GetComponent<PlayerController3>())
        {
           
            fighter.EquipWeapon(weapon); 
            //SwordText.text = fighter.CaptureState().ToString();
            StartCoroutine(HideForSeconds(respawnTime));
        }
    }
    private void Update()
    {
        //if(weapon!=null)SwordText.text = weapon.name;
    }
    private void Start()
    {
        //SwordText.text = fighter.CaptureState().ToString();
    }
    private IEnumerator HideForSeconds(float seconds)
    {
        ShowPickup(false);
        yield return new WaitForSeconds(seconds);
        ShowPickup(true);

    }

    private void ShowPickup(bool shouldShow)
    {
        //transform.Find("childname") returns the child with name childname
        GetComponent<SphereCollider>().enabled = shouldShow;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(shouldShow);
        }
    }

   
    
}
