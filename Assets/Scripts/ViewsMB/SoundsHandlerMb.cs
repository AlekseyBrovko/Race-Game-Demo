using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SoundsHandlerMb : MonoBehaviour
{
    public static SoundsHandlerMb Instance { get; private set; }

    [SerializeField] private EventReference _menuMusicEvent;
    [SerializeField] private EventReference _inGameMusicEvent;

    private EventInstance _menuMusicInstance;
    private EventInstance _inGameMusicInstance;

    private bool _menuState;
    private bool _inGameState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Init();
            //PlayMenuMusic();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Init()
    {
        _menuMusicInstance = RuntimeManager.CreateInstance(_menuMusicEvent);
        _inGameMusicInstance = RuntimeManager.CreateInstance(_inGameMusicEvent);
    }

    public void PlayMenuMusic()
    {
        if (_menuState)
            return;

        _menuMusicInstance.getPaused(out bool isPaused);
        if (isPaused)
            _menuMusicInstance.setPaused(false);

        _menuState = true;
        _inGameState = false;

        _menuMusicInstance.start();
        _inGameMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void PlayInGameMusic()
    {
        if (_inGameState)
            return;

        _inGameMusicInstance.getPaused(out bool isPaused);
        if (isPaused)
            _inGameMusicInstance.setPaused(false);

        _menuState = false;
        _inGameState = true;

        _inGameMusicInstance.start();
        _menuMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void PauseMusic()
    {
        _menuMusicInstance.setPaused(true);
        _inGameMusicInstance.setPaused(true);
    }

    public void ContinueMusic()
    {
        if (_menuState)
        {
            _menuMusicInstance.setPaused(false);
            _inGameMusicInstance.setPaused(true);
        }
        else if (_inGameState)
        {
            _menuMusicInstance.setPaused(true);
            _inGameMusicInstance.setPaused(false);
        }
    }

    private void OnDestroy()
    {
        _menuMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _inGameMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        _menuMusicInstance.release();
        _inGameMusicInstance.release();
    }
}