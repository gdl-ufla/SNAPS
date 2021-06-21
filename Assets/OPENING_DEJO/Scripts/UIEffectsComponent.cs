using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEffectsComponent : MonoBehaviour
{
    #region variables
    public UnityEngine.UI.MaskableGraphic graphic;
    private Vector2 originalPosition;
    private bool isFadingOut;
    private bool isFadingIn;
    private bool isFloating;
    private float effectDuration = 1.5f;
    private float delay = 1.25f;
    private float timeLeft;
    private float colorChangeValue;
    private float positionChangeValue = 0.1f;
    #endregion
    private void Awake() 
    {
        originalPosition = transform.position;
        timeLeft = effectDuration + delay;
        colorChangeValue = 1 / (60 * effectDuration) + 0.005f;
    }
    private void Update() {
        FadingOut();
        FadingIn();
        Floating();
    }
    public void FadeOut(float duration = 1f)
    {
        isFadingIn = false;
        UpdateDuration(duration);
        gameObject.SetActive(true);
        ResetColor();
        isFadingOut = true;
        // print(timeLeft);
    }
    private void FadingOut()
    {
        if (isFadingOut)
        {
            if (timeLeft > 0)
            {
                if (timeLeft < effectDuration)
                {
                    DecreaseColorAlpha();
                }

                timeLeft -= Time.deltaTime;
            }
            else
            {
                isFadingOut = false;
                timeLeft = effectDuration + delay;
            }
        }
    }
    public void FadeIn(float duration = 1f)
    {
        UpdateDuration(duration);
        gameObject.SetActive(true);
        ZerateAlpha();
        isFadingIn = true;
    }
    private void FadingIn()
    {
        if (isFadingIn)
        {
            if (timeLeft > 0)
            {
                IncreaseColorAlpha();
                timeLeft -= Time.deltaTime;
            }
            else
            {
                isFadingIn = false;
                timeLeft = effectDuration;
            }
        }
    }
    public void Float()
    {
        transform.position = originalPosition;
        gameObject.SetActive(true);
        isFloating = true;
    }
    private void Floating()
    {
        if (isFloating)
        {
            if (timeLeft > 0)
            {
                IncreaseYPosition();
                timeLeft -= Time.deltaTime;
            }
            else
            {
                isFloating = false;
                timeLeft = effectDuration;
                FadeOut();
            }
        }
    }
    public void ResetColor()
    {
        Color color = graphic.color;
        color.a = 1;
        graphic.color = color;
    }
    public void ZerateAlpha()
    {
        Color color = graphic.color;
        color.a = 0;
        graphic.color = color;
    }
    public void ResetEffects()
    {
        isFadingOut = false;
        isFadingIn = false;
        isFloating = false;
        transform.position = originalPosition;
    }
    private void UpdateDuration(float duration)
    {
        timeLeft = duration + delay;
        colorChangeValue = 1 / (60 * effectDuration) + 0.005f;
    }
    private void DecreaseColorAlpha()
    {
        Color color = graphic.color;
        color.a -= colorChangeValue;
        graphic.color = color;
    }
    private void IncreaseColorAlpha()
    {
        Color color = graphic.color;
        color.a += colorChangeValue;
        graphic.color = color;
    }
    private void IncreaseYPosition()
    {
        Vector2 nextPos = graphic.rectTransform.anchoredPosition;
        nextPos.y += positionChangeValue;
        graphic.rectTransform.anchoredPosition = nextPos;
    }
    public UIEffectsComponent[] GetAllUIEffectsFromSlider(Slider slider)
    {
        UIEffectsComponent[] effects = new UIEffectsComponent[3];
        effects[0] = slider.transform.Find("Background").GetComponent<UIEffectsComponent>();
        effects[1] = slider.transform.Find("Fill Area").transform.GetChild(0).GetComponent<UIEffectsComponent>();
        effects[2] = slider.transform.Find("Handle Slide Area").transform.GetChild(0).GetComponent<UIEffectsComponent>();
        return effects;
    }
    public void SetFadeOutDuration(float time)
    {
        effectDuration = time;
    }
    public void SetDelayTime(float time)
    {
        delay = time;
    }
    public void SetColorDecreaseValue(float value)
    {
        colorChangeValue = value;
    }
    public void SetYPositionChangeValue(float value)
    {
        positionChangeValue = value;
    }
}
