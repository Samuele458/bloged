using BlogedWebapp.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BlogedWebbapp.UnitTests.Helpers
{
    [TestClass]
    public class AlphabetTests
    {
        [TestMethod]
        public void DefaultConstructor_ConstructsEmptyAlphabet_AlphabetIsEmpty()
        {
            Alphabet alphabet = new Alphabet();

            Assert.IsTrue(alphabet.ToString().Equals(""));
        }

        [TestMethod]
        public void AddNumbers_AddsNumebersToAlphabet_NumbersAdded()
        {
            Alphabet alphabet = new Alphabet();
            alphabet.AddNumbers();

            Assert.IsTrue(alphabet.ToString().Equals("0123456789"));
        }

        [TestMethod]
        public void AddLettersUppercase_AddsLettersToAlphabet_LettersAdded()
        {
            Alphabet alphabet = new Alphabet();
            alphabet.AddLettersUppercase();

            Assert.IsTrue(alphabet.ToString().Equals("ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        }

        [TestMethod]
        public void AddLettersLowercase_AddsLettersToAlphabet_LettersAdded()
        {
            Alphabet alphabet = new Alphabet();
            alphabet.AddLettersLowerCase();

            Assert.IsTrue(alphabet.ToString().Equals("abcdefghijklmnopqrstuvwxyz"));
        }

        [TestMethod]
        public void AddSpecials_AddsSpecialsToAlphabet_SpecialCharsAdded()
        {
            Alphabet alphabet = new Alphabet();
            alphabet.AddSpecials();

            Assert.IsTrue(alphabet.ToString().Equals("£$%&/()=-_.;,+*"));
        }

        [TestMethod]
        public void AddAllCharacters_AddsAllCharsToAlphabet_AllCharsAdded()
        {
            Alphabet alphabet = new Alphabet();
            alphabet.AddAllCharacters();

            string alphabetString = alphabet.ToString();

            Assert.IsTrue(alphabetString.Contains("£$%&/()=-_.;,+*"));
            Assert.IsTrue(alphabetString.Contains("0123456789"));
            Assert.IsTrue(alphabetString.Contains("abcdefghijklmnopqrstuvwxyz"));
            Assert.IsTrue(alphabetString.Contains("ABCDEFGHIJKLMNOPQRSTUVWXYZ"));

        }


    }

    [TestClass]
    public class StringHelperTests
    {
        [TestMethod]
        public void GenerateRandomString_PassingZeroLength_EmptyStringReturned()
        {
            Alphabet alphabet = new Alphabet();
            alphabet.AddLettersLowerCase();
            string randomString = StringHelper.GenerateRandomString(0, alphabet);

            Assert.IsTrue(randomString.Equals(""));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateRandomString_PassingEmptyAlphabet_ExceptionThrown()
        {
            Alphabet alphabet = new Alphabet();

            StringHelper.GenerateRandomString(10, alphabet);
        }

        [TestMethod]
        public void GenerateRandomString_PassingNormalParameters_GeneratedRandomString()
        {
            Alphabet alphabet = new Alphabet();
            alphabet.AddAllCharacters();

            string s1 = StringHelper.GenerateRandomString(64, alphabet);
            string s2 = StringHelper.GenerateRandomString(64, alphabet);

            Assert.IsTrue(s1.Length == 64);
            Assert.IsTrue(s2.Length == 64);
            Assert.IsTrue(!s1.Equals(s2));
        }

        [TestMethod]

        public void GenerateRandomString_PassingLengthEqualsToOne_GeneratedRandomString()
        {
            Alphabet alphabet = new Alphabet();
            alphabet.AddCustom("A");

            string s1 = StringHelper.GenerateRandomString(10, alphabet);

            Assert.IsTrue(s1.Length == 10);
            Assert.IsTrue(s1.Equals("AAAAAAAAAA"));
        }
    }
}
