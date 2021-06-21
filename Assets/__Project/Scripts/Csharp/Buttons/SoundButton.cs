using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YoukaiFox.UnityExtensions;
using NaughtyAttributes;

public class SoundButton : MonoBehaviour
{
    #pragma warning disable 0649 
    [SerializeField]
    private TriggerType _triggerType;

    [SerializeField] [ShowIf(nameof(IsEventType))]
    private SoundManager.SoundEventType _soundType;

    [SerializeField] [ShowIf(nameof(UsesAudioClip))]
    private AudioClip _onClickSound;
    #pragma warning restore 0649 
    
    public static System.Action<SoundManager.SoundEventType> OnClick;
    private Button _button;
    private bool IsEventType => _triggerType == TriggerType.Event;
    private bool UsesAudioClip => _triggerType == TriggerType.AudioClip;

    public enum TriggerType
    {
        Event, AudioClip
    }

    private void Awake() 
    {
        _button = GetComponent<Button>();

        if (!_button)
            _button = gameObject.AddComponent<Button>();
    }

    private void Start() 
    {
        _button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        if (IsEventType)
        {
            if (OnClick != null)
                OnClick(_soundType);
        }
        else
        {
            if (!SoundManager.Instance)
                return;

            SoundManager.Instance.PlaySfx(_onClickSound);
        }
            
    }
}
