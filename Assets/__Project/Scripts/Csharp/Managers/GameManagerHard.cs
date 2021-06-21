using System.Collections;
using TMPro;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using YoukaiFox.SystemExtensions;
using YoukaiFox.UnityExtensions;
using System;
using YoukaiFox.Tools.GooglePlay;

public class GameManagerHard : GameManager
{
    #region Serialized fields
    #pragma warning disable 0649 
    
    [SerializeField] [BoxGroup("Gameplay values")]
    private int _pointsOnCorrectAnswer = 50;

    [SerializeField] [BoxGroup("Gameplay values")]
    private int _pointsOnWrongAnswer = 0;

    [SerializeField] [BoxGroup("Image references")]
    private Image[] _heartIcons;

    [SerializeField] [BoxGroup("Button references")]
    private BlockHardMode[] _optionBlocks;

    [SerializeField] [BoxGroup("Window references")]
    private GameObject _nextQuestionWarning;

    [SerializeField] [BoxGroup("Text references")]
    private TextMeshProUGUI _questionTxt;

    [SerializeField] [BoxGroup("Text references")]
    private TextMeshProUGUI _questionIndexText;

    [SerializeField] [BoxGroup("Graphic references")]
    private Image _systemImageDisplay;

    [SerializeField] [BoxGroup("Particle references")]
    private ParticleSystem _heartParticles;

    [SerializeField] [BoxGroup("Audio references")]
    private AudioClip _successSound;

    [SerializeField] [BoxGroup("Audio references")]
    private AudioClip _errorSound;

    [SerializeField] [BoxGroup("Debug values")]
    private float _warningDuration = 1.25f;
    
    #pragma warning restore 0649 
    #endregion

    #region Non-serialized fields

    private int _currentLives;
    private bool _questionEnded = false;

    #endregion

    #region Constant fields

    private const int NumberOfQuestions = 8;
    private const int MaxLives = 3;
    private const string FullLifeAchievement = "CgkI8ajHtfUZEAIQDg";
    private const string GoodScoreAchievement = "CgkI8ajHtfUZEAIQBw";
    private const string PerfectScoreAchievement = "CgkI8ajHtfUZEAIQCg";

    #endregion

    #region Custom structures

    private struct AnswerData
    {
        public string Answer;
        public bool IsAnswerCorrect;

        internal void SetAnswer(string answer, bool v)
        {
            Answer = answer;
            IsAnswerCorrect = v;
        }
    }

    #endregion

    #region Unity events
    #endregion

    #region Public methods
    #endregion

    #region Protected methods

    #region Overridden methods

    protected override void Initialize()
    {
        base.Initialize();
        _nextQuestionWarning.gameObject.SetActive(false);
        _currentLives = _heartIcons.Length;
        LoadNextQuestion();
    }

    protected override void Enable()
    {
        base.Enable();
        BlockHardMode.OnPathEndReach += OnQuestionEnd;
        BlockHardMode.OnPlayerCollision += CheckAnswer;
    }

    protected override void Disable()
    {
        base.Enable();
        BlockHardMode.OnPathEndReach -= OnQuestionEnd;
        BlockHardMode.OnPlayerCollision -= CheckAnswer;
    }

    protected override int GetNumberOfQuestions()
    {
        return NumberOfQuestions;
    }

    protected override void LoadNextQuestion()
    {
        UpdateQuestionWarning();
        SetBlocksData();
        RestoreBlocks();
        UpdateQuestionText();
        UpdateHintText();
        UpdateSystemImage();
    }

    protected override void ShuffleQuestionBank()
    {
        Questions.Shuffle();
    }

    protected override void EndRound()
    {
        base.EndRound();
        _questionEnded = false;
    }

    protected override void EndGame()
    {
        base.EndGame();
        KillBlocks();
        Destroy(FindObjectOfType<MovableRectTransform>().gameObject);
    }

    #endregion

    #endregion

    #region Private methods

