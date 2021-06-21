using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using YoukaiFox.UnityExtensions;
using DG.Tweening;
using NaughtyAttributes;

public class BlockHardMode : MonoBehaviour
{
    #region Actions

    public static Action OnPathEndReach;
    public static Action<bool> OnPlayerCollision;

    #endregion

    #region Serialized fields
    #pragma warning disable 0649 

    [SerializeField]
    [BoxGroup("Gameplay values")]
    private float _timeToReachFreezeZone = 3f;

    [SerializeField]
    [BoxGroup("Gameplay values")]
    private float _freezingDuration = 3f;

    [SerializeField]
    [BoxGroup("Gameplay values")]
    private float _timeToReachDestination = 5f;

    [SerializeField]
    [BoxGroup("Position references")]
    private Transform _initialPosition;

    [SerializeField]
    [BoxGroup("Position references")]
    private Transform _freezingPosition;

    [SerializeField]
    [BoxGroup("Position references")]
    private Transform _destinationPosition;

    [SerializeField]
    [BoxGroup("Particle references")]
    private ParticleSystem _collisionParticles;

    [SerializeField] 
    [BoxGroup("Particle references")]
    private ParticleSystem _successParticles;

    [SerializeField] 
    [BoxGroup("Particle references")]
    private ParticleSystem _failureParticles;

    #pragma warning restore 0649 
    #endregion

    #region Non-serialized fields

    private Transform _transform;
    private TextMeshProUGUI _displayText;
    private Sequence _sequence;
    private RectTransform _rectTransform;
    [ShowNonSerializedField]
    private bool _isAnswerCorrect;
    private BoxCollider2D _collider;

    #endregion

    #region Unity events

    private void Awake() 
    {
        _displayText = gameObject.GetComponentInSelfOrChildren<TextMeshProUGUI>();
        _rectTransform = gameObject.GetComponentInSelfOrChildren<RectTransform>();
        _collider = gameObject.GetOrAddComponent<BoxCollider2D>();
    }

    private void Start() 
    {
        CreateSequence();
    }

    private void OnDestroy() 
    {
        _sequence.Kill();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        OnCollisionBehaviour();
    }

    #endregion

    #region Public methods

    public void Kill()
    {
        _sequence.Kill();

        if (gameObject.activeInHierarchy)
            SpawnCollisionParticles();

        gameObject.SetActive(false);
    }

    public void SetAnswer(string text, bool isAnswerCorrect)
    {
        _displayText.text = text;
        _isAnswerCorrect = isAnswerCorrect;
    }

    public void Restore()
    {
        gameObject.SetActive(true);
        _collider.enabled = true;

        if ((_sequence.IsActive()) && (_sequence.IsPlaying()))
            _sequence.Pause();

        transform.position = _initialPosition.position;
        _sequence.Restart();
    }

    public void DisableCollider()
    {
        _collider.enabled = false;
    }

    #endregion

    #region Private methods

    private void CreateSequence()
    {
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMoveY(_freezingPosition.position.y, _timeToReachFreezeZone).SetEase(Ease.Linear));
        _sequence.AppendInterval(_freezingDuration);
        _sequence.Append(transform.DOMoveY(_destinationPosition.position.y, _timeToReachDestination).SetEase(Ease.Linear));

        if (OnPathEndReach != null)
            _sequence.AppendCallback(() => OnPathEndReach());
            
        _sequence.SetAutoKill(false);
    }

    private void OnCollisionBehaviour()
    {
        if (OnPlayerCollision != null)
            OnPlayerCollision(_isAnswerCorrect);

        SpawnCollisionParticles();

        if (_isAnswerCorrect)
        {
            SpawnSuccessParticles();
        }
        else
        {
            SpawnFailureParticles();
        }

        gameObject.SetActive(false);
    }

    private void SpawnCollisionParticles()
    {
        _collisionParticles.transform.position = transform.position;
        _collisionParticles.PlayForced();
    }

    private void SpawnSuccessParticles()
    {
        _successParticles.transform.position = transform.position;
        _successParticles.PlayForced();
    }

    private void SpawnFailureParticles()
    {
        _failureParticles.transform.position = transform.position;
        _failureParticles.PlayForced();
    }

    #endregion
}
