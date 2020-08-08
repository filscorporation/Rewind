using System;

namespace Source
{
    [Serializable]
    public class LimitedStack<T>
    {
        private T[] array;
        private int size;
        private int position;

        private int lastPosition => position == 0 ? MaxSize - 1 : position - 1;

        public int Count => size;
        
        private int MaxSize { get; }
        
        public LimitedStack(int maxsize)
        {
            MaxSize = maxsize;
            array = new T[MaxSize];
            size = 0;
            position = 0;
        }
        
        public void Clear()
        {
            Array.Clear(array, 0, MaxSize);
            size = 0;
            position = 0;
        }

        public T Peek()
        {
            return array[lastPosition];
        }

        public T Pop()
        {
            T item = array[lastPosition];
            array[lastPosition] = default(T);
            size--;
            position--;
            if (position < 0)
                position = MaxSize - 1;
            return item;
        }

        public void Push(T item)
        {
            array[position] = item;
            position++;
            if (position == MaxSize)
                position = 0;
            if (size < MaxSize)
                size++;
        }
    }
}