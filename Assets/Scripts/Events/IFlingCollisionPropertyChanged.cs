using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IFlingCollisionPropertyChanged : IReactiveEventInterface
{
    public delegate void FlingCollisionsCleared();
    public delegate void FlingCollisionHappened(FlingCollision collision);
    public delegate void FlingCollisionChainDone();

    public event FlingCollisionsCleared OnFlingCollisionsCleared;
    public event FlingCollisionHappened OnFlingCollisionHappened;
    public event FlingCollisionChainDone OnFlingCollisionChainDone;
}
