using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GetFileHash
{
    class DataEntropyUTF8
    {
        // Stores the number of times each symbol appears
        SortedList<byte, int> distributionDict;
        // Stores the entropy for each character
        SortedList<byte, double> probabilityDict;
        // Stores the last calculated entropy
        double overalEntropy;
        // Used for preventing unnecessary processing
        bool isDirty;

        public int DataSampleSize { get; private set; }

        public int UniqueSymbols
        {
            get { return distributionDict.Count; }
        }

        public double Entropy
        {
            get { return GetEntropy(); }
        }

        public Dictionary<byte, int> Distribution
        {
            get { return GetSortedDistribution(); }
        }

        public Dictionary<byte, double> Probability
        {
            get { return GetSortedProbability(); }
        }

        public byte GetGreatestDistribution()
        {
            return distributionDict.Keys[0];
        }

        public byte GetGreatestProbability()
        {
            return probabilityDict.Keys[0];
        }

        public double GetSymbolDistribution(byte symbol)
        {
            return distributionDict[symbol];
        }

        public double GetSymbolEntropy(byte symbol)
        {
            return probabilityDict[symbol];
        }

        Dictionary<byte, int> GetSortedDistribution()
        {
            List<Tuple<int, byte>> entryList = new List<Tuple<int, byte>>();
            foreach (KeyValuePair<byte, int> entry in distributionDict)
            {
                entryList.Add(new Tuple<int, byte>(entry.Value, entry.Key));
            }
            entryList.Sort();
            entryList.Reverse();

            Dictionary<byte, int> result = new Dictionary<byte, int>();
            foreach (Tuple<int, byte> entry in entryList)
            {
                result.Add(entry.Item2, entry.Item1);
            }
            return result;
        }

        Dictionary<byte, double> GetSortedProbability()
        {
            List<Tuple<double, byte>> entryList = new List<Tuple<double, byte>>();
            foreach (KeyValuePair<byte, double> entry in probabilityDict)
            {
                entryList.Add(new Tuple<double, byte>(entry.Value, entry.Key));
            }
            entryList.Sort();
            entryList.Reverse();

            Dictionary<byte, double> result = new Dictionary<byte, double>();
            foreach (Tuple<double, byte> entry in entryList)
            {
                result.Add(entry.Item2, entry.Item1);
            }
            return result;
        }

        double GetEntropy()
        {
            // If nothing has changed, dont recalculate
            if (!isDirty)
            {
                return overalEntropy;
            }
            // Reset values
            overalEntropy = 0;
            probabilityDict = new SortedList<byte, double>();

            foreach (KeyValuePair<byte, int> entry in distributionDict)
            {
                // Prabability = Freq of symbol / # symbols examined thus far
                probabilityDict.Add(
                    entry.Key,
                    (double)distributionDict[entry.Key] / DataSampleSize
                );
            }

            foreach (KeyValuePair<byte, double> entry in probabilityDict)
            {
                // Entropy = probability * Log2(1/probability)
                overalEntropy += entry.Value * Math.Log((1 / entry.Value), 2);
            }

            isDirty = false;
            return overalEntropy;
        }

        public void ExamineChunk(byte[] chunk)
        {
            if (chunk.Length < 1 || chunk == null)
            {
                return;
            }

            isDirty = true;
            DataSampleSize += chunk.Length;

            foreach (byte bite in chunk)
            {
                if (!distributionDict.ContainsKey(bite))
                {
                    distributionDict.Add(bite, 1);
                    continue;
                }
                distributionDict[bite]++;
            }
        }

        public void ExamineChunk(string chunk)
        {
            ExamineChunk(StringToByteArray(chunk));
        }

        byte[] StringToByteArray(string inputString)
        {
            char[] c = inputString.ToCharArray();
            IEnumerable<byte> b = c.Cast<byte>();
            return b.ToArray();
        }

        void Clear()
        {
            isDirty = true;
            overalEntropy = 0;
            DataSampleSize = 0;
            distributionDict = new SortedList<byte, int>();
            probabilityDict = new SortedList<byte, double>();
        }

        public DataEntropyUTF8(string fileName)
        {
            Clear();
            if (File.Exists(fileName))
            {
                ExamineChunk(File.ReadAllBytes(fileName));
                GetEntropy();
                GetSortedDistribution();
            }
        }

        public DataEntropyUTF8()
        {
            Clear();
        }
    }
}