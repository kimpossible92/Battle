using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioPhaseController novelPhaseAudios;
    [SerializeField] private AudioPhaseController deckBuildingPhaseAudios;
    [SerializeField] private AudioPhaseController battlePhaseAudios;
    [SerializeField] private AudioPhaseController mainMenuPhaseAudios;
    [SerializeField] private AudioPhaseController gameEndAudios;

    private void Awake()
    {
        instance = this;
    }

    public void PlayMainMenuPhaseAudios()
    {
        StopAll();
        mainMenuPhaseAudios.PlayAudioPhaseAt(0);
    }

    public void PlayNovelPhaseAudios()
    {
        StopAll();
        novelPhaseAudios.PlayAudioPhaseAt(0);
    }

    public void PlayDeckBuildingPhaseAudios()
    {
        StopAll();
        deckBuildingPhaseAudios.PlayAudioPhaseAt(0);
    }

    public void PlayBattlePhaseAudios()
    {
        StopAll();
        battlePhaseAudios.PlayAudioPhaseAt(0);
    }

    public void PlayGameEndAudios()
    {
        StopAll();
        gameEndAudios.PlayAudioPhaseAt(0);
    }

    public void StopAll()
    {
        mainMenuPhaseAudios.FadeAndStopPhase();
        novelPhaseAudios.FadeAndStopPhase();
        deckBuildingPhaseAudios.FadeAndStopPhase();
        battlePhaseAudios.FadeAndStopPhase();
        gameEndAudios.FadeAndStopPhase();
    }
}
