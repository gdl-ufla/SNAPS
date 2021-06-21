using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SystemImage
{
    [SerializeField] private string _name;
    [SerializeField] private string _compactName;
    [SerializeField] private Sprite _graphic;

    public SystemImage(string name, string compactName, Sprite graphic)
    {
        _name = name;
        _compactName = compactName;
        _graphic = graphic;
    }

    public string Name => _name;
    public string CompactName => _compactName;
    public Sprite Graphic => _graphic;
}
