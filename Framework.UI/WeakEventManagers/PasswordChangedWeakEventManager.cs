namespace Framework.WeakEventManagers
{
    public class PasswordChangedWeakEventManager : WeakEventManager<PasswordChangedWeakEventManager>
    {
        protected override string EventName { get { return "PasswordChanged"; } }
    }
}
