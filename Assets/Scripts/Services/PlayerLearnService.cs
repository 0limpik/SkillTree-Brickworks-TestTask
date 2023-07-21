using System;
using Assets.Scripts.Configuration;

namespace Assets.Scripts.Services
{
    internal class PlayerLearnService
    {
        public event Action OnLearnStateChange;

        private readonly PlayerWallet wallet;
        private readonly SkillLearnService learnService;
        private readonly SkillCostService skillCostService;

        public PlayerLearnService(PlayerWallet wallet, SkillLearnService learnService, SkillCostService skillCostService)
        {
            this.wallet = wallet;
            this.learnService = learnService;
            this.skillCostService = skillCostService;
        }

        public void Subscribe()
        {
            wallet.OnChange += PointsChange;
            learnService.SkillChanged += SkillChange;
        }

        public void Unscribe()
        {
            wallet.OnChange -= PointsChange;
            learnService.SkillChanged -= SkillChange;
        }

        public bool CanLearn(SkillConfig config)
        {
            var nodeConfig = skillCostService.GetTreeConfig(config);
            return CanLearn(config, nodeConfig);
        }

        public void Learn(SkillConfig config)
        {
            var nodeConfig = skillCostService.GetTreeConfig(config);
            if (!CanLearn(config, nodeConfig))
            {
                throw new InvalidOperationException();
            }

            learnService.Learn(config);
            wallet.Learn(nodeConfig);
        }

        public bool CanForget(SkillConfig config) => learnService.CanForget(config);

        public void Forget(SkillConfig config) => learnService.Forget(config);

        private bool CanLearn(SkillConfig config, SkillNodeConfig nodeConfig)
            => wallet.CanLearn(nodeConfig) && learnService.CanLearn(config);

        private void SkillChange(SkillConfig config) => OnLearnStateChange?.Invoke();
        private void PointsChange(int points) => OnLearnStateChange?.Invoke();
    }
}
