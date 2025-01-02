using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace CSharpBencchmark
{
    /*

    . net 8
    | Method         | Mean     | Error     | StdDev    |
    |--------------- |---------:|----------:|----------:|
    | test_delegate  | 4.932 ns | 0.0384 ns | 0.0359 ns |
    | test_funcptr   | 4.272 ns | 0.0241 ns | 0.0189 ns |
    | test_interface | 4.059 ns | 0.0610 ns | 0.0571 ns |

    .net 9
    | Method         | Mean     | Error     | StdDev    |
    |--------------- |---------:|----------:|----------:|
    | test_delegate  | 4.230 ns | 0.0682 ns | 0.0638 ns |
    | test_funcptr   | 3.685 ns | 0.0263 ns | 0.0233 ns |
    | test_interface | 3.647 ns | 0.0120 ns | 0.0107 ns |

    */
    public unsafe class Bench_func_ptr
    {
        Func<float> _delegate;
        delegate*<float> _funcptr;
        ifrand _rand;

        static float frand() => Random.Shared.NextSingle();

        public Bench_func_ptr()
        {
            _delegate = frand;
            _funcptr = &frand;
            _rand = new frand();
        }

        [Benchmark]
        public void test_delegate()
        {
            var r = _delegate();
        }

        [Benchmark]
        public void test_funcptr()
        {
            var r = _funcptr();
        }

        [Benchmark]
        public void test_interface()
        {
            var r = _rand.rand();
        }
    }

    interface ifrand
    {
        public float rand();
    }

    class frand : ifrand
    {
        public float rand()
        {
            return Random.Shared.NextSingle();
        }
    }
}
