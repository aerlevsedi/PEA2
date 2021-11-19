using System;
using System.Collections.Generic;
using System.IO;

namespace PEA_Project2
{
    public class Matrix
    {
        private int[,] _matrix;
        public int Size { get; set; }
        public Matrix(string path)
        {
            ReadFile(path);
        }

        public int[,] ReadFile(string path)
        {
            var firstLine = true;
            var counter = -1;
            var vertexCounter = 0;
            var lineNumber = 0;
            
            // Read the file and display it line by line.  
            foreach (var line in File.ReadLines(
                $"{path}"))
            {
                foreach (var s in line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (firstLine)
                    {
                        Size = int.Parse(s);
                        firstLine = false;
                        _matrix = new int[Size, Size];
                        continue;
                    }

                    if (vertexCounter == counter)
                    {
                        _matrix[counter, vertexCounter] = int.MaxValue;
                    }
                    else
                    {
                        _matrix[counter, vertexCounter] = int.Parse(s);
                    }

                    vertexCounter++;

                    if (Size < vertexCounter)
                    {
                        PrintException();
                    }
                }

                if (vertexCounter < Size - 1 && lineNumber > 0)
                {
                    PrintException();
                }

                lineNumber++;
                if (lineNumber > Size)
                {
                    break;
                }

                counter++;
                vertexCounter = 0;
            }


            return _matrix;
        }

        public int CalculateRouteCost(IList<int> route)
        {
            var cost = 0;
            for (var iteration = 0; iteration < route.Count - 1; iteration++)
            {
                var prev = route[iteration];
                var next = route[iteration + 1];
                cost += _matrix[prev, next];
            }

            cost += _matrix[route[route.Count - 1], 0];
            return cost;
        }

        private static void PrintException()
        {
            Console.WriteLine("Błąd!");
        }
    }
}