using System.Collections.Generic;
using System;

public class PriorityQueue<T>
{
    private List<Node> _queue = new List<Node>();
    private int _heapSize = -1;
    private bool _isMinPriorityQueue;
    public int Count { get { return _queue.Count; } }
    
    public PriorityQueue(bool isMinPriorityQueue = false)
    {
        _isMinPriorityQueue = isMinPriorityQueue;
    }

    public void Enqueue(int priority, T obj)
    {
        Node node = new Node() { priority = priority, item = obj };
        _queue.Add(node);
        _heapSize++;
        //Maintaining heap
        if (_isMinPriorityQueue)
            BuildHeapMin(_heapSize);
        else
            BuildHeapMax(_heapSize);
    }

    public T Dequeue()
    {
        if (_heapSize > -1)
        {
            var returnVal = _queue[0].item;
            _queue[0] = _queue[_heapSize];
            _queue.RemoveAt(_heapSize);
            _heapSize--;
            //Maintaining lowest or highest at root based on min or max queue
            if (_isMinPriorityQueue)
                MinHeapify(0);
            else
                MaxHeapify(0);
            return returnVal;
        }
        else
        {
            throw new Exception("Queue is empty");
        }
    }

    public void UpdatePriority(T obj, int priority)
    {
        int i = 0;
        for (; i <= _heapSize; i++)
        {
            Node node = _queue[i];
            if (object.ReferenceEquals(node.item, obj))
            {
                node.priority = priority;
                if (_isMinPriorityQueue)
                {
                    BuildHeapMin(i);
                    MinHeapify(i);
                }
                else
                {
                    BuildHeapMax(i);
                    MaxHeapify(i);
                }
            }
        }
    }

    public bool IsInQueue(T obj)
    {
        foreach (Node node in _queue)
            if (object.ReferenceEquals(node.item, obj))
                return true;
        return false;
    }

    private void BuildHeapMax(int i)
    {
        while (i >= 0 && _queue[(i - 1) / 2].priority < _queue[i].priority)
        {
            Swap(i, (i - 1) / 2);
            i = (i - 1) / 2;
        }
    }
    private void BuildHeapMin(int i)
    {
        while (i >= 0 && _queue[(i - 1) / 2].priority > _queue[i].priority)
        {
            Swap(i, (i - 1) / 2);
            i = (i - 1) / 2;
        }
    }
    private void MaxHeapify(int i)
    {
        int left = ChildIndexLeft(i);
        int right = ChildIndexRight(i);

        int highest = i;

        if (left <= _heapSize && _queue[highest].priority < _queue[left].priority)
            highest = left;
        if (right <= _heapSize && _queue[highest].priority < _queue[right].priority)
            highest = right;

        if(highest != i)
        {
            Swap(highest, i);
            MaxHeapify(highest);
        }
    }
    private void MinHeapify(int i)
    {
        int left = ChildIndexLeft(i);
        int right = ChildIndexRight(i);

        int lowest = i;

        if (left <= _heapSize && _queue[lowest].priority > _queue[left].priority)
            lowest = left;
        if (right <= _heapSize && _queue[lowest].priority > _queue[right].priority)
            lowest = right;

        if (lowest != i)
        {
            Swap(lowest, i);
            MinHeapify(lowest);
        }
    }

    private void Swap(int i, int j)
    {

    }

    private int ChildIndexLeft(int i)
    {
        return i * 2 + 1;
    }
    private int ChildIndexRight(int i)
    {
        return i * 2 + 2;
    }
    private int ParentIndex(int i)
    {
        if (i == 0) { return 0; }
        return i / 2;
    }



    

    private class Node
    {
        public int priority;
        public T item;
    }
}
