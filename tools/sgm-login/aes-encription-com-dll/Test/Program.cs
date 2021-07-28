using EncryptionToVB6;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program 
    {
        static void Main(string[] args)
        {

            EncryptedPasswordManager encryptedPasswordManager = new EncryptedPasswordManager();

            //string cifrado = "jRGY-THmH9PHspfn_jsS6PZEX812SBl815UajJwn67AY7idI997cJCZJIHHxxtyREo3lYgepUdCJobHGJxt0tKqbcN2CpNvHGssHy2n-c9SbZuDzeeF4-hy3UvGIsgTexyLv4yByEoZdWgz8yyz6OhTFvg8M9459kBu8HNiNqkU1";

           string  cifrado =  encryptedPasswordManager.Encryptation("aabarca", 1);

            var result = EncryptedPasswordManager.ForPassword("Diarco").Decrypt(cifrado, true);

            //var resultado = Encryptation("david");

            Console.WriteLine(result);
            Console.ReadKey();
        }
        public string Encryptation(string user)
        {
            throw new NotImplementedException();
        }
    }
}
