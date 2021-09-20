
using UnityEngine;

namespace FSMTest
{
    public delegate void StateEvent( IState state );

    public interface IState
    {
        void Start( IStateRunner runner );
        void Update( IStateRunner runner );
        void FixedUpdate( IStateRunner runner );
        void Complete( IStateRunner runner );

        StateEvent onSwitch{ get; set; }
    }

    public interface IStateRunner
    {
        ScratchPad sharedData { get; }
    }

    public interface IPhysicsControllable
    {
        void Push( float x, float y, float z );
        void Drag( float x, float y, float z );
    }
}