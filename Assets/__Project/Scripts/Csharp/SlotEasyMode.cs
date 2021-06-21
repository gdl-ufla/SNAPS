using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SlotEasyMode : Slot
{
    #pragma warning disable 0649 
    [SerializeField][ShowIf(nameof(IsAnswerSlot))][BoxGroup("System type parameters")]
    private bool _useSystemTypeChecking;

    [SerializeField] [ShowIf(nameof(CanShowSystemType))] [BoxGroup("System type parameters")]
    private Answer.SystemType _systemType;

    [SerializeField] [ShowIf(nameof(IsAnswerSlot))] [BoxGroup("Visual effect references")]
    private AlphaPingPong _successEffect;

    [SerializeField] [ShowIf(nameof(IsAnswerSlot))] [BoxGroup("Visual effect references")]
    private AlphaPingPong _failEffect;

    #pragma warning restore 0649 

    public bool UseSystemTypeChecking => _useSystemTypeChecking;
    public Answer.SystemType SystemType => _systemType;

    public bool CanShowSystemType => IsAnswerSlot && _useSystemTypeChecking;

    #region Overriden methods

    public override void OnSuccess()
    {
        if (!IsAnswerSlot) return;
        
        _successEffect.gameObject.SetActive(true);
    }

    public override void OnFail()
    {
        if (!IsAnswerSlot) return;

        _failEffect.gameObject.SetActive(true);
    }

    public override void DeactivateEffects()
    {
        if (!IsAnswerSlot) return;

        _successEffect.gameObject.SetActive(false);
        _failEffect.gameObject.SetActive(false);
    }

    protected override void Initialize()
    {
        base.Initialize();

        if (IsAnswerSlot)
        {
            _successEffect.gameObject.SetActive(false);
            _failEffect.gameObject.SetActive(false);
        }
    }

    #endregion

    public DraggableButtonEasyMode GetDraggableButtonEasyMode()
    {
        if (!HasDraggableButton())
        {
            print($"Slot {gameObject} doesn't have a draggable button as its child.");
            return null;
        }
            
        return GetComponentInChildren<DraggableButtonEasyMode>();
    }
}
