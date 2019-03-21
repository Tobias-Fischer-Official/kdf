using System;

namespace KDF.Exceptions
{
    /// <summary>
    /// Wird ausgelöst wenn ein Falsches Token verwendet wird
    /// </summary>#
    ///<seealso cref="KDF.Networks.Core.KnuddelsClient"/>
    [Serializable]
    public class WrongTokenException : Exception
    {
        /// <summary>
        /// Inititalisiert eine neue Instanz der WrongTokenException-Klasse
        /// </summary>
        /// <param name="tokenID">Die ID des Tokens welches fälschlicherweise verwendet wurde</param>
        /// <remarks>Die ID des Tokens steht im Data-Attribut der Exception</remarks>
        /// <example>
        /// <code>
        ///using System;
        ///
        ///namespace Test
        ///{
        ///    class Program
        ///    {
        ///        static void Main(string[] args)
        ///        {
        ///            try
        ///            {
        ///                throw new WrongTokenException("id");
        ///            }
        ///            catch (WrongTokenException ex)
        ///            {
        ///                Console.WriteLine(ex.Data["WrongToken"]);
        ///            }
        ///            Console.ReadLine();
        ///        }
        ///    }
        ///    
        ///    class WrongTokenException : Exception
        ///    {       
        ///        public WrongTokenException(string tokenID)
        ///        {
        ///            this.Data.Add("WrongToken", tokenID);
        ///        }
        ///    }
        ///}
        ///</code>
        /// </example>
        public WrongTokenException(string tokenID)
        {
            try
            {
                this.Data.Add("WrongToken", tokenID);
            }
            catch { }
        }
    }
}
