using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Riverside.Extensions.PInvoke
{
    /// <summary>
    /// A static helper class for handling COM errors.
    /// </summary>
    public static class COMErrorHandling
    {
        /// <summary>
        /// Checks an HRESULT for an error and throws an exception if an error occurred.
        /// </summary>
        /// <param name="hResult">The HRESULT to check.</param>
        public static void CheckHResult(int hResult)
        {
            if (hResult < 0)
            {
                Marshal.ThrowExceptionForHR(hResult);
            }
        }
    }
}
