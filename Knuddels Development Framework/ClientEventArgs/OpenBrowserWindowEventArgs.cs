using System;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnOpenBrowserWindow-Ereignis bereit
    /// </summary>
    public class OpenBrowserWindowEventArgs : EventArgs
    {
        private string _url;
        /// <summary>
        /// Die URL die geöffnet werden soll
        /// </summary>
        public string Url
        {
            get { return _url; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der OpenBrowserWindowEventArgs-Klasse.
        /// </summary>
        /// <param name="url">Die url die geöffnet werden soll</param>
        public OpenBrowserWindowEventArgs(string url)
        {
            _url = url;
        }
    }
}
