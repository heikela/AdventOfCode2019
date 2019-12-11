﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Common
{
    class IntCodeComputer
    {
        Dictionary<BigInteger, BigInteger> Memory;
        BigInteger PC;
        BigInteger RelativeBase;

        struct AddrLens
        {
            public Action<BigInteger> Set;
            public Func<BigInteger> Get;
        }

        public IntCodeComputer(string input)
        {
            string[] numbers = input.Split(',');
            IEnumerable<(int pos, BigInteger val)> addressValuePairs = Enumerable.Range(0, numbers.Length)
                .Zip(numbers.Select(s => BigInteger.Parse(s)));
            Memory = addressValuePairs.ToDictionary<(int pos, BigInteger val), BigInteger, BigInteger>(
                pair => pair.pos,
                pair => pair.val);
            PC = 0;
            RelativeBase = 0;
        }

        BigInteger GetMem(BigInteger addr)
        {
            if (addr < 0)
            {
                throw new Exception($"Attempted to address negative memory address {addr}");
            }
            return Memory.GetOrElse(addr, 0);
        }

        void SetMem(BigInteger addr, BigInteger val)
        {
            if (addr < 0)
            {
                throw new Exception($"Attempted to address negative memory address {addr}");
            }
            Memory.AddOrSet(addr, val);
        }

        AddrLens DecodeParam(BigInteger opcode, int param)
        {
            int pow = 100;
            for (int i = 0; i < param; ++i)
            {
                pow *= 10;
            }
            bool immediate = (opcode / pow) % 10 == 1;
            bool relative = (opcode / pow) % 10 == 2;
            BigInteger paramValue = GetMem(PC + param + 1);
            if (immediate)
            {
                return new AddrLens()
                {
                    Get = () => paramValue,
                    Set = (newVal) => throw new Exception("Cannot write to immediate mode param")
                };
            }
            else
            {
                if (relative)
                {
                    paramValue += RelativeBase;
                }
                return new AddrLens()
                {
                    Get = () => GetMem(paramValue),
                    Set = (val) => SetMem(paramValue, val)
                };
            }
        }

        void ExecuteBinOp(BigInteger opcode, Func<BigInteger, BigInteger, BigInteger> op)
        {
            DecodeParam(opcode, 2).Set(
                op(
                    DecodeParam(opcode, 0).Get(),
                    DecodeParam(opcode, 1).Get()));
        }

        public (bool running, List<BigInteger> output) RunIntCode(Queue<BigInteger> input)
        {
            List<BigInteger> output = new List<BigInteger>();
            while (true)
            {
                BigInteger opcode = GetMem(PC);
                switch ((int)(opcode % 100))
                {
                    case 1:
                        {
                            ExecuteBinOp(opcode, (a, b) => a + b);
                            PC += 4;
                            break;
                        }
                    case 2:
                        {
                            ExecuteBinOp(opcode, (a, b) => a * b);
                            PC += 4;
                            break;
                        }
                    case 3:
                        {
                            if (input.Any())
                            {
                                DecodeParam(opcode, 0).Set(input.Dequeue());
                                PC += 2;
                                break;
                            }
                            else
                            {
                                return (true, output);
                            }
                        }
                    case 4:
                        {
                            output.Add(DecodeParam(opcode, 0).Get());
                            PC += 2;
                            break;
                        }
                    case 5:
                        {
                            BigInteger val = DecodeParam(opcode, 0).Get();
                            BigInteger dest = DecodeParam(opcode, 1).Get();
                            PC += 3;
                            if (val != 0)
                            {
                                PC = dest;
                            }
                            break;
                        }
                    case 6:
                        {
                            BigInteger val = DecodeParam(opcode, 0).Get();
                            BigInteger dest = DecodeParam(opcode, 1).Get();
                            PC += 3;
                            if (val == 0)
                            {
                                PC = dest;
                            }
                            break;
                        }
                    case 7:
                        {
                            ExecuteBinOp(opcode, (a, b) => a < b ? 1 : 0);
                            PC += 4;
                            break;
                        }
                    case 8:
                        {
                            ExecuteBinOp(opcode, (a, b) => a == b ? 1 : 0);
                            PC += 4;
                            break;
                        }
                    case 9:
                        {
                            BigInteger change = DecodeParam(opcode, 0).Get();
                            RelativeBase = change + RelativeBase;
                            PC += 2;
                            break;
                        }
                    case 99:
                        return (false, output);
                    default:
                        throw new Exception($"ERROR: unknown Opcode {opcode} at address {PC}");
                }
            }
        }
    }
}
