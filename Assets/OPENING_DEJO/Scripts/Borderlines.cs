using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Borderlines : MonoBehaviour
{
    private bool isFadingOut;
    private float timeToFade = 0.5f;
    private float lastDecrease = float.MinValue;
    private SpriteRenderer spriteRenderer;
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update() {
        FadingOut();
    }
    public void StartSound() {
        OpeningCredits.Instance.PlayFirstClip();
    }
    public void PlayNextAnimation() {
        OpeningCredits.Instance.PlayAnimation();
    }
    public void FadeAudioOut() {
        OpeningCredits.Instance.AudioFadeOut();
    }
    public void FadeOut()
    {
        gameObject.SetActive(true);
        isFadingOut = true;
    }
    private void FadingOut()
    {
        if (isFadingOut)
        {
            if (timeToFade > 0 && Time.time > lastDecrease + 0.1f)
            {
                lastDecrease = Time.time;
                Color color = spriteRenderer.color;
                color.a -= 0.1f;
                spriteRenderer.color = color;
                timeToFade -= Time.deltaTime;
            }
        }
    }
}
