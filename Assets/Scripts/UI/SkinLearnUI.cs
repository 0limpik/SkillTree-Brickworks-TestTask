using Assets.Scripts.Model;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    internal class SkinLearnUI : MonoBehaviour
    {
        [Header("Links")]
        [SerializeField] private SkillInfoUI skillInfo;

        [SerializeField] private Button earn;
        [SerializeField] private Button learn;
        [SerializeField] private Button forget;

        [Header("Setup")]
        [SerializeField] private int startPoints;
        [SerializeField] private int earnPoints;

        private PlayerWalletService playerWallet;
        private SkillLearnService skillLearn;
        private PlayerLearnService playerLearn;
        private SkillSelector skillSelector;

        private SkillNodeUI selectedNode;

        public void Construct(SkillCostService skillCost, SkillSelector skillSelector)
        {
            this.skillSelector = skillSelector;

            playerWallet = new PlayerWalletService(startPoints);
            skillLearn = new SkillLearnService();
            playerLearn = new PlayerLearnService(playerWallet, skillLearn, skillCost);

            skillInfo.Construct(skillSelector, skillCost, playerWallet);
        }

        public void Subscribe()
        {
            skillSelector.OnSelect += Select;
            playerLearn.OnSkillChange += UpdateButtons;
            playerLearn.Subscribe();
            skillInfo.Subscribe();
        }

        public void Unscribe()
        {
            skillSelector.OnSelect -= Select;
            playerLearn.OnSkillChange -= UpdateButtons;
            playerLearn.Unscribe();
            skillInfo.Unscribe();
        }

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

        public void AddTree(SkillTree tree) => skillLearn.AddTree(tree);
        public void RemoveTree(SkillTree tree) => skillLearn.RemoveTree(tree);

        private void Select(SkillNodeUI selectedNode)
        {
            this.selectedNode = selectedNode;
            UpdateButtons();
        }

        private void OnEarn() => playerWallet.Earn(earnPoints);

        private void OnLearn()
        {
            playerLearn.Learn(selectedNode.Config);
            selectedNode.Learn();
        }

        private void OnForget()
        {
            playerLearn.Forget(selectedNode.Config);
            selectedNode.Forget();
        }

        private void UpdateButtons()
        {
            var notSelected = selectedNode != null;

            learn.interactable = notSelected && playerLearn.CanLearn(selectedNode.Config);
            forget.interactable = notSelected && playerLearn.CanForget(selectedNode.Config);
        }
    }
}
