public class AIPlayerStateManager : StateManager<AIPlayerStates, AIControl, BaseState<AIPlayerStates, AIControl>>
{
    public override void InitializeStateManager(AIControl ctx, AIPlayerStates startinState, StateManagerUpdate updateType)
    {
        _states.Add(AIPlayerStates.Idle, new AIPlayerIdleState(AIPlayerStates.Idle, ctx));
        _states.Add(AIPlayerStates.Throwing, new AIPlayerThrowingState(AIPlayerStates.Throwing, ctx));
        _states.Add(AIPlayerStates.Cooldown, new AIPlayerCooldownState(AIPlayerStates.Cooldown, ctx));
        base.InitializeStateManager(ctx, startinState, updateType);
    }
}
