using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class PeakRearQueue<T>
    {
        LinkedList<T> list;

        public PeakRearQueue()
        {
            list = new LinkedList<T>();
        }

        public bool isEmpty()
        {
            return list.First == null;
        }

        public void Enqueue(T value)
        {
            list.AddLast(value);
        }

        public T Dequeue()
        {
            T value = list.First.Value;
            list.RemoveFirst();

            return value;
        }

        public T PeakFront()
        {
            return list.First.Value;
        }

        public T PeakRear()
        {
            return list.Last.Value;
        }

    }
}
