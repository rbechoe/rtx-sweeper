namespace FSMTest
{
    public abstract class AState : IState
    {
        public virtual void Start( IStateRunner runner ){}
        public virtual void Update( IStateRunner runner ){}
        public virtual void FixedUpdate( IStateRunner runner ){}
        public virtual void Complete( IStateRunner runner ){}

        public StateEvent onSwitch{ get; set; }
    }
}