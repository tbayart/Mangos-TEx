using System;

namespace Framework
{
    public class EventArgs<T> : EventArgs
    {
        public T Arg { get; protected set; }
    }
}
