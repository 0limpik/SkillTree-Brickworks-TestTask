using System.Linq;
using Assets.Scripts.Configuration;
using Assets.Scripts.Model;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.View
{
    internal class SkinLearnUI : MonoBehaviour
    {
        [SerializeField] private SkillInfoUI skillInfo;

        [SerializeField] private Button earn;
        [SerializeField] private Button learn;
        [SerializeField] private Button forget;

        [SerializeField] private int startPoints;
        [SerializeField] private int earnPoints;
        private PlayerWallet playerWallet;
        private PlayerLearnService learnService;

        private SkillConfig selectedConfig;

        void OnEnable()
        {
            earn.onClick.AddListener(OnEarn);
            learn.onClick.AddListener(OnLearn);
            forget.onClick.AddListener(OnForget);
        }

        void OnDisable()
        {
            earn.onClick.RemoveListener(OnEarn);
            learn.onClick.RemoveListener(OnLearn);
            forget.onClick.RemoveListener(OnForget);
        }

        public void Set(SkillTree tree, SkillCostService skillCostService, SkillSelectorView skillSelector)
        {
            playerWallet = new PlayerWallet(startPoints);

            var learnService = new SkillLearnService(tree);

            var rootSkills = tree.Nodes
                .Where(x => x.Necessary.Count() == 0)
                .Select(x => x.Config);

            learnService.AddRootSkill(rootSkills);

            skillInfo.Set(skillSelector, skillCostService, playerWallet);
            var skillCostSevice = new SkillCostService();

            this.learnService = new PlayerLearnService(playerWallet, learnService, skillCostService);
            this.learnService.OnLearnStateChange += UpdateButtons;
            this.learnService.Subscribe();
        }

        public void Select(SkillConfig config)
        {
            this.selectedConfig = config;
            UpdateButtons();
        }

        private void OnEarn() => playerWallet.Earn(earnPoints);

        private void OnLearn()
        {
            learnService.Learn(selectedConfig);
        }

        private void OnForget()
        {
            learnService.Forget(selectedConfig);
        }

        private void UpdateButtons()
        {
            var notSelected = selectedConfig != null;

            learn.interactable = notSelected && learnService.CanLearn(selectedConfig);
            forget.interactable = notSelected && learnService.CanForget(selectedConfig);
        }
    }
}
