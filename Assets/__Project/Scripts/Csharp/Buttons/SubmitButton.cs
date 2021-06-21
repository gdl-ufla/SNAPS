using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitButton : MonoBehaviour
{
    private Button _button;
    public static System.Action OnSubmit;

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
        if (OnSubmit != null)
            OnSubmit();
    }
}
