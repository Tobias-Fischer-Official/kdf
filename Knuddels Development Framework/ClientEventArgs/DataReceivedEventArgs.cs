using System;
namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnDataReceived-Ereignis bereit
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        private string _data;
        /// <summary>
        ///  Ruft die empfangenen Daten ab oder legt diese fest.
        /// </summary>
        public string Data
        {
            get { return _data; }            
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der DataReceivedEventArgs-Klasse.
        /// </summary>
        /// <param name="data">Die empfangenen Daten</param>
        public DataReceivedEventArgs(string data)
        {
            _data = data;            
        }
    }
}
