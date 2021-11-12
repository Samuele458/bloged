using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Helpers
{
    /// <summary>
    ///  Alphabet
    /// </summary>
    public class Alphabet
    {

        private string alphabet;

        public Alphabet()
        {
            alphabet = "";
        }

        /// <summary>
        ///  Convert to string
        /// </summary>
        /// <returns>Current alphabet characters set</returns>
        public string toString()
        {
            return this.alphabet;
        }

        /// <summary>
        ///  Add numbers characters to alphabet
        /// </summary>
        /// <returns>Current alphabet reference</returns>
        public Alphabet AddNumbers()
        {
            this.alphabet += "0123456789";
            return this;
        }

        /// <summary>
        ///  Add letters characters (uppercase) to alphabet
        /// </summary>
        /// <returns>Current alphabet reference</returns>
        public Alphabet AddLettersUppercase()
        {
            this.alphabet += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return this;
        }

        /// <summary>
        ///  Add letters characters (lowercase) to alphabet
        /// </summary>
        /// <returns>Current alphabet reference</returns>
        public Alphabet AddLettersLowerCase()
        {
            this.alphabet += "abcdefghijklmnopqrstuvwxyz";
            return this;
        }

        /// <summary>
        ///  Add special characters to alphabet
        /// </summary>
        /// <returns>Current alphabet reference</returns>
        public Alphabet AddSpecials()
        {
            this.alphabet += "£$%&/()=-_.;,+*";
            return this;
        }


    }
 
    /// <summary>
    ///  Utils for handling strings
    /// </summary>
    public class StringHelper
    {

        /// <summary>
        ///  Generate random string
        /// </summary>
        /// <param name="length">String length</param>
        /// <param name="alphabet">Alphabet on which base the random generation</param>
        /// <returns>Random string</returns>
        public static string GenerateRandomString( int length, Alphabet alphabet )
        {
            var random = new Random();
            var chars = alphabet.toString();

            return new string(Enumerable
                                .Repeat(chars, length)
                                .Select(s => s[random.Next(s.Length)]).ToArray());
            
        }
    }
}
