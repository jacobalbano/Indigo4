using Alexandria;
using Glide;
using Indigo.Core.Logging;
using Indigo.Inputs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Engine.Implementation
{
    internal class SubSystems : IDisposable
    {
        public Library Library { get; internal set; }
        public Window Window { get; internal set; }
        public Renderer Renderer { get; internal set; }
        public Tweener Tweener { get; internal set; }
        public GameTime GameTime { get; internal set; }
        public Logger Logger { get; internal set; }

        public Mouse Mouse { get; internal set; }
        //public Keyboard Keyboard { get; internal set; }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

        void IDisposable.Dispose() => Dispose(true);
        #endregion
    }
}
