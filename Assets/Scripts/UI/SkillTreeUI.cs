using Assets.Scripts.Configuration;
using Assets.Scripts.Model;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(SkillNodesUI))]
    [RequireComponent(typeof(SkillLinksUI))]
    internal class SkillTreeUI : MonoBehaviour
    {
        [Header("Links")]
        [SerializeField] private SkinLearnUI learn;

        [Header("Setup")]
        [SerializeField] private SkillTreeConfig treeConfig;

        private SkillNodesUI nodes;
        private SkillLinksUI links;

        private SkillSelector skillSelector = new();
        private SkillCostService skillCost = new();

        void Awake()
        {
            nodes = this.GetComponent<SkillNodesUI>();
            links = this.GetComponent<SkillLinksUI>();

            nodes.Construct(skillSelector);
            learn.Construct(skillCost, skillSelector);
        }

        void Start() => CreateTree();

        void OnEnable() => learn.Subscribe();
        void OnDisable() => learn.Unscribe();

        void OnDestroy() => ClearTree();

        [ContextMenu(nameof(CreateTree))]
        public void CreateTree()
        {
            skillCost.AddTree(treeConfig);

            var tree = new SkillTree();
            tree.AddTree(treeConfig);
            learn.AddTree(tree);

            var nodes = this.nodes.CreateNodes(tree.Nodes);
            links.CreateLinks(nodes);
        }

        [ContextMenu(nameof(ClearTree))]
        public void ClearTree()
        {
            nodes.Clear();
            links.Clear();
        }
    }
}
