using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Extensions.PInvoke
{
    /// <summary>
    /// A wrapper class for COM objects to ensure proper disposal.
    /// </summary>
    /// <typeparam name="T">The type of the COM object.</typeparam>
    public class ObjectWrapper<T> : IDisposable where T : class
    {
        private T _comObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectWrapper{T}"/> class.
        /// </summary>
        /// <param name="progId">The ProgID of the COM object.</param>
        /// <exception cref="ArgumentException">Thrown when the ProgID is not found.</exception>
        public ObjectWrapper(string progId)
        {
            _comObject = InteropUtilities.CreateCOMObject<T>(progId);
        }

        /// <summary>
        /// Gets the wrapped COM object.
        /// </summary>
        public T COMObject => _comObject;

        /// <summary>
        /// Releases the COM object.
        /// </summary>
        public void Dispose()
        {
            InteropUtilities.ReleaseCOMObject(_comObject);
            _comObject = null;
        }
    }
}
