using System;
using System.Diagnostics;

namespace MatrixRankCalculator
{
    public class SequentialRankCalculator
    {
        public TimeSpan LastExecutionTime { get; private set; }

        public int CalculateRank(double[,] matrix)
        {
            var stopwatch = Stopwatch.StartNew();

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] matrixCopy = new double[rows, cols];
            Array.Copy(matrix, matrixCopy, matrix.Length);

            int rank = PerformGaussianElimination(matrixCopy, rows, cols);

            stopwatch.Stop();
            LastExecutionTime = stopwatch.Elapsed;

            return rank;
        }

        private int PerformGaussianElimination(double[,] matrix, int rows, int cols)
        {
            const double EPSILON = 1e-10;
            int rank = 0;
            bool[] rowUsed = new bool[rows];

            for (int col = 0; col < cols; col++)
            {
                int pivotRow = -1;
                double maxValue = EPSILON;

                for (int row = 0; row < rows; row++)
                {
                    if (!rowUsed[row] && Math.Abs(matrix[row, col]) > maxValue)
                    {
                        maxValue = Math.Abs(matrix[row, col]);
                        pivotRow = row;
                    }
                }

                if (pivotRow == -1)
                {
                    continue;
                }

                rowUsed[pivotRow] = true;
                rank++;

                double pivotValue = matrix[pivotRow, col];
                for (int j = col; j < cols; j++)
                {
                    matrix[pivotRow, j] /= pivotValue;
                }

                for (int row = 0; row < rows; row++)
                {
                    if (row != pivotRow && Math.Abs(matrix[row, col]) > EPSILON)
                    {
                        double factor = matrix[row, col];
                        for (int j = col; j < cols; j++)
                        {
                            matrix[row, j] -= factor * matrix[pivotRow, j];
                        }
                    }
                }
            }

            return rank;
        }
    }
}

