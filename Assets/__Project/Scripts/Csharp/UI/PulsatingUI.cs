using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PulsatingUI : MonoBehaviour
{
    [SerializeField]
    private float _reductionRate = 0.025f;

    [SerializeField]
    private float _animationDuration = 5f;

    private Image _image;
    private Sequence _sequence;
    private Vector2 _initialScale;

    private void Awake() 
    {
        _image = GetComponent<Image>();
    }

    private void Start() 
    {
        _initialScale = _image.transform.localScale;
        _image.transform.localScale = _initialScale * (1 - _reductionRate);
        CreateSequence();
    }

    private void OnDisable() 
    {
        _sequence.Kill();
    }

    private void CreateSequence()
    {
        _sequence = DOTween.Sequence();
        _sequence.Append(_image.transform.DOScale(_initialScale * (1 + _reductionRate), _animationDuration).SetEase(Ease.Linear));
        _sequence.Append(_image.transform.DOScale(_initialScale * (1 - _reductionRate), _animationDuration).SetEase(Ease.Linear));
        _sequence.SetAutoKill(false);
        _sequence.SetLoops(-1);
    }
}
