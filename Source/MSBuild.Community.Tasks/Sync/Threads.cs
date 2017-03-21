using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace johnshope.Sync {

	public class Task {
		public bool IsFinished { get; private set; }
		public ManualResetEvent Event = new ManualResetEvent(false);
		public Task(Action body) { Body = body; IsFinished = false; }
		public Action Body { get; set; }
		public void Await() {
			Event.WaitOne();
		}
		public void Run() {
			try { Body(); } catch { }
			if (Finished != null) Finished(this, EventArgs.Empty);
			IsFinished = true;
			Event.Set();
		}
		public event EventHandler Finished;
	}

	public class Threads {
		
		public int N;
		List<List<Thread>> threads = new List<List<Thread>>();
		List<ResourceQueue<Task>> items = new List<ResourceQueue<Task>>();
		ResourceQueue<Task> alltasks = new ResourceQueue<Task>();

		void Schedule(object state) {
			int level = (int)state;
			do {
				Task t = items[level].DequeueOrBlock();
				if (t != null) t.Run();
			} while (true);
		}

		void Create(int level) {
			if (level >= threads.Count) {
				threads.Add(new List<Thread>());
				items.Add(new ResourceQueue<Task>());
			}
			var t = new Thread(Schedule);
			threads[level].Add(t);
			t.Name = "Private Thread Pool [" + level + ", " + (threads[level].Count - 1) + "]";
			t.Start(level);
		}

		static Dictionary<Thread, int> level = new Dictionary<Thread,int>();
		static int Level {
			get {
				var t = Thread.CurrentThread;
				if (level.ContainsKey(t)) return level[t];
				else level.Add(t, -1);
				return -1;
			}
			set {
				var t = Thread.CurrentThread;
				if (level.ContainsKey(t)) level[t] = value;
				else level.Add(t, value);
			}
		}

		public Task Do(Task t) {
			alltasks.Enqueue(t);
			int level = 0;
			var direct = false;
			lock (items) {
				if (items.Count <= level) Create(level);
				direct = items[level].Count >= N;
			}
			if (direct) t.Run();
			else {
				lock (items) {
					items[level].Enqueue(t);
					if (items[level].Count > threads[level].Count) Create(level);
				} 
			}
			return t;
		}

		public Task DoAsync(Action a) {
			var t = new Task(a);
			alltasks.Enqueue(t);
			int level = 1;
			lock (items) {
				if (items.Count <= level) Create(level);
				items[level].Enqueue(t);
				while (items[level].Count > threads[level].Count) Create(level);
			}
			return t;
		}

		public void Await() {
			var t = alltasks.Dequeue();
			while (t != null) {
				if (!t.IsFinished) t.Await();
				t = alltasks.Dequeue();
			}
		}

		public void Abort() {
			foreach (var level in threads) {
				foreach (var t in level) t.Abort();
			}
		}

		public void Do(Action a) {
			var t = new Task(a);
			Do(t);
		}
	}
}
