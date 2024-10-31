using Client;
using UnityEngine;

public abstract class QuestCheckpoint : MonoBehaviour
{
    protected GameState _state;
    protected QuestChapter _questChapter;
    protected bool _isInteractable;

    public virtual void Init(GameState state, QuestChapter questChapter)
    {
        _state = state;
        _questChapter = questChapter;
        _isInteractable = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerCar carMb))
        {
            if (_isInteractable)
            {
                _isInteractable = false;
                OnTriggerExecute();
            }
        }   
    }

    protected virtual void OnTriggerExecute() { }
}