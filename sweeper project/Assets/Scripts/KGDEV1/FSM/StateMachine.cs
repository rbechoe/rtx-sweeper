using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSMTest
{
    public class StateMachine
    {
        private IStateRunner owner;
        private IState currentState;

        public StateMachine( IStateRunner _owner )
        {
            owner = _owner;
        }

        public void Update()
        {
            if ( currentState != null )
            {
                currentState.Update(owner);
            }
        }

        public void FixedUpdate()
        {
            if ( currentState != null )
            {
                currentState.FixedUpdate(owner);
            }
        }

        public void SetState( IState newState )
        {
            if ( currentState != null )
            {
                currentState.Complete(owner);
                currentState.onSwitch -= SetState;
            }

            newState.Start(owner);
            newState.onSwitch += SetState;

            currentState = newState;
        }
    }
}
