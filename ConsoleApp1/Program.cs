using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var randomByte = new byte[8192];
            for (long i = 8; i < 256; i++)
            {
                var filename = String.Format("D:\\keys\\private\\privateKeys{0}.bin", i);
                using (FileStream dest = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
                {
                    dest.Seek(0x01ffffff, SeekOrigin.Begin);
                    long needs = 536870912L;
                    long al = 0x01ffffffL;
                    needs -= al;
                    int a = 0;
                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        while (needs > 0)
                        {
                            a = Math.Min((int)needs, 8192);
                            rng.GetBytes(randomByte, 0, a);
                            dest.Write(randomByte, 0, a);
                            needs -= a;
                        }
                    }
                }
                Console.WriteLine(String.Format("{0} done.", filename));
            }
        }
    }
}
