using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

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
        private static Stopwatch stopwatch = new Stopwatch();

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
                        Console.WriteLine("\n1. Reverse\n2. Swap \n");
                        switch (Console.ReadLine())
                        {
                            case "1":
                                neighbourFunc = Algorithms.Reverse;
                                break;
                            case "2":
                                neighbourFunc = Algorithms.Swap;
                                break;
                            default:
                                Console.WriteLine("Niepoprawny wybór sąsiedztwa");
                                break;
                        }

                        break;
                    case "5":
                        Console.WriteLine("\n5. Algorytm symulowanego wyżarzania\n");
                        var timesThread = new Thread(() =>
                        {
                            const int n = 6;
                            for (var i = 0; i < n; i++)
                            {
                                Thread.Sleep(timeToStop * 1000 / n);
                                Console.WriteLine(
                                    $"{stopwatch.ElapsedMilliseconds / 1000.0}s: {_matrix.CalculateRouteCost(Algorithms.bestPath)}");
                            }
                        });
                        timesThread.Start();
                        stopwatch.Restart();
                        _bestPath = Algorithms.SimulatedAnnealing(_matrix, timeToStop, neighbourFunc);
                        stopwatch.Stop();
                        timesThread.Join();
                        Console.WriteLine(_matrix.CalculateRouteCost(_bestPath));
                        break;
                    case "6":
                        Console.WriteLine("\n6. Algorytm tabu search\n");
                        var timesThread2 = new Thread(() =>
                        {
                            const int n = 10;
                            for (var i = 0; i < n; i++)
                            {
                                Thread.Sleep(timeToStop * 1000 / n);
                                Console.WriteLine(
                                    $"{stopwatch.ElapsedMilliseconds / 1000.0}s: {_matrix.CalculateRouteCost(Algorithms.bestPath)}");
                            }
                        });
                        timesThread2.Start();
                        stopwatch.Restart();
                        _bestPath = Algorithms.TabuSearch(_matrix, timeToStop, neighbourFunc, diversification);
                        stopwatch.Stop();
                        timesThread2.Join();
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
    }
}