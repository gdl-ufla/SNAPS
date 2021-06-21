using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DraggableButtonEasyMode : DraggableButton
{
    #pragma warning disable 0649 
    [SerializeField, BoxGroup("System type parameters")]
    private bool _useSystemTypeChecking;

    [SerializeField, BoxGroup("System type parameters"), ShowIf(nameof(_useSystemTypeChecking))]
    private Answer.SystemType _systemType;

    #pragma warning restore 0649 
    public bool UseSystemTypeChecking => _useSystemTypeChecking;
    public Answer.SystemType SystemType => _systemType;

    public void Restore(string answer, bool isAnswerCorrect, Answer.SystemType systemType, bool isInteractable = true)
    {
        Restore(answer, isAnswerCorrect);
        _systemType = systemType;
    }
}
