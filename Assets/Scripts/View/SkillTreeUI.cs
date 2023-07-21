using Assets.Scripts.Configuration;
using Assets.Scripts.Model;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.View
{
    [RequireComponent(typeof(SkillNodesUI))]
    [RequireComponent(typeof(SkillLinksUI))]
    internal class SkillTreeUI : MonoBehaviour
    {
        [SerializeField] private SkinLearnUI learnUI;
        private SkillNodesUI nodes;
        private SkillLinksUI links;

        [SerializeField] private SkillTreeConfig treeConfig;

        private SkillSelectorView skillSelector = new();

        void Awake()
        {
            nodes = this.GetComponent<SkillNodesUI>();
            links = this.GetComponent<SkillLinksUI>();

            Construct();
        }

        void Start() => CreateTree();
        void OnDestroy() => ClearTree();

        public void Construct()
        {
            nodes.Construct(skillSelector);
        }

        [ContextMenu(nameof(CreateTree))]
        public void CreateTree()
        {
            var tree = new SkillTree();
            tree.Fill(treeConfig);
            var cost = new SkillCostService();
            cost.AddTree(treeConfig);

            var skillCost = new SkillCostService();
            skillCost.AddTree(treeConfig);

            learnUI.Set(tree, skillCost, skillSelector);

            skillSelector.OnSelect += SelectNode;

            var nodes = this.nodes.CreateNodes(tree.Nodes);
            links.CreateLinks(nodes);
        }

        [ContextMenu(nameof(ClearTree))]
        public void ClearTree()
        {
            nodes.Clear();
            links.Clear();
        }

        private void SelectNode(SkillNodeUI node) => learnUI.Select(node.Config);
    }
}
