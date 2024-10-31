namespace Client
{
    struct HitPedestrianByCarEvent
    {
        public int CarEntity;
        public int PadastrianEntity;
    }

    struct HitPedestrianByPhysicalObjectEvent
    {
        public int PadastrianEntity;
        public IPhysicalObject PhysicalObject;
    }

    struct HitByCarDumpEvent
    {
        public int CarEntity;
        public int PadastrianEntity;
    }
}