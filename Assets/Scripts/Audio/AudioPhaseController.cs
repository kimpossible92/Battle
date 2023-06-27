using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioPhaseController : MonoBehaviour
{
    public List<AudioSourceHolder> audioSourceHolders;
    public float fadeDuaration = 1f;

    private int currentIndex = 0;

    public void PlayAudioPhaseAt(int index)
    {
        currentIndex += index;
        AudioSource prev = currentIndex > 0 ?
            audioSourceHolders[currentIndex - 1].audioSource : null;
        if (currentIndex >= audioSourceHolders.Count)
            currentIndex = 0;

        StartFade(audioSourceHolders[currentIndex].audioSource, prev);

        if (!audioSourceHolders[currentIndex].loop)
        {
            audioSourceHolders[currentIndex].audioSource.loop = false;
            StartTimer(audioSourceHolders[currentIndex].fadingTime, () =>
            {
                PlayAudioPhaseAt(currentIndex + 1);
            });
        }
        else
        {
            audioSourceHolders[currentIndex].audioSource.loop = true;
        }
    }

    public void FadeAndStopPhase()
    {
        if (IFadeAudioHelper != null)
            StopCoroutine(IFadeAudioHelper);
        IFadeAudioHelper = null;

        if (IAudioTimerHelper != null)
            StopCoroutine(IAudioTimerHelper);
        IAudioTimerHelper = null;

        foreach (AudioSourceHolder ash in audioSourceHolders)
        {
            ash.audioSource.Stop();
        }

    }

    private void StartFade(AudioSource next, AudioSource previous)
    {
        if (IFadeAudioHelper != null)
            StopCoroutine(IFadeAudioHelper);
        IFadeAudioHelper = StartCoroutine(IFadeAudio(next, previous));
    }

    private void StartTimer(float time, UnityAction onTimerEnd)
    {
        if (IAudioTimerHelper != null)
            StopCoroutine(IAudioTimerHelper);
        IAudioTimerHelper = StartCoroutine(IAudioTimer(time, onTimerEnd));
    }

    private Coroutine IFadeAudioHelper;
    private IEnumerator IFadeAudio(AudioSource next, AudioSource prev)
    {
        float t = 0f;
        if (next != null)
            next.Play();
        while (t < fadeDuaration)
        {
            if (prev != null)
                prev.volume = Mathf.Lerp(1f, 0f, t / fadeDuaration);
            if (next != null)
                next.volume = Mathf.Lerp(0f, 1f, t / fadeDuaration);

            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (prev != null)
            prev.volume = 0f;
        if (next != null)
            next.volume = 1f;
        if (prev != null)
            prev.Stop();

        IFadeAudioHelper = null;
    }

    private Coroutine IAudioTimerHelper;
    private IEnumerator IAudioTimer(float time,UnityAction onTimerEnd)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        onTimerEnd?.Invoke();

        IAudioTimerHelper = null;
    }
}
