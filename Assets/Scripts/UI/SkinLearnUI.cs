using Assets.Scripts.Configuration;
using Assets.Scripts.Model;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    internal class SkinLearnUI : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private int startPoints;
        [SerializeField] private int earnPoints;

        [Header("Links")]
        [SerializeField] private SkillInfoUI skillInfo;
        [SerializeField] private SkillLinksSelector linksSelector;

        [SerializeField] private Button earn;
        [SerializeField] private Button learn;
        [SerializeField] private Button forget;
        [SerializeField] private Button forgetAll;

        private SkillSelector skillSelector;
        private SkillLearnService skillLearn;

        private PlayerWalletService playerWallet;
        private PlayerLearnService playerLearn;

        private SkillConfig SelectedConfig => skillSelector.Selected?.Config;

        public void Construct(SkillCostService skillCost, SkillSelector skillSelector, SkillLearnService skillLearn)
        {
            this.skillSelector = skillSelector;
            this.skillLearn = skillLearn;

            playerWallet = new PlayerWalletService(startPoints);
            playerLearn = new PlayerLearnService(playerWallet, skillLearn, skillCost);

            skillInfo.Construct(skillSelector, skillCost, playerWallet);
            linksSelector.Construct(skillSelector, skillLearn);
        }

        public void Subscribe()
        {
            skillSelector.OnSelect += Select;
            playerLearn.OnSkillChange += UpdateButtons;
            playerLearn.Subscribe();

            skillInfo.Subscribe();
            linksSelector.Subscribe();
        }

        public void Unscribe()
        {
            skillSelector.OnSelect -= Select;
            playerLearn.OnSkillChange -= UpdateButtons;
            playerLearn.Unscribe();

            skillInfo.Unscribe();
            linksSelector.Unscribe();
        }

        void OnEnable()
        {
            earn.onClick.AddListener(OnEarn);
            learn.onClick.AddListener(OnLearn);
            forget.onClick.AddListener(OnForget);
            forgetAll.onClick.AddListener(OnForgetAll);
        }

        void OnDisable()
        {
            earn.onClick.RemoveListener(OnEarn);
            learn.onClick.RemoveListener(OnLearn);
            forget.onClick.RemoveListener(OnForget);
            forgetAll.onClick.RemoveListener(OnForgetAll);
        }

        public void AddTree(SkillTree tree) => skillLearn.AddTree(tree);
        public void RemoveTree(SkillTree tree) => skillLearn.RemoveTree(tree);
        private void Select(SkillNodeUI selectedNode) => UpdateButtons();
        private void OnEarn() => playerWallet.Earn(earnPoints);
        private void OnLearn() => playerLearn.Learn(SelectedConfig);
        private void OnForget() => playerLearn.Forget(SelectedConfig);
        private void OnForgetAll() => playerLearn.ForgetAll();

        private void UpdateButtons()
        {
            var notSelected = SelectedConfig != null;

            learn.interactable = notSelected && playerLearn.CanLearn(SelectedConfig);
            forget.interactable = notSelected && playerLearn.CanForget(SelectedConfig);
        }
    }
}
