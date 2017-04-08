using System;
using System.Diagnostics;
using System.IO;
using BinaryPuzzleSolver.Engine.Utilities;
using JetBrains.Annotations;

namespace BinaryPuzzleSolver.DemoApp
{
    /// <summary>
    /// Writes execution duration of a block of code to the console (or any other
    /// <see cref="System.IO.TextWriter" />). This class is intended to be wrapped in a using statement (see example below).
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// using (new CodeTimer("Save order"))
    /// {
    ///     provider.SaveOrder(order);
    /// }
    /// ]]></code>
    /// </example>
    public sealed class CodeTimer : IDisposable
    {
        [NotNull]
        private readonly string operation;

        [NotNull]
        private readonly TextWriter outputWriter;

        [NotNull]
        private readonly Stopwatch stopwatch = new Stopwatch();

        public CodeTimer([NotNull] string operation, [NotNull] TextWriter outputWriter)
        {
            Guard.NotNull(operation, nameof(operation));
            Guard.NotNull(outputWriter, nameof(outputWriter));

            this.operation = operation;
            this.outputWriter = outputWriter;
            stopwatch.Start();
        }

        public CodeTimer([NotNull] string operation)
            : this(operation, Console.Out)
        {
        }

        public void Dispose()
        {
            stopwatch.Stop();
            outputWriter.WriteLine("Duration of '{0}': {1} ({2} ms).", operation, stopwatch.Elapsed,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
