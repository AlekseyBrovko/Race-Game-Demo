using Client;
using UnityEngine;

public abstract class PanelBase : MonoBehaviour
{
    public string Id { get; private set; }

    protected GameState _state;
    protected CanvasBehaviour _canvasMb;
    protected IOpenPanelData _openPanelData;

    public virtual void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        Id = id;
        _state = state;
        _canvasMb = canvasMb;
        _openPanelData = openData;
    }

    public virtual void ShowPanel() =>
        this.gameObject.SetActive(true);

    public virtual void HidePanel() =>
        this.gameObject.SetActive(false);

    public virtual void OnBackButtonClick() { }

    public virtual void CleanupPanel() { }

    public virtual void DestroyPanel() =>
        Destroy(this.gameObject);
}