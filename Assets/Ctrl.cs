using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ctrl : ScriptableObject
{
	public ICh ich { get; set; }

	public abstract void Init();
	public abstract void OnIChUpdate();
	public abstract void OnIChFixedUpdate();
}
