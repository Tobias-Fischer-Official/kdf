using System;

namespace KDF.Exceptions
{
    /// <summary>
    /// Wird ausgelöst, wenn in der Parserklasse ein Fehler auftritt
    /// </summary>
    [Serializable]
    public class ParsingException : Exception
    {
        private string _tokenString;
        /// <summary>
        /// Ruft den betreffenden Tokenstring ab
        /// </summary>
        public string TokenString
        {
            get { return _tokenString; }            
        }

        private string _parserMethod;
        /// <summary>
        /// Ruf die Methode des Parsers ab, in welcher die Exception geworfen wurde
        /// </summary>
        public string ParserMethod
        {
            get { return _parserMethod; }
        }
        /// <summary>
        /// Inititalisiert eine neue Instanz der WrongTokenException-Klasse
        /// </summary>
        /// <param name="tokenString">Gibt den betreffenden Tokenstring an</param>
        /// <param name="parserMethod">Gibt die Methode des Parsers an, in welcher die Exception geworfen wurde</param>
        public ParsingException(string tokenString, string parserMethod)
        {
            _tokenString = tokenString;
            _parserMethod = parserMethod;
        }
      
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
