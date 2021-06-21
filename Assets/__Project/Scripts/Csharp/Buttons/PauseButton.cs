using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    private Button _button;
    public static System.Action OnPauseButtonPress;

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
        if (OnPauseButtonPress != null)
            OnPauseButtonPress();
    }
}
