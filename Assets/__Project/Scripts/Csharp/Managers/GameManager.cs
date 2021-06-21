using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using NaughtyAttributes;
using YoukaiFox.Tools.GooglePlay;

public abstract class GameManager : MonoBehaviour 
{
    #region Actions

    public static System.Action OnRoundEnd;

    #endregion

    #region Serialized fields
    #pragma warning disable 0649 

    [SerializeField] [BoxGroup("Sound settings")]
    private AudioClip _bgm;

    [SerializeField] [BoxGroup("Gameplay values")]
    private DataController.Difficulty _difficulty;
    
    [SerializeField] [BoxGroup("Gameplay values")]
    private float _timeIntervalBetweenRounds;

    [SerializeField] [BoxGroup("References")]
    private SystemDataBase _systemDataBase;

    [SerializeField] [BoxGroup("References")]
    private TextAsset _jsonQuestionBank;

    [SerializeField] [BoxGroup("Window references")]
    private GameObject _pauseWindow;

    [SerializeField] [BoxGroup("Window references")]
    private GameObject _newRecordWindow;

    [SerializeField] [BoxGroup("Window references")]
    private GameObject _gameOverWindow;

    [SerializeField] [BoxGroup("Text references")]
    private TextMeshProUGUI _currentScoreText;

    [SerializeField] [BoxGroup("Text references")]
    private TextMeshProUGUI _finalScoreText;

    [SerializeField] [BoxGroup("Text references")]
    private TextMeshProUGUI _hintText;

    #pragma warning restore 0649 
    #endregion

    #region Non-serialized fields

    private QuestionBank _questions;
    private bool _isPaused;
    private int _currentScore;
    private int _questionIndex;
    private SystemData[] _systemData;

    #endregion

    #region Constant fields

    private const string StartingAchievement = "CgkI8ajHtfUZEAIQAA";
    private const string LeaderboardId = "CgkI8ajHtfUZEAIQDw";

    #endregion

    #region Properties

    public bool IsPaused => _isPaused;
    public int QuestionIndex { get => _questionIndex; protected set => _questionIndex = value; }
    public Question[] Questions => _questions.questions;
    public Question CurrentQuestion => _questions.questions[_questionIndex];
    public TextAsset JsonQuestionBank => _jsonQuestionBank;
    public float TimeIntervalBetweenRounds => _timeIntervalBetweenRounds;
    public int CurrentScore => _currentScore;

    #endregion

    #region Public methods

    #endregion

    #region Protected methods

    protected void SetHint(string hint)
    {
        _hintText.text = hint;
    }

    protected void ResetHintText()
    {
        _hintText.text = "";
    }

    protected SystemData GetSystemData(string systemCode)
    {
        foreach (SystemData data in _systemData)
        {
            if (data.systemCode.Equals(systemCode))
            {
                return data;
            }
        }

        print($"Erro ao procurar código {systemCode}");
        return null;
    }

    protected Sprite GetSystemImage(string systemCode)
    {
        foreach (SystemData data in _systemData)
        {
            if (data.systemCode.Equals(systemCode))
                return data.DisplayImage;
        }

        print($"Erro ao procurar código {systemCode}");
        return null;
    }

    protected string GetSystemPtbrArticle(string systemCode)
    {
        foreach (SystemData data in _systemData)
        {
            if (data.systemCode.Equals(systemCode))
                return data.Article;
        }

        print($"Erro ao procurar código {systemCode}");
        return "ao";
    }

    #endregion

    #region Unity events

    private void Start() 
    {
        Initialize();
        PlayBgm();
    }

    private void OnEnable() 
    {
        Enable();
    }

    private void OnDisable() 
    {
        Disable();
    }

    #endregion

    #region Abstract methods

    protected abstract void ShuffleQuestionBank();

    protected abstract void LoadNextQuestion();

    protected abstract int GetNumberOfQuestions();

    #endregion

    #region Virtual methods

    protected virtual void Initialize()
    {
        ReadJson();
        ShuffleQuestionBank();
        _isPaused = false;
        _pauseWindow.SetActive(false);
        _gameOverWindow.SetActive(false);
        _newRecordWindow.SetActive(false);
        _currentScoreText.text = "0";
        _questionIndex = 0;
        _systemData = _systemDataBase.GetData();
        CheckAchievements();
    }

    protected virtual void Enable()
    {
        PauseButton.OnPauseButtonPress += PauseGame;
    }

    protected virtual void Disable()
    {
        PauseButton.OnPauseButtonPress -= PauseGame;
    }

    protected virtual void UpdateScore(int score)
    {
        _currentScore += score;

        if (_currentScore < 0)
            _currentScore = 0;

        _currentScoreText.text = _currentScore.ToString();
    }

    protected virtual void EndRound()
    {
        _questionIndex++;
        
        if (_questionIndex >= GetNumberOfQuestions())
            EndGame();
        else
            PlayNextRound();

        if (OnRoundEnd != null)
            OnRoundEnd();
    }

    protected virtual void PlayNextRound()
    {
        LoadNextQuestion();
    }

    protected virtual void EndGame()
    {
        CheckHighScore();
        _finalScoreText.text = _currentScore.ToString();
        _gameOverWindow.SetActive(true);
        _isPaused = true;
        OnGameOver();
    }

    protected virtual void PauseGame()
    {
        _isPaused = !_isPaused;
        _pauseWindow.SetActive(_isPaused);
    }

    protected virtual void CheckHighScore()
    {
        if (!DataController.Instance)
        {
            print("Data controller not found, high score not updated.");
            return;
        }

        int highScore = DataController.Instance.GetHighScore(_difficulty);

        if (_currentScore > highScore)
        {
            print($"Current score: {_currentScore}");
            print($"High score: {highScore}");
            UpdateHighScore();
        }

        UpdateLeaderboard();
    }

    protected virtual void UpdateHighScore()
    {
        DataController.Instance.SetHighScore(_difficulty, _currentScore);
        _newRecordWindow.SetActive(true);
    }

    #endregion

    #region Private methods

    private void ReadJson()
    {
        _questions = JsonConvert.DeserializeObject<QuestionBank>(_jsonQuestionBank.text);
    }

    private void PlayBgm()
    {
        if (SoundManager.Instance)
            SoundManager.Instance.PlayBgm(_bgm);
    }

    protected virtual void OnGameOver()
    {
    }

    private void CheckAchievements()
    {
#if UNITY_ANDROID
        GooglePlayManager.Instance.Achieve(StartingAchievement);
#endif
    }

    private void UpdateLeaderboard()
    {
#if UNITY_ANDROID
        var easyScore = DataController.Instance.GetHighScore(DataController.Difficulty.Easy);
        var mediumScore = DataController.Instance.GetHighScore(DataController.Difficulty.Medium);
        var hardScore = DataController.Instance.GetHighScore(DataController.Difficulty.Hard);
        GooglePlayManager.Instance.SendScoreToLeaderboard(LeaderboardId, easyScore + mediumScore + hardScore);
#endif
    }

    #endregion
}