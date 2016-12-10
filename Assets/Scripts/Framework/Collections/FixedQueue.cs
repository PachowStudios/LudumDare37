using System.Collections;
using System.Collections.Generic;

namespace PachowStudios.Framework.Collections
{
  /// <summary>
  /// A fixed-size queue that dequeues the first item once the size is reached.
  /// </summary>
  public class FixedQueue<T> : IEnumerable<T>
  {
    private readonly Queue<T> queue;

    public int Size { get; }

    public FixedQueue(int size)
    {
      this.queue = new Queue<T>(size);
      Size = size;
    }

    public void Enqueue(T item)
    {
      if (this.queue.Count == Size)
        this.queue.Dequeue();

      this.queue.Enqueue(item);
    }

    public IEnumerator<T> GetEnumerator()
      => this.queue.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();
  }
}
