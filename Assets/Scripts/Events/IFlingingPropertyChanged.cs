
using UnityEngine;

public interface IFlingingPropertyChanged : IReactiveEventInterface
{
    public delegate void FlingingStarted();
    public delegate void FlingCompleted(Vector2 fling);
    public delegate void FlingingCancelled();

    public event FlingingStarted OnFlingingStarted;
    public event FlingCompleted OnFlingCompleted;
    public event FlingingCancelled OnFlingCancelled;
}