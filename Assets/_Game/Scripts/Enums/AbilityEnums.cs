// Character has 3 abilities, which all have the following states:

public enum AbilityStateEnum
{
    Disabled, // Not unlocked, cannot be used
    Idle,     // Ability unlocked but cant be used yet
    Ready,    // Ability active and ready to be used
}

public enum AbilityActionEnum
{
    InputDetected,         // Ability input detected
    AttemptedUnsuccessful, // After input, ability was not successful
    AttemptedSuccessful,   // After input, ability was successful
    Acted,                 // Acted On / Ability used
}