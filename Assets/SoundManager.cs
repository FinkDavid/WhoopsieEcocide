using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SoundEffects
{
    public AudioClip Explosion;
    public AudioClip BlockDestroy;
    public AudioClip BlockPlace;
    public AudioClip PlayerDeath;
    public AudioClip PlayerJump;
    public AudioClip PlayerStun;
    public AudioClip PushBack;
    public AudioClip OilFreeze;
    public AudioClip StartSound;
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip _countdown;
    [SerializeField] private AudioClip _soundTrack;
    [SerializeField] private AudioClip _menu;
    [SerializeField] private AudioClip _winning;
    [SerializeField] private AudioClip _loosing;

    [SerializeField] private AudioSource _soundTrackSource;
    [SerializeField] private AudioSource _endScreendSource;

    [Header("Sound Effects")]
    [SerializeField] private SoundEffects _soundEffects;

    private AudioSource _source;

    public ref readonly SoundEffects SoundEffects => ref _soundEffects;

    private Coroutine _startCountdownCoroutine;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlayMenu()
    {
        _source.clip = _menu;
        _source.loop = true;
        _source.Play();
    }

    public void StopMenu()
    {
        _source.Stop();
        _source.loop = false;
    }

    public void PlayWinning()
    {
        _endScreendSource.clip = _winning;
        _endScreendSource.loop = true;
        _endScreendSource.Play();
    }

    public void StopWinning()
    {
        StartCoroutine(FadeOutSoundtrack(_endScreendSource, 1f));
    }

    public void StartCountdown()
    {
        _startCountdownCoroutine = StartCoroutine(StartCountdownCoroutine());
    }

    public void StopSoundtrack()
    {
        // if the intro countdown is now playing we have to stop the coroutine
        if (_startCountdownCoroutine != null)
        {
            StartCoroutine(FadeOutSoundtrack(_source, 1f));
            // soundtrack source is playing on delayed
            _soundTrackSource.Stop();
            StopCoroutine(_startCountdownCoroutine);
            _startCountdownCoroutine = null;
        }
        else
        {
            StartCoroutine(FadeOutSoundtrack(_soundTrackSource, 1f));
        }     
    }

    public void PlaySoundEffect(AudioClip effect, float? volume = null)
    {
        _source.PlayOneShot(effect, volume ?? UnityEngine.Random.Range(.1f, .3f));
    }

    private IEnumerator StartCountdownCoroutine()
    {
        PlaySoundEffect(SoundEffects.StartSound, .3f);
        _source.clip = _countdown;
        _source.loop = false;
        _source.Play();
        _soundTrackSource.PlayDelayed(_countdown.length);
        yield return new WaitForSeconds(_countdown.length);
        _source.Stop();
        _startCountdownCoroutine = null;

        //_source.clip = _soundTrack;
        //_source.loop = true;  
    }

    private IEnumerator FadeOutSoundtrack(AudioSource source, float fadeTime)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / fadeTime;
            //yield return new WaitForEndOfFrame();
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }
}
