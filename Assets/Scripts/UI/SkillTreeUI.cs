using System.Linq;
using Assets.Scripts.Configuration;
using Assets.Scripts.Model;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.UI
{
    internal class SkillTreeUI : MonoBehaviour
    {
        [Header("Links")]
        [SerializeField] private SkinLearnUI learn;

        [SerializeField] private SkillNodesUI nodes;
        [SerializeField] private SkillLinksUI links;

        [Header("Setup")]
        [SerializeField] private SkillTreeConfig[] treeConfigs;

        private readonly SkillSelector skillSelector = new();
        private readonly SkillCostService skillCost = new();
        private readonly SkillTree tree = new();

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void Reconstruct() => GameObject
            .FindObjectsOfType<SkillTreeUI>()
            .ToList()
            .ForEach(x => x.Construct());
#endif

        private void Construct()
        {
            nodes.Construct(skillSelector);
            links.Consturct();
            learn.Construct(skillCost, skillSelector);
        }

        void Awake() => Construct();

        void Start() => CreateTree();

        void OnEnable() => learn.Subscribe();
        void OnDisable() => learn.Unscribe();

        void OnDestroy() => ClearTree();

        [ContextMenu(nameof(CreateTree))]
        public void CreateTree()
        {
            foreach (var treeConfig in treeConfigs)
            {
                skillCost.AddTree(treeConfig);
                tree.AddTree(treeConfig);
            }

            learn.AddTree(tree);

            var nodes = this.nodes.CreateNodes(tree.Nodes);
            links.CreateLinks(nodes);
        }

        [ContextMenu(nameof(ClearTree))]
        public void ClearTree()
        {
            links.Clear();
            nodes.Clear();


            learn.RemoveTree(tree);

            foreach (var treeConfig in treeConfigs
                .AsEnumerable()
                .Reverse())
            {
                tree.RemoveTree(treeConfig);
                skillCost.RemoveTree(treeConfig);
            }
        }
    }
}
