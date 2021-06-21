using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class DataController : MonoBehaviour
{
    #region Static fields
    
    public static DataController Instance;
    
    #endregion

    #region Serialized fields
    #pragma warning disable 0649 
    
    [SerializeField] 
    private GameObject _androidImmersiveModePrefab;

    [SerializeField] [Scene]
    private string _mainMenuSceneIndex;
    
    #pragma warning restore 0649 
    #endregion

    #region Private fields
    
    private PlayerProgress playerProgress;
    
    #endregion

    #region Custom structures

    private struct PlayerProgress 
    {
        public int HighScoreEasy;
        public int HighScoreMedium;
        public int HighScoreHard;
        public int IsFormSent;
        public int IsSoundMuted;
        public float MusicVolume;
        public float SfxVolume;
    }

    public enum Difficulty
    {
        Easy, Medium, Hard
    }

    #endregion

    #region Unity events
    
    private void Awake() 
    {
        SetUpSingleton();
        LoadPlayerProgress();
        CreateNecessaryComponents();
    }

    private void Start() 
    {
    //     InitializeGoogleServices();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        OpenMainMenu();
    }
    
    #endregion

    #region Public methods

    public void SavePlayerProgress()
    {
        PlayerPrefs.SetInt(nameof(playerProgress.HighScoreEasy), playerProgress.HighScoreEasy);
        PlayerPrefs.SetInt(nameof(playerProgress.HighScoreMedium), playerProgress.HighScoreMedium);
        PlayerPrefs.SetInt(nameof(playerProgress.HighScoreHard), playerProgress.HighScoreHard);
        PlayerPrefs.SetInt(nameof(playerProgress.IsFormSent), playerProgress.IsFormSent);
        PlayerPrefs.SetInt(nameof(playerProgress.IsSoundMuted), playerProgress.IsSoundMuted);
        PlayerPrefs.SetFloat(nameof(playerProgress.MusicVolume), playerProgress.MusicVolume);
        PlayerPrefs.SetFloat(nameof(playerProgress.SfxVolume), playerProgress.SfxVolume);
    }

    public int GetHighScore(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return playerProgress.HighScoreEasy;
            case Difficulty.Medium:
                return playerProgress.HighScoreMedium;
            case Difficulty.Hard:
                return playerProgress.HighScoreHard;
            default:
                print($"Difficulty {difficulty} not found");
                return -1;
        }
    }

    public void SetHighScore(Difficulty difficulty, int score)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                playerProgress.HighScoreEasy = score;
                break;
            case Difficulty.Medium:
                playerProgress.HighScoreMedium = score;
                break;
            case Difficulty.Hard:
                playerProgress.HighScoreHard = score;
                break;
            default:
                print($"Difficulty {difficulty} not found");
                break;
        }

        SavePlayerProgress();
    }

    public SoundManager.SoundSettings GetSoundSettings()
    {
        return new SoundManager.SoundSettings(playerProgress.IsSoundMuted == 1, 
                                              playerProgress.MusicVolume, 
                                              playerProgress.SfxVolume);
    }

    public void SetSoundSettings(SoundManager.SoundSettings soundSettings)
    {
        playerProgress.IsSoundMuted = soundSettings.IsMuted ? 1 : 0;
        playerProgress.MusicVolume = soundSettings.MusicVolume;
        playerProgress.SfxVolume = soundSettings.SfxVolume;
        SavePlayerProgress();
    }

    public void ResetData() 
    {
        PlayerPrefs.DeleteAll();
        playerProgress.HighScoreEasy = 0;
        playerProgress.HighScoreMedium = 0;
        playerProgress.HighScoreHard = 0;
        playerProgress.IsFormSent = 0;
        playerProgress.IsSoundMuted = 0;
        playerProgress.MusicVolume = 1f;
        playerProgress.SfxVolume = 1f;
        SavePlayerProgress();
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene(_mainMenuSceneIndex);
    }
    
    public void SendForm(bool hasSentForm)
    {
        if ((playerProgress.IsFormSent == 1) && (!hasSentForm))
            return;

        playerProgress.IsFormSent = hasSentForm ? 1 : 0;
        SavePlayerProgress();
    }
    
    #endregion

    #region Private methods

    private void LoadPlayerProgress()
    {
        playerProgress = new PlayerProgress();

        if (PlayerPrefs.HasKey(nameof(playerProgress.HighScoreEasy)))
            playerProgress.HighScoreEasy = PlayerPrefs.GetInt(nameof(playerProgress.HighScoreEasy));
        else
            playerProgress.HighScoreEasy = 0;

        if (PlayerPrefs.HasKey(nameof(playerProgress.HighScoreMedium)))
            playerProgress.HighScoreMedium = PlayerPrefs.GetInt(nameof(playerProgress.HighScoreMedium));
        else
            playerProgress.HighScoreMedium = 0;

        if (PlayerPrefs.HasKey(nameof(playerProgress.HighScoreHard)))
            playerProgress.HighScoreHard = PlayerPrefs.GetInt(nameof(playerProgress.HighScoreHard));
        else
            playerProgress.HighScoreHard = 0;

        if (PlayerPrefs.HasKey(nameof(playerProgress.IsFormSent)))
            playerProgress.IsFormSent = PlayerPrefs.GetInt(nameof(playerProgress.IsFormSent));
        else
            playerProgress.IsFormSent = 0;

        if (PlayerPrefs.HasKey(nameof(playerProgress.IsSoundMuted)))
            playerProgress.IsSoundMuted = PlayerPrefs.GetInt(nameof(playerProgress.IsSoundMuted));
        else
            playerProgress.IsSoundMuted = 0;

        if (PlayerPrefs.HasKey(nameof(playerProgress.MusicVolume)))
            playerProgress.MusicVolume = PlayerPrefs.GetFloat(nameof(playerProgress.MusicVolume));
        else
            playerProgress.MusicVolume = 1f;

        if (PlayerPrefs.HasKey(nameof(playerProgress.SfxVolume)))
            playerProgress.SfxVolume = PlayerPrefs.GetFloat(nameof(playerProgress.SfxVolume));
        else
            playerProgress.SfxVolume = 1f;
    }

    private void SetUpSingleton()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void CreateNecessaryComponents()
    {
        if (_androidImmersiveModePrefab)
        {
            Instantiate(_androidImmersiveModePrefab, null, false);
        }
    }

    #endregion
}
