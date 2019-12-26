using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Day12
{
    class Program
    {
        static void Accelerate(List<IntPoint3D> pos, List<IntPoint3D> vel)
        {
            for (int i = 0; i < pos.Count - 1; ++i)
            {
                for (int j = i + 1; j < pos.Count; ++j)
                {
                    IntPoint3D dist = pos[j] - pos[i];
                    IntPoint3D sign = dist.Sign();
                    vel[i] = vel[i] + sign;
                    vel[j] = vel[j] - sign;
                }
            }
        }

        static void Move(List<IntPoint3D> pos, List<IntPoint3D> vel)
        {
            for (int i = 0; i < pos.Count; ++i)
            {
                pos[i] = pos[i] + vel[i];
            }
        }

        static int TotalEnergy(List<IntPoint3D> pos, List<IntPoint3D> vel)
        {
            int sum = 0;
            for (int i = 0; i < pos.Count; ++i)
            {
                sum += pos[i].ManhattanDist() * vel[i].ManhattanDist();
            }
            return sum;
        }

        static void Main(string[] args)
        {
            List<IntPoint3D> pos = new List<IntPoint3D>()
            {
                new IntPoint3D() { X = 17, Y = 5, Z = 1 },
                new IntPoint3D() { X = -2, Y = -8, Z = 8 },
                new IntPoint3D() { X = 7, Y = -6, Z = 14 },
                new IntPoint3D() { X = 1, Y = -10, Z = 4 }
            };
            List<IntPoint3D> vel = pos.Select(x => new IntPoint3D()).ToList();
            int time = 0;
            PeriodDetector<(int, int, int, int, int, int, int, int)> XPeriod = new PeriodDetector<(int, int, int, int, int, int, int, int)>();
            PeriodDetector<(int, int, int, int, int, int, int, int)> YPeriod = new PeriodDetector<(int, int, int, int, int, int, int, int)>();
            PeriodDetector<(int, int, int, int, int, int, int, int)> ZPeriod = new PeriodDetector<(int, int, int, int, int, int, int, int)>();
            while (!XPeriod.PeriodFound() || !YPeriod.PeriodFound() || !ZPeriod.PeriodFound())
            {
                var xState = (pos[0].X, pos[1].X, pos[2].X, pos[3].X, vel[0].X, vel[1].X, vel[2].X, vel[3].X);
                XPeriod.DetectPeriod(xState);
                var yState = (pos[0].Y, pos[1].Y, pos[2].Y, pos[3].Y, vel[0].Y, vel[1].Y, vel[2].Y, vel[3].Y);
                YPeriod.DetectPeriod(yState);
                var zState = (pos[0].Z, pos[1].Z, pos[2].Z, pos[3].Z, vel[0].Z, vel[1].Z, vel[2].Z, vel[3].Z);
                ZPeriod.DetectPeriod(zState);
                Accelerate(pos, vel);
                Move(pos, vel);
                time++;
                if (time == 1000)
                {
                    Console.WriteLine($"Total Energy {TotalEnergy(pos, vel)}");
                }
            }
            long cycle = MathUtils.LCD(XPeriod.GetPeriod(),
                MathUtils.LCD(YPeriod.GetPeriod(), ZPeriod.GetPeriod()));
            Console.WriteLine($"The moons follow a cycle of {cycle} steps");
        }
    }
}
