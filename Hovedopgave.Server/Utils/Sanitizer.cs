namespace Hovedopgave.Server.Utils
{
    public class Sanitizer
    {
        private static List<char> validCharacters = new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        public static bool CheckInputValidity(string input) 
        {
            string lowerCased = input.ToLower();
            bool valid = true;

            foreach (char element in lowerCased) if (!validCharacters.Contains(element)) valid = false;

            return valid;
        }
    }
}
