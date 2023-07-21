using System;
using Assets.Scripts.Configuration;
using Assets.Scripts.Model;

namespace Assets.Scripts.Services
{
    internal class PlayerWallet
    {
        public event Action<int> OnChange
        {
            add => onChange.OnChange += value;
            remove => onChange.OnChange -= value;
        }
        private SelfInvokeEvent<int> onChange;

        private int points;

        public PlayerWallet(int points)
        {
            this.points = points;
            onChange = new(() => this.points);
        }

        public void Earn(int points)
        {
            this.points += points;
            onChange.Invoke();
        }

        public bool CanLearn(SkillNodeConfig config) => points - config.Cost >= 0;

        public void Learn(SkillNodeConfig config)
        {
            if (!CanLearn(config))
            {
                throw new InvalidOperationException();
            }

            points -= config.Cost;
            onChange.Invoke();
        }
    }
}
