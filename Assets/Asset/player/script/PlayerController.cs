using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	// Player animator
	private Animator m_animator;

	private Vector3 OldPos;
	[SerializeField]private float CoefX = 1f,CoefY = 1f;
	// Use this for initialization
	void Start ()
	{
		OldPos = transform.position;
		// Get the animator controller of the player
		m_animator = GetComponent<Animator> ();
		//m_animator.SetTrigger ("WalkBegin");

	}
	
	// Update is called once per frame
	void Update () {
		//var Xmove = Input.GetAxis("Horizontal");
		//var Ymove = Input.GetAxis("Vertical");
		//transform.Translate(Xmove*CoefX,0,Ymove*CoefY);
		// check if the user ask for walk
		transform.localPosition = Vector3.zero;
		if (Input.GetAxis ("Horizontal") > 0||Input.GetAxis ("Vertical") > 0) { 
			m_animator.SetBool("IsWalking", true);
		} else {
			m_animator.SetBool("IsWalking", false);
		}

		if (transform.position.y < -4f)
		{
			transform.position = OldPos;
		}
		// check if the user ask for back
		if (Input.GetAxis ("Horizontal") < 0||Input.GetAxis ("Vertical") < 0) { 
			m_animator.SetBool("IsBacking", true);
		} else {
			m_animator.SetBool("IsBacking", false);
		}

		// Punch!
		if (Input.GetButtonDown ("Fire1")) {
			m_animator.SetTrigger("Punch");
		}

	}
}
