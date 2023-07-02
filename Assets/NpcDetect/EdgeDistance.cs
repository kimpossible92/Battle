using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EdgeDistance : MonoBehaviour
{
    public Transform FirstObject;
    public Transform SecondObject; 
    private float _edgeDistance = 0.0f;

    void FindRaycastEdgeDistance()
    {
        _edgeDistance = Vector3.Distance(FirstObject.position, SecondObject.position); // Найдём расстояние между центрами объектов

        RaycastHit rayHit;
	var direction = Vector3.zero - (FirstObject.position - SecondObject.position).normalized;
        if (Physics.Raycast(FirstObject.position, direction, out rayHit) ) 
        {
            _edgeDistance -= Vector3.Distance(rayHit.point, SecondObject.position); // Найдём и вычтем | (b1, b2) |
        }
     
	direction = Vector3.zero - (FirstObject.position - SecondObject.position).normalized;
        if (Physics.Raycast(SecondObject.position, direction, out rayHit) ) 
        {
            _edgeDistance -= Vector3.Distance(rayHit.point, FirstObject.position); // Найдём и вычтем | (a1, a2) |
        }
        
        Debug.Log($"Длина между гранями: {_edgeDistance}");
    }
	private Collider _firstObjectCollider;
	private Collider _secondObjectCollider;

	private void Start()
	{
		_firstObjectCollider = FirstObject.GetComponent<Collider>();
		_secondObjectCollider = SecondObject.GetComponent<Collider>();
	}

	void FindShortEdgeDistance()
	{
		var firstClosestSurfacePoint = _firstObjectCollider.ClosestPointOnBounds(SecondObject.position); // Найдём ближайщую ко второму объекту точку на грани первого
		var secondClosestSurfacePoint = _secondObjectCollider.ClosestPointOnBounds(FirstObject.position); // Найдём ближайщую к первому объекту точку на грани второго
		_edgeDistance = Vector3.Distance(firstClosestSurfacePoint, secondClosestSurfacePoint); // Найдём расстояние между ними.
		
		Debug.Log($"Длина между гранями: {_edgeDistance}");
	}
}