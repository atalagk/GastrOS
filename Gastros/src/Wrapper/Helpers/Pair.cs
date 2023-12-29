namespace GastrOs.Wrapper.Helpers
{
    public struct Pair<T1, T2>
    {
        private T1 first;
        private T2 second;

        public Pair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }

        public T1 First
        {
            get
            {
                return first;
            }
            set
            {
                first = value;
            }
        }

        public T2 Second
        {
            get
            {
                return second;
            }
            set
            {
                second = value;
            }
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Pair<T1, T2>)
            {
                Pair<T1, T2> other = (Pair<T1, T2>) obj;
                return First.Equals(other.First) && Second.Equals(other.Second);
            }
            return false;
        }
    }
}
