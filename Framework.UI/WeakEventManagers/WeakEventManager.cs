using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Framework.WeakEventManagers
{
    /// <summary>
    /// Generic WeakEventManager template
    /// Automatically generate event handler on event name
    /// </summary>
    /// <typeparam name="T">The manager class</typeparam>
    public abstract class WeakEventManager<T> : System.Windows.WeakEventManager
        where T : System.Windows.WeakEventManager, new()
    {
        #region Fields
        private static readonly Dictionary<Type, EventInfo> _eventInfos = new Dictionary<Type, EventInfo>();
        private static readonly Dictionary<Type, Delegate> _eventHandlers = new Dictionary<Type, Delegate>();
        #endregion Fields

        #region WeakEventManager Members
        public static WeakEventManager<T> CurrentManager
        {
            get
            {
                Type manager_type = typeof(T);
                WeakEventManager<T> manager = System.Windows.WeakEventManager.GetCurrentManager(manager_type) as WeakEventManager<T>;

                if (manager == null)
                {
                    manager = new T() as WeakEventManager<T>;
                    System.Windows.WeakEventManager.SetCurrentManager(manager_type, manager);
                }

                return manager;
            }
        }

        public static void AddListener(object source, System.Windows.IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        public static void RemoveListener(object source, System.Windows.IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        protected override void StartListening(object source)
        {
            EventInfo eventInfo = GetEventInfo(source);
            Delegate handler = GetHandler(eventInfo.EventHandlerType);
            eventInfo.AddEventHandler(source, handler);
        }

        protected override void StopListening(object source)
        {
            EventInfo eventInfo = GetEventInfo(source);
            Delegate handler = GetHandler(eventInfo.EventHandlerType);
            eventInfo.RemoveEventHandler(source, handler);
        }
        #endregion WeakEventManager Members

        #region Methods
        /// <summary>
        /// retrieve EventInfo data from the source's Type
        /// </summary>
        /// <param name="sourceType">source.GetType()</param>
        /// <returns>EventInfo data</returns>
        protected EventInfo GetEventInfo(object source)
        {
            Type sourceType = source.GetType();
            EventInfo eventInfo;
            if (_eventInfos.TryGetValue(sourceType, out eventInfo) == false)
            {
                eventInfo = sourceType.GetEvent(EventName);
                if (eventInfo == null)
                    throw new InvalidOperationException(String.Format("Could not resolve event name {0}", EventName));
                _eventInfos[sourceType] = eventInfo;
            }
            return eventInfo;
        }

        /// <summary>
        /// return a Delegate to handle an event from the eventHandlerType of a source
        /// </summary>
        /// <param name="eventHandlerType">eventInfo.EventHandlerType</param>
        /// <returns>the Delegate according to the event</returns>
        protected Delegate GetHandler(Type eventHandlerType)
        {
            Delegate eventHandler;
            if (_eventHandlers.TryGetValue(eventHandlerType, out eventHandler) == false)
            {
                eventHandler = BuildEvent(eventHandlerType);
                _eventHandlers[eventHandlerType] = eventHandler;
            }
            return eventHandler;
        }

        // we have to create a method dynamically to bind an event handler as a delegate
        // the method is like this one :
        // void method(object sender, EventArgs e)
        // {
        //     CurrentManager.DeliverEvent(sender, e);
        // }
        // but with the second parameter (EventArgs) strongly typed for specific event
        // so let's create the method using Reflection.Emit
        public Delegate BuildEvent(Type eventHandlerType)
        {
            // retrieve the Invoke method for the event
            MethodInfo eventInvoke = eventHandlerType.GetMethod("Invoke");
            // extract the parameters of the event
            ParameterInfo[] eventInvokeParameterInfo = eventInvoke.GetParameters();
            // create a list of types from parameters, +1 because we have to push the instance of this first
            Type[] eventInvokeParams = new Type[eventInvokeParameterInfo.Length + 1];
            // first parameter will be instance of this
            eventInvokeParams[0] = this.GetType();
            // other parameters will be thoses specifics to EventHandlerType
            for (int index = 0; index < eventInvokeParameterInfo.Length; ++index)
                eventInvokeParams[index + 1] = eventInvokeParameterInfo[index].ParameterType;

            // retrieve the get method for the CurrentManager
            MethodInfo getCurrentManager = typeof(WeakEventManager<T>).GetProperty("CurrentManager").GetGetMethod();
            // retrieve the DeliverEvent method
            MethodInfo currentManagerDeliverEventMethod = typeof(System.Windows.WeakEventManager).GetMethod(
                    "DeliverEvent", BindingFlags.Instance | BindingFlags.NonPublic,
                    null, new Type[] { typeof(Object), typeof(EventArgs) }, null);

            // create a dynamic method to use as an event handler
            DynamicMethod dm = new DynamicMethod("", null, eventInvokeParams, GetType());
            ILGenerator ilGenerator = dm.GetILGenerator();

            // call getCurrentManager
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.EmitCall(OpCodes.Call, getCurrentManager, null);
            // loads args (object and EventArgs)
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldarg_2);
            // call DeliverEvent
            ilGenerator.EmitCall(OpCodes.Callvirt, currentManagerDeliverEventMethod, null);
            // end of method
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ret);

            // now we can build our dynamic method and store it as a delegate
            return dm.CreateDelegate(eventHandlerType, this);
        }
        #endregion Methods

        #region Properties
        protected abstract string EventName { get; }
        #endregion Properties
    }
}
