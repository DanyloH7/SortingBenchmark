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

            // 1. JIT Warm-up (Прогрів JIT-компілятора та кешу)
            WarmUp();

            using (StreamWriter writer = new StreamWriter(csvPath))
            {
                writer.WriteLine("RunNo,ArraySize,Algorithm,ExecutionTimeMicroseconds");

                Console.WriteLine("=== Benchmark Execution & CSV Export (Mac M4) ===");
                Console.WriteLine($"{"Array Size (N)",-15} | {"Algorithm",-15} | {"Avg Time (µs)",-15} | {"Min Time (µs)",-15}");
                Console.WriteLine(new string('-', 70));

                foreach (int size in sizes)
                {
                    double[] insertionTimes = new double[iterations];
                    double[] quickTimes = new double[iterations];

                    for (int i = 0; i < iterations; i++)
                    {
                        // Використовуємо seed = 42 + i, щоб кожен прогін мав однаковий, але унікальний масив
                        Random rand = new Random(42 + i);
                        int[] baseArray = Enumerable.Range(0, size).Select(_ => rand.Next()).ToArray();

                        // --- 1. Insertion Sort ---
                        int[] arrInsert = (int[])baseArray.Clone();
                        insertionTimes[i] = MeasureSortingTime(() => InsertionSort(arrInsert), size);
                        if (!IsSorted(arrInsert)) throw new Exception("InsertionSort failed validation!");
                        writer.WriteLine($"{i + 1},{size},InsertionSort,{insertionTimes[i]:F4}");

                        // --- 2. QuickSort ---
                        int[] arrQuick = (int[])baseArray.Clone();
                        quickTimes[i] = MeasureSortingTime(() => QuickSort(arrQuick, 0, arrQuick.Length - 1), size);
                        if (!IsSorted(arrQuick)) throw new Exception("QuickSort failed validation!");
                        writer.WriteLine($"{i + 1},{size},QuickSort,{quickTimes[i]:F4}");
                    }

                    // Вивід результатів у консоль
                    Console.WriteLine($"{size,-15} | {"Insertion Sort",-15} | {insertionTimes.Average(),-15:F2} | {insertionTimes.Min(),-15:F2}");
                    Console.WriteLine($"{size,-15} | {"QuickSort",-15} | {quickTimes.Average(),-15:F2} | {quickTimes.Min(),-15:F2}");
                    Console.WriteLine(new string('-', 70));
                }
            }

            Console.WriteLine($"\n[Success] Raw data successfully exported to '{csvPath}'");
        }

        // Метод прогріву JIT
        static void WarmUp()
        {
            int[] dummy = { 5, 2, 8, 1, 9 };
            InsertionSort((int[])dummy.Clone());
            QuickSort((int[])dummy.Clone(), 0, dummy.Length - 1);
        }

        // Вимірювання часу з урахуванням малого N
        static double MeasureSortingTime(Action sortAction, int size)
        {
            // Якщо N = 10, виконуємо 1000 повторень для точності заміру
            int repeatCount = (size <= 10) ? 1000 : 1;

            Stopwatch sw = Stopwatch.StartNew();
            for (int r = 0; r < repeatCount; r++)
            {
                // Для циклу повторень потрібна копія (лише для повторів)
                sortAction();
            }
            sw.Stop();

            // Повертаємо час у мікросекундах за ОДНЕ сортування
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