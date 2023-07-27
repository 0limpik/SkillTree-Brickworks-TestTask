using TestTask.Services;
using TMPro;
using UnityEngine;

namespace TestTask.UI
{
    internal class SkillInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private TMP_Text costText;

        private SkillSelector skillSelector;
        private SkillCostService skillCost;
        private PlayerWalletService wallet;

        public void Construct(
            SkillSelector skillSelector,
            SkillCostService skillCost, 
            PlayerWalletService wallet)
        {
            this.skillSelector = skillSelector;
            this.skillCost = skillCost;
            this.wallet = wallet;
        }

        public void Subscribe()
        {
            skillSelector.OnSelect += SetCost;
            wallet.OnChange += SetPoints;
            SetPoints();
        }

        public void Unscribe()
        {
            skillSelector.OnSelect -= SetCost;
            wallet.OnChange -= SetPoints;
        }

        void Start() => SetCost(null);

        private void SetPoints()
            => pointsText.text = $"{wallet.Points}";

        private void SetCost(SkillNodeUI node)
        {
            if (node == null)
                costText.text = "-";
            else
                costText.text = $"{skillCost.GetTreeConfig(node.Config).Cost}";
        }
    }
}
