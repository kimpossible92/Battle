using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
namespace Assets.Code
{
    public class Dialog
    {
        [SerializeField] List<string> _lines;
        public List<string> Lines
        {
            get { return _lines; }
        }
    }
}
[Serializable]
public class Dialog
{
    public string Persion = null;
    [TextArea()]
    public string text = null;

    public UnityEvent onThisDiologStart = null;

    public bool checkTrustPoints = false;
    public int trustPointsLimit;
    public UnityEvent onTrustPointPassedTheLimit = null;

    public List<FactChecker> factCheckers = null;
    public List<Question> questions = null;

    public UnityEvent onNext = null;
}


[Serializable]
public class Question
{
    public string question = null;
    public UnityEvent onQuestion = null;
}

[Serializable]
public class FactChecker
{
    public Fact fact = null;
    public UnityEvent onHasNotFact = null;
}
