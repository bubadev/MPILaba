using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPILaba
{
    internal class MatrixConverter
    {
        public static int[] Multiply(int[] vector, int[,] matrix)
        {
            try
            {
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);

                if (vector.Length != rows)
                {
                    throw new Exception("The dimensionality of the vector does not correspond to the number of columns in the matrix");
                }

                int[] resultVector = new int[cols];

                for (int i = 0; i < cols; i++)
                {
                    int sum = 0;
                    for (int j = 0; j < rows; j++)
                    {
                        sum += vector[j] * matrix[j, i];
                    }
                    resultVector[i] = sum;
                }
                return resultVector;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new int[0];
            }

        }
    }
}
