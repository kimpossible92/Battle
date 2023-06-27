public enum CardTypes
{
    Attack = 0,
    Skill = 1,
    Equipment = 2,
    Curse = 3
}

public enum Rarity
{
    Rare,
    Equipment,
    Common
}

public enum InPhobiaEventType
{
    OnTurnStart,
    OnTurnEnd,
    OnStepStart,
    OnStepEnd,
    OnAttack,
    OnDefense,
    //OnPlayed
}

public enum CardUIType
{
    TherapistCard,
    PatientCard,
    defaultCard
}
public enum ScreenPart
{
    PatientHand,
    Middle,
    Therapist
}

public enum CurveType
{
    Parabola,
    Bezier
}

public enum AffectType
{
    AddActionPoints = 0,
    AddBlock = 1,
    AddHealth = 2,
    AddPoison = 3,
    AddPower = 4,
    AddSpikes = 5,
    AddWeaknessOnDefense = 6,
    Armor = 7,
    Attack = 8,
    AttackOnDefense = 9,
    BlockTheDamage = 10,
    Discard = 11,
    DiscardAndAddBlockForEach = 12,
    DoubleNextAffect = 13,
    DoubleBlock = 14,
    DropKickWithoutAttack = 15,
    Exhaust = 16,
    GiveEnemyWeaknessOnHit = 17,
    MultiplyBlock = 18,
    Power = 19,
    PullCard = 20,
    SaveBlock = 21,
    SteelBlock = 22,
    TurnWeaknessIntoPoison = 23,
    Vulnerability = 24,
    Weakness = 25
}
