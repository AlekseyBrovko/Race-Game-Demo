namespace Client 
{
    struct SkipTutorialEvent { }
    struct TutorialComp { }
    struct TutorialStageComp
    {
        public Enums.TutorialPartType TutorialPartType;
    }

    struct ShowHowToRideComp
    {

    }

    struct ContinueTutorialEvent
    {
        public Enums.TutorialPartType TutorialPartType;
    }

    struct TutorialTimerComp
    {
        public float Timer;
    }
}