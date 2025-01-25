using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Riverside.Extensions.PInvoke
{
    /// <summary>
    /// Provides utility methods for COM interop.
    /// </summary>
    public static class InteropUtilities
    {
        private static readonly Dictionary<Delegate, int> EventHandlerCookies = new Dictionary<Delegate, int>();

        /// <summary>
        /// Releases a COM object.
        /// </summary>
        /// <param name="comObject">The COM object to release.</param>
        public static void ReleaseCOMObject(object comObject)
        {
            if (comObject != null && Marshal.IsComObject(comObject))
            {
                Marshal.ReleaseComObject(comObject);
            }
        }

        /// <summary>
        /// Creates a COM object from a ProgID.
        /// </summary>
        /// <typeparam name="T">The type of the COM object.</typeparam>
        /// <param name="progId">The ProgID of the COM object.</param>
        /// <returns>The created COM object.</returns>
        /// <exception cref="ArgumentException">Thrown when the ProgID is not found.</exception>
        public static T CreateCOMObject<T>(string progId) where T : class
        {
            Type comType = Type.GetTypeFromProgID(progId);
            return comType == null ? throw new ArgumentException($"ProgID {progId} not found.") : Activator.CreateInstance(comType) as T;
        }

        /// <summary>
        /// Attaches an event handler to a COM object.
        /// </summary>
        /// <typeparam name="T">The type of the COM event interface.</typeparam>
        /// <param name="comObject">The COM object.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="handler">The event handler delegate.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the COM object does not support event handling.</exception>
        public static void AttachEventHandler<T>(object comObject, string eventName, Delegate handler)
        {
            if (comObject == null)
            {
                throw new ArgumentNullException(nameof(comObject));
            }

            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentException("Event name cannot be null or empty.", nameof(eventName));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!(comObject is IConnectionPointContainer connectionPointContainer))
            {
                throw new ArgumentException("COM object does not support event handling.", nameof(comObject));
            }

            Guid guid = typeof(T).GUID;
            connectionPointContainer.FindConnectionPoint(ref guid, out IConnectionPoint connectionPoint);
            connectionPoint.Advise(handler, out int cookie);
            EventHandlerCookies[handler] = cookie;
        }

        /// <summary>
        /// Detaches an event handler from a COM object.
        /// </summary>
        /// <typeparam name="T">The type of the COM event interface.</typeparam>
        /// <param name="comObject">The COM object.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="handler">The event handler delegate.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the COM object does not support event handling or the event handler is not found.</exception>
        public static void DetachEventHandler<T>(object comObject, string eventName, Delegate handler)
        {
            if (comObject == null)
            {
                throw new ArgumentNullException(nameof(comObject));
            }

            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentException("Event name cannot be null or empty.", nameof(eventName));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            IConnectionPointContainer connectionPointContainer = comObject as IConnectionPointContainer;
            if (connectionPointContainer == null)
            {
                throw new ArgumentException("COM object does not support event handling.", nameof(comObject));
            }

            if (!EventHandlerCookies.TryGetValue(handler, out int cookie))
            {
                throw new ArgumentException("Event handler not found.", nameof(handler));
            }

            Guid guid = typeof(T).GUID;
            connectionPointContainer.FindConnectionPoint(ref guid, out IConnectionPoint connectionPoint);
            connectionPoint.Unadvise(cookie);
            EventHandlerCookies.Remove(handler);
        }
    }
}
