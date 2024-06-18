using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundtrackManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource firstTrackPlayer;
    [SerializeField]
    private AudioSource secondTrackPlayer;
    [SerializeField]
    private AudioSource bgTrackPlayer;
    private AudioSource currentTrackPlayer;

    [SerializeField]
    private float fadeTracksDuration;
    [SerializeField]
    private float defaultVolume;
    private float targetVolume;
    private int intensityLevel;
    private int currentSoundtrack;

    [Header("Background")]
    [SerializeField]
    private AudioClip bgClip;

    [Header("Endless Tracks")]
    [SerializeField]
    private Soundtrack[] dynamicTracksByIntensity;

    [System.Serializable]
    private class Soundtrack
    {
        public List<AudioClip> variationsByIntensity;
        private int currentIndex;

        public AudioClip GetNextClipInSequence()
        {
            currentIndex++;

            if (currentIndex > variationsByIntensity.Count)
            {
                Debug.LogException(new Exception("There are no more soundtrack variations"));
                return null;
            }
            
            return variationsByIntensity[currentIndex];
        }

        public void ResetToBeginning() => currentIndex = 0;

        public AudioClip GetCurrentTrack() => variationsByIntensity[currentIndex];
        public AudioClip GetTrackByIntensity(int intensity) => variationsByIntensity[intensity];
    } 

    void OnEnable() {
        targetVolume = defaultVolume;
        PlayBackgroundNoise();
        currentTrackPlayer = secondTrackPlayer;
    }

    public void ChooseRandomTrack() {
        int rnd = UnityEngine.Random.Range(0, dynamicTracksByIntensity.Length);
        if(rnd == currentSoundtrack) {
            rnd++;
            if(rnd > dynamicTracksByIntensity.Length) {
                rnd = 0;
            }
        }

        currentSoundtrack = rnd;
    }
    public void InitializeTrack(GameSettingsData settings) {
        UpdateIntensityByDifficulty(settings.difficulty);
        if(bgTrackPlayer.volume > 0) {
            DoTrackFade(bgTrackPlayer, true, 0.25f);
        }
        DoCrossfadeTracks(dynamicTracksByIntensity[currentSoundtrack].GetTrackByIntensity(intensityLevel), fadeTracksDuration);
    }

    public void ModifyVolume(float clampedModifier) => targetVolume = (defaultVolume *= clampedModifier);

    public void ResetVolume() => targetVolume = defaultVolume;
    
    public void SilenceAll() {
        DoFullFade(true, 0.25f);
        DoTrackFade(bgTrackPlayer, true);
        firstTrackPlayer.clip = null;
        secondTrackPlayer.clip = null;
        bgTrackPlayer.clip = null;
    }

    public void PlayBackgroundNoise() {
        bgTrackPlayer.clip = bgClip;
        DoTrackFade(bgTrackPlayer, false, fadeTracksDuration);
    }

    private void UpdateIntensityByDifficulty(float difficulty) {
        intensityLevel = (int)(Mathf.Lerp(0, dynamicTracksByIntensity.Length - 1, Mathf.Round(difficulty - 1)));
    }

    private void DoCrossfadeTracks(AudioClip target, float duration) {
        if(currentTrackPlayer.clip == target)
            return;

        AudioSource targetPlayer = currentTrackPlayer == firstTrackPlayer ? secondTrackPlayer : firstTrackPlayer;
        targetPlayer.clip = target;
        targetPlayer.Play();
        targetPlayer.time = currentTrackPlayer.clip == null ? 0 : currentTrackPlayer.time > targetPlayer.clip.length ? 0 : currentTrackPlayer.time;
        targetPlayer.DOFade(targetVolume, duration);
        currentTrackPlayer.DOFade(0, duration).OnComplete(() => SwapCurrentTrack(targetPlayer));
    }

    private void DoFullFade(bool outFade, float duration = -1) {
        float targetFadeVolume = outFade ? 0 : targetVolume;
        if(duration < 0)
            duration = fadeTracksDuration;
        
        if(firstTrackPlayer.clip != null) {
            Tween t = firstTrackPlayer.DOFade(targetFadeVolume, duration);
            if(outFade) {
                t.OnComplete(() => firstTrackPlayer.Stop());
            }
            else {
                firstTrackPlayer.Play();
            }
        }

        if(secondTrackPlayer.clip != null) {
            Tween t = secondTrackPlayer.DOFade(targetFadeVolume, duration);
            if(outFade) {
                t.OnComplete(() => secondTrackPlayer.Stop());
            }
            else {
                secondTrackPlayer.Play();
            }
        }
    }

    private void DoTrackFade(AudioSource source, bool outFade, float duration = -1) {
        float targetFadeVolume = outFade ? 0 : targetVolume;
        if(duration < 0)
            duration = fadeTracksDuration;

        if(source.clip != null) {
            Tween t = source.DOFade(targetFadeVolume, duration);
            if(outFade) {
                t.OnComplete(() => source.Stop());
            }
            else {
                source.Play();
            }
        }
    }
    private void SwapCurrentTrack(AudioSource target) {
        currentTrackPlayer = target;
    }
}
