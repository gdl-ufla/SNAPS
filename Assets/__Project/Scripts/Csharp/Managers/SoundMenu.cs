using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class SoundMenu : MonoBehaviour
{
    #region Serialized fields
    #pragma warning disable 0649 

    [SerializeField] [BoxGroup("UI References")]
    private Toggle _mutedSoundCheckbox;

    [SerializeField] [BoxGroup("UI References")]
    private Slider _musicSlider;

    [SerializeField] [BoxGroup("UI References")]
    private Slider _sfxSlider;

    [SerializeField] [BoxGroup("Debug")]
    private AudioClip _testClip;

    #pragma warning restore 0649 
    #endregion

    #region Non-serialized fields
    #endregion

    #region Unity events

    private void Start() 
    {
        UpdateCurrentSoundMenu();
        RecoverSoundSettings();
        SetUpdateListener();
    }

    #endregion

    #region Public methods

    public void UpdateSoundSettings()
    {
        if (!SoundManager.Instance)
            return;

        var settings =  new SoundManager.SoundSettings(
            _mutedSoundCheckbox.isOn, _musicSlider.value, _sfxSlider.value);

        SoundManager.Instance.PlaySfx(_testClip);
        SoundManager.Instance.UpdateSettings(settings);
    }

    #endregion

    #region Private methods

    private void RecoverSoundSettings()
    {
        if (!SoundManager.Instance)
            return;

        SoundManager.SoundSettings settings = SoundManager.Instance.GetSoundSettings();
        _mutedSoundCheckbox.isOn = settings.IsMuted;
        _musicSlider.value = settings.MusicVolume;
        _sfxSlider.value = settings.SfxVolume;
    }

    private void UpdateCurrentSoundMenu()
    {
        if (!SoundManager.Instance)
            return;

        SoundManager.Instance.SetCurrentMenu(this);
    }
    
    private void SetUpdateListener()
    {
        _mutedSoundCheckbox.onValueChanged.AddListener(delegate {UpdateSoundSettings();});
        _musicSlider.onValueChanged.AddListener(delegate {UpdateSoundSettings();});
        _sfxSlider.onValueChanged.AddListener(delegate {UpdateSoundSettings();});
    }

    #endregion    
}
