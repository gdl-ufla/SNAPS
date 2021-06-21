using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/SystemData")]
public class SystemData : ScriptableObject
{
    public string systemCode;
    public string FullName;
    public string CompactName;
    public Sprite DisplayImage;
    public string Article = "ao";
}
