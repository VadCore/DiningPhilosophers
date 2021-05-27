using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiningPhilosophers
{
    public class Philosopher
    {
        static readonly SemaphoreSlim GetApprovedToEat = new SemaphoreSlim(2);
        private static readonly object SyncObject = new object();
        public int Name { get; set; }
        private Fork LeftFork { get; set; }
        private Fork RightFork { get; set; }
        private PhilosopherStatus Status { get; set; } = PhilosopherStatus.Thinking;
        public int EatCount { get; private set; }
        public int TotalEatingTime { get; private set; }
        public int EatingConflictCount { get; private set; }

        // Спросить о других обедающих философах
        private readonly Philosophers _allPhilosophers;

        private readonly Random rand;
        private readonly int maxThinkDuration = 800;
        private readonly int minThinkDuration = 50;

        public Philosopher(int name, Fork leftFork, Fork rightFork, Philosophers allPhilosophers)
        {
            Name = name;
            LeftFork = leftFork;
            RightFork = rightFork;
            _allPhilosophers = allPhilosophers;

            rand = new Random(Name);
        }

        public void EatingHabit(CancellationToken stopEat)
        {
            int timeBeforeRequstEatPermission = 20;

            for (int i = 0; ; ++i)
            {
                if (stopEat.IsCancellationRequested)
                {
                    Console.WriteLine($"Philosopher{Name} is asked to stop the dining");
                    stopEat.ThrowIfCancellationRequested();
                }

                try
                {
                    // Wait for eating permission.
                    GetApprovedToEat.WaitAsync().Wait();
                    Console.WriteLine($"Philosopher Ph{Name} will attempt to eat. Attempt: {i}.");

                    bool isOkToEat;
                    lock (SyncObject)
                    {
                        // Check for Fork availability
                        isOkToEat = IsForksAvailable();
                        if (isOkToEat)
                            GetForks();
                    }

                    if (isOkToEat)
                    {
                        PhilosopherEat();
                        ReleaseForks();
                    }
                }

                finally
                {
                    GetApprovedToEat.Release();
                }

                Task.Delay(timeBeforeRequstEatPermission).Wait();
            }
        }

        private bool IsForksAvailable()
        {
            lock (SyncObject)
            {
                if (LeftFork.IsUse)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.ffff} Philosopher Ph{Name} cannot eat " +
                                      $"cuz his Left Fork, F{LeftFork.ForkName}, is in use (by philosopher Ph{LeftFork.WhichPhilosopher.Name})");
                    return false;
                }

                if (RightFork.IsUse)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.ffff} Philosopher Ph{Name} cannot eat " +
                                      $"cuz his right Fork, F{RightFork.ForkName}, is in use (by philosopher Ph{RightFork.WhichPhilosopher.Name})");
                    return false;
                }
            }

            return true;
        }

        private IEnumerable<Philosopher> EatingPhilosphers()
        {
            lock (SyncObject)
                return _allPhilosophers.Where(p => p.Status == PhilosopherStatus.Eating);
        }

        private void PhilosopherEat()
        {
            var eatingDuration = rand.Next(maxThinkDuration) + minThinkDuration;

            var eatingPhilosophers = EatingPhilosphers().Select(p => p.Name).ToList();
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.ffff} Philosopher Ph{Name} is eating.  " +
                              $"There are: {eatingPhilosophers.Count} " +
                              $"({string.Join(", ", eatingPhilosophers.Select(p => $"Ph{p}"))}) eating philosophers.");

            Thread.Sleep(eatingDuration);

            Console.WriteLine($"{DateTime.Now:HH:mm:ss.ffff} Philosopher Ph{Name} is thinking after eating for " +
                $"{eatingDuration} milliseconds.");


            ++EatCount;
            TotalEatingTime += eatingDuration;


        }

        private void GetForks() // получаем вилки
        {
            lock (SyncObject)
            {
                LeftFork.IsUse = true;
                LeftFork.WhichPhilosopher = this;
                RightFork.IsUse = true;
                RightFork.WhichPhilosopher = this;

                Status = PhilosopherStatus.Eating;
                Console.WriteLine($"Philosopher Ph{Name} get forks: " +
                    $"(F{LeftFork.ForkName}, F{RightFork.ForkName}).");
            }
        }

        private void ReleaseForks() // Освобождаем вилки
        {
            lock (SyncObject)
            {
                LeftFork.IsUse = false;
                LeftFork.WhichPhilosopher = null;
                RightFork.IsUse = false;
                RightFork.WhichPhilosopher = null;

                Status = PhilosopherStatus.Thinking;
                Console.WriteLine($"{DateTime.Now:HH:mm:ss.ffff} Philosopher Ph{Name} released forks: (F{LeftFork.ForkName}, F{RightFork.ForkName}).");
            }
        }
    }
}
