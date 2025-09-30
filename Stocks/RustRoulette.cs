using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamba
{
    
    public class RustRoulette
    {
        public const int ONE = 1;
        public const int THREE = 3;
        public const int FIVE = 5;
        public const int TEN = 10;
        public const int TWENTY = 20;
        private static Random _random = new Random();
        private int[] _roulette = new int[25];
        public int Multiplier { get; set; }
        public delegate void RoulettePayoutHandler();
        private bool _isRunning { get; set; } = false;
        private RoulettePayoutHandler? _roulettePayoutHandlers;
 
        // 20:1, 10:2, 5:4, 3:6, 1:12
        public RustRoulette()
        {
            for (int i = 0; i < 25; i++)
            {
                if (i == 0) _roulette[i] = TWENTY;
                if (i >= 1 && i < 3) _roulette[i] = TEN;
                if (i >= 3 && i < 7) _roulette[i] = FIVE;
                if (i >= 7 && i < 13) _roulette[i] = THREE;
                if (i >= 13 && i < 25) _roulette[i] = ONE;
            }
            Shuffle();
        }

        public async Task Start()
        {
            _isRunning = true;
            while(_isRunning)
            {
                await SpinRoulette();
                _roulettePayoutHandlers?.Invoke();
                await RoulettePrep();
            }
        }
        public void Stop()
        {
            _isRunning = false;
        }

        public void Shuffle()
        {
            for (int i = _roulette.Length - 1; i > 0; i--)
            {
                int j = _random.Next(0, i + 1);
                int temp_i = _roulette[i];
                _roulette[i] = _roulette[j];
                _roulette[j] = _roulette[i];
            }
        }

        public async Task SpinRoulette()
        {
            int secondsPassed = 0;
            while (secondsPassed < 5)
            {
                Console.WriteLine($"Spinning wheel: {++secondsPassed}");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            Shuffle();
            Multiplier = _random.Next(_roulette.Length);
        }

        public async Task RoulettePrep()
        {

            int secondsLeft = 5;
            while (secondsLeft > 0)
            {
                Console.WriteLine($"{secondsLeft--} seconds left until spinning the wheel.");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public void RegisterPlayer(RoulettePayoutHandler roulettePayout)
        {
            _roulettePayoutHandlers += roulettePayout;
        }

    }
}
