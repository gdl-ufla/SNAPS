using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class SlotMediumMode : Slot
{
    #region Serialized fields
    #pragma warning disable 0649 

    [SerializeField] [ShowIf(nameof(IsAnswerSlot))] [BoxGroup("Visual effect references")]
    private Image _successEffect;

    [SerializeField] [ShowIf(nameof(IsAnswerSlot))] [BoxGroup("Visual effect references")]
    private Image _failEffect;

    #pragma warning restore 0649 
    #endregion

    #region Non-serialized fields
    private DraggableButton _clonedButton;
    #endregion

    #region Unity events
    #endregion

    #region Public methods
    #endregion

    #region Protected methods

    public void DestroyClonedButton()
    {
        Destroy(_clonedButton?.gameObject);
    }
    
    public override void DeactivateEffects()
    {
        _successEffect.gameObject.SetActive(false);
        _failEffect.gameObject.SetActive(false);
    }

    public override void OnFail()
    {
        _failEffect.gameObject.SetActive(true);
    }

    public override void OnSuccess()
    {
        _successEffect.gameObject.SetActive(true);
    }

    protected override void OnParentButton()
    {
        if (!IsAnswerSlot)
            return;

        DraggableButton childButton = GetDraggableButton();
        DropChildButton();
        _clonedButton = Instantiate(childButton, transform, false);
        _clonedButton.OwnTransform.localPosition = Vector2.zero;
        _clonedButton.Restore(childButton.Answer, childButton.IsAnswerCorrect);
        _clonedButton.DisableButton();
    }

    #endregion

    #region Private methods
    #endregion
}
