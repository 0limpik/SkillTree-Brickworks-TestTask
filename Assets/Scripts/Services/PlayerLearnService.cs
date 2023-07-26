using System;
using Assets.Scripts.Configuration;
using Unity.VisualScripting.FullSerializer;

namespace Assets.Scripts.Services
{
    internal class PlayerLearnService
    {
        public event Action OnSkillChange;

        private readonly PlayerWalletService wallet;
        private readonly SkillLearnService learnService;
        private readonly SkillCostService skillCost;

        public PlayerLearnService(PlayerWalletService wallet, SkillLearnService learnService, SkillCostService skillCost)
        {
            this.wallet = wallet;
            this.learnService = learnService;
            this.skillCost = skillCost;
        }

        public void Subscribe()
        {
            wallet.OnChange += PointsChange;
            learnService.OnLearn += SkillChange;
            learnService.OnForget += SkillChange;
        }

        public void Unscribe()
        {
            wallet.OnChange -= PointsChange;
            learnService.OnLearn -= SkillChange;
            learnService.OnForget -= SkillChange;
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
            learnService.Learn(config);
            wallet.Take(nodeConfig);
        }

        public bool CanForget(SkillConfig config) => learnService.CanForget(config);

        public void Forget(SkillConfig config)
        {
            if (!CanForget(config))
            {
                throw new InvalidOperationException();
            }

            learnService.Forget(config);
            var nodeConfig = skillCost.GetTreeConfig(config);
            wallet.Receive(nodeConfig);
        }

        public void ForgetAll()
        {
            foreach(var skill in learnService.ForgetAll())
            {
                var nodeConfig = skillCost.GetTreeConfig(skill);
                wallet.Receive(nodeConfig);
            }
        }

        private bool CanLearn(SkillConfig config, SkillNodeConfig nodeConfig)
            => wallet.CanTake(nodeConfig) && learnService.CanLearn(config);

        private void SkillChange(SkillConfig config) => OnSkillChange?.Invoke();
        private void PointsChange() => OnSkillChange?.Invoke();
    }
}
