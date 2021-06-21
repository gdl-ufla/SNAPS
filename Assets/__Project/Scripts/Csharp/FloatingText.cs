using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using YoukaiFox.UnityExtensions;

public class FloatingText : MonoBehaviour
{
    #region Serialized fields

    [SerializeField]
    private float _movementDistance = 0.5f;

    [SerializeField]
    private float _movementDuration = 1.5f;

    [SerializeField]
    private TextMeshProUGUI _displayText;

    #endregion

    #region Non-serialized fields

    private Tween _tween;
    private Transform _transform;

    #endregion

    #region Unity events

    private void Awake() 
    {
        _transform = transform;
        _displayText = gameObject.GetComponentInSelfOrChildren<TextMeshProUGUI>();
    }

    private void Start() 
    {
        gameObject.SetActive(false);
        _tween = _transform.DOMoveY(_transform.position.y + _movementDistance, _movementDuration);
        _tween.Pause();
    }

    private void OnDisable() 
    {
        _tween.Kill();
    }

    #endregion

    #region Public methods

    public void SetText(string text)
    {
        _displayText.text = text;
    }

    public void PlayAnimation()
    {
        gameObject.SetActive(true);
        _tween.Play();
    }

    #endregion

    #region Private methods
    #endregion    
}
