using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintBuyButton : MonoBehaviour
{
    #pragma warning disable 0649 
    [SerializeField] private Answer.SystemType _systemType;
    #pragma warning restore 0649 

    private Button _button;
    public static System.Action<Answer.SystemType> OnSystemHintBought;
    public static System.Action OnSimpleHintBought;

    private void Awake() 
    {
        _button = GetComponent<Button>();
    }

    private void Start() 
    {
        _button.onClick.AddListener(ActionOnClick);
    }

    private void ActionOnClick()
    {
        if (OnSystemHintBought != null)
            OnSystemHintBought(_systemType);

        if (OnSimpleHintBought != null)
            OnSimpleHintBought();
    }
}
