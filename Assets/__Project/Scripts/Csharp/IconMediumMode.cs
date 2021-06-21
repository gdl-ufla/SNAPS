using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using YoukaiFox.UnityExtensions;

public class IconMediumMode : MonoBehaviour
{
    #region Serialized fields
    #pragma warning disable 0649 

    [SerializeField] [BoxGroup("Icon sprites")]
    private Sprite _interrogationMark;

    [SerializeField] [BoxGroup("Icon sprites")]
    private Sprite _successMark;

    [SerializeField] [BoxGroup("Icon sprites")]
    private Sprite _errorMark;

    #pragma warning restore 0649 
    #endregion

    #region Non-serialized fields

    private DisplayState _currentState;
    private Image _displayImage;

    #endregion

    #region Custom structures

    public enum DisplayState
    {
        Interrogation, Success, Error
    }

    #endregion

    #region Unity events

    private void Awake() 
    {
        _displayImage = gameObject.GetComponentInSelfOrChildren<Image>();
    }
    
    #endregion

    #region Public methods

    public void SetState(DisplayState set)
    {
        _currentState = set;
        ChangeSprite();
    }

    #endregion

    #region Private methods

    private void ChangeSprite()
    {
        switch (_currentState)
        {
            case DisplayState.Interrogation:
                _displayImage.sprite = _interrogationMark;
                break;
            case DisplayState.Success:
                _displayImage.sprite = _successMark;
                break;
            case DisplayState.Error:
                _displayImage.sprite = _errorMark;
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }

    #endregion    
}
