using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Framework.Collections;

namespace Framework.InstanceProvider
{
    public abstract class InstanceProviderBase<T> : MarkupExtension
    {
        #region Fields
        private static TupleList<Type, string, T> _instancesWithKey = new TupleList<Type, string, T>();
        private Type _type;
        private T _instance;
        #endregion Fields

        #region Ctor
        //public InstanceProviderBase(object parameter)
        //{
        //    _parameter = parameter;
        //}
        public InstanceProviderBase(Type type)
        {
            _type = type;
        }
        public InstanceProviderBase(string typeName)
        {
            _type = ResolveTypeName(typeName);
        }
        #endregion Ctor

        #region Properties
        public string Key { get; set; }
        #endregion Properties

        #region Methods
        protected virtual T DoGetInstance()
        {
            if (_instance == null)
            {
                CheckType(_type);
                _instance = GetInstance(_type, Key);
            }

            return _instance;
        }

        protected Type ResolveTypeName(string typeName)
        {
            return typeName.Contains('.') == false
                ? FindTypeByName(typeName)
                : FindTypeByFullName(typeName);
        }

        protected Type FindTypeByName(string typeName)
        {
            Type type = GetInterfaceTypes().FirstOrDefault(o => o.Name == typeName);
            if (type == null)
                throw BuildException(typeName);

            return type;
        }

        protected Type FindTypeByFullName(string typeFullName)
        {
            Type type = GetInterfaceTypes()
                .FirstOrDefault(o => o.FullName == typeFullName);

            if (type == null)
                throw BuildException(typeFullName);

            return type;
        }

        private InstanceProviderException BuildException(string typeName)
        {
            string message = string.Format("Unable to resolve parameter{0}type {1}{0}from assembly {2}", Environment.NewLine, typeName, this.GetType().Assembly.FullName);
            return new InstanceProviderException(message);
        }

        protected IEnumerable<Type> GetInterfaceTypes()
        {
            return this.GetType().Assembly
                .GetTypes()
                .Where(t => typeof(T).IsAssignableFrom(t));
        }
        #endregion Methods

        #region Static Methods
        protected static void CheckType(Type type)
        {
            if (type == null)
            {
                string message = string.Format("{0} : Type not specified", typeof(InstanceProviderBase<T>).Name);
                throw new InstanceProviderException(message);
            }
            if (typeof(T).IsAssignableFrom(type) == false)
            {
                string message = string.Format("{0} : {2} cant be assigned to {1}", typeof(InstanceProviderBase<T>).Name, typeof(T).Name, type.FullName);
                throw new InstanceProviderException(message);
            }
        }

        public static T GetInstance(Type type, string key = null)
        {
            CheckType(type);

            // no key, return a new instance
            if (string.IsNullOrEmpty(key) == true)
                return (T)Activator.CreateInstance(type);

            // try to find an existing instance with that key
            var tuple = _instancesWithKey.FirstOrDefault(o => o.Item1 == type && o.Item2 == key);
            if (tuple != null)
                return tuple.Item3;

            // create a new instance and register it
            T instance = (T)Activator.CreateInstance(type);
            _instancesWithKey.Add(type, key, instance);
            return instance;
        }

        public static CT GetInstance<CT>(string key = null)
            where CT : T
        {
            return (CT)GetInstance(typeof(CT), key);
        }
        #endregion Static Methods

        #region MarkupExtension Members
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return DoGetInstance();
        }
        #endregion MarkupExtension Members
    }
}
