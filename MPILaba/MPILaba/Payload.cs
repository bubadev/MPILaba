using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPILaba
{
    [Serializable]
    internal class Payload
    {
        public int[] Vector { get; set; }
        public int[,] Matrix { get; set; }
    }
}
