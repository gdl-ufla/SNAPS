using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class SplashElement : MonoBehaviour
{
    #region Serialized fields
    #pragma warning disable 0649 

    [SerializeField]
    [HideIf((nameof(_isSprite)))]
    private bool _applyParentSettings = false;
    // UI
    [SerializeField] [HideIf((nameof(_isSprite)))] [BoxGroup("UI element settings")]
    private bool _isUiGraphic;

    // Sprite
    [SerializeField] [HideIf((nameof(_isUiGraphic)))] [BoxGroup("Sprite renderer settings")]
    private bool _isSprite;

    // Sprite animation
    [SerializeField] [ShowIf((nameof(_isSprite)))] [BoxGroup("Sprite animation settings")]
    private bool _hasAnimation;

    [SerializeField] [ShowIf((nameof(UseAnimation)))] [BoxGroup("Sprite animation settings")]
    private AnimationClip _animationClip;

    // Effects
    [SerializeField] [DisableIf(nameof(_applyParentSettings))] [BoxGroup("General display options")]
    private float _displayDuration;

    [SerializeField] [BoxGroup("General display options")]
    private bool _applySettingsToChildren;

    [SerializeField] [BoxGroup("General display options")]
    private Color _initialColor;

    [SerializeField] [BoxGroup("General display options")]
    private Color _clearColor;

    // Audio
    [SerializeField] [DisableIf(nameof(_applyParentSettings))] [BoxGroup("Audio playback settings")]
    private bool _playAudioClipAtStart;

    [BoxGroup("Audio playback settings")]
    [SerializeField, ShowIf(nameof(_playAudioClipAtStart)), DisableIf(nameof(_applyParentSettings))]
    private AudioClip _audioClipPlayedAtStart;

    [BoxGroup("Audio playback settings")]
    [SerializeField, ShowIf(nameof(_playAudioClipAtStart)), DisableIf(nameof(_applyParentSettings))]
    private bool _stopAudioFeedbackOnEnd;

    [BoxGroup("Audio playback settings")]
    [SerializeField, DisableIf(nameof(_applyParentSettings))]
    private bool _playAudioClipAtEnd;

    [BoxGroup("Audio playback settings")]
    [SerializeField, ShowIf(nameof(_playAudioClipAtEnd)), DisableIf(nameof(_applyParentSettings))]
    private AudioClip _audioClipPlayedAtEnd;

    // Transition settings
    [SerializeField] [DisableIf(nameof(_applyParentSettings))] [BoxGroup("Element transition settings")]
    private ActivationType _activationType;

     [BoxGroup("Element transition settings")]
    [SerializeField, ShowIf(nameof(HasFadeIn)), DisableIf(nameof(_applyParentSettings))]
    private float _fadeInDuration;

     [BoxGroup("Element transition settings")]
    [SerializeField, DisableIf(nameof(_applyParentSettings))]
    private DeactivationType _deactivationType;

     [BoxGroup("Element transition settings")]
    [SerializeField, ShowIf(nameof(HasFadeOut)), DisableIf(nameof(_applyParentSettings))]
    private float _fadeOutDuration;

     [BoxGroup("Element transition settings")]
    [SerializeField, DisableIf(nameof(_applyParentSettings))]
    private bool _hasTransition;

     [BoxGroup("Element transition settings")]
    [SerializeField, ShowIf(nameof(HasFadeOut))]
    [DisableIf(nameof(_applyParentSettings))]
    private bool _crossFadeWithTransition;

     [BoxGroup("Element transition settings")]
    [SerializeField] [MinMaxSlider(0f, 1f)]
    [ShowIf(nameof(HasCrossFade)), DisableIf(nameof(_applyParentSettings))]
    private float _blendingValue;

     [BoxGroup("Element transition settings")]
    [SerializeField, ShowIf(nameof(_hasTransition)), DisableIf(nameof(_applyParentSettings))]
    private SplashElement _transitionElement;

     [BoxGroup("Element transition settings")]
    [SerializeField][DisableIf(nameof(_applyParentSettings))]
    private bool _deactivateOnlyWhenTransitionObjectEnds;

    [SerializeField, BoxGroup("Debug settings")]
    private string _label;

    #pragma warning restore 0649 
    #endregion

    #region Non-serialized fields
    private SpriteRenderer _spriteRenderer;
    private UnityEngine.UI.MaskableGraphic _uiGraphic;
    private Vector2 _initialPosition;
    private UIEffectsComponent _uIEffectsComponent;
    private AudioSource _audioSource;
    private SplashElement[] _childrenElements;
    private SplashElement _linkedElement;
    private SplashElement _parentElement;
    private Animator _animator;
    private bool _isTrackingAnimationDuration = false;
    private float _remainingAnimationDurationTime;
    private Tween _fadeTween;
    private const float MaxAudioVolume = 1f;
    public static System.Action OnNoTransitionsLeft;
    private bool _isRestored;
    #endregion

    #region Enums
    public enum ActivationType
    {
        Instant, FadeIn
    }

    public enum DeactivationType
    {
        Instant, FadeOut
    }
    #endregion

    #region Attribute conditionals

    private bool UseAnimation => _hasAnimation && _isSprite;
    private bool HasCrossFade => _hasTransition && _crossFadeWithTransition;
    private bool HasFadeOut => _hasTransition && _deactivationType == DeactivationType.FadeOut;
    private bool HasFadeIn() {return _activationType == ActivationType.FadeIn;}
    #endregion

    #region Unity events
    private void Awake() 
    {
        AssignComponents();
    }

    private void Start() {
        _initialPosition = transform.position;

        if (_label.Equals(""))
            _label = gameObject.name;
            
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }

    private void OnEnable() 
    {
        Restore();
    }

    private void OnDisable() 
    {
        _fadeTween.Kill();
    }

    private void Update() 
    {
        TrackAnimationTime();
    }
    #endregion

    #region Public methods
    public void Play()
    {
        if ((!_uiGraphic) && (!_spriteRenderer))
        {
            Debug.LogError($"Splash element {gameObject} doesn't reference a graphic.");
            return;
        }

        Restore();
        gameObject.SetActive(true);
        OnStartingStageEnter();
    }

    public void Deactivate()
    {
        if (_deactivationType == DeactivationType.Instant)
        {
            if (_isUiGraphic)
                _uiGraphic.enabled = false;
            else if (_isSprite)
                _spriteRenderer.enabled = false;
        }
        else if (_deactivationType == DeactivationType.FadeOut)
        {
            FadeOut();
        }

        _linkedElement?.Deactivate();
    }

    public void SetLinkedElement(SplashElement element)
    {
        _linkedElement = element;
    }
    
    public bool CanApplyParentEffects() {return _applyParentSettings;}
    #endregion

    #region Events
    private void OnStartingStageEnter()
    {
        if (_playAudioClipAtStart)
        {
            PlayAudioClip(_audioClipPlayedAtStart);
        }

        if (_hasAnimation)
        {
            PlayAnimation();
        }

        if ((_activationType == ActivationType.Instant) && (!_hasAnimation))
        {
            OnStartingStageExit();
        }
        else if (_activationType == ActivationType.FadeIn)
        {
            FadeIn();
        }

        if ((_applySettingsToChildren) && (_childrenElements.Length > 0))
        {
            foreach (SplashElement splashChild in _childrenElements)
            {
                if (splashChild._applyParentSettings)
                    splashChild.Play();
            }
        }
    }

    private void OnStartingStageExit()
    {
        Invoke(nameof(OnEndingStageEnter), _displayDuration);
    }

    private void OnAnimationStageExit()
    {

    }

    private void OnEndingStageEnter()
    {
        StartCoroutine(DoTransition());

        if (_deactivateOnlyWhenTransitionObjectEnds)
        {
            _transitionElement.SetLinkedElement(this);
        }
        else
        {
            Deactivate();
        }

        if (_stopAudioFeedbackOnEnd)
        {
            _audioSource.Stop();
        }

        if (_playAudioClipAtEnd)
        {
            PlayAudioClip(_audioClipPlayedAtEnd);
        }
    }
    #endregion

    #region Private methods
    private void Restore()
    {
        if ((_isUiGraphic) && (_uiGraphic))
        {
            _uiGraphic.enabled = true;
            _uiGraphic.color = Color.white;
        }
        else if ((_isSprite) && (_spriteRenderer))
        {
            _spriteRenderer.enabled = true;
            _spriteRenderer.color = Color.white;
        }

        if ((_applySettingsToChildren) && (_childrenElements?.Length > 0))
        {
            foreach (SplashElement splash in _childrenElements)
            {
                if (splash.CanApplyParentEffects())
                    splash.Restore();
            }
        }

        if (_hasAnimation)
        {
            _remainingAnimationDurationTime = _animationClip.length;
        }

        if (_applyParentSettings)
        {
            CopyParentElementSettings();
        }
    }

    private void AssignComponents()
    {
        if (_isUiGraphic)
        {
            _uiGraphic = GetComponent<UnityEngine.UI.MaskableGraphic>();
        }
        else if (_isSprite)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (_applySettingsToChildren)
        {
            _childrenElements = GetComponentsInChildren<SplashElement>();
        }

        if (_hasAnimation)
        {
            _animator = GetComponent<Animator>();
        }

        if (_playAudioClipAtStart || _playAudioClipAtEnd)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.loop = false;
            _audioSource.playOnAwake = false;
        }
    }

    private void CopyParentElementSettings()
    {
        SplashElement parentElement = transform.parent?.GetComponent<SplashElement>();

        if (!parentElement)
        {
            _applyParentSettings = false;
            gameObject.SetActive(false);
            print($"Couldn't find a SplashElement as parent to {_label} splash element.");
            return;
        }

        _isUiGraphic =  parentElement._isUiGraphic;
        _isSprite = parentElement._isSprite;
        _displayDuration = parentElement._displayDuration;
        _activationType = parentElement._activationType;
        _fadeInDuration = parentElement._fadeInDuration;
        _deactivationType = parentElement._deactivationType;
        _fadeOutDuration = parentElement._fadeOutDuration;
    }

    private void TrackAnimationTime()
    {
        if (!_isTrackingAnimationDuration) return;

        if (_remainingAnimationDurationTime > 0)
        {
            _remainingAnimationDurationTime -= Time.deltaTime;
        }
        else
        {
            _isTrackingAnimationDuration = false;
            OnStartingStageExit();
            OnAnimationStageExit();
        }
    }

    private IEnumerator DoTransition()
    {
        if (_applyParentSettings) yield break;

        if (!_hasTransition) 
        {
            if (OnNoTransitionsLeft != null)
                OnNoTransitionsLeft();
                
            yield break;
        }

        if (_crossFadeWithTransition)
            yield return new WaitForSeconds(_fadeOutDuration * _blendingValue);

        _transitionElement.Play();
    }

    private void PlayAnimation()
    {
        if ((_isSprite) && (_animator))
        {
            _animator.Play(_animationClip.name);
            _isTrackingAnimationDuration = true;
        }
    }

    private void PlayAudioClip(AudioClip clip) 
    {
        if (!_applyParentSettings)
        {
            _audioSource.volume = MaxAudioVolume;
            _audioSource.PlayOneShot(clip);
        }
    }
    
    private void FadeIn()
    {
        if ((_fadeTween != null) && (_fadeTween.IsPlaying()))
            _fadeTween.Pause();

        if (_isUiGraphic)
        {
            _uiGraphic.color = _clearColor;
            _fadeTween = _uiGraphic.DOColor(_initialColor, _fadeInDuration).OnComplete(OnStartingStageExit);
        }
        else if (_isSprite)
        {
            _spriteRenderer.color = _clearColor;
            _fadeTween = _spriteRenderer.DOColor(_initialColor, _fadeInDuration).OnComplete(OnStartingStageExit);
        }
    }

    private void FadeOut()
    {
        if ((_fadeTween != null) && (_fadeTween.IsPlaying()))
            _fadeTween.Pause();

        if (_isUiGraphic)
        {
            _uiGraphic.color = _initialColor;
            _fadeTween = _uiGraphic.DOColor(_clearColor, _fadeOutDuration);
        }
        else if (_isSprite)
        {
            _spriteRenderer.color = _initialColor;
            _fadeTween = _spriteRenderer.DOColor(_clearColor, _fadeOutDuration);
        }
    }
    #endregion
}
