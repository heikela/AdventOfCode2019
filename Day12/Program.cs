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
            //int maxT = 10;
            bool Xrepeated = false;
            bool Yrepeated = false;
            bool Zrepeated = false;
            int Xcycle = 0;
            int Ycycle = 0;
            int Zcycle = 0;
            Dictionary<(int, int, int, int, int, int, int, int), int> timeByX = new Dictionary<(int, int, int, int, int, int, int, int), int>();
            Dictionary<(int, int, int, int, int, int, int, int), int> timeByY = new Dictionary<(int, int, int, int, int, int, int, int), int>();
            Dictionary<(int, int, int, int, int, int, int, int), int> timeByZ = new Dictionary<(int, int, int, int, int, int, int, int), int>();
            while (!Xrepeated || !Yrepeated || !Zrepeated)
            {
                if (!Xrepeated) {
                    var xState = (pos[0].X, pos[1].X, pos[2].X, pos[3].X, vel[0].X, vel[1].X, vel[2].X, vel[3].X);
                    if (timeByX.ContainsKey(xState))
                    {
                        int prevTime = timeByX[xState];
                        int delta = time - prevTime;
                        Console.WriteLine($"Judging by coordinate X, at Time {time} we have a state similar to time {prevTime}, {delta} steps earlier.");
                        Xrepeated = true;
                        Xcycle = delta;
                    }
                    else
                    {
                        timeByX.Add(xState, time);
                    }
                }
                if (!Yrepeated)
                {
                    var yState = (pos[0].Y, pos[1].Y, pos[2].Y, pos[3].Y, vel[0].Y, vel[1].Y, vel[2].Y, vel[3].Y);
                    if (timeByY.ContainsKey(yState))
                    {
                        int prevTime = timeByY[yState];
                        int delta = time - prevTime;
                        Console.WriteLine($"Judging by coordinate Y, at Time {time} we have a state similar to time {prevTime}, {delta} steps earlier.");
                        Yrepeated = true;
                        Ycycle = delta;
                    }
                    else
                    {
                        timeByY.Add(yState, time);
                    }
                }
                if (!Zrepeated) {
                    var zState = (pos[0].Z, pos[1].Z, pos[2].Z, pos[3].Z, vel[0].Z, vel[1].Z, vel[2].Z, vel[3].Z);
                    if (timeByZ.ContainsKey(zState))
                    {
                        int prevTime = timeByZ[zState];
                        int delta = time - prevTime;
                        Console.WriteLine($"Judging by coordinate Z, at Time {time} we have a state similar to time {prevTime}, {delta} steps earlier.");
                        Zrepeated = true;
                        Zcycle = delta;
                    }
                    else
                    {
                        timeByZ.Add(zState, time);
                    }
                }
                Accelerate(pos, vel);
                Move(pos, vel);
                time++;
                if (time == 1000)
                {
                    Console.WriteLine($"Total Energy {TotalEnergy(pos, vel)}");
                }
            }
            Console.WriteLine($"The moons follow a cycle of {MathUtils.LCD(Xcycle, MathUtils.LCD(Ycycle, Zcycle))}");
        }
    }
}
