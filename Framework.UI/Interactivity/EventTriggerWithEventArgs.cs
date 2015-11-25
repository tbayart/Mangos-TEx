using System;
using System.Windows;

namespace Framework.Interactivity
{
    /// <summary>
    /// EventTrigger extended to keep EventArgs
    /// </summary>
    public class EventTriggerWithEventArgs : System.Windows.Interactivity.EventTrigger
    {
        #region EventArgsProperty
        public static readonly DependencyProperty EventArgsProperty =
            DependencyProperty.Register("EventArgs", typeof(EventArgs), typeof(EventTriggerWithEventArgs));

        public EventArgs EventArgs
        {
            get { return (EventArgs)GetValue(EventArgsProperty); }
            set { SetValue(EventArgsProperty, value); }
        }
        #endregion EventArgsProperty

        #region EventTrigger Members
        protected override void OnEvent(EventArgs eventArgs)
        {
            // we store the eventArgs so the Actions can use it
            // ie : as a CommandParameter to InvokeCommandAction
            EventArgs = eventArgs;
            base.OnEvent(eventArgs);
        }
        #endregion EventTrigger Members
    }
}
