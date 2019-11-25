using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Input.InputDevice
{
    public abstract class InputDeviceBase<TSelf, TInputMessage, TInputDriver>
        where TSelf : InputDeviceBase<TSelf, TInputMessage, TInputDriver>
        where TInputMessage : InputDeviceBase<TSelf, TInputMessage, TInputDriver>.InputMessageBase
        where TInputDriver : InputDeviceBase<TSelf, TInputMessage, TInputDriver>.InputDriverBase, new()
    {
        protected TInputDriver Driver { get; }
        protected abstract TInputMessage GetMessage();

        protected InputDeviceBase()
        {
            Driver = new TInputDriver();
            driverProxy = Driver as IDriverInternal;
        }

        public abstract class InputMessageBase : IMessageInternal
        {
            public bool AcceptNewInputs { get; private set; }

            bool IMessageInternal.AcceptNewInputs { set => AcceptNewInputs = value; }
        }

        public class InputDriverBase : IDriverInternal
        {
            public delegate void OnUpdate(TInputMessage msg);

            public event OnUpdate Update;

            protected virtual void PreUpdate(TInputMessage msg) { }
            protected virtual void PostUpdate(TInputMessage msg) { }

            void IDriverInternal.InvokeUpdate(TInputMessage msg)
            {
                PreUpdate(msg);
                Update?.Invoke(msg);
                PostUpdate(msg);
            }
        }

        internal void Update(bool captureNewPresses)
        {
            var msg = GetMessage();
            (msg as IMessageInternal).AcceptNewInputs = captureNewPresses;
            driverProxy.InvokeUpdate(msg);
        }

        private IDriverInternal driverProxy;
        private interface IDriverInternal { void InvokeUpdate(TInputMessage msg); }
        private interface IMessageInternal { bool AcceptNewInputs { set; } }
    }
}
