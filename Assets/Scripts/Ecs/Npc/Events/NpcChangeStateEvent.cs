using System;

namespace Client
{
    public struct NpcChangeStateEvent
    {
        public Type ClassOfEvent;
        public Type StateType;
        public IDataForSetState NewStateData;
    }
}