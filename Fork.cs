using System;

namespace DiningPhilosophers
{
    public class Fork
    {
        public int ForkName { get; }
        private bool _isUse;

        public bool IsUse
        {
            get => _isUse;

            set
            {
                if (!(value != _isUse))
                {
                    var msg = value
                        ? $"Cannot assign fork {ForkName} when it is already assigned"
                        : $"Cannot free fork {ForkName} when it is already free";
                    throw new Exception(msg);
                }
                _isUse = value;
            }
        }
        private Philosopher _whichPhilosopher;

        public Philosopher WhichPhilosopher
        {
            get => _whichPhilosopher;
            set
            {
                if (!(value == null != (_whichPhilosopher == null)))
                {
                    var msg = value == null
                        ? $"Cannot assign fork {ForkName} to no-Philosopher when it is already free"
                        : $"Cannot assign fork {ForkName} to philosopher Ph{value} while it is assigned to philosopher {_whichPhilosopher}";
                    throw new Exception(msg);
                }
                _whichPhilosopher = value;
            }
        }

        public Fork(int name)
        {
            ForkName = name;
            _isUse = false;
            _whichPhilosopher = null;
        }
    }
}
