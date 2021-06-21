using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Answer
{
    private bool _isAnswerCorrect;
    
    public enum SystemType
    {
        None, Neurotransmissor, Receptor, Efeito
    }
}
