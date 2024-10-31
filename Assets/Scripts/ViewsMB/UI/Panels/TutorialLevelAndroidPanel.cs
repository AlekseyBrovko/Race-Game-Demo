using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class TutorialLevelAndroidPanel : PanelBase, IPopupPanel
{
    [SerializeField] private Button _continueButton;

    private TutorialLevelPanelData _panelData;

    private EcsWorld _world;
    private EcsPool<ContinueTutorialEvent> _continuePool;

    public override void Initialize(string id, GameState state, 
        CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _continueButton.onClick.AddListener(() => OnContinueButtonClick());

        _panelData = (TutorialLevelPanelData)openData;

        _world = state.EcsWorld;
        _continuePool = _world.GetPool<ContinueTutorialEvent>();

        BuildPanel();
    }

    private void BuildPanel()
    {
        switch(_panelData.TutorialPartType)
        {
            case Enums.TutorialPartType.ShowHowToRide:

                break;

            case Enums.TutorialPartType.ShowMinimap:

                break;

            case Enums.TutorialPartType.ShowCheckpoint:

                break;

            case Enums.TutorialPartType.ShowRespawnButton:
                
                break;

            case Enums.TutorialPartType.ShowPickups:
                
                break;

            case Enums.TutorialPartType.ShowHealth:
                
                break;

            case Enums.TutorialPartType.ShowFuel:
                
                break;

            case Enums.TutorialPartType.ShowZombies:

                break;
        }
    }

    private void OnContinueButtonClick()
    {
        _canvasMb.PlayClickSound();

        //под вопросом
        _state.StartPlaySystems();

        ClosePanel();
    }

    private void ClosePanel() =>
        _canvasMb.DestroyPanelById(Id);
}

public class TutorialLevelPanelData : IOpenPanelData
{
    public TutorialLevelPanelData(Enums.TutorialPartType partType) =>
        TutorialPartType = partType;

    public Enums.TutorialPartType TutorialPartType { get; private set; }
}