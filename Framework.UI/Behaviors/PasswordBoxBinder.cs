using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Framework.Helpers;
using Framework.WeakEventManagers;

namespace Framework.Behaviors
{
    public class PasswordBoxBinder : Behavior<PasswordBox>, IWeakEventListener
    {
        #region SecurePasswordProperty
        // Using a DependencyProperty as the backing store for SecurePassword.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecurePasswordProperty =
            DependencyProperty.Register("SecurePassword", typeof(SecureString), typeof(PasswordBoxBinder),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSecurePasswordPropertyChanged));

        public SecureString SecurePassword
        {
            get { return (SecureString)GetValue(SecurePasswordProperty); }
            set { SetValue(SecurePasswordProperty, value); }
        }

        private static void OnSecurePasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((PasswordBoxBinder)sender).OnSecurePasswordChanged();
        }
        #endregion SecurePasswordProperty

        #region Methods
        private void HookAssociatedObject()
        {
            if (AssociatedObject == null)
                return;

            // make sure the AssociatedObject is not already hooked
            UnhookAssociatedObject();
            // setup the PasswordBox with the current password
            OnSecurePasswordChanged();
            // Listen for PasswordChanged routed event
            PasswordChangedWeakEventManager.AddListener(AssociatedObject, this); 
        }

        private void UnhookAssociatedObject()
        {
            if (AssociatedObject == null)
                return;

            // Stop listening for PasswordChanged routed event
            PasswordChangedWeakEventManager.RemoveListener(AssociatedObject, this);
        }

        private bool _associatedObjectSecurePasswordChanging = false;
        private void OnSecurePasswordChanged()
        {
            if (AssociatedObject != null && _associatedObjectSecurePasswordChanging == false
            && object.ReferenceEquals(AssociatedObject.SecurePassword, SecurePassword) == false)
                AssociatedObject.Password = SecurePassword.ConvertToUnsecureString();
        }
        #endregion Methods

        #region Behavior
        protected override void OnAttached()
        {
            base.OnAttached();
            HookAssociatedObject();
        }

        protected override void OnChanged()
        {
            base.OnChanged();
            HookAssociatedObject();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            UnhookAssociatedObject();
        }
        #endregion Behavior

        #region IWeakEventListener
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if(managerType == typeof(PasswordChangedWeakEventManager))
                return OnAssociatedObjectSecurePasswordChanged();
            return false;
        }

        private bool OnAssociatedObjectSecurePasswordChanged()
        {
            // avoid re-entrancy
            _associatedObjectSecurePasswordChanging = true;
            SecurePassword = AssociatedObject.SecurePassword;
            _associatedObjectSecurePasswordChanging = false;
            return true;
        }
        #endregion IWeakEventListener
    }
}
