using YoukaiFox.SystemExtensions;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using UnityEngine.UI;
using YoukaiFox.Tools.GooglePlay;

public class GameManagerMedium : GameManager
{
    #region Actions

    public static System.Action OnAnswerCheck;

    #endregion

    #region Serialized fields
    #pragma warning disable 0649 

    [SerializeField] [BoxGroup("Gameplay values")]
    private int _initialHintsQnty = 2;

    [SerializeField] [BoxGroup("Gameplay values")]
    private int _pointsFirstQuestion = 15;

    [SerializeField] [BoxGroup("Gameplay values")]
    private int _pointsSecondQuestion = 35;

    [SerializeField] [BoxGroup("Gameplay values")]
    private int _pointsThirdQuestion = 50;

    [SerializeField] [BoxGroup("Gameplay values")]
    private int _pointsAllQuestions = 50;

    [SerializeField] [BoxGroup("Text references")]
    private TextMeshProUGUI _hintsQntyTxt;

    [SerializeField] [BoxGroup("Text references")]
    private TextMeshProUGUI _questionTxt;

    [SerializeField] [BoxGroup("Image references")]
    private Image _systemImageDisplay;

    [SerializeField] [BoxGroup("Window references")]
    private GameObject _noHintsWindow;

    [SerializeField] [BoxGroup("Window references")]
    private GameObject _hintWindow;

    [SerializeField] [BoxGroup("Button references")]
    private DraggableButtonMediumMode _draggableButtonPrefab;

    [SerializeField] [BoxGroup("Button references")]
    private GameObject _submitButton;

    [SerializeField] [BoxGroup("Button references")]
    private SlotMediumMode[] _initialSlots;

    [SerializeField] [BoxGroup("Button references")]
    private SlotMediumMode[] _answerSlots;

    #pragma warning disable 0649 
    #endregion

    #region Non-serialized fields

    private int _hintsQnty;
    private DraggableButtonMediumMode[] _draggableButtons;
    private bool _isSubmitingAnswer = false;
    private bool _usedHints = false;

    [ReadOnly]
    public Question[] _currentQuestionsBridge;

    #endregion

    #region Constant fields

