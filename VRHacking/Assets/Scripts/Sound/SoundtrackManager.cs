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

    [Header("Background")]
    [SerializeField]
    private AudioClip bgClip;

    [Header("Endless Tracks")]
    [SerializeField]
    private AudioClip[] dynamicTracksByIntensity;

    void OnEnable() {
        targetVolume = defaultVolume;
        PlayBackgroundNoise();
        currentTrackPlayer = secondTrackPlayer;
    }
    public void InitializeTrack(int targetIntensity = 0) {
        intensityLevel = targetIntensity;
        if(bgTrackPlayer.isPlaying) {
            DoTrackFade(bgTrackPlayer, true, 2f);
        }
        DoCrossfadeTracks(dynamicTracksByIntensity[intensityLevel], fadeTracksDuration);
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

    public void UpdateIntensityByDifficulty(float difficulty) {
        intensityLevel = (int)(Mathf.Lerp(0, dynamicTracksByIntensity.Length - 1, Mathf.Round(difficulty - 1)));

        DoCrossfadeTracks(dynamicTracksByIntensity[intensityLevel], fadeTracksDuration);
    }

    private void DoCrossfadeTracks(AudioClip target, float duration) {
        if(currentTrackPlayer.clip == target)
            return;

        AudioSource targetPlayer = currentTrackPlayer == firstTrackPlayer ? secondTrackPlayer : firstTrackPlayer;
        targetPlayer.clip = target;
        targetPlayer.Play();
        targetPlayer.time = currentTrackPlayer.clip == null ? 0 : currentTrackPlayer.time;
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
