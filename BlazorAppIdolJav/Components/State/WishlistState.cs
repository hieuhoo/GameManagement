namespace GameManagement.Components.State
{
    public class WishlistState
    {
        public int Count { get; private set; }

        public event Action? OnChange;

        public void SetCount(int count)
        {
            Count = count;
            OnChange?.Invoke();
        }

        public void Increase()
        {
            Count++;
            OnChange?.Invoke();
        }

        public void Decrease()
        {
            if (Count > 0)
                Count--;

            OnChange?.Invoke();
        }
    }
}
