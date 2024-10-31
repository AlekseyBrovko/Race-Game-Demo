using UnityEngine;
using UnityEngine.Audio;

public class AudioBehaviourMB : MonoBehaviour
{
    public AudioSource PlayerAudioSource => _playerAudioSource;
    public AudioSource UiAudioSource => _uiAudioSource;
    public AudioSource EnvironmentAudioSource => _environmentAudioSource;
    public AudioSource EnemyAudioSource => _enemyAudioSource;

    [SerializeField] private AudioSource _playerAudioSource;
    [SerializeField] private AudioSource _uiAudioSource;
    [SerializeField] private AudioSource _environmentAudioSource;
    [SerializeField] private AudioSource _enemyAudioSource;
    [SerializeField] private AudioMixer _mixer;

    public void SetSoundVolume(float value)
    {
        float valueForMixer = value * 80f - 80f;
        SetSoundsVol(valueForMixer);
    }

    public void SetMusicVolume(float value)
    {
        float valueForMixer = value * 80f - 80f;
        SetMusicVol(valueForMixer);
    }

    public void TurnOffPlaySounds() =>
        _mixer.SetFloat("PlaySounds", -80f);

    public void TurnOnPlaySounds() =>
        _mixer.SetFloat("PlaySounds", 0f);

    public void OffMusicVol() =>
        SetMusicVol(-80f);

    public void OnMusicVol() =>
        SetMusicVol(0f);

    public void OffSoundsVol() =>
        SetSoundsVol(-80f);

    public void OnSoundsVol() =>
        SetSoundsVol(0f);

    private void SetMusicVol(float vol) =>
        _mixer.SetFloat("MusicVol", vol);

    private void SetSoundsVol(float vol) =>
        _mixer.SetFloat("SoundsVol", vol);
}