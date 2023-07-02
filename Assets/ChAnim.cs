using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class IChAnimParameterId
{
    public static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
    public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
    public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
}
public class ChAnim : MonoBehaviour
{
	private Animator _animator;
	private ICh _character;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_character = GetComponent<ICh>();
	}

	public void UpdateState()
	{
		float normHorizontalSpeed = _character.HorizontalVelocity.magnitude / _character.mvSt.MaxHorSpeed;
		//_animator.SetFloat(IChAnimParameterId.HorizontalSpeed, normHorizontalSpeed);

		float jumpSpeed = _character.mvSt.JSpeed;
		float normVerticalSpeed = _character.VerticalVelocity.y.Remap(-jumpSpeed, jumpSpeed, -1.0f, 1.0f);
		//_animator.SetFloat(IChAnimParameterId.VerticalSpeed, normVerticalSpeed);

		//_animator.SetBool(IChAnimParameterId.IsGrounded,
		//	_character.IsGrounded);
	}
}
