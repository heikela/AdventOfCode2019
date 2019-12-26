using System;
using System.Collections.Generic;

namespace Common
{
    public class PeriodDetector<T> where T : IEquatable<T>
    {
        Dictionary<T, int> SeenStates;
        int Time;
        int? Period;

        public PeriodDetector()
        {
            SeenStates = new Dictionary<T, int>();
            Time = 0;
            Period = null;
        }

        public bool PeriodFound()
        {
            return Period.HasValue;
        }

        public bool DetectPeriod(T newValue)
        {
            if (SeenStates.ContainsKey(newValue))
            {
                if (Period.HasValue)
                {
                    int potentialPeriod = Time - SeenStates[newValue];
                    if (potentialPeriod % Period.Value != 0)
                    {
                        throw new Exception("Disagreement on period value");
                    }
                    Time++;
                    return true;
                }
                else
                {
                    Period = Time - SeenStates[newValue];
                    Time++;
                    return true;
                }
            }
            else
            {
                SeenStates[newValue] = Time++;
                return false;
            }
        }

        public int GetPeriod()
        {
            if (!Period.HasValue)
            {
                throw new Exception("Period not detected yet");
            }
            return Period.Value;
        }
    }
}
