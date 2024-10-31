using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : PanelBase, IPopupPanel
{
    [SerializeField] private Button _crossButton;
    [SerializeField] private Button _backButton;

    private TutorialPanelData _data;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _data = (TutorialPanelData)openData;
        _crossButton.onClick.AddListener(() => OnCrossButtonClick());
        _backButton.onClick.AddListener(() => OnCrossButtonClick());
    }

    private void OnCrossButtonClick()
    {
        _canvasMb.PlayClickSound();
        _canvasMb.DestroyPanelById(this.Id);

        if (_data == null)
        {
            if (_state.SceneType == Enums.SceneType.PlayScene)
                _state.StartPlaySystems();
        }
        else
        {
            if (_data.StartPlaySystemsAfterBrif)
                _state.StartPlaySystems();
        }
    }   
}

public class TutorialPanelData : IOpenPanelData
{
    public TutorialPanelData(bool StartPlaySystemsAfterBrif) =>
        this.StartPlaySystemsAfterBrif = StartPlaySystemsAfterBrif;

    public bool StartPlaySystemsAfterBrif { get; set; }
}