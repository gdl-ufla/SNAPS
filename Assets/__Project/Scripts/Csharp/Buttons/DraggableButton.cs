using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class DraggableButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static DraggableButton DraggingButton;

    #pragma warning disable 0649 
    [SerializeField] [NaughtyAttributes.Tag]
    private string slotTag;
    #pragma warning restore 0649 

    [ShowNonSerializedField]
    private bool _isAnswerCorrect;
    private TextMeshProUGUI _displayText;
    private bool _isInteractable;
    private Image _answerButtonImage;
    private Transform _transform;
    private CanvasGroup _canvasGroup;
    private Transform _startingParent;
    private RectTransform _rectTransform;
    public string Answer => _displayText.text;
    public bool IsAnswerCorrect => _isAnswerCorrect;
    public Transform OwnTransform => _transform;
    public static System.Action<SoundManager.SoundEventType> OnDragStart;

    private void Awake() 
    {
        _transform = transform;
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _answerButtonImage = GetComponent<Image>();
        _displayText = GetComponent<TextMeshProUGUI>();

        if (!_displayText)
            _displayText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnBeginDrag(PointerEventData eventData) 
    {
        if (!_isInteractable) return;

        _startingParent = _transform.parent;        
        DraggingButton = this;
        _canvasGroup.blocksRaycasts = false;
        _transform.SetParent(_transform.parent.parent.parent);
        _transform.SetAsLastSibling();

        if (OnDragStart != null)
            OnDragStart(SoundManager.SoundEventType.Drag);
    }

    public void OnDrag(PointerEventData eventData) 
    {
        if (!_isInteractable) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition, Camera.main, out Vector2 localPosition);
        _transform.position = _transform.TransformPoint(localPosition);
        _answerButtonImage.color = Color.gray;
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        if ((_transform.parent == null) || (!_transform.parent.gameObject.CompareTag(slotTag)))
            ResetPosition();

        _answerButtonImage.color = Color.white;
        DraggingButton = null;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Restore(string answer, bool isAnswerCorrect, bool isInteractable = true)
    {
        _displayText.text = answer;
        _isAnswerCorrect = isAnswerCorrect;
        SetInteractable(isInteractable);
        _answerButtonImage.color = Color.white;
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    public void SetInteractable(bool set) { _isInteractable = set; }

    public void DisableButton()
    {
        SetInteractable(false);
        _answerButtonImage.color = Color.gray;
    }

    private void ResetPosition()
    {
        _transform.SetParent(_startingParent, false);
        _transform.localPosition = Vector2.zero;
    }
}
