using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SceneTransitionHelperMb : MonoBehaviour
{
    public EventReference MenuMusicRef;
    public EventInstance MenuMusic { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        //MenuMusic = RuntimeManager.CreateInstance(MenuMusicRef);
        //MenuMusic.start();
    }

    [ContextMenu("PlayMusic()")]
    private void PlayMusic()
    {
        MenuMusic = RuntimeManager.CreateInstance(MenuMusicRef);
        MenuMusic.start();
    }
}