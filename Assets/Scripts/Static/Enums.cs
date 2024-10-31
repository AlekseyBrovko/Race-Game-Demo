public static class Enums
{
    public enum SoundEnum
    {
        BuySound, ClickSound, WarningSound, MoneyIncreaseSound,
        ZombieDead, ZombieAttack, ZombieHitCar,
        Explosion, Pickup,
        CarHitZombie, CarHitWood, CarHitDefault, CarHitMetal,

        MissionPartCompleteSound, CollectMissionSound, CheckpointMissionSound,
        MissionCompleteSound
    }

    public enum MovingType
    {
        Linear, FastOnStart, VeryFastOnStart, FastOnEnd, VeryFastOnEnd
    }

    public enum CarType
    {
        Simple, Race, Drift, Offroad
    }

    public enum StartStopEnum
    {
        Start, Stop
    }

    public enum OnOffEnum
    {
        On, Off
    }

    public enum SideEnum
    {
        Right, Left
    }

    public enum OrientationEnum
    {
        Front, Rear, Custom
    }

    public enum WheelDriveType
    {
        Fwd, Rwd, Awd, Custom
    }

    public enum NpcGlobalStateType
    {
        PeaceState, FightState, InactiveState
    }

    public enum BodyPartType
    {
        Head, LeftHand, RightHand, LeftLeg, RightLeg
    }

    public enum QuestState
    {
        NotAvailable = 0,
        Available = 1,
        Failed = 2,
        Complete = 3,
        InProgress = 4
    }

    public enum MissionType
    {
        CollectOneByOne, 
        CollectAllOnMap,

        RideByCheckpointsOneByOne,              //++
        RideByCheckpointsAllOnMap,

        Kill,                                   //++
        KillOnZone,                             //++

        Custom
    }

    public enum QuestStartType
    {
        FromPoint, FromDialog, Custom
    }

    public enum QuestChapterType
    {
        Collect, Ride, Kill, Custom
    }

    public enum MessageTypeEnum
    {
        Custom = 0,
        NotEnoughMoney = 1,
        MissionStartMessage = 2,
        MissionStartNameMessage = 3,
        MissionComplete = 4, 
        MissionNameComplete = 5,
        MissionFailed = 6, 
        MissionNameFailed = 7
    }

    public enum SceneType
    {
        PlayScene, StartScene, LobbyScene
    }

    public enum NpcType
    {
        Melee, Range, Quest, Boss
    }

    public enum CarUpgradeType
    {
        MaxSpeed, MotorPower, Hp, Fuel
    }

    public enum DamageType
    {
        Melee, Explosion, TrowObject, CarHit, ObjectHit, DamageFromCarDump
    }

    public enum EntityType
    {
        Car, Npc
    }

    public enum NpcStateType
    {
        Idle, Patroll, Chasing, Aim, HurtByCar, Dead, InPool, MeleeAttack, RangeAttack, AttackCoolDown, Spawn
    }

    public enum PickupType
    {
        Random, Health, Time, Money
    }

    public enum CarMotorType
    {
        OldDiesel, Truck, V8, FamilySedan
    }

    public enum PhysicalInteractableType
    {
        Default, Flash, Metal, Wood
    }

    public enum LoseType
    {
        Death, Fuel
    }

    public enum GameModType
    { 
        FreeRide, Story
    }

    public enum PlayPromoVideoReason
    {
        None = 0,
        RestoreHpAndFuel = 1,
        DoubleMissionMoney = 2
    }

    public enum IslandName
    {
        Farm, City, PowerStation
    }

    public enum TutorialPartType
    {
        ShowHowToRide,
        ShowMinimap, 
        ShowCheckpoint, 
        ShowRespawnButton, 
        ShowPickups,
        ShowHealth, 
        ShowFuel, 
        ShowZombies
    }
}