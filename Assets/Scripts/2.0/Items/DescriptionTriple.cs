using System;
using UnityEngine;

[Serializable]
public class DescriptionTriple
{
    [SerializeField] private string _nameKey;
    [SerializeField] private string _answerKey;
    [SerializeField] private string _descriptionKey;

    public string NameKey => _nameKey;
    public string AnswerKey => _answerKey;
    public string DescriptionKey => _descriptionKey;

    public DescriptionTriple() : this("", "", "") { }

    public DescriptionTriple(string nameKey, string answerKey, string descriptionKey)
    {
        _nameKey = nameKey;
        _answerKey = answerKey;
        _descriptionKey = descriptionKey;
    }

    public override string ToString()
    {
        return $"({_nameKey}, {_answerKey}, {_descriptionKey})";
    }
}