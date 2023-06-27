using UnityEngine;
using System;

[Serializable]
public class AudioSourceHolder
{
    public string name;
    public AudioSource audioSource;
    public float fadingTime;
    public bool loop;
}
