using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SplashManager : MonoBehaviour
{
    #pragma warning disable 0649 
    [SerializeField] [Scene] [BoxGroup("Following scene configuration")] 
    private string _followingSceneName;

    [SerializeField] [ReorderableList] [BoxGroup("Splash elements")]
    private List<SplashElement> _splashElements;

    #pragma warning restore 0649 

    #region Unity events
    private void OnEnable() 
    {
        SplashElement.OnNoTransitionsLeft += PlayNextScene;
    }

    private void OnDisable() 
    {
        SplashElement.OnNoTransitionsLeft -= PlayNextScene;
    }

    private void Start() 
    {
        PlayFromStart();
    }
    #endregion

    #region Private methods
    private void PlayNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_followingSceneName);
    }

    private void PlayFromStart()
    {
        if (_splashElements.Count <= 0)
        {
            print($"Splash elements list is empty.");
            return;
        }

        _splashElements[0].Play();
    }
    #endregion
}
