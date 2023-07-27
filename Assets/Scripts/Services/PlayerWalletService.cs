using System;
using TestTask.Configuration;

namespace TestTask.Services
{
    internal class PlayerWalletService
    {
        public event Action OnChange;

        public int Points { get; private set; }

        public PlayerWalletService(int points)
        {
            this.Points = points;
        }

        public void Earn(int points)
        {
            Points += points;
            OnChange?.Invoke();
        }

        public bool CanTake(SkillNodeConfig nodeConfig) => Points - nodeConfig.Cost >= 0;

        public void Take(SkillNodeConfig nodeConfig)
        {
            if (!CanTake(nodeConfig))
            {
                throw new InvalidOperationException();
            }

            Points -= nodeConfig.Cost;
            OnChange?.Invoke();
        }

        public void Receive(SkillNodeConfig nodeConfig)
        {
            Points += nodeConfig.Cost;
            OnChange?.Invoke();
        }
    }
}
