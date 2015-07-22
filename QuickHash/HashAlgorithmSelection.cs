using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuickHash
{
    public class HashAlgorithmSelection
    {
        public string Name { get; set; }

        public Func<HashAlgorithm> ConstructorFunc { get; set; }
    }
}
