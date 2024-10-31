using Client;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanelBehaviour : PanelBase, IPopupPanel
{
    [SerializeField] private Text _simpleMessageText;
    [SerializeField] private Text _bigMessageText;
    [SerializeField] private Text _missionNameText;

    private DataForMessagePanel _messageData;

    //TODO переписать на List если будет расширяться
    private Sequence _firstSequence;
    private Sequence _secondSequence;

    private Color _startSimpleMessageTextColor;
    private Color _startBigMessageTextColor;

    public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
    {
        base.Initialize(id, state, canvasMb, openData);

        _messageData = openData as DataForMessagePanel;

        _simpleMessageText.gameObject.SetActive(false);
        _bigMessageText.gameObject.SetActive(false);

        _startSimpleMessageTextColor = _simpleMessageText.color;
        _startBigMessageTextColor = _bigMessageText.color;

        ShowMessage();
    }

    private void ShowMessage()
    {
        switch(_messageData.MessageType)
        {
            case Enums.MessageTypeEnum.Custom:
                ShowSimpleMessage(_messageData.MessageText);
                break;

            case Enums.MessageTypeEnum.NotEnoughMoney:
                ShowSimpleMessage("Не достаточно денег");
                break;

            case Enums.MessageTypeEnum.MissionStartMessage:
                ShowMessageMissionStart();
                break;

            case Enums.MessageTypeEnum.MissionStartNameMessage:
                ShowMessageMissionStart(_messageData.MessageText);
                break;

            case Enums.MessageTypeEnum.MissionComplete:
                ShowMessageMissionComplete();
                break;

            case Enums.MessageTypeEnum.MissionNameComplete:
                ShowMessageMissionComplete(_messageData.MessageText);
                break;

            case Enums.MessageTypeEnum.MissionFailed:
                ShowMessageMissionFailed();
                break;

            case Enums.MessageTypeEnum.MissionNameFailed:
                ShowMessageMissionFailed(_messageData.MessageText);
                break;
        }
    }

    private void ShowSimpleMessage(string message)
    {
        _simpleMessageText.gameObject.SetActive(true);
        _simpleMessageText.text = message;
        SimpleBlinkFadeSequence(_firstSequence, _simpleMessageText, OnAnimationComplete);
    }

    private void ShowMessageMissionStart(string missionName = null) =>
        ShowMissionMessage("Миссия началась", missionName);

    private void ShowMessageMissionComplete(string missionName = null) =>
        ShowMissionMessage("Миссия выполнена", missionName);

    private void ShowMessageMissionFailed(string missionName = null) =>
        ShowMissionMessage("Миссия провалена", missionName);

    private void ShowMissionMessage(string message, string missionName = null)
    {
        _bigMessageText.gameObject.SetActive(true);
        _bigMessageText.text = message;
        SimpleFadeSequence(_firstSequence, _bigMessageText, OnAnimationComplete);

        if (missionName != null)
        {
            _missionNameText.gameObject.SetActive(true);
            _missionNameText.text = missionName;
            SimpleFadeSequence(_secondSequence, _missionNameText);
        }
    }

    private void OnAnimationComplete() =>
        _canvasMb.DestroyPanelById(Id);

    public override void CleanupPanel()
    {
        _firstSequence?.Kill();
        _secondSequence?.Kill();
    }

    private void SimpleBlinkFadeSequence(Sequence sequence, Text text, Action callback)
    {
        float duration = 0.2f;
        sequence = DOTween.Sequence();
        sequence
            .Append(text.DOFade(1f, duration))
            .AppendInterval(duration)
            .Append(text.DOFade(0.8f, duration))
            .AppendInterval(duration)
            .Append(text.DOFade(1f, duration))
            .AppendInterval(duration)
            .Append(text.DOFade(0f, duration))
            .AppendCallback(() =>{ callback(); });
    }

    private void SimpleFadeSequence(Sequence sequence, Text text, Action callback = null)
    {
        float duration = 0.2f;
        sequence = DOTween.Sequence();
        sequence
            .Append(text.DOFade(1f, duration))
            .AppendInterval(duration * 5f)
            .Append(text.DOFade(0f, duration));

        //TODO это нужно проверить
        if (callback != null)
            sequence.AppendCallback(() => { callback(); });
    }
}