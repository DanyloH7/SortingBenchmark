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

            // Типи розподілу даних для дослідження
            string[] distributions = { "Random", "Sorted", "ReverseSorted" };

            // 1. JIT Warm-up
            WarmUp();

            using (StreamWriter writer = new StreamWriter(csvPath))
            {
                // Оновлюємо заголовок CSV, додаючи колонку розподілу даних
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
                            // Генеруємо базовий масив відповідно до типу розподілу
                            int[] baseArray = GenerateArray(size, dist, i);

                            // --- 1. Insertion Sort ---
                            int[] arrInsert = (int[])baseArray.Clone();
                            insertionTimes[i] = MeasureSortingTime(() => InsertionSort(arrInsert), size);
                            if (!IsSorted(arrInsert)) throw new Exception("InsertionSort failed validation!");
                            writer.WriteLine($"{i + 1},{size},{dist},InsertionSort,{insertionTimes[i]:F4}");

                            // --- 2. QuickSort ---
                            int[] arrQuick = (int[])baseArray.Clone();
                            quickTimes[i] = MeasureSortingTime(() => QuickSort(arrQuick, 0, arrQuick.Length - 1), size);
                            if (!IsSorted(arrQuick)) throw new Exception("QuickSort failed validation!");
                            writer.WriteLine($"{i + 1},{size},{dist},QuickSort,{quickTimes[i]:F4}");
                        }

                        // Вивід результатів у консоль
                        Console.WriteLine($"{size,-10} | {dist,-15} | {"Insertion Sort",-15} | {insertionTimes.Average(),-15:F2}");
                        Console.WriteLine($"{size,-10} | {dist,-15} | {"QuickSort",-15} | {quickTimes.Average(),-15:F2}");
                        Console.WriteLine(new string('-', 65));
                    }
                }
            }

            Console.WriteLine($"\n[Success] New results with distributions exported to '{csvPath}'");
        }

        // Генератор масивів залежно від типу розподілу
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
            // Якщо "Random", залишаємо як є

            return array;
        }

        static void WarmUp()
        {
            int[] dummy = { 5, 2, 8, 1, 9 };
            InsertionSort((int[])dummy.Clone());
            QuickSort((int[])dummy.Clone(), 0, dummy.Length - 1);
        }

        static double MeasureSortingTime(Action sortAction, int size)
        {
            int repeatCount = (size <= 10) ? 1000 : 1;

            Stopwatch sw = Stopwatch.StartNew();
            for (int r = 0; r < repeatCount; r++)
            {
                sortAction();
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