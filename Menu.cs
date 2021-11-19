using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PEA_Project2
{
    public class Menu
    {
        private const string Welcome =
            "\n 1. Wczytanie danych z pliku \n 2. Wprowadzenie kryterium stopu \n 3. Włączenie/Wyłączenie dywersyfikacji \n 4. Wybór sąsiedztwa \n 5. Algorytm symulowanego wyżarzania \n 6. Algorytm tabu search \n 7. Exit \n";

        private static string path;
        private static IList<int> _bestPath;
        private static int timeToStop;
        private static bool diversification;
        private static Matrix _matrix;
        private static Action<int[], int, int> neighbourFunc = Algorithms.Swap;

        public static void MenuText()
        {
            while (true)
            {
                Console.WriteLine(_matrix == null ? "Brak pliku" : "Wczytano plik");

                Console.WriteLine($"Dywersyfikacja: {diversification}");

                if (neighbourFunc == Algorithms.Reverse)
                {
                    Console.WriteLine("Sąsiedztwo: Reverse");
                }
                else
                {
                    Console.WriteLine("Sąsiedztwo: Swap");
                }
                
                Console.WriteLine($"Czas wykonywania algorytmu: {timeToStop}s");

                Console.WriteLine(Menu.Welcome);

                var answer = Console.ReadLine();
                const string choice = "\nWybór: ";
                switch (answer)
                {
                    case "1":
                        Console.WriteLine("\nPodaj ścieżkę pliku \n");
                        path = Console.ReadLine();
                        try
                        {
                            _matrix = new Matrix(path);
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message + "\n");
                        }

                        break;

                    case "2":
                        Console.WriteLine(choice + "Wprowadz kryterium stopu [s]\n");
                        timeToStop = int.Parse(Console.ReadLine() ?? string.Empty);
                        break;

                    case "3":
                        Console.WriteLine("Przełączono tryb dywersyfikacji");
                        diversification = !diversification;
                        break;
                    case "4":
                        Console.WriteLine("\n4. Wybór sąsiedztwa \n");
                        Console.WriteLine("\n1. Reverse \n 2. Swap \n");
                        switch (Console.ReadLine())
                        {
                            case "1":
                                neighbourFunc = Algorithms.Reverse;
                                break;
                            case  "2":
                                neighbourFunc = Algorithms.Swap;
                                break;
                            default:
                                Console.WriteLine("Niepoprawny wybór sąsiedztwa");
                                    break;
                            
                        }
                        break;
                    case "5":
                        Console.WriteLine("\n5. Algorytm symulowanego wyżarzania\n");
                        _bestPath = Algorithms.SimulatedAnnealing(_matrix, timeToStop, neighbourFunc);
                        Console.WriteLine(_matrix.CalculateRouteCost(_bestPath));
                        break;
                    case "6":
                        Console.WriteLine("\n6. Algorytm tabu search\n");
                        _bestPath = Algorithms.TabuSearch(_matrix, timeToStop, neighbourFunc, diversification);
                        Console.WriteLine(_matrix.CalculateRouteCost(_bestPath));
                        break;

                    case "7":
                    {
                        Environment.Exit(0);
                        break;
                    }

                    default:
                        Console.WriteLine("Wrong choice!\n");
                        continue;
                }
            }
        }

        private static void RunAlgorithm(Func<int[,], IList<int>> algorithm, int[,] graph)
        {
            // Stopwatch sw;
            // sw = Stopwatch.StartNew();
            // bestPath = algorithm(graph);
            // sw.Stop();
            // var elapsedTime = sw.ElapsedMilliseconds;
            // Console.WriteLine($"Elapsed time: {elapsedTime / 1000.0} s");
            // Console.WriteLine("Best path:");
            // Algorithms.WriteList(bestPath);
            // Console.WriteLine($"Path cost: {Algorithms.bestPathCost}");
        }

        

        private static double TimeMeasure(Func<int[,], IList<int>> algorithm, int[,] graph)
        {
            Stopwatch sw;
            var nanosecondsPerTick = 1000000000L / Stopwatch.Frequency;
            sw = Stopwatch.StartNew();
            _bestPath = algorithm(graph);
            sw.Stop();
            return sw.ElapsedTicks * nanosecondsPerTick;
        }

        private static void AutomaticTest()
        {
            var size = 10;
            var bruteForceResults = new List<double>();
            var dynamicProgrammingResults = new List<double>();
            int[,] graph;
            while (size < 21)
            {
                double bfTime = 0;
                double dpTime = 0;

                var bfAverageTime = bfTime / 100000000.0;
                var dpAverageTime = dpTime / 100000000.0;
                bruteForceResults.Add(bfAverageTime);
                dynamicProgrammingResults.Add(dpAverageTime);
                Console.WriteLine($"Brute Force for size {size}: {bfAverageTime}");
                Console.WriteLine(
                    $"Dynamic Programming for size {size}: {dpAverageTime}");
                size++;
            }

            using (var file = new StreamWriter("wyniki.csv"))
            {
                foreach (var result in bruteForceResults)
                {
                    file.Write($"{result};");
                }

                file.WriteLine();
                foreach (var result in dynamicProgrammingResults)
                {
                    file.Write($"{result};");
                }
            }
        }

        private static Boolean Warning(string p)
        {
            if (p != null)
            {
                Console.WriteLine("Nie wczytano pliku!");

                return false;
            }
            return true;
        }
    }
}