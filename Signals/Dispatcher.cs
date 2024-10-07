using System.Collections.Generic;

namespace K3.Mech.Signals {
    public static class Dispatcher {
        static List<ISignalListener> listeners = new();

        public static void Send<T>(Signal<T> signal) {
            foreach (var l in listeners) l.ReceiveSignalUpdate(signal);
        }

        public static void Register(ISignalListener listener) {
            listeners.Add(listener);
        }
    }

    public interface ISignalListener {
        void ReceiveSignalUpdate<T>(Signal<T> signal);
    }
}
