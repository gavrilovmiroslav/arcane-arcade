
public interface IGamePropertyChanged : IReactiveEventInterface
{
    public delegate void PrepareForTurnCleanup();
    public delegate void TurnCleanup();
    public delegate void NextTurnStarted();
    public delegate void TurnOrderUnitRegistered(TurnBasedUnit turn);

    public event PrepareForTurnCleanup OnPrepareForTurnCleanup;
    public event TurnCleanup OnTurnCleanup;
    public event NextTurnStarted OnStartNextTurn;
    public event TurnOrderUnitRegistered OnRegisterTurnOrder;
}