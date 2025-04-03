using System;
using System.Diagnostics;
using MPI;

namespace MatrixRankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            using (new MPI.Environment(ref args))
            {
                try
                {
                    int rank = Communicator.world.Rank;
                    int size = Communicator.world.Size;

                    if (rank == 0)
                    {
                        Console.WriteLine("Parallel Matrix Rank Calculator");
                        Console.WriteLine($"Running on {size} MPI processes");
                    }

                    int matrixSize = 3000; 
                    double[,] matrix = null;
                    double sequentialTime = 0;
                    int sequentialRank = 0;

                    if (rank == 0)
                    {
                        Console.WriteLine($"Generating random {matrixSize}x{matrixSize} matrix...");
                        matrix = MatrixGenerator.GenerateRandomMatrix(matrixSize, matrixSize);

                        var sequentialCalculator = new SequentialRankCalculator();
                        var stopwatch = Stopwatch.StartNew();
                        sequentialRank = sequentialCalculator.CalculateRank(matrix);
                        stopwatch.Stop();
                        sequentialTime = stopwatch.ElapsedMilliseconds;
                        Console.WriteLine($"Sequential rank calculation: {sequentialRank}, Time: {sequentialTime} ms");
                    }

                    var parallelCalculator = new ParallelRankCalculator();

                    Stopwatch parallelStopwatch = null;
                    if (rank == 0)
                    {
                        parallelStopwatch = Stopwatch.StartNew();
                    }

                    int parallelRank = parallelCalculator.CalculateRank(matrix, Communicator.world);

                    if (rank == 0)
                    {
                        parallelStopwatch.Stop();
                        double parallelTime = parallelStopwatch.ElapsedMilliseconds;
                        Console.WriteLine($"Parallel rank calculation: {parallelRank}, Time: {parallelTime} ms");

                        double speedup = sequentialTime / parallelTime;
                        double efficiency = speedup / size;

                        Console.WriteLine($"Speedup: {speedup:F2}x");
                        Console.WriteLine($"Efficiency: {efficiency:F2}");

                        var performanceAnalyzer = new PerformanceAnalyzer();
                        performanceAnalyzer.AnalyzePerformance(sequentialTime, parallelTime, size, 48);

                        var faultToleranceAnalyzer = new FaultToleranceAnalyzer();
                        faultToleranceAnalyzer.AnalyzeSystemReliability(size, 8760, 1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }

                Console.ReadLine();
            }
        }
    }
}

