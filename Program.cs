using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace crossword_puzzle
{

    struct WordPlace : IEquatable<WordPlace>
    {
        public int x;
        public int y;
        public int length;
        public bool isRightDirection;

        public WordPlace(int x, int y, int length, bool isRightDirection)
        {
            this.x = x;
            this.y = y;
            this.length = length;
            this.isRightDirection = isRightDirection;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WordPlace))
                return false;

            WordPlace o = (WordPlace)obj;
            return x == o.x && y == o.y && length == o.length && isRightDirection == o.isRightDirection;

        }

        bool IEquatable<WordPlace>.Equals(WordPlace other)
        {
            return this.Equals(other);
        }

        public static bool operator ==(WordPlace c1, WordPlace c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(WordPlace c1, WordPlace c2)
        {
            return !c1.Equals(c2);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var inputPrefix = "inputs\\inp";
            var outputPrefix = "inputs\\out";
            for (var i = 0; i < 2; i++)
            {
                try
                {
                    var inpArr = new char[10, 10];
                    var words = new List<string>();
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
                            words.Add(word);
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

                    var places = FindPlaces(inpArr, words);
                    //Console.WriteLine("Places:");
                    //foreach (var place in places)
                    //    Console.WriteLine(place.Item1 + " " + place.Item2 + " " + place.Item3 + " " + place.Item4);
                    var res = Solve(inpArr, words, places);
                    if (!res.Item2) Console.WriteLine("We couldn't find solution");
                    var resArr = res.Item1;

                    var equal = resArr.Rank == outArr.Rank &&
                        Enumerable.Range(0, resArr.Rank).All(dimension => resArr.GetLength(dimension) == outArr.GetLength(dimension)) &&
                        resArr.Cast<char>().SequenceEqual(outArr.Cast<char>());

                    Console.WriteLine("Test " + i);
                    if (equal)
                    {
                        Console.WriteLine("OK");
                    }
                    else
                    {
                        Console.WriteLine("Fail");
                        Console.WriteLine("Yours      Proper");
                        PrintArray(resArr, outArr);

                    }

                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static void PrintArray(char[,] arr, char[,] good)
        {
            for (var i = 0; i < good.GetLength(0); i++)
            {
                for (var j = 0; j < arr.GetLength(1); j++)
                    Console.Write(arr[i, j]);
                Console.Write(' ');
                for (var j = 0; j < good.GetLength(1); j++)
                    Console.Write(good[i, j]);
                Console.WriteLine();
            }
        }
        static Tuple<char[,], bool> Solve(char[,] input, List<string> words, List<WordPlace> places)
        {
            var word = words[0];

            foreach (var place in places.Where(x => x.length == word.Length))
            {
                if (CheckWord(input, place, word))
                {
                    char[,] arrCopy = (char[,])input.Clone();
                    SaveWord(arrCopy, place, word);
                    if(words.Count()==1) return Tuple.Create(arrCopy,true);
                    var wordsCopy = new List<string>(words.Skip(1));
                    var placesCopy = new List<WordPlace>(places.Where(x => x != place));
                    var res = Solve(arrCopy, wordsCopy, placesCopy);
                    if (res.Item2) return res;
                }
            }

            return Tuple.Create(input, false);
        }

        ///Checks if we can save the word
        static bool CheckWord(char[,] arr, WordPlace place, string word)
        {
            var res = true;
            for (int i = 0; i < word.Length; i++)
            {
                char letter = arr[place.x + (place.isRightDirection ? 0 : i), place.y + (place.isRightDirection ? i : 0)];
                if (letter != '-' && letter != word[i]) res = false;
            }
            return res;
        }

        ///Saves the word
        static void SaveWord(char[,] arr, WordPlace place, string word)
        {
            for (int i = 0; i < word.Length; i++)
                arr[place.x + (place.isRightDirection ? 0 : i), place.y + (place.isRightDirection ? i : 0)] = word[i];

        }

        static List<WordPlace> FindPlaces(char[,] input, List<string> words)
        {
            var places = new List<WordPlace>(); 

            //Right direction
            for (var i = 0; i < input.GetLength(0); i++)
            {
                var start = -1;
                for (var j = 0; j < input.GetLength(1); j++)
                {

                    if (input[i, j] == '-' && (j == 0 || input[i, j - 1] != '-')) // We are at the beginning
                    {
                        start = j;
                    }
                    else if (input[i, j] == '-' && (j == input.GetLength(1) - 1 || input[i, j + 1] != '-')) //We are at the end
                    {
                        places.Add(new WordPlace(i, start, j - start + 1, true));
                        start = -1;
                    }
                }
            }

            //Down direction
            for (var i = 0; i < input.GetLength(1); i++)
            {
                var start = -1;
                for (var j = 0; j < input.GetLength(0); j++)
                {

                    if (input[j, i] == '-' && (j == 0 || input[j - 1, i] != '-')) // We are at the beginning
                    {
                        start = j;
                    }
                    else if (input[j, i] == '-' && (j == input.GetLength(0) - 1 || input[j + 1, i] != '-')) //We are at the end
                    {
                        places.Add(new WordPlace(start, i, j - start + 1, false));
                        start = -1;
                    }
                }
            }


            return places;
        }
    }

}
