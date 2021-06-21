using YoukaiFox.Tools.GooglePlay;
using UnityEngine;
using TMPro;

public class Teste : MonoBehaviour
{
    public bool _hasManager;
    public TextMeshProUGUI _hasManagerText;
    
    void Update()
    {
        _hasManager = GooglePlayManager.Instance != null;

        if (_hasManagerText != null)
        {
            _hasManagerText.text = GooglePlayManager.Instance.ToString();
        }
    }
}
