using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiningPhilosophers
{
	public class Philosophers : List<Philosopher>
	{
		private readonly int philosopherCount = 5;
		private readonly int forkCount = 5;

		public Philosophers InitializePhilosophers()
		{
			// Добавляем вилки потому что каждому философу нужно по 2 вилки

			var forks = new List<Fork>();
			Enumerable.Range(0, forkCount).ToList().ForEach(name => forks.Add(new Fork(name)));

			// Инициализируем философов
			// Philosopher[i] необходима
			//		Fork[(i - 1) % 5] его вилка слева
			//		Fork[i] и вилка справа
			//
			int LeftForkName(int phName) => (forkCount + phName - 1) % forkCount;
			int RightForkName(int phName) => phName;

			Enumerable.Range(0, philosopherCount).ToList().ForEach(name =>
				Add(new Philosopher(name, forks[LeftForkName(name)], forks[RightForkName(name)], this)));

			return this;
		}
	}
}
