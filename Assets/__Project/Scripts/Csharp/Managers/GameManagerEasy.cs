using UnityEngine;
using TMPro;
using UnityEngine.UI;
using YoukaiFox.SystemExtensions;
using NaughtyAttributes;
using YoukaiFox.Tools.GooglePlay;

public class GameManagerEasy : GameManager
{
    #region Serialized fields
    #pragma warning disable 0649 

    [SerializeField, BoxGroup("Gameplay values")]
    private int _hintCost;

    [SerializeField] [BoxGroup("Gameplay values")]
    private int _initialCoins;

    [SerializeField] [BoxGroup("Gameplay values")]
    private int _coinsReceivedOnCorrectAnswer;

    [SerializeField] [BoxGroup("Gameplay values")]
    private int _pointsReceivedOnCorrectAnswer;

    [SerializeField] [BoxGroup("Gameplay values")]
    private float _timeLimit;

    [SerializeField] [BoxGroup("Text references")]
    private TextMeshProUGUI _timerText;

    [SerializeField] [BoxGroup("Text references")]
    private TextMeshProUGUI _currentCoinsText;

    [SerializeField] [BoxGroup("Text references")]
    private TextMeshProUGUI _systemTypeText;

    [SerializeField] [BoxGroup("Text references")]
    private TextMeshProUGUI _systemNameText;

    [SerializeField] [BoxGroup("Window references")]
    private GameObject _noCoinsWindow;

    [SerializeField] [BoxGroup("Button references")]
    private SlotEasyMode[] _initialSlots;

    [SerializeField] [BoxGroup("Button references")]
    private SlotEasyMode[] _answerSlots;

    [SerializeField] [BoxGroup("Button references")]
    private DraggableButtonEasyMode _draggableButtonPrefab;

    [SerializeField] [BoxGroup("Button references")]
    private GameObject _submitButton;

    [SerializeField] [BoxGroup("Graphic references")]
    private Image _systemImageDisplay;

    #pragma warning restore 0649 
    #endregion

    #region Non-serialized fields

    private float _currentTime;
    private int _currentCoins;
    private bool _isTimerActive;
    private bool _usedCoins = false;

    [SerializeField]
    [ReadOnly]
    private CompoundQuestion[] _compoundQuestions;
    private DraggableButtonEasyMode[] _draggableButtons;

    [ShowNonSerializedField]
    private bool _isSubmitingAnswer = false;

    #endregion

    #region Constant fields

    private const int NumberOfButtons = 6;
    private const int NumberOfQuestions = 6;
    private const string NoHintAchievement = "CgkI8ajHtfUZEAIQDA";
    private const string GoodScoreAchievement = "CgkI8ajHtfUZEAIQBg";
    private const string PerfectScoreAchievement = "CgkI8ajHtfUZEAIQCQ";
    
    #endregion

    #region Unity events

    private void Update() 
    {
        UpdateTimer();
    }
    
    #endregion

    #region Public methods
    #endregion

    #region Protected methods

    #region Overridden methods
    
    protected override void Initialize()
    {
        base.Initialize();
        _submitButton.SetActive(false);
        _noCoinsWindow.SetActive(false);
        _isTimerActive = true;
        _currentTime = _timeLimit;
        _timerText.text = _currentTime.ToString();
        _currentCoins = _initialCoins;
        _currentCoinsText.text = _currentCoins.ToString();
        CreateDraggableButtons();
        LoadNextQuestion();
    }
    
    protected override void ShuffleQuestionBank()
    {
        FormatData();
        _compoundQuestions.Shuffle();
    }

    protected override void LoadNextQuestion()
    {
        Restore();
        ResetHintText();
        string systemName = _compoundQuestions[QuestionIndex].NeuroQuestion.System;
        string systemType = _compoundQuestions[QuestionIndex].NeuroQuestion.SystemType;
        UpdateQuestion(systemName, systemType);
        UpdateSystemImage();
    }
    protected override int GetNumberOfQuestions()
    {
        return NumberOfQuestions;
    }