    private void OnQuestionEnd()
    {
        if (_questionEnded)
            return;

        _questionEnded = true;

        Invoke(nameof(EndRound), TimeIntervalBetweenRounds);
    }

    private void CheckAnswer(bool isAnswerCorrect)
    {
        DisableBlocksColliders();

        if (isAnswerCorrect)
        {
            SoundManager.Instance?.PlaySfx(_successSound);
            UpdateScore(_pointsOnCorrectAnswer);
        }
        else
        {
            SoundManager.Instance?.PlaySfx(_errorSound);
            UpdateScore(-_pointsOnWrongAnswer);
            LoseHeart();
        }

        Invoke(nameof(EndRound), 1f);
    }

    private void RestoreBlocks()
    {
        foreach (var block in _optionBlocks)
        {
            block.Restore();
        }
    }

    private void UpdateQuestionWarning()
    {
        _questionIndexText.text = (QuestionIndex + 1).ToString();
        _nextQuestionWarning.gameObject.SetActive(true);
        Invoke(nameof(DisableQuestionWarning), _warningDuration);
    }

    private void UpdateSystemImage()
    {
        _systemImageDisplay.sprite = GetSystemImage(CurrentQuestion.SystemCode);
    }

    private void DisableBlocksColliders()
    {
        foreach (var block in _optionBlocks)
        {
            block.DisableCollider();
        }
    }

    private void KillBlocks()
    {
        foreach (var block in _optionBlocks)
        {
            block.Kill();
        }
    }

    private void LoseHeart()
    {
        _currentLives--;
        Image heart = _heartIcons[_currentLives];
        heart.enabled = false;
        SpawnHeartParticles(heart.transform.position);

        if (_currentLives == 0)
            TriggerGameOver();
            
    }

    private void TriggerGameOver()
    {
        KillBlocks();
        Invoke(nameof(EndGame), TimeIntervalBetweenRounds + 1f);
    }

    private void SpawnHeartParticles(Vector2 position)
    {
        _heartParticles.transform.position = position;
        _heartParticles.PlayForced();
    }

    private void SetBlocksData()
    {
        AnswerData[] data = new AnswerData[_optionBlocks.Length];

        for (int i = 0; i < _optionBlocks.Length; i++)
        {
            if (i == 0)
                data[i].SetAnswer(CurrentQuestion.Answer, true);
            else
                data[i].SetAnswer(CurrentQuestion.options[i - 1], false);
        }

        data.Shuffle();

        for (int i = 0; i < _optionBlocks.Length; i++)
        {
            _optionBlocks[i].SetAnswer(data[i].Answer, data[i].IsAnswerCorrect);
        }
    }

    private void UpdateQuestionText()
    {
        if (CurrentQuestion.SystemType.Equals("Geral"))
        {
            _questionTxt.text = CurrentQuestion.QuestionTheme;
            return;
        }
        
        string article = GetSystemPtbrArticle(CurrentQuestion.SystemCode);
        string questionText = $"Qual o {CurrentQuestion.QuestionTheme.ToLower()} relacionado ";
        questionText += $"{article} {CurrentQuestion.System.ToLower()} no sistema ";
        questionText += $"nervoso autônomo {CurrentQuestion.SystemType.ToLower()}?";
        _questionTxt.text = questionText;
    }

    private void UpdateHintText()
    {
        SetHint(CurrentQuestion.Hint);
    }

    private void DisableQuestionWarning()
    {
        _nextQuestionWarning.gameObject.SetActive(false);
    }

    private void CheckAchievements()
    {
#if UNITY_ANDROID
        var maxPossiblePoints = NumberOfQuestions * _pointsOnCorrectAnswer;

        if (CurrentScore >= maxPossiblePoints * 0.6f)
            GooglePlayManager.Instance.Achieve(GoodScoreAchievement);

        if (CurrentScore == maxPossiblePoints)
            GooglePlayManager.Instance.Achieve(PerfectScoreAchievement);

        if (_currentLives == MaxLives)
            GooglePlayManager.Instance.Achieve(FullLifeAchievement);
#endif
    }

    #endregion
}
