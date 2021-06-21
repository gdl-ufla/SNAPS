using UnityEngine;
using UnityEngine.UI;

public class AlphaPingPong : MonoBehaviour {

    private Image _image;
    private float _alphaChange;
    private Color _changingColor;
    private void Awake() 
    {
        _image = GetComponent<Image>();
        
    }
    private void Start() 
    {
        _alphaChange = 0.5f;
        _changingColor = _image.color;
    }

    private void Update() 
    {
        _alphaChange = Mathf.PingPong(Time.time, 0.75f);
        _changingColor.a = _alphaChange;
        _image.color = _changingColor;
    }
}
