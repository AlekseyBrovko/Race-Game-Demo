using Client;
using UnityEngine;
using UnityEngine.UI;

public class FadePopupPanelBehaviour : PanelBase, IPopupPanel
{
    [SerializeField] private Image _fadeImage;
    //private float _fadeDuration = 2f;

    //TODO не забыть про эту заготовку
    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

    }

    //private IEnumerator FadeCoroutine()
    //{

    //}
}
