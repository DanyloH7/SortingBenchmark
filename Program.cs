using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SortingBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] sizes = { 10, 100, 1000, 10000 };
            int iterations = 10;
            string csvPath = "results.csv";

            // Data distribution scenarios to investigate
            string[] distributions = { "Random", "Sorted", "ReverseSorted" };

            // 1. JIT Warm-up to eliminate compilation overhead on first runs
            WarmUp();

            using (StreamWriter writer = new StreamWriter(csvPath))
            {
                // CSV header including the data distribution column
                writer.WriteLine("RunNo,ArraySize,Distribution,Algorithm,ExecutionTimeMicroseconds");

                Console.WriteLine("=== Benchmark Execution & CSV Export (Data Distributions) ===");
                Console.WriteLine($"{"Size (N)",-10} | {"Distribution",-15} | {"Algorithm",-15} | {"Avg Time (µs)",-15}");
                Console.WriteLine(new string('-', 65));

                foreach (int size in sizes)
                {
                    foreach (string dist in distributions)
                    {
                        double[] insertionTimes = new double[iterations];
                        double[] quickTimes = new double[iterations];

                        for (int i = 0; i < iterations; i++)
                        {
                            // Generate base array according to the current distribution type
                            int[] baseArray = GenerateArray(size, dist, i);

                            // --- 1. Insertion Sort ---
                            // Validate correctness on a separate check clone
                            int[] checkInsert = (int[])baseArray.Clone();
                            InsertionSort(checkInsert);
                            if (!IsSorted(checkInsert)) throw new Exception("InsertionSort failed validation!");

                            // Measure time with fresh cloning per repetition (fixes the instructor's feedback)
                            insertionTimes[i] = MeasureSortingTime(baseArray, arr => InsertionSort(arr), size);
                            writer.WriteLine($"{i + 1},{size},{dist},InsertionSort,{insertionTimes[i]:F4}");

                            // --- 2. QuickSort ---
                            // Validate correctness on a separate check clone
                            int[] checkQuick = (int[])baseArray.Clone();
                            QuickSort(checkQuick, 0, checkQuick.Length - 1);
                            if (!IsSorted(checkQuick)) throw new Exception("QuickSort failed validation!");

                            // Measure time with fresh cloning per repetition
                            quickTimes[i] = MeasureSortingTime(baseArray, arr => QuickSort(arr, 0, arr.Length - 1), size);
                            writer.WriteLine($"{i + 1},{size},{dist},QuickSort,{quickTimes[i]:F4}");
                        }

                        // Output aggregated results to the console
                        Console.WriteLine($"{size,-10} | {dist,-15} | {"Insertion Sort",-15} | {insertionTimes.Average(),-15:F2}");
                        Console.WriteLine($"{size,-10} | {dist,-15} | {"QuickSort",-15} | {quickTimes.Average(),-15:F2}");
                        Console.WriteLine(new string('-', 65));
                    }
                }
            }

            Console.WriteLine($"\n[Success] New results with distributions exported to '{csvPath}'");
        }

        // Generates an array based on the requested distribution type
        static int[] GenerateArray(int size, string distribution, int seedOffset)
        {
            Random rand = new Random(42 + seedOffset);
            int[] array = Enumerable.Range(0, size).Select(_ => rand.Next()).ToArray();

            if (distribution == "Sorted")
            {
                Array.Sort(array);
            }
            else if (distribution == "ReverseSorted")
            {
                Array.Sort(array);
                Array.Reverse(array);
            }
            // If "Random", leave the array unsorted

            return array;
        }

        // Triggers JIT compilation and CPU instruction caching beforehand
        static void WarmUp()
        {
            int[] dummy = { 5, 2, 8, 1, 9 };
            InsertionSort((int[])dummy.Clone());
            QuickSort((int[])dummy.Clone(), 0, dummy.Length - 1);
        }

        // Measures sorting duration with high precision; scales iterations for small N and clones fresh per repeat
        static double MeasureSortingTime(int[] baseArray, Action<int[]> sortAction, int size)
        {
            int repeatCount = (size <= 10) ? 1000 : 1;

            Stopwatch sw = Stopwatch.StartNew();
            for (int r = 0; r < repeatCount; r++)
            {
                // CRITICAL FIX: Fresh clone on each repeat iteration so data distribution remains intact
                int[] workArray = (int[])baseArray.Clone();
                sortAction(workArray);
            }
            sw.Stop();

            double totalMicroseconds = (sw.Elapsed.TotalMilliseconds * 1000.0);
            return totalMicroseconds / repeatCount;
        }

        static void InsertionSort(int[] array)
        {
            for (int i = 1; i < array.Length; i++)
            {
                int key = array[i];
                int j = i - 1;
                while (j >= 0 && array[j] > key)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                array[j + 1] = key;
            }
        }

        static void QuickSort(int[] array, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(array, low, high);
                QuickSort(array, low, pivotIndex - 1);
                QuickSort(array, pivotIndex + 1, high);
            }
        }

        static int Partition(int[] array, int low, int high)
        {
            int pivot = array[high];
            int i = low - 1;
            for (int j = low; j < high; j++)
            {
                if (array[j] < pivot)
                {
                    i++;
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
            int swap = array[i + 1];
            array[i + 1] = array[high];
            array[high] = swap;
            return i + 1;
        }

        // Verifies correctness of the sorting algorithm
        static bool IsSorted(int[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i] > array[i + 1]) return false;
            }
            return true;
        }
    }
}