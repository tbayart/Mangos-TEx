namespace Framework.WeakEventManagers
{
    public class MouseMoveWeakEventManager : WeakEventManager<MouseMoveWeakEventManager>
    {
        protected override string EventName { get { return "MouseMove"; } }
    }
}
