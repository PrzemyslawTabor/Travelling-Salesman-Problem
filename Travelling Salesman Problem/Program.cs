using System;

namespace Travelling_Salesman_Problem
{
    class Program
    {
        static void Main(string[] args)
        {
            TSPData data = new TSPData(40);
            TournamentSelection tournament = new TournamentSelection(data);
        }
    }
}
