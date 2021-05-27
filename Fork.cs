using System;

namespace DiningPhilosophers
{
    public class Fork
    {
        public int ForkName { get; }
        private bool isUse { get; set; }

        public bool IsUse
        {
            get => isUse;

            set
            {
                if (!(value ^ isUse))
                {
                    var msg = value
                        ? $"Cannot assign fork {ForkName} when it is already assigned"
                        : $"Cannot free fork {ForkName} when it is already free";
                    throw new Exception(msg);
                }
                isUse = value;
            }
        }
        private Philosopher whichPhilosopher;

        public Philosopher WhichPhilosopher
        {
            get => whichPhilosopher;
            set
            {
                if (!((value == null) ^ (whichPhilosopher == null)))
                {
                    var msg = value == null
                        ? $"Cannot assign fork {ForkName} to no-Philosopher when it is already free"
                        : $"Cannot assign fork {ForkName} to philosopher Ph{value} while it is assigned to philosopher {whichPhilosopher}";
                    throw new Exception(msg);
                }
                whichPhilosopher = value;
            }
        }

        public Fork(int name)
        {
            ForkName = name;
            IsUse = false;
            WhichPhilosopher = null;
        }
    }
}
