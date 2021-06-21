using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class SceneLoadButton : MonoBehaviour
{
    #pragma warning disable 0649 
    [SerializeField][Scene]
    private string _targetScene;
    
    #pragma warning restore 0649 

    private Button _button;

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
        UnityEngine.SceneManagement.SceneManager.LoadScene(_targetScene);
    }
}
