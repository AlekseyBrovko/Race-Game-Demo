using System.Collections.Generic;

namespace Client
{
    public struct NpcStateComp
    {
        public List<string> States;
        public Enums.NpcStateType PreviousState;
        public Enums.NpcStateType State;
    }
}