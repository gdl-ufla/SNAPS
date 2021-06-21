using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public abstract class Slot : MonoBehaviour, IDropHandler
{
    #pragma warning disable 0649 
    [SerializeField] 
    private bool _isAnswerSlot;

    [SerializeField]
    private bool _isInteractable;
    #pragma warning restore 0649 

    private Transform _transform;
    private int _initialChildCount = 0;
    private Image _slotImage;
    private TextMeshProUGUI _slotText;
    public bool IsAnswerSlot => _isAnswerSlot;
    public static System.Action OnDropButton;
    public static System.Action OnDropButtonInAnswerSlot;

    #region Unity events

    private void Awake() 
    {
        _transform = transform;
        _slotImage = GetComponent<Image>();
        _slotText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start() 
    {
        Initialize();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_isInteractable)
            return;

        if (HasDraggableButton()) 
            return;

        if (!DraggableButton.DraggingButton)
        {
            print("Static reference to draggable button not found.");
            return;
        }
        
        ParentButton();
    }

    #endregion

    #region Public methods

    public virtual void TurnActive(bool active)
    {
        _slotImage.color = active ? Color.white : Color.black;
        SetInteractable(active);
        _slotText.enabled = active;
    }

    public void SetInteractable(bool set) { _isInteractable = set; }

    public bool HasDraggableButton() {return _transform.childCount > _initialChildCount;}

    public DraggableButton GetDraggableButton()
    {
        if (!HasDraggableButton())
        {
            print($"Slot {gameObject} doesn't have a draggable button as its child.");
            return null;
        }
            
        return GetComponentInChildren<DraggableButton>();
    }

    public abstract void OnSuccess();

    public abstract void OnFail();

    public abstract void DeactivateEffects();

    #endregion

    #region Protected methods

    protected void DropChildButton()
    {
        GetDraggableButton().transform.SetParent(null, false);
    }

    protected virtual void Initialize()
    {
        _isInteractable = true;
        DefineInitialChildCount();
    }

    protected virtual void OnParentButton() {}

    #endregion
    
    #region Private methods

    private void DefineInitialChildCount()
    {
        _initialChildCount = 0;
        DraggableButton[] childrenButtons = _transform.GetComponentsInChildren<DraggableButton>();
        _initialChildCount = _transform.childCount - childrenButtons.Length;
    }

    private void ParentButton()
    {
        DraggableButton.DraggingButton.OwnTransform.SetParent(_transform, false);
        DraggableButton.DraggingButton.OwnTransform.localPosition = Vector2.zero;

        OnParentButton();

        if ((OnDropButtonInAnswerSlot != null) && (_isAnswerSlot))
            OnDropButtonInAnswerSlot();

        if (OnDropButton != null)
            OnDropButton();
    }
    

    #endregion
}
