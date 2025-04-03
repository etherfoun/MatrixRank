using System;

namespace MatrixRankCalculator
{
    public class PerformanceAnalyzer
    {
        public void AnalyzePerformance(double sequentialTime, double parallelTime, int nodeCount, int coresPerNode)
        {
            int totalCores = nodeCount * coresPerNode;
            double speedup = sequentialTime / parallelTime;
            double efficiency = speedup / totalCores;

            Console.WriteLine("\nPerformance Analysis:");
            Console.WriteLine($"Sequential execution time: {sequentialTime:F2} ms");
            Console.WriteLine($"Parallel execution time: {parallelTime:F2} ms");
            Console.WriteLine($"Speedup: {speedup:F2}x");
            Console.WriteLine($"Efficiency: {efficiency:F2}");

            AnalyzeScalability(speedup, totalCores);
        }

        private void AnalyzeScalability(double currentSpeedup, int currentCores)
        {
            Console.WriteLine("\nScalability Analysis:");

            double strongScalingEfficiency = currentSpeedup / currentCores;
            Console.WriteLine($"Strong scaling efficiency: {strongScalingEfficiency:P2}");

            Console.WriteLine("\nPredicted speedup for different core counts:");

            double parallelFraction = EstimateParallelFraction(currentSpeedup, currentCores);
            Console.WriteLine($"Estimated parallel fraction: {parallelFraction:F4}");

            int[] coreCounts = { currentCores * 2, currentCores * 4, currentCores * 8, currentCores * 16 };
            foreach (int cores in coreCounts)
            {
                double predictedSpeedup = 1 / ((1 - parallelFraction) + (parallelFraction / cores));
                Console.WriteLine($"  {cores} cores: {predictedSpeedup:F2}x speedup");
            }
        }

        private double EstimateParallelFraction(double speedup, int cores)
        {
            // Reverse Amdahl's Law to estimate parallel fraction
            // 1/S = (1-p) + p/N
            // p = (N/S - 1)/(N - 1)

            double p = (cores / speedup - 1) / (cores - 1);

            return Math.Max(0.5, Math.Min(0.99, p));
        }
    }
}

