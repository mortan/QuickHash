using System;
using System.Security.Cryptography;

namespace QuickHash
{
    public class HashAlgorithmSelection
    {
        public string Name { get; set; }

        public Func<HashAlgorithm> ConstructorFunc { get; set; }
    }
}
