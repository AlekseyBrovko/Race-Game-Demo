namespace Client
{
    public struct CarControllComp
    {
        public float HorizontalCurrent;

        public float HorizontalInput;
        public float HorizontalInputPreviousFrame;
        
        public float VerticalInput;
        public float VerticalInputPreviousFrame;

        public float TurningRadius;
        public float TurningRadiusByLimiter;

        public bool IsHandBrake;
    }
}