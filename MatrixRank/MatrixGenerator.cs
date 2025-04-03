using System;

namespace MatrixRankCalculator
{
    public static class MatrixGenerator
    {
        private static Random random = new Random(42); 

        public static double[,] GenerateRandomMatrix(int rows, int cols)
        {
            double[,] matrix = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = random.NextDouble() * 20 - 10;
                }
            }

            return matrix;
        }

        public static double[,] GenerateMatrixWithKnownRank(int size, int rank)
        {
            if (rank > size)
            {
                throw new ArgumentException("Rank cannot be greater than matrix size");
            }

            double[,] matrix = new double[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = 0;
                }
            }

            for (int i = 0; i < rank; i++)
            {
                matrix[i, i] = 1;
            }

            ShuffleMatrix(matrix, size);

            return matrix;
        }

        private static void ShuffleMatrix(double[,] matrix, int size)
        {
            for (int i = 0; i < size * 2; i++)
            {
                int row1 = random.Next(size);
                int row2 = random.Next(size);

                if (row1 != row2)
                {
                    double multiplier = random.NextDouble() * 2 - 1;
                    for (int j = 0; j < size; j++)
                    {
                        matrix[row1, j] += multiplier * matrix[row2, j];
                    }
                }
            }
        }
    }
}

