using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using static System.Environment;
using static System.String;

namespace Ambient
{
    public class Op : IDisposable
    {
        public static IObservable<string> Log => Subject;
        static Subject<string> Subject { get; } = new Subject<string>();
        static AsyncLocal<Op> Context { get; } = new AsyncLocal<Op>();

        public Op([CallerMemberName] string text = null)
        {
            Parent = Context.Value;
            Context.Value = this;
            Stopwatch = Stopwatch.StartNew();
            Indent = Parent == null ? "" : Parent.Indent + "  ";
            Frame = new List<(string, Func<string>)>();
            Frame.Add((Indent + text, () => $"took {Stopwatch.ElapsedMilliseconds} ms"));
        }

        Op Parent { get; }
        Stopwatch Stopwatch { get; }
        string Indent { get; }

        List<(string Text, Func<string> Time)> Frame { get; }

        public void Dispose()
        {
            Stopwatch.Stop();
            Context.Value = Parent;
            if (Parent == null)
                Subject.OnNext(ToString());
            else
                lock (Parent.Frame)
                    lock (Frame)
                        Parent.Frame.AddRange(Frame);
        }

        public static void Trace(string text)
        {
            var op = Context.Value;
            if (op == null)
                Subject.OnNext(text);
            else
            {
                var ms = $"after {op.Stopwatch.ElapsedMilliseconds} ms";
                lock (op.Frame)
                    op.Frame.Add((op.Indent + "  " + text, () => ms));
            }
        }

        public override string ToString()
        {
            lock (Frame)
                return Join(NewLine,
                    from row in Frame
                    select $"{row.Text} {row.Time()}");
        }
    }
}
