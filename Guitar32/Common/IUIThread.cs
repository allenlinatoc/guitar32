using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Common
{
    /// <summary>
    /// Interface for implementing UIThread-related standard operations
    /// </summary>
    public interface IUIThread
    {
        /// <summary>
        /// Initialize UIThread binders
        /// </summary>
        void InitializeUIThreads();
    }
}
