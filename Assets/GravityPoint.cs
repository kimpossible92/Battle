using UnityEngine;
using System.Collections;
[AddComponentMenu("Corgi Engine/Environment/Gravity Point")]
public class GravityPoint : MonoBehaviour
{
    public float DistanceOfEffect;

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, DistanceOfEffect);
    }
    public float updowndistance;
    public bool updown;
    private void Update()
    {
        
    }

}
