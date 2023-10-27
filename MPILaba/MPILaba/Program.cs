using MPI;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace MPILaba
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var mpiClient = new MPIClient(/*Console.WriteLine*/);
            mpiClient.CalculateMatrix(ref args, 10000);
        }
    }
    public class MatrixGenerator 
    {
        private Random _rand = new Random();
        public IEnumerable<int[,]> Generate(int count, int countRows, int countColumns)
        {
            for(int i = 0; i < count; i++)
            {
                yield return Generate(countRows, countColumns);
            }
        }
        private int[,] Generate(int countRows, int countColumns)
        {
            var result = new int[countRows, countColumns];
            for(int row = 0; row < countRows; row++)
            {
                for (int column = 0; column < countColumns; column++)
                {
                    result[row, column] = _rand.Next(1,9);
                }
            }
            return result;
        }
    }


}