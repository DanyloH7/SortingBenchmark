using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        int[] sizes = { 10, 100, 1000, 10000 };
        int iterations = 10;
        Random rand = new Random(42);

        string csvPath = "results.csv";
        using (StreamWriter swCsv = new StreamWriter(csvPath))
        {
            // Записуємо заголовки CSV
            swCsv.WriteLine("RunID,ArraySize,Algorithm,ExecutionTimeMicroseconds");

            Console.WriteLine("=== Benchmark Execution & CSV Export (Mac M4) ===");
            Console.WriteLine($"{"Array Size (N)",-15} | {"Algorithm",-15} | {"Avg Time (µs)",-15} | {"Min Time (µs)",-15}");
            Console.WriteLine(new string('-', 65));

            foreach (int size in sizes)
            {
                int[] baseArray = Enumerable.Range(0, size).Select(_ => rand.Next(0, 100000)).ToArray();

                double[] insertionTimes = new double[iterations];
                double[] quickTimes = new double[iterations];

                // 1. Тестуємо Insertion Sort
                for (int i = 0; i < iterations; i++)
                {
                    int[] arr = (int[])baseArray.Clone();
                    
                    Stopwatch sw = Stopwatch.StartNew();
                    InsertionSort(arr);
                    sw.Stop();

                    // Використовуємо мікросекунди для точності при малих N
                    double elapsedMicroseconds = sw.Elapsed.TotalMicroseconds;
                    insertionTimes[i] = elapsedMicroseconds;

                    // Зберігаємо сирі дані у CSV
                    swCsv.WriteLine($"{i + 1},{size},InsertionSort,{elapsedMicroseconds:F4}");

                    // Перевірка коректності сортування
                    if (!IsSorted(arr))
                    {
                        Console.WriteLine($"[ERROR] InsertionSort failed to sort array of size {size}!");
                    }
                }

                // 2. Тестуємо QuickSort
                for (int i = 0; i < iterations; i++)
                {
                    int[] arr = (int[])baseArray.Clone();
                    
                    Stopwatch sw = Stopwatch.StartNew();
                    QuickSort(arr, 0, arr.Length - 1);
                    sw.Stop();

                    double elapsedMicroseconds = sw.Elapsed.TotalMicroseconds;
                    quickTimes[i] = elapsedMicroseconds;

                    // Зберігаємо сирі дані у CSV
                    swCsv.WriteLine($"{i + 1},{size},QuickSort,{elapsedMicroseconds:F4}");

                    // Перевірка коректності сортування
                    if (!IsSorted(arr))
                    {
                        Console.WriteLine($"[ERROR] QuickSort failed to sort array of size {size}!");
                    }
                }

                Console.WriteLine($"{size,-15} | {"Insertion Sort",-15} | {insertionTimes.Average(),-15:F2} | {insertionTimes.Min(),-15:F2}");
                Console.WriteLine($"{size,-15} | {"QuickSort",-15} | {quickTimes.Average(),-15:F2} | {quickTimes.Min(),-15:F2}");
                Console.WriteLine(new string('-', 65));
            }
        }

        Console.WriteLine($"\n[Success] Raw data successfully exported to '{csvPath}'");
    }

    static bool IsSorted(int[] arr)
    {
        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] < arr[i - 1]) return false;
        }
        return true;
    }

    static void InsertionSort(int[] arr)
    {
        for (int i = 1; i < arr.Length; i++)
        {
            int key = arr[i];
            int j = i - 1;
            while (j >= 0 && arr[j] > key)
            {
                arr[j + 1] = arr[j];
                j--;
            }
            arr[j + 1] = key;
        }
    }

    static void QuickSort(int[] arr, int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(arr, low, high);
            QuickSort(arr, low, pi - 1);
            QuickSort(arr, pi + 1, high);
        }
    }

    static int Partition(int[] arr, int low, int high)
    {
        int pivot = arr[high];
        int i = (low - 1);
        for (int j = low; j < high; j++)
        {
            if (arr[j] < pivot)
            {
                i++;
                int temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
        }
        int temp1 = arr[i + 1];
        arr[i + 1] = arr[high];
        arr[high] = temp1;
        return i + 1;
    }
}