    protected override void Enable()
    {
        base.Enable();
        Slot.OnDropButton += CheckIfAnswersAreAssigned;
        HintBuyButton.OnSystemHintBought += BuyHint;
        SubmitButton.OnSubmit += SubmitAnswers;
    }

    protected override void Disable()
    {
        base.Disable();
        Slot.OnDropButton -= CheckIfAnswersAreAssigned;
        HintBuyButton.OnSystemHintBought -= BuyHint;
        SubmitButton.OnSubmit -= SubmitAnswers;
    }

    protected override void OnGameOver()
    {
        CheckAchievements();
    }

    #endregion

    #endregion

    #region Private methods

    private void UpdateQuestion(string systemName, string systemType)
    {
        _systemNameText.text = systemName;
        _systemTypeText.text = systemType;
    }
    
    private void CheckIfAnswersAreAssigned()
    {
        if (_answerSlots.Length <= 0) return;

        foreach (Slot slot in _answerSlots)
        {
            if (!slot.HasDraggableButton())
            {
                if (_submitButton.activeInHierarchy)
                    _submitButton.SetActive(false);

                return;
            }
        }
        
        if (!_submitButton.activeInHierarchy)
            _submitButton.SetActive(true);
    }

    private void UpdateTimer()
    {
        if (IsPaused) return;
        if (!_isTimerActive) return;

        if (_currentTime > 0)
        {
            _currentTime -= Time.deltaTime;
            _timerText.text = Mathf.RoundToInt(_currentTime).ToString();
        }
        else
        {
            _isTimerActive = true;
            _timerText.text = "0";
            EndGame();
        }
    }

    private void BuyHint(Answer.SystemType systemType)
    {
        if (_isSubmitingAnswer)
            return;

        if (_currentCoins < _hintCost)
        {
            _noCoinsWindow.SetActive(true);
            return;
        }

        SetHint(_compoundQuestions[QuestionIndex].GetQuestion(systemType).Hint);
        _currentCoins -= _hintCost;
        _currentCoinsText.text = _currentCoins.ToString();
        _usedCoins = true;
    }

    private void UpdateCoins(int coins)
    {
        _currentCoins += coins;
        _currentCoinsText.text = _currentCoins.ToString();
    }

    private void SubmitAnswers()
    {
        _submitButton.gameObject.SetActive(false);
        _isSubmitingAnswer = true;
        DeactivateDraggableButtons();
        CheckCorrectAnswers();
    }

    private void DeactivateDraggableButtons()
    {
        foreach (DraggableButton draggable in _draggableButtons)
        {
            draggable.SetInteractable(false);
        }
    }
    
    private void CheckCorrectAnswers()
    {
        for (int i = 0; i < _answerSlots.Length; i++)
        {
            if ((!_answerSlots[i].IsAnswerSlot) || (!_answerSlots[i].UseSystemTypeChecking))
            {
                Debug.LogError($"Slot {_answerSlots[i].gameObject} is not properly configured for this game mode.");
                return;
            }

            DraggableButtonEasyMode answer = _answerSlots[i].GetDraggableButtonEasyMode();

            if (!answer.UseSystemTypeChecking)
            {
                Debug.LogError($"Draggable button {answer.gameObject} is not properly configured for this game mode.");
                return;
            }
            
            if ((answer.IsAnswerCorrect) && (answer.SystemType == _answerSlots[i].SystemType))
            {
                _answerSlots[i].OnSuccess();
                UpdateScore(_pointsReceivedOnCorrectAnswer);
                UpdateCoins(_coinsReceivedOnCorrectAnswer);
            }
            else
            {
                _answerSlots[i].OnFail();
            }
        }

        Invoke(nameof(EndRound), TimeIntervalBetweenRounds);
        Invoke(nameof(DeactivateSubmiting), TimeIntervalBetweenRounds);
    }

