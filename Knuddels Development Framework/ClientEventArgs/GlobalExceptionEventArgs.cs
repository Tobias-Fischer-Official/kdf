using System;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Gibt das Modul des Frameworks an
    /// </summary>
    public enum KDFModule
    {
        /// <summary>
        /// Stellt die Grundlegende Verbindung zu Knuddels zur Verfügung
        /// </summary>
        Networks,
        /// <summary>
        /// Stellt alle Objekte, Methoden und Eigenschaften die zu einer Client-Instanz gehören zur Verfügung
        /// </summary>
        Client,
        /// <summary>
        /// Parsed die empfangenen Token-Strings
        /// </summary>
        Parsing,
        /// <summary>
        /// Hilfsklasse zum Senden vorgefertigter Datenpakete und Kommandos an den Chat
        /// </summary>
        Commands,
        /// <summary>
        /// Andere Teile des Frameworks
        /// </summary>
        Other,
        /// <summary>
        /// Die Verbindung zum PW-Hash-Server von K-Script
        /// </summary>
        HashServer,
    }

    /// <summary>
    /// Stellt Daten für das OnGlobalException-Ereignis bereit
    /// </summary>
    public class GlobalExceptionEventArgs : EventArgs
    {
        private Exception _exception;
        /// <summary>
        /// Ruft ab welcher Fehler auftrat
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        private KDFModule _kdfModule;
        /// <summary>
        /// Ruft ab, in welchem Modul der Fehler auftrat
        /// </summary>
        public KDFModule KdfModule
        {
            get { return _kdfModule; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der GlobalExceptionEventArgs-Klasse.
        /// </summary>
        /// <param name="exception">Gibt den Fehler an welcher auftrat</param>
        /// <param name="kdfModule">Gibt das Modul an in welchem der Fehler auftrat</param>
        public GlobalExceptionEventArgs(Exception exception, KDFModule kdfModule)
        {
            _exception = exception;
            _kdfModule = kdfModule;
        }
    }
}
