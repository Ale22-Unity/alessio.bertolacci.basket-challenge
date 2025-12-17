using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class GameManagerWaitMatchEndState : BaseState<GameManagerStates, GameManager>
{
    public GameManagerWaitMatchEndState(GameManagerStates key, GameManager ctx) : base(key, ctx)
    {
    }

    public override void EnterState()
    {
        WaitEndGame().Forget();
    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdate()
    {

    }

    public override void FrameUpdate()
    {

    }

    public override GameManagerStates GetNextState()
    {
        return GameManagerStates.WaitMatchEnd;
    }

    public override void OnDestroy()
    {

    }

    private async UniTask WaitEndGame()
    {
        List<UniTask> tasks = new List<UniTask>();
        tasks.Add(UniTask.Delay(3000, false, PlayerLoopTiming.Update, _ctx.destroyCancellationToken));
        tasks.Add(_ctx.WaitForAllThrows());
        await UniTask.WhenAll(tasks);
        _ctx.EndGame();
    }
}