    private void Restore()
    {
        RestoreButtons();
        RestoreSlots();
    }

    private void RestoreSlots()
    {
        foreach (SlotEasyMode slot in _answerSlots)
        {
            slot.DeactivateEffects();
        }
    }

    private void RestoreButtons()
    {
        _draggableButtons[0].Restore(_compoundQuestions[QuestionIndex].NeuroQuestion.Answer, true, Answer.SystemType.Neurotransmissor);
        _draggableButtons[1].Restore(_compoundQuestions[QuestionIndex].NeuroQuestion.options[0], false, Answer.SystemType.Neurotransmissor);
        _draggableButtons[2].Restore(_compoundQuestions[QuestionIndex].ReceptorQuestion.Answer, true, Answer.SystemType.Receptor);
        _draggableButtons[3].Restore(_compoundQuestions[QuestionIndex].ReceptorQuestion.options[0], false, Answer.SystemType.Receptor);
        _draggableButtons[4].Restore(_compoundQuestions[QuestionIndex].EffectQuestion.Answer, true, Answer.SystemType.Efeito);
        _draggableButtons[5].Restore(_compoundQuestions[QuestionIndex].EffectQuestion.options[0], false, Answer.SystemType.Efeito);

        ShuffleAnswerOptions();

        for (int i = 0; i < NumberOfButtons; i++)
        {
            _draggableButtons[i].transform.SetParent(_initialSlots[i].transform, false);
            
            if (!_draggableButtons[i].gameObject.activeInHierarchy)
                _draggableButtons[i].gameObject.SetActive(true);
        }
    }

    private void CreateDraggableButtons()
    {
        _draggableButtons = new DraggableButtonEasyMode[NumberOfButtons];

        for (int i = 0; i < NumberOfButtons; i++)
        {
            _draggableButtons[i] = Instantiate(_draggableButtonPrefab, _initialSlots[i].transform, false);
        }
    }

    private void FormatData()
    {
        if (Questions.Length % 3 != 0)
        {
            Debug.LogError($"The json file {JsonQuestionBank.name} is not properly formatted.");
            return;
        }

        _compoundQuestions = new CompoundQuestion[Questions.Length / 3];

        for (int i = 0; i < _compoundQuestions.Length; i++)
        {
            _compoundQuestions[i] = new CompoundQuestion();
        }

        for (int i = 0; i < Questions.Length; i++)
        {
            switch (i % 3)
            {
                case 0:
                    _compoundQuestions[i / 3].NeuroQuestion = Questions[i];
                    break;
                case 1:
                    _compoundQuestions[i / 3].ReceptorQuestion = Questions[i];
                    break;
                case 2:
                    _compoundQuestions[i / 3].EffectQuestion = Questions[i];
                    break;
                default:
                    break;
            }
        }
    }

    private void UpdateSystemImage()
    {
        string systemCode = _compoundQuestions[QuestionIndex].NeuroQuestion.SystemCode;
        _systemImageDisplay.sprite = GetSystemImage(systemCode);
    }

    private void DeactivateSubmiting()
    {
        _isSubmitingAnswer = false;
    }

    private void ShuffleAnswerOptions()
    {
        _draggableButtons.Shuffle();
    }

    private void CheckAchievements()
    {
#if UNITY_ANDROID
        var maxPossiblePoints = NumberOfQuestions * _answerSlots.Length * _pointsReceivedOnCorrectAnswer;

        if (CurrentScore >= maxPossiblePoints * 0.6f)
            GooglePlayManager.Instance.Achieve(GoodScoreAchievement);

        if (CurrentScore == maxPossiblePoints)
            GooglePlayManager.Instance.Achieve(PerfectScoreAchievement);

        if (!_usedCoins)
            GooglePlayManager.Instance.Achieve(NoHintAchievement);
#endif
    }

    #endregion
}
