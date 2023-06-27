using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



class GameTime : MonoBehaviour
{
    public float time;
    public bool startOnEnable;
    public UnityEvent OnStart;
    public UnityEvent OnEnd;
    public void OnEnable()
    {
        if (startOnEnable)
        {
            StartTimer();
        }
    }
    public void OnDisable()
    {
        StopTimer();
    }

    public void StartTimer()
    {
        if (ITimerHelper == null)
            ITimerHelper = StartCoroutine(ITimer());
    }

    public void StopTimer()
    {
        if (ITimerHelper != null)
            StopCoroutine(ITimerHelper);
    }
    private Coroutine ITimerHelper;
    private IEnumerator ITimer()
    {
        float t = 0;
        if (time <= 0f)
        {
            ITimerHelper = null;
            yield break;
        }
        OnStart?.Invoke();
        while (t / time <= 1f)
        {
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        OnEnd?.Invoke();
        ITimerHelper = null;
    }
}
