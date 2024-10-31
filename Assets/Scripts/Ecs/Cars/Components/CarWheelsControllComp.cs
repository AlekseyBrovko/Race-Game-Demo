namespace Client
{
    struct CarWheelsControllComp
    {
        public Wheel FrontRightWheel;
        public Wheel FrontLeftWheel;
        public Wheel RearRightWheel;
        public Wheel RearLeftWheel;

        public Wheel[] DrivingWheels;
        public Wheel[] HandBrakeWheels;
        public Wheel[] Wheels;

        public int[] WheelsEntities;

        public WheelAxisPair[] WheelAxisPairs;
        public WheelAxisPair[] DrivingWheelAxisPairs;
    }
}