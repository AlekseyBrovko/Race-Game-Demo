namespace Client
{
    /// <summary>
    /// Нужен при маленькой скорости авто, чтобы зафиксировать урон
    /// </summary>
    struct ColisionPedestrianByCarEvent
    {
        public int CarEntity;
        public int PadastrianEntity;
    }

    struct CollisionPedestrianByPhysicalObjectEvent
    {
        public IPhysicalObject PhysicalObject;
        public int PadastrianEntity;
    }
}