﻿using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        public static string BytesToStr(byte[] bytes)
        {
            StringBuilder str = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                str.AppendFormat("{0:X2}", bytes[i]);

            return str.ToString();
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            byte[] b = new byte[65 * 16777216];
            using (FileStream stream = new FileStream(args[0], FileMode.Open, FileAccess.Read))
            using (BinaryReader r = new BinaryReader(stream))
            {
                r.Read(b, 0, b.Length);
            }
            using (SHA512 sha512 = SHA512.Create())
            using (RIPEMD160 ripemd160 = RIPEMD160.Create())
            using (RandomNumberGenerator generator = RandomNumberGenerator.Create())
            {
                bool con = true;
                byte[] buf = new byte[3];
                do
                {
                    generator.GetBytes(buf);
                    int index = ((buf[0] & 0xff) << 16) + ((buf[1] & 0xff) << 8) + ((buf[2] & 0xff));
                    int offset = (index << 6) + index;
                    for (int i = 0; i < 1090519040; i += 65)
                    {
                        sha512.TransformBlock(b, offset, 65, null, 0);
                        sha512.TransformFinalBlock(b, i, 65);
                        if ((BinaryPrimitives.ReadUInt64BigEndian(ripemd160.ComputeHash(sha512.Hash)) & 0xffffffffffff0000L) == 0)
                        {
                            Console.WriteLine("{0},{1}", index, i / 65);
                            con = false;
                        }
                        sha512.Initialize();
                        ripemd160.Initialize();
                    }
                    Console.Error.WriteLine("{0} done", index);
                } while (con);
            }
        }
    }
}
