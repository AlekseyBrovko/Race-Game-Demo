using FMOD.Studio;
using System.Collections.Generic;

public class SoundsHandler
{
    private List<EventInstance> _eventsToDispose = new List<EventInstance>();

    public void AddEventInstanceToDisposeList(EventInstance eventInstance) =>
        _eventsToDispose.Add(eventInstance);

    public void DisposeAllEventInstances()
    {
        foreach (EventInstance eventInstance in _eventsToDispose)
        {
            //eventInstance.
        }
    }
}