using Assets.Scripts.Services;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.View
{
    internal class SkillInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private TMP_Text costText;

        private SkillSelectorView skillSelector;
        private SkillCostService skillCost;
        private PlayerWallet playerWallet;

        public void Set(SkillSelectorView skillSelector, SkillCostService skillCost, PlayerWallet playerWallet)
        {
            this.skillSelector = skillSelector;
            this.skillCost = skillCost;
            this.playerWallet = playerWallet;

            skillSelector.OnSelect += SetCost;
            playerWallet.OnChange += SetPoints;
        }

        private void SetPoints(int points)
            => pointsText.text = $"{points}";

        private void SetCost(SkillNodeUI node)
            => costText.text = $"{skillCost.GetTreeConfig(node.Config).Cost}";
    }
}
