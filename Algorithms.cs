using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PEA_Project2
{
    public class Algorithms
    {
        private static Random random = new Random();
        public static int [] bestPath;

        private static void Shuffle(int[] array)
        {
            var n = array.Length;
            while (n > 1)
            {
                n--;
                var i = random.Next(1, n + 1);
                (array[i], array[n]) = (array[n], array[i]);
            }
        }

        public static void Swap(int[] array, int i, int j)
        {
            (array[i], array[j]) = (array[j], array[i]);
        }

        public static void Reverse(int[] array, int i, int j)
        {
            if (j < i)
            {
                (j, i) = (i, j);
            }

            Array.Reverse(array, i, j - i + 1);
        }

        public static int[] SimulatedAnnealing(Matrix matrix, int time, Action<int[], int, int> neighbourFunc)
        {
            // wygeneruj losowa sciezke bedaca poczatkowym rozwiazaniem

            var currentPath = GenerateRandomPath(matrix, out var currentCost);
            double temperature = matrix.Size * matrix.Size;

            const double alpha = 0.99;

            // wygenerowana losowa sciezka przypisywana jest jako najlepsze rozwiazanie
            bestPath = currentPath;
            var bestPathCost = currentCost;
            var stopwatch = new Stopwatch();
            time *= 1000; //zamiana sekund na milisekundy
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds <= time) // warunek zatrzymania
            {
                for (var k = 0; k < matrix.Size * 10; k++)
                {
                    var neighbourPath = currentPath.Clone() as int[];
                    var i = random.Next(1, matrix.Size);
                    var j = random.Next(1, matrix.Size);
                    
                    // sprawdzenie czy nie sa te same
                    while (i == j)
                    {
                        j = random.Next(1, matrix.Size);
                    }    
                    
                    // wylosowanie sciezki z sasiedztwa aktualnego rozwiazania

                    neighbourFunc(neighbourPath, i, j);

                    var neighbourCost = matrix.CalculateRouteCost(neighbourPath);

                    if (neighbourCost <=
                        bestPathCost) // jesli koszt wylosowanej sciezki jest mniejszy od najlepszego rozwiazania
                    {
                        // najlepszym i aktualnym rozwiazaniem jest ta nowa sciezka

                        bestPath = neighbourPath;
                        bestPathCost = neighbourCost;
                        currentCost = neighbourCost;
                        currentPath = neighbourPath;
                    }
                    else
                    {
                        if (neighbourCost >= currentCost &&
                            !(random.NextDouble() < Math.Exp(-(neighbourCost - currentCost) / temperature))) continue;
                        // przyjmujemy je jako aktualne rozwiazanie, jesli jest wystarczajaco goraco 
                        // (im jest cieplej, tym wieksza szansa na to, ze to nowe bedzie wziete, mimo ze jest gorsze)

                        currentCost = neighbourCost;
                        currentPath = neighbourPath;
                    }
                }

                temperature *= alpha;
            }

            return bestPath;
        }

        private static int[] GenerateRandomPath(Matrix matrix, out int currentCost)
        {
            var currentPath = Enumerable.Range(0, matrix.Size).ToArray();
            Shuffle(currentPath);

            currentCost = matrix.CalculateRouteCost(currentPath);
            return currentPath;
        }

        public static int[] TabuSearch(Matrix matrix, int time, Action<int[], int, int> neighbourFunc,
            bool diversification)
        {
            var currentPath = GenerateRandomPath(matrix, out var currentCost);
            bestPath = currentPath;
            var bestPathCost = currentCost;
            var tabuList = new Dictionary<(int, int), int>();

            var maxTerm = matrix.Size/2;
            var iterations = 0;
            var maxIterations = matrix.Size * matrix.Size;

            var stopwatch = new Stopwatch();
            time *= 1000; //zamiana sekund na milisekundy
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds <= time) // warunek zatrzymania
            {
                int [] bestNeighbour = Array.Empty<int>();
                var bestNeighbourCost = int.MaxValue;

                var bestPair = (0, 0);
                for (var i = 1; i < matrix.Size; i++)
                {
                    for (var j = i + 1; j < matrix.Size; j++)
                    {
                        var neighbourPath = currentPath.Clone() as int[];
                        neighbourFunc(neighbourPath, i, j);
                        var neighbourCost = matrix.CalculateRouteCost(neighbourPath);

                        if ((neighbourCost < bestNeighbourCost && !tabuList.ContainsKey((i, j))) ||
                            (neighbourCost < bestPathCost))
                        {
                            bestNeighbour = neighbourPath;
                            bestNeighbourCost = neighbourCost;
                            bestPair = (i, j);
                        }
                    }
                }

                try
                {
                    tabuList.Add(bestPair, maxTerm); // zmiana
                }
                catch (Exception e)
                {
                    // bo jak sie sprobuje dodac jeszcze raz te sama pare to wyjatek, ale nam to nie przeszakdza
                }

                foreach (var key in tabuList.Keys)
                {
                    tabuList[key]--;
                    if (tabuList[key] == 0)
                    {
                        tabuList.Remove(key);
                    }
                }

                if (bestNeighbourCost < bestPathCost)
                {
                    bestPath = bestNeighbour;
                    bestPathCost = bestNeighbourCost;
                    currentPath = bestNeighbour;
                    currentCost = bestNeighbourCost;
                }
                else if (bestNeighbourCost < currentCost)
                {
                    currentPath = bestNeighbour;
                    currentCost = bestNeighbourCost;
                }
                if (diversification)
                {
                    iterations++;
                    if (iterations > maxIterations)
                    {
                        iterations = 0;
                        currentPath = GenerateRandomPath(matrix, out currentCost);
                        if (currentCost < bestPathCost)
                        {
                            bestPath = currentPath;
                            bestPathCost = currentCost;
                        }
                    }
                }
            }

            return bestPath;
        }
    }
}