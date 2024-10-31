using SaverPlayerPrefs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private FadeCanvas _fadeCanvas;

    private void Start()
    {
        PlayerPrefsSaver saver = new PlayerPrefsSaver();
        PlayerPrefsSaver.TutorialData tutorialData = saver.LoadTutorialData();

        //TODO ��������� ���� ������ ��������
        //TODO ��������� �� ��� ����� ��������

        _fadeCanvas.Init();

        if (tutorialData.StartGameTutorial)
            SceneManager.LoadScene(ScenesIdHolder.TutorialSceneId);
        else
            SceneManager.LoadScene(ScenesIdHolder.StartSceneId);
    }
}