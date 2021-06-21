using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class SinapseController : MonoBehaviour
{
    #region Static fields

    public static System.Action OnSinapseEnd;
    public static System.Action<int> OnQuestionCorrect;

    #endregion

    #region Serialized fields
    #pragma warning disable 0649 

    [SerializeField] [BoxGroup("UI references")]
    private IconMediumMode[] _icons;

    [SerializeField] [BoxGroup("UI references")]
    private SlotMediumMode[] _answerSlots;

    [SerializeField] [BoxGroup("Transform references")]
    private ParticleSystem _sinapseRepresentation;

    [SerializeField] [BoxGroup("Transform references")]
    private Transform _bridgeEntrancePos;

    [SerializeField] [BoxGroup("Transform references")]
    private Transform _firstSlotPos;

    [SerializeField] [BoxGroup("Transform references")]
    private Transform _secondSlotPos;

    [SerializeField] [BoxGroup("Transform references")]
    private Transform _thirdSlotPos;

    [SerializeField] [BoxGroup("Transform references")]
    private Transform _bridgeExitPos;

    [SerializeField] [BoxGroup("Sound references")]
    private AudioClip _sinapseSound;

    [SerializeField] [BoxGroup("Sound references")]
    private AudioClip _firstSuccessSound;

    [SerializeField] [BoxGroup("Sound references")]
    private AudioClip _secondSuccessSound;

    [SerializeField] [BoxGroup("Sound references")]
    private AudioClip _thirdSuccessSound;

    [SerializeField] [BoxGroup("Sound references")]
    private AudioClip _fourthSuccessSound;

    [SerializeField] [BoxGroup("Sound references")]
    private AudioClip _errorSound;

    [SerializeField] [BoxGroup("Debug")]
    private float _firstAnswerAnimationTime = 1.25f;

    [SerializeField] [BoxGroup("Debug")]
    private float _secondAnswerAnimationTime = 1f;

    [SerializeField] [BoxGroup("Debug")]
    private float _thirdAnswerAnimationTime = 0.75f;

    [SerializeField] [BoxGroup("Debug")]
    private float _exitBridgeAnimationTime = 0.5f;
    
    #pragma warning restore 0649 
    #endregion

    #region Non-serialized fields

    private Sequence _sequence;
    SoundManager.SoundInfo _soundInfo;
    private Transform _sinapseTransform;
    
    #endregion

    #region Unity events

    private void Start() 
    {
        _sinapseTransform = _sinapseRepresentation.transform;
        _soundInfo = new SoundManager.SoundInfo();
        _sinapseRepresentation.gameObject.SetActive(false);
        CreateSequence();
        ResetIcons();
    }

    private void OnEnable() 
    {
        GameManagerMedium.OnAnswerCheck += MoveSinapse;
        GameManager.OnRoundEnd += ResetIcons;
    }

    private void OnDisable() 
    {
        GameManagerMedium.OnAnswerCheck -= MoveSinapse;
        GameManager.OnRoundEnd -= ResetIcons;
    }

    private void OnDestroy() 
    {
        _sequence.Kill();
    }

    #endregion

    #region Public methods
    #endregion

    #region Private methods

    private void MoveSinapse()
    {
        if (SoundManager.Instance)
            _soundInfo = SoundManager.Instance.PlaySfx(_sinapseSound, true);

        _sinapseRepresentation.gameObject.SetActive(true);
        _sinapseRepresentation.Play();
        _sinapseTransform.position = _bridgeEntrancePos.position;
        _sequence.Restart();
        ActivateIcons();
    }

    private void StartNextRound()
    {
        _sinapseRepresentation.gameObject.SetActive(false);
        _sinapseRepresentation.Stop();

        if (SoundManager.Instance)
            SoundManager.Instance.StopSound(_soundInfo);

        if (OnSinapseEnd != null)
            OnSinapseEnd();
    }

    private void CreateSequence()
    {
        _sequence = DOTween.Sequence();
        _sequence.Pause();
        _sequence.Append(_sinapseTransform.DOMoveX(_firstSlotPos.position.x, _firstAnswerAnimationTime).SetEase(Ease.Linear));
        _sequence.AppendCallback(() => CheckSlot(0));
        _sequence.Append(_sinapseTransform.DOMoveX(_secondSlotPos.position.x, _secondAnswerAnimationTime).SetEase(Ease.Linear));
        _sequence.AppendCallback(() => CheckSlot(1));
        _sequence.Append(_sinapseTransform.DOMoveX(_thirdSlotPos.position.x, _thirdAnswerAnimationTime).SetEase(Ease.Linear));
        _sequence.AppendCallback(() => CheckSlot(2));
        _sequence.Append(_sinapseTransform.DOMoveX(_bridgeExitPos.position.x, _exitBridgeAnimationTime).SetEase(Ease.Linear));
        _sequence.AppendCallback(
            () => 
            {
                SoundManager.Instance.PlaySfx(_fourthSuccessSound);
                OnQuestionCorrect(3);
            });
        _sequence.AppendCallback(StartNextRound);
        _sequence.SetAutoKill(false);
    }

    private void CheckSlot(int slotIndex)
    {
        var button = _answerSlots[slotIndex].GetDraggableButton();

        if (button.IsAnswerCorrect)
        {
            if (OnQuestionCorrect != null)
                OnQuestionCorrect(slotIndex);

            _answerSlots[slotIndex].OnSuccess();
            _icons[slotIndex].SetState(IconMediumMode.DisplayState.Success);
            PlaySuccessSound(slotIndex);
        }
        else
        {
            SoundManager.Instance.PlaySfx(_errorSound);
            _answerSlots[slotIndex].OnFail();
            _icons[slotIndex].SetState(IconMediumMode.DisplayState.Error);
            _sequence.Pause();
            StartNextRound();
        }
    }

    private void ActivateIcons()
    {
        foreach (var icon in _icons)
        {
            icon.gameObject.SetActive(true);
        }
    }

    private void ResetIcons()
    {
        foreach (var icon in _icons)
        {
            icon.SetState(IconMediumMode.DisplayState.Interrogation);
            icon.gameObject.SetActive(false);
        }
    }

    private void PlaySuccessSound(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0:
                SoundManager.Instance.PlaySfx(_firstSuccessSound);
                break;
            case 1:
                SoundManager.Instance.PlaySfx(_secondSuccessSound);
                break;
            case 2:
                SoundManager.Instance.PlaySfx(_thirdSuccessSound);
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }

    #endregion    
}
