using UnityEngine;
using TMPro;
using NaughtyAttributes;
#if UNITY_ANDROID
using YoukaiFox.Tools.GooglePlay;
#endif

public class MainMenuManager : MonoBehaviour
{
    #region Serialized fields
    public TextMeshProUGUI SignInStatusText;

    #pragma warning disable 0649 

    [SerializeField] 
    private AudioClip _bgm;

    [SerializeField] [Scene]
    private string _permanentScene;

    [SerializeField] [BoxGroup("Text display references")]
    private TextMeshProUGUI _highScoreTxtEasy;
    
    [SerializeField] [BoxGroup("Text display references")]
    private TextMeshProUGUI _highScoreTxtMedium;

    [SerializeField] [BoxGroup("Text display references")]
    private TextMeshProUGUI _highScoreTxtHard;

    #pragma warning restore 0649 
    #endregion

    #region Non-serialized fields
    #endregion

    #region Unity events
    
    private void Start() 
    {
        PlayBgm();
        CheckPersistance();
        UpdateRecords();
    }

#if UNITY_ANDROID
    private void Update() 
    {
        SignInStatusText.text = $"Sign in Status: {GooglePlayManager.Instance.CurrentSignInStatus.ToString()}";
    }
#endif

    #endregion

    #region Public methods

    public void ResetData()
    {
        if (!DataController.Instance)
        {
            return;
        }

        DataController.Instance.ResetData();
    }

    #endregion

    #region Private methods
    private void PlayBgm()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlayBgm(_bgm);
        }
    }

    private void CheckPersistance()
    {
        if (!DataController.Instance)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(_permanentScene);
        }
    }

    private void UpdateRecords()
    {
        if (!DataController.Instance)
            return;
            
        _highScoreTxtEasy.text = DataController.Instance.GetHighScore(DataController.Difficulty.Easy).ToString();
        _highScoreTxtMedium.text = DataController.Instance.GetHighScore(DataController.Difficulty.Medium).ToString();
        _highScoreTxtHard.text = DataController.Instance.GetHighScore(DataController.Difficulty.Hard).ToString();
    }
    #endregion    
}
