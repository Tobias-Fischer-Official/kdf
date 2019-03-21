using System;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnWindowOpened-Ereignis bereit
    /// </summary>
    public class WindowOpenedEventArgs :EventArgs
    {
        private string _windowToken;
        /// <summary>
        /// Ruft den Titel des Fensters ab
        /// </summary>
        public string WindowToken
        {
            get { return _windowToken; }            
        }
        private string _windowTitle;
        /// <summary>
        /// Ruft die Daten ab, welche das empfangene k-Token beinhaltet
        /// </summary>
        public string WindowTitle
        {
            get { return _windowTitle; }
        }

        private HelperClasses.Parser.Popup.PopupParser _popupParser;
        /// <summary>
        /// Ruft die erstellt PopupParser-Instanz ab
        /// </summary>
        public HelperClasses.Parser.Popup.PopupParser PopupParser
        {
            get { return _popupParser; }
        }
        /// <summary>
        /// Initialisiert eine neue Instanz der WindowOpenedEventArgs-Klasse.
        /// </summary>
        /// <param name="windowTitle">Gibt den Titel des Fensters an, welches sich geöffnet hat</param>
        /// <param name="windowToken">Gibt die Daten an, welche das empfangene k-Token beinhaltet</param>
        public WindowOpenedEventArgs(string windowTitle, string windowToken, HelperClasses.Parser.Popup.PopupParser popup)
        {
            _windowTitle = windowTitle;
            _windowToken = windowToken;
            _popupParser = popup;
        }
    }
}
