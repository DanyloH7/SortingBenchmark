# Sorting Algorithm Benchmark (Insertion Sort vs QuickSort)

Preliminary empirical performance analysis conducted on Apple Silicon (M4 Mac) using .NET 9 (C#).

## Prerequisites
- .NET 9 SDK (or later) installed on macOS, Linux, or Windows.
- Git (optional, for cloning).

## How to Run the Experiment
1. Clone or download this repository.
2. Open a terminal inside the project directory (`SortingBenchmark`).
3. Execute the benchmark using the .NET CLI:
   ```bash
   dotnet run
   ```
 The program will execute 10 iterations for dataset sizes N = 10, 100, 1000, 10000, verify sorting correctness, print aggregate results, and generate a raw data file named results.csv.

## Experimental Setup & Verification
- Input Generation: Arrays are populated using a seeded Random instance (seed = 42) to ensure reproducibility. Both Insertion Sort and QuickSort receive identical clones of each generated base array.

- Correctness: Each sorting operation is programmatically validated using an IsSorted assertion check to prevent false metrics from corrupted data.

- Timing Mechanism: Measured via high-resolution System.Diagnostics.Stopwatch tracking execution time in microseconds to prevent precision loss on small inputs (N = 10).