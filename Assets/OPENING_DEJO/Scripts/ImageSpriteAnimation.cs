using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSpriteAnimation : MonoBehaviour
{
    public float constantFrameRate = 0.1f;
    public Sprite[] frames;
    private Image image;
    private bool animationEnded;
    private void Awake() 
    {
        image = GetComponent<Image>();
    }
    public void PlayAnimation() {StartCoroutine(PlayAnimationRoutine());}
    public IEnumerator PlayAnimationRoutine()
    {
        for (int i = 0; i < frames.Length; i++)
        {
            image.sprite = frames[i];
            yield return new WaitForSeconds(constantFrameRate);
        }

        animationEnded = true;
    }
    public bool GetAnimationEnded() {return animationEnded;}
}
