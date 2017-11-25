using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace crossword_puzzle
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputPrefix = "inp";
            var outputPrefix = "out";
            for (var i = 0; i < 2; i++)
            {
                try
                {
                    var inpArr = new char[10, 10];
                    var words = new List<Tuple<string, int>>();
                    var outArr = new char[10, 10];

                    using (StreamReader sr = new StreamReader(inputPrefix + i + ".txt"))
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            var line = sr.ReadLine().ToCharArray();
                            for (int k = 0; k < line.Length; k++)
                            {
                                inpArr[j, k] = line[k];
                            }
                        }
                        foreach (var word in sr.ReadLine().Split(';'))
                        {
                            words.Add(Tuple.Create(word, word.Length));
                        }
                    }

                    using (StreamReader sr = new StreamReader(outputPrefix + i + ".txt"))
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            var line = sr.ReadLine().ToCharArray();
                            for (int k = 0; k < line.Length; k++)
                            {
                                outArr[j, k] = line[k];
                            }
                        }
                    }

                    var resArr = Solve(inpArr, words);

                    var equal = resArr.Rank == outArr.Rank &&
                        Enumerable.Range(0, resArr.Rank).All(dimension => resArr.GetLength(dimension) == outArr.GetLength(dimension)) &&
                        resArr.Cast<char>().SequenceEqual(outArr.Cast<char>());

                    Console.WriteLine("Test "+i);
                    if(equal)
                    {
                        Console.WriteLine("OK");
                    }
                    else
                    {
                        Console.WriteLine("Fail");
                        Console.WriteLine("Yours");
                        PrintArray(resArr);
                        Console.WriteLine("Proper");
                        PrintArray(outArr);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
        }

        static void PrintArray(char[,] arr)
        {
            for(var i=0;i<arr.GetLength(0);i++)
            {
                for(var j=0;j<arr.GetLength(1);j++)
                    Console.Write(arr[i,j]);
                Console.WriteLine();
            }
        }
        static char[,] Solve(char[,] input, List<Tuple<string, int>> words)
        {

        }
    }

}