    private const int NumberOfButtons = 4;
    private const int NumberOfQuestionsPerRound = 3;
    private const int NumberOfRounds = 5;
    private const string NoHintAchievement = "CgkI8ajHtfUZEAIQDQ";
    private const string GoodScoreAchievement = "CgkI8ajHtfUZEAIQCA";
    private const string PerfectScoreAchievement = "CgkI8ajHtfUZEAIQCw";

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
        _currentQuestionsBridge = new Question[NumberOfQuestionsPerRound];
        _submitButton.SetActive(false);
        _noHintsWindow.SetActive(false);
        _hintsQnty = _initialHintsQnty;
        _hintsQntyTxt.text = _hintsQnty.ToString();
        CreateDraggableButtons();
        LoadNextQuestion();
    }

    protected override void Enable()
    {
        base.Enable();
        Slot.OnDropButtonInAnswerSlot += OnButtonDrop;
        HintBuyButton.OnSimpleHintBought += BuyHint;
        SubmitButton.OnSubmit += SubmitAnswers;
        SinapseController.OnSinapseEnd += StartNextRound;
        SinapseController.OnQuestionCorrect += CalculateScore;
    }

    protected override void Disable()
    {
        base.Disable();
        Slot.OnDropButtonInAnswerSlot -= OnButtonDrop;
        HintBuyButton.OnSimpleHintBought -= BuyHint;
        SubmitButton.OnSubmit -= SubmitAnswers;
        SinapseController.OnSinapseEnd -= StartNextRound;
        SinapseController.OnQuestionCorrect -= CalculateScore;
    }

    protected override void OnGameOver()
    {
        CheckAchievements();
    }

    protected override void ShuffleQuestionBank()
    {
        Questions.Shuffle();
    }

    protected override void LoadNextQuestion()
    {
        StoreCurrentQuestion();
        UpdateSystemImage();
        Restore();
        ResetHintText();
        UpdateQuestionText();
        SetupSlots();
    }

    protected override int GetNumberOfQuestions()
    {
        return NumberOfQuestionsPerRound * NumberOfRounds;
    }

    #endregion

    #endregion

    #region Private methods

    private void Restore()
    {
        RestoreButtons();
        ShuffleAnswerOptions();
        ParentButtons();
        _isSubmitingAnswer = false;
    }

    private void RestoreButtons()
    {
        _draggableButtons[0].Restore(CurrentQuestion.Answer, true);

        for (int i = 1; i < _draggableButtons.Length; i++)
        {
            _draggableButtons[i].Restore(CurrentQuestion.options[i - 1], false);
        }
    }

    private void RestoreSlots()
    {
        foreach (var slot in _answerSlots)
        {
            slot.DestroyClonedButton();
            slot.DeactivateEffects();
        }
    }

    private void ParentButtons()
    {
        for (int i = 0; i < NumberOfButtons; i++)
        {
            _draggableButtons[i].transform.SetParent(_initialSlots[i].transform, false);

            if (!_draggableButtons[i].gameObject.activeInHierarchy)
                _draggableButtons[i].gameObject.SetActive(true);
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

    private void CreateDraggableButtons()
    {
        _draggableButtons = new DraggableButtonMediumMode[NumberOfButtons];

        for (int i = 0; i < NumberOfButtons; i++)
        {
            _draggableButtons[i] = Instantiate(_draggableButtonPrefab, _initialSlots[i].transform, false);
        }
    }

    private void BuyHint()
    {
        if (_isSubmitingAnswer)
            return;

        if (_hintsQnty <= 0)
        {
            _noHintsWindow.gameObject.SetActive(true);
            return;
        }

        SetHint(CurrentQuestion.Hint);
        _hintsQnty--;
        _hintsQntyTxt.text = _hintsQnty.ToString();
        _hintWindow.gameObject.SetActive(true);
        _usedHints = true;
    }

    private void SubmitAnswers()
    {
        _submitButton.gameObject.SetActive(false);
        _isSubmitingAnswer = true;
        DeactivateDraggableButtons();
        CheckCorrectAnswers();
    }

    private void CheckCorrectAnswers()
    {
        if (OnAnswerCheck != null)
            OnAnswerCheck();
    }

    private void CalculateScore(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0:
                UpdateScore(_pointsFirstQuestion);
                break;
            case 1:
                UpdateScore(_pointsSecondQuestion);
                break;
            case 2:
                UpdateScore(_pointsThirdQuestion);
                break;
            case 3:
                UpdateScore(_pointsAllQuestions);
                AddHint();
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }

    private void AddHint()
    {
        _hintsQnty++;
        _hintsQntyTxt.text = _hintsQnty.ToString();
    }

    private void DeactivateDraggableButtons()
    {
        foreach (DraggableButton draggable in _draggableButtons)
        {
            draggable.SetInteractable(false);
        }
    }

    private void OnButtonDrop()
    {
        if (GetCurrentRoundQuestionIndex() == NumberOfQuestionsPerRound - 1)
            _submitButton.gameObject.SetActive(true);
        else
            EndRound();
    }

    private void StoreCurrentQuestion()
    {
        int index = GetCurrentRoundQuestionIndex();
        _currentQuestionsBridge[index] = CurrentQuestion;
    }

    private void UpdateSystemImage()
    {
        _systemImageDisplay.sprite = GetSystemImage(CurrentQuestion.SystemCode);
    }

    private void StartNextRound()
    {
        Invoke(nameof(EndRound), TimeIntervalBetweenRounds);
        Invoke(nameof(RestoreSlots), TimeIntervalBetweenRounds);
    }

    private int GetCurrentRoundQuestionIndex()
    {
        return QuestionIndex % NumberOfQuestionsPerRound;
    }

    private void SetupSlots()
    {
        int targetSlotIndex = GetCurrentRoundQuestionIndex();

        for (int i = 0; i < _answerSlots.Length; i++)
        {
            _answerSlots[i].TurnActive(i == targetSlotIndex);
        }
    }

    private void ShuffleAnswerOptions()
    {
        _draggableButtons.Shuffle();
    }

    private void CheckAchievements()
    {
#if UNITY_ANDROID
        var maxPointsPerRound = _pointsFirstQuestion + _pointsSecondQuestion + _pointsThirdQuestion + _pointsThirdQuestion;
        var maxPossiblePoints = NumberOfQuestionsPerRound * NumberOfQuestionsPerRound * maxPointsPerRound;

        if (CurrentScore >= maxPossiblePoints * 0.6f)
            GooglePlayManager.Instance.Achieve(GoodScoreAchievement);

        if (CurrentScore == maxPossiblePoints)
            GooglePlayManager.Instance.Achieve(PerfectScoreAchievement);

        if (!_usedHints)
            GooglePlayManager.Instance.Achieve(NoHintAchievement);
#endif
    }

    #endregion
}
