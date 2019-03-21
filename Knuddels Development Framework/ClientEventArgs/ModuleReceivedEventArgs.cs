using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KDF.Networks.Protocol.Module;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnModuleReceived-Ereignis bereit
    /// </summary>
    public class ModuleReceivedEventArgs : EventArgs
    {
        private KnModule _module;
        /// <summary>
        /// Ruft die Instanz der KnModule Klasse ab
        /// </summary>
        public KnModule Module
        {
            get { return _module; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der ModuleReceivedEventArgs-Klasse.
        /// </summary>
        /// <param name="module">Gibt die empfangene Instanz der KnModule Klasse an</param>
        public ModuleReceivedEventArgs(KnModule module)
        {
            _module = module;
        }
    }
}
