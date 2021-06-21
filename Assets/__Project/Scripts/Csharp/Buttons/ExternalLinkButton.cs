using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExternalLinkButton : MonoBehaviour
{
    #pragma warning disable 0649 
    [SerializeField]
    private string url;
    #pragma warning restore 0649 
    private Button _button;

    private void Awake() 
    {
        _button = GetComponent<Button>();

        if (!_button)
            _button = gameObject.AddComponent<Button>();
    }

    private void Start() 
    {
        _button.onClick.AddListener(OpenUrl);
    }

    private void OpenUrl()
    {
        Application.OpenURL(url);
    }
}
