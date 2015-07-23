using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuickHash
{
    public class HashService
    {
        public int BufferSize { get; set; } = 16384;

        public Task<string> CalculateHash(string fileName, HashAlgorithm algorithm, IProgress<int> progress)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!File.Exists(fileName))
                {
                    return "not a file";
                }

                using (var file = File.OpenRead(fileName))
                {
                    var buffer = new byte[BufferSize];
                    var fileInfo = new FileInfo(fileName);
                    long totalRead = 0;
                    double onePercent = fileInfo.Length / 100.0;
                    int oldProgress = 0;
                    while (totalRead < fileInfo.Length)
                    {
                        int read = file.Read(buffer, 0, BufferSize);
                        algorithm.TransformBlock(buffer, 0, read, buffer, 0);
                        totalRead += read;

                        int newProgress = (int)Math.Floor(totalRead / onePercent);
                        if (newProgress != oldProgress)
                        {
                            oldProgress = newProgress;
                            progress.Report(newProgress);
                        }
                    }

                    algorithm.TransformFinalBlock(buffer, 0, 0);

                    if (oldProgress != 100)
                    {
                        progress.Report(100);
                    }
                }

                return ByteArrayToString(algorithm.Hash);
            });
        }

        private string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
    }
}
