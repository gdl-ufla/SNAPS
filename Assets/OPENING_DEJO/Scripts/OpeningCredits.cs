using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class OpeningCredits : MonoBehaviour 
{
    public string nameOfNextScene;
    public float pauseTime = 0.25f;
    public float scrollFadeInTime = 0.25f;
    public float uflaLogoDuration = 2f;
    public float elementsFadeOutTime = 0.5f;
    public float logoShowTime = 2f;
    public UIEffectsComponent bg;
    public UIEffectsComponent scroll;
    public UIEffectsComponent uflaLogo;
    public UIEffectsComponent labTxt;
    public Borderlines borderlines;
    public AudioClip sndEffect01;
    public AudioClip sndEffect03;
    private AudioSource audioSource;
    private bool audioIsFadingOut;
    private float audioFadeOutTime = 0.25f;
    private float decreaseInterval;
    private float lastDecreaseTime = float.MinValue;
    public static OpeningCredits Instance { get; private set; }
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy (this.gameObject);
        } else {
            Instance = this;
        }

        audioSource = GetComponent<AudioSource>();
    }
    private void Start() {
        audioSource.loop = true;
    }
    private void Update() {
        AudioFadingOut();
    }
    private void StartGame() {
        // SceneManager.LoadScene(nameOfNextScene);
    }
    public void PlayAnimation() {StartCoroutine(PlayAnimationRoutine());}
    private IEnumerator PlayAnimationRoutine() {
        audioIsFadingOut = false;
        audioSource.Stop();
        yield return new WaitForSeconds(pauseTime);
        labTxt.FadeIn(scrollFadeInTime);
        scroll.FadeIn(scrollFadeInTime);
        audioSource.loop = false;
        PlayClip(sndEffect03);
        yield return new WaitForSeconds(logoShowTime);
        FadeLogoOut();
        yield return new WaitForSeconds(elementsFadeOutTime);
        uflaLogo.FadeIn();
        yield return new WaitForSeconds(uflaLogoDuration / 2);
        bg.gameObject.SetActive(false);
        scroll.gameObject.SetActive(false);
        labTxt.gameObject.SetActive(false);
        borderlines.gameObject.SetActive(false);
        yield return new WaitForSeconds(uflaLogoDuration / 2);
        StartGame();
    }
    private void FadeLogoOut() {
        bg.FadeOut(elementsFadeOutTime);
        scroll.FadeOut(elementsFadeOutTime);
        labTxt.FadeOut(elementsFadeOutTime);
        borderlines.FadeOut();
    }
    public void AudioFadeOut(float duration = 1f)
    {
        gameObject.SetActive(true);
        audioIsFadingOut = true;
        decreaseInterval = audioFadeOutTime / 10;
    }
    private void AudioFadingOut()
    {
        if (audioIsFadingOut)
        {
            if (audioFadeOutTime > 0 && Time.time > lastDecreaseTime + decreaseInterval)
            {
                lastDecreaseTime = Time.time;
                audioSource.volume -= 0.1f;
                audioFadeOutTime -= Time.deltaTime;
            }
        }
    }
    public void PlayFirstClip() {
        PlayClip(sndEffect01);
    }
    private void PlayClip(AudioClip clip) {
        if (audioSource.isPlaying)
            audioSource.Stop();
        
        audioSource.volume = 1;
        audioSource.clip = clip;
        audioSource.Play();
    }
}