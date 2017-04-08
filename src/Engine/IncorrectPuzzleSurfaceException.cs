using System;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// The exception that is thrown when <see cref="SurfaceValidator" /> detects an invalid puzzle surface.
    /// </summary>
    public class IncorrectPuzzleSurfaceException : Exception
    {
        public IncorrectPuzzleSurfaceException(string message)
            : base(message)
        {
        }
    }
}
