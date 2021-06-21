using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string SystemCode;
    public string SystemType;
    public string System;
    public string QuestionTheme;
    public string Answer;
    public string Hint;
    public string[] options;

    public override string ToString()
    {
        return $@"Code: {SystemCode}
                   Type: {SystemType}
                   System: {System}
                   Theme: {QuestionTheme}
                   Answer: {Answer}
                   Hint: {Hint}";
    }
}

[System.Serializable]
public class QuestionBank
{
    public Question[] questions;
}

[System.Serializable]
public class CompoundQuestion 
{
    public Question NeuroQuestion;
    public Question ReceptorQuestion;
    public Question EffectQuestion;

    public Question GetQuestion(Answer.SystemType systemType)
    {
        switch (systemType)
        {
            case Answer.SystemType.Neurotransmissor:
                return NeuroQuestion;
            case Answer.SystemType.Receptor:
                return ReceptorQuestion;
            case Answer.SystemType.Efeito:
                return EffectQuestion;
            default:
                return null;
        }
    }
}
