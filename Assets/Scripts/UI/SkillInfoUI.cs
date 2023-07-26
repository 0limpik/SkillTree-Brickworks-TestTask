using Assets.Scripts.Services;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    internal class SkillInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private TMP_Text costText;

        private SkillSelector skillSelector;
        private SkillCostService skillCost;
        private PlayerWalletService playerWallet;

        public void Construct(SkillSelector skillSelector, SkillCostService skillCost, PlayerWalletService playerWallet)
        {
            this.skillSelector = skillSelector;
            this.skillCost = skillCost;
            this.playerWallet = playerWallet;
        }

        public void Subscribe()
        {
            skillSelector.OnSelect += SetCost;
            playerWallet.OnChange += SetPoints;
        }

        public void Unscribe()
        {
            skillSelector.OnSelect -= SetCost;
            playerWallet.OnChange -= SetPoints;
        }

        void Start() => SetCost(null);

        private void SetPoints(int points)
            => pointsText.text = $"{points}";

        private void SetCost(SkillNodeUI node)
        {
            if (node == null)
                costText.text = "Unselected";
            else
                costText.text = $"{skillCost.GetTreeConfig(node.Config).Cost}";
        }
    }
}
