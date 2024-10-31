using Client;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PanelsController
{
    public PanelBase MainPanel { get; private set; }
    public List<PanelBase> PanelsOnScene { get; private set; }

    private CanvasBehaviour _canvasControllerMb;
    private GameState _state;
    private UIPanelConfig _panelConfig;

    public PanelsController(GameState state, CanvasBehaviour canvasController)
    {
        _state = state;
        _canvasControllerMb = canvasController;
        _panelConfig = state.UiPanelConfig;
        PanelsOnScene = new List<PanelBase>();
    }

    public void ShowPanelById(string panelId, IOpenPanelData data = null)
    {
        if (IsPanelOnScene(panelId, out PanelBase panelOnSceneMb))
        {
            panelOnSceneMb.ShowPanel();
        }
        else
        {
            var panelMb = InstantiatePanelById(panelId, data);
            if (panelMb is IMainPanel)
            {
                if (MainPanel != null)
                {
                    PanelBase lastMainPanel = MainPanel;
                    DestroyPanelById(lastMainPanel.Id);
                }

                panelMb.gameObject.transform.SetAsLastSibling();
                MainPanel = panelMb;

                //TODO тут подправить выскакивает ошибка, если одновременно в одном кадре панели меняются
                foreach (var panel in PanelsOnScene)
                {
                    ISecondPanel secondPanel = panel as ISecondPanel;
                    IPopupPanel popupPanel = panel as IPopupPanel;

                    if (secondPanel != null)
                    {
                        if (secondPanel is ISecondPanelWithMainPanel)
                        {
                            ISecondPanelWithMainPanel secondPanelWithParent = secondPanel as ISecondPanelWithMainPanel;
                            if (secondPanelWithParent.MainPanelId == panelMb.Id)
                                panel.ShowPanel();
                            else
                                panel.HidePanel();
                        }
                    }

                    if (popupPanel != null)
                        DestroyPanelById(panel.Id);
                }
            }
            else if (panelMb is ISecondPanel)
            {
                if (panelMb is ISecondPanelWithMainPanel)
                {
                    ISecondPanelWithMainPanel secondParentPanel = panelMb as ISecondPanelWithMainPanel;
                    secondParentPanel.MainPanelId = MainPanel.Id;
                }
                if (MainPanel != null)
                    MainPanel.transform.SetAsLastSibling();
            }
            else if (panelMb is IPopupPanel)
            {
                panelMb.gameObject.transform.SetAsLastSibling();
            }
            PanelsOnScene.Add(panelMb);
            panelMb.Initialize(panelId, _state, _canvasControllerMb, data);
            panelMb.ShowPanel();
        }
    }

    public void HidePanelById(string id)
    {
        if (IsPanelOnScene(id, out PanelBase panelOnSceneMb))
            panelOnSceneMb.HidePanel();
    }

    public void DestroyPanelById(string id)
    {
        if (IsPanelOnScene(id, out PanelBase panelOnSceneMb))
        {
            DestroyPanel(panelOnSceneMb);
            PanelsOnScene.Remove(panelOnSceneMb);
        }
    }

    private GameObject InstantiatePanelById(string id, out PanelBase panelMb, IOpenPanelData data = null)
    {
        PanelInStorage storPanel = _panelConfig.Panels.FirstOrDefault(x => x.PanelId == id);

        if (storPanel == null)
        {
            Debug.Log("По этому Id нет панели");
            panelMb = null;
            return null;
        }

        GameObject panelPrefab = storPanel.PanelPrefab;
        GameObject panelGo = GameObject.Instantiate(panelPrefab, _canvasControllerMb.transform);
        panelMb = panelGo.GetComponent<PanelBase>();
        return panelGo;
    }

    private PanelBase InstantiatePanelById(string id, IOpenPanelData data = null)
    {
        PanelInStorage storPanel = _panelConfig.Panels.FirstOrDefault(x => x.PanelId == id);

        if (storPanel == null)
        {
            Debug.Log("По этому Id нет панели");
            return null;
        }

        GameObject panelPrefab = storPanel.PanelPrefab;
        GameObject panelGo = GameObject.Instantiate(panelPrefab, _canvasControllerMb.transform);
        var panelMb = panelGo.GetComponent<PanelBase>();
        return panelMb;
    }

    private void DestroyAllPanels()
    {
        foreach (var panel in PanelsOnScene)
            DestroyPanel(panel);

        PanelsOnScene.Clear();
    }

    private void DestroyPanel(PanelBase panelMb)
    {
        panelMb.CleanupPanel();
        panelMb.DestroyPanel();
    }

    private bool IsPanelOnScene(string id, out PanelBase panelMb)
    {
        panelMb = PanelsOnScene.FirstOrDefault(x => x.Id == id);
        if (panelMb == null)
            return false;
        return true;
    }
}