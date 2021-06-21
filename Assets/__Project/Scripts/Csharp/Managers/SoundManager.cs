using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukaiFox.UnityExtensions;

public class SoundManager : MonoBehaviour
{
    #region Static fields
    
    public static SoundManager Instance;
    
    #endregion
    #region Serialized fields

    #pragma warning disable 0649 
    [SerializeField] private List<SoundEvent> _soundEvents;
    
    #pragma warning restore 0649 

    #endregion

    #region Non-serialized fields

    private AudioSource _bgmPlayer;
    private AudioSource[] _sfxPlayer = new AudioSource[5];
    private SoundMenu _currentSoundMenu;

    #endregion

    #region Custom structures

    public struct SoundSettings
    {
        public bool IsMuted;
        public float MusicVolume;
        public float SfxVolume;

        public SoundSettings(bool isMuted, float musicVolume, float sfxVolume)
        {
            IsMuted = isMuted;
            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
        }
    }

    [System.Serializable]
    public struct SoundEvent
    {
        public SoundEventType EventType;
        public AudioClip AudioClip;
    }

    public struct SoundInfo
    {
        public AudioSource SourceUsed;
    }

    public enum SoundEventType
    {
        Confirm, Cancel, Warning, Danger, Success, Failure, Buy, RoundEnd, Drag, Drop, Pause
    }

    #endregion

    #region Unity events

    private void OnEnable() 
    {
        SoundButton.OnClick += PlaySfxOnEvent;
        Slot.OnDropButton += PlayDropSound;
        DraggableButton.OnDragStart += PlaySfxOnEvent;
    }

    private void OnDisable() 
    {
        SoundButton.OnClick -= PlaySfxOnEvent;
        Slot.OnDropButton -= PlayDropSound;
        DraggableButton.OnDragStart -= PlaySfxOnEvent;
    }

    private void Awake() 
    {
        SetUpSingleton();
        AssignComponents();
        RecoverSoundSettings();
    }

    #endregion

    #region Public methods

    public void UpdateSettings(SoundSettings updatedSettings)
    {
        UpdateCurrentSettings(updatedSettings);
        SaveSettings();
        VerifyBgmStatus();
    }

    public SoundSettings GetSoundSettings()
    {
        return new SoundSettings(_bgmPlayer.mute, _bgmPlayer.volume, _sfxPlayer[0].volume);
    }

    public void PlayBgm(AudioClip clip) 
    {
        if (_bgmPlayer.mute)
            return;

        if (!clip)
            return;

        if (clip == _bgmPlayer.clip)
            return;

        _bgmPlayer.PlayForced(clip);
    }

    public SoundInfo PlaySfx(AudioClip clip, bool looped = false) 
    {
        SoundInfo soundInfo = new SoundInfo();
        AudioSource source = GetAvailableAudioSource();
        soundInfo.SourceUsed = source;

        if (!source)
            return soundInfo;
        
        if (source.mute)
            return soundInfo;

        source.loop = looped;
        source.PlayForced(clip);
        return soundInfo;
    }

    public void PlaySfxOnEvent(SoundEventType eventType)
    {
        AudioClip clip = FindSoundEventClip(eventType);
        PlaySfx(clip);
    }

    public void StopSound(SoundInfo soundInfo)
    {
        if (soundInfo.SourceUsed == null)
            return;

        soundInfo.SourceUsed.Stop();
    }

    public void SetCurrentMenu(SoundMenu soundMenu) { _currentSoundMenu = soundMenu; }

    #endregion

    #region Protected methods
    #endregion

    #region Private methods

    private void RecoverSoundSettings()
    {
        if (!DataController.Instance)
            return;
        
        SoundSettings soundSettings = DataController.Instance.GetSoundSettings();

        if (soundSettings.IsMuted)
        {
            _bgmPlayer.mute = true;

            foreach (var sfxPlayer in _sfxPlayer)
            {
                sfxPlayer.mute = true;
            }
        }
        else
        {
            _bgmPlayer.mute = false;
            _bgmPlayer.volume = soundSettings.MusicVolume;

            foreach (var sfxPlayer in _sfxPlayer)
            {
                sfxPlayer.mute = false;
                sfxPlayer.volume = soundSettings.SfxVolume;
            }
        }

        VerifyBgmStatus();
    }

    private void AssignComponents()
    {
        if (!_bgmPlayer)
        {
            _bgmPlayer = gameObject.AddComponent<AudioSource>();
            _bgmPlayer.loop = true;
        }

        for (int i = 0; i < _sfxPlayer.Length; i++)
        {
            _sfxPlayer[i] = gameObject.AddComponent<AudioSource>();
            _sfxPlayer[i].playOnAwake = false;
        }
    }

    private void VerifyBgmStatus()
    {
        if (_bgmPlayer.mute)
        {
            if (_bgmPlayer.isPlaying)
                _bgmPlayer.Stop();
        }
        else
        {
            if (!_bgmPlayer.isPlaying)
                _bgmPlayer.Play();
        }
    }

    private void UpdateCurrentSettings(SoundSettings updatedSettings)
    {
        _bgmPlayer.mute = updatedSettings.IsMuted;
        _bgmPlayer.volume = updatedSettings.MusicVolume;

        foreach (var sfxPlayer in _sfxPlayer)
        {
            sfxPlayer.mute = updatedSettings.IsMuted;
            sfxPlayer.volume = updatedSettings.SfxVolume;
        }
    }

    private void SaveSettings()
    {
        if (!DataController.Instance)
            return;

        SoundSettings updatedSettings = new SoundSettings(_bgmPlayer.mute, _bgmPlayer.volume, _sfxPlayer[0].volume);
        DataController.Instance.SetSoundSettings(updatedSettings);
    }

    private AudioClip FindSoundEventClip(SoundEventType eventType)
    {
        foreach (var soundEvent in _soundEvents)
        {
            if (soundEvent.EventType == eventType)
                return soundEvent.AudioClip;
        }

        return null;
    }

    private void SetUpSingleton()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void PlayDropSound()
    {
        PlaySfxOnEvent(SoundEventType.Drop);
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in _sfxPlayer)
        {
            if (!source.isPlaying)
                return source;
        }

        return null;
    }

    #endregion    
}
