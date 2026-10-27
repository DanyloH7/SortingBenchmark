# Sorting Algorithm Performance Benchmark on Apple Silicon M4

An empirical research study evaluating the runtime characteristics of **Insertion Sort** and **QuickSort** under diverse data distributions on Apple Silicon (M4, .NET 9).

---

## 📌 Research Overview & Motivation

While theoretical computer science dictates the asymptotic complexity of algorithms ($O(n \log n)$ vs $O(n^2)$), real-world performance depends heavily on platform architecture, runtime optimizations (JIT compilation), and input data structures.

This experiment investigates:
1. **JIT & Measurement Precision:** Eliminating JIT compilation overhead via explicit warm-up routines and applying microsecond-accurate loop scaling for small array sizes ($N = 10$).
2. **Data Distribution Impact:** Comparing performance across **Random**, **Already Sorted**, and **Reverse-Sorted** arrays to validate theoretical best- and worst-case scenarios on modern hardware.

---

## 🛠️ Environment & Methodology

- **Hardware:** Apple Silicon M4
- **Runtime:** .NET 9.0 (macOS arm64)
- **Measurement Tool:** `System.Diagnostics.Stopwatch` ($\mu s$ microsecond scale)
- **Tested Array Sizes ($N$):** $10$, $100$, $1,000$, $10,000$
- **Iterations:** 10 independent runs per scenario (with deterministic seeds for reproducibility)

### Key Methodological Features
- **JIT Warm-up:** Dummy sorting passes are executed prior to benchmarking to force JIT compilation and populate CPU instruction caches.
- **Small $N$ Loop Scaling:** For $N = 10$, execution is scaled over 1,000 repetitions per run to eliminate timing quantization errors ($0.0000\,\mu s$).

---

## 📊 Benchmark Results

Below is the aggregated average execution time ($\mu s$) across different data distributions:

| Size ($N$) | Data Distribution | Insertion Sort Avg ($\mu s$) | QuickSort Avg ($\mu s$) | Key Observation |
|---|---|---|---|---|
| **10** | Random | `0.02` | `0.20` | Insertion Sort faster (no recursion overhead) |
| **10** | Sorted | `0.02` | `0.18` | Both algorithms highly optimal |
| **10** | ReverseSorted | `0.02` | `0.19` | Negligible impact at micro-scale |
| **100** | Random | `5.09` | `4.86` | Transition point between $O(n^2)$ and $O(n \log n)$ |
| **100** | Sorted | `0.23` | `14.97` | Insertion Sort hits $O(n)$ best-case |
| **100** | ReverseSorted | `8.44` | `14.52` | QuickSort partitioning overhead |
| **1,000** | Random | `415.61` | `79.98` | QuickSort ~5.2x faster |
| **1,000** | Sorted | `2.21` | `1,496.34` | QuickSort hits worst-case $O(n^2)$ |
| **1,000** | ReverseSorted | `813.04` | `1,131.33` | Both degrade under unfavorable order |
| **10,000**| Random | `39,665.91` | `895.38` | QuickSort ~44.3x faster |
| **10,000**| Sorted | `19.98` | `137,331.88` | **Insertion Sort is ~6,873x faster than QuickSort!** |
| **10,000**| ReverseSorted | `76,644.42` | `114,488.36` | Heavy degradation for both algorithms |

---

## 💡 Key Findings & Theoretical Insights

1. **Insertion Sort Best-Case Efficiency:**  
   When input data is **already sorted**, Insertion Sort achieves linear runtime $O(n)$, executing $N = 10,000$ elements in just **$19.98\,\mu s$**.
2. **QuickSort Worst-Case Vulnerability:**  
   With fixed end-element pivot selection, sorted/reverse-sorted inputs cause unbalanced partitions, degrading QuickSort to $O(n^2)$ complexity (**$137.33\,\text{ms}$** for $N = 10,000$).
3. **Small $N$ Overhead:**  
   For small inputs ($N \le 100$), simpler algorithms like Insertion Sort outperform QuickSort due to lower call stack and recursion overhead.

---

## 🚀 How to Run

1. Clone the repository:
   ```bash
   git clone https://github.com/DanyloH7/SortingBenchmark.git
   cd SortingBenchmark
   ```
   Execute the benchmark:
   ```bash
dotnet run
```
2.Raw microsecond measurements will be written to results.csv.