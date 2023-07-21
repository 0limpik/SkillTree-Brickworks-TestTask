using System;

namespace Assets.Scripts.Model
{
    internal class SelfInvokeEvent<T>
    {
        public event Action<T> OnChange
        {
            add
            {
                onChange += value;
                value(GetValue());
            }
            remove => onChange -= value;
        }
        private event Action<T> onChange;

        private readonly Func<T> GetValue;

        public SelfInvokeEvent(Func<T> GetValue)
        {
            this.GetValue = GetValue;
        }

        public void Invoke() => onChange?.Invoke(GetValue());
    }
}
