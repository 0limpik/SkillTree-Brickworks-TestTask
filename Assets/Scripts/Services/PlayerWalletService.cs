using System;
using Assets.Scripts.Configuration;
using Assets.Scripts.Model;

namespace Assets.Scripts.Services
{
    internal class PlayerWalletService
    {
        public event Action<int> OnChange
        {
            add => onChange.OnChange += value;
            remove => onChange.OnChange -= value;
        }
        private readonly SelfInvokeEvent<int> onChange;

        private int points;

        public PlayerWalletService(int points)
        {
            this.points = points;
            onChange = new(() => this.points);
        }

        public void Earn(int points)
        {
            this.points += points;
            onChange.Invoke();
        }

        public bool CanTake(SkillNodeConfig nodeConfig) => points - nodeConfig.Cost >= 0;

        public void Take(SkillNodeConfig nodeConfig)
        {
            if (!CanTake(nodeConfig))
            {
                throw new InvalidOperationException();
            }

            points -= nodeConfig.Cost;
            onChange.Invoke();
        }

        public void Receive(SkillNodeConfig nodeConfig)
        {
            points += nodeConfig.Cost;
            onChange.Invoke();
        }
    }
}
