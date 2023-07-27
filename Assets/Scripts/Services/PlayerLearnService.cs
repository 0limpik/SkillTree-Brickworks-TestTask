using System;
using TestTask.Configuration;

namespace TestTask.Services
{
    internal class PlayerLearnService
    {
        public event Action OnSkillChange;

        private readonly PlayerWalletService wallet;
        private readonly SkillLearnService skillLearn;
        private readonly SkillCostService skillCost;

        public PlayerLearnService(
            PlayerWalletService wallet,
            SkillLearnService skillLearn,
            SkillCostService skillCost)
        {
            this.wallet = wallet;
            this.skillLearn = skillLearn;
            this.skillCost = skillCost;
        }

        public void Subscribe()
        {
            wallet.OnChange += PointsChange;
            skillLearn.OnLearn += SkillChange;
            skillLearn.OnForget += SkillChange;
        }

        public void Unscribe()
        {
            wallet.OnChange -= PointsChange;
            skillLearn.OnLearn -= SkillChange;
            skillLearn.OnForget -= SkillChange;
        }

        public bool CanLearn(SkillConfig config)
        {
            var nodeConfig = skillCost.GetTreeConfig(config);
            return CanLearn(config, nodeConfig);
        }

        public void Learn(SkillConfig config)
        {
            var nodeConfig = skillCost.GetTreeConfig(config);
            if (!CanLearn(config, nodeConfig))
            {
                throw new InvalidOperationException();
            }
            skillLearn.Learn(config);
            wallet.Take(nodeConfig);
        }

        public bool CanForget(SkillConfig config) => skillLearn.CanForget(config);

        public void Forget(SkillConfig config)
        {
            if (!CanForget(config))
            {
                throw new InvalidOperationException();
            }

            skillLearn.Forget(config);
            var nodeConfig = skillCost.GetTreeConfig(config);
            wallet.Receive(nodeConfig);
        }

        public void ForgetAll()
        {
            foreach (var skill in skillLearn.ForgetAll())
            {
                var nodeConfig = skillCost.GetTreeConfig(skill);
                wallet.Receive(nodeConfig);
            }
        }

        private bool CanLearn(SkillConfig config, SkillNodeConfig nodeConfig)
            => wallet.CanTake(nodeConfig) && skillLearn.CanLearn(config);

        private void SkillChange(SkillConfig config) => OnSkillChange?.Invoke();
        private void PointsChange() => OnSkillChange?.Invoke();
    }
}
