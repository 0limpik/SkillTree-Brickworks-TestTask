using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configuration;
using Assets.Scripts.Model;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SkillTreeUI : MonoBehaviour
    {
        [Header("Links")]
        [SerializeField] private SkinLearnUI learn;

        [SerializeField] private SkillNodesUI nodes;
        [SerializeField] private SkillLinksUI links;

        [Header("Setup")]
        [SerializeField] public List<SkillTreeConfig> treeConfigs;

        public readonly SkillTree tree = new();
        private readonly SkillSelector skillSelector = new();
        private readonly SkillCostService skillCost = new();

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void Reconstruct()
        {
            foreach (var treeUI in GameObject.FindObjectsOfType<SkillTreeUI>())
            {
                treeUI.Construct();
                treeUI.Start();
            }
        }
#endif

        private void Construct()
        {
            nodes.Construct(skillSelector);
            links.Consturct();
            learn.Construct(skillCost, skillSelector);
        }

        void Awake() => Construct();

        void Start()
        {
            foreach (var config in treeConfigs)
            {
                CreateTree(config);
            }
        }

        void OnEnable() => learn.Subscribe();
        void OnDisable() => learn.Unscribe();

        void OnDestroy()
        {
            foreach (var config in treeConfigs
                .AsEnumerable()
                .Reverse())
            {
                ClearTree(config);
            }
        }

        public void CreateTree(SkillTreeConfig treeConfig)
        {
            tree.AddTree(treeConfig);
            skillCost.AddTree(treeConfig);

            learn.AddTree(tree);

            var nodes = this.nodes.CreateNodes(tree.Nodes);
            links.CreateLinks(nodes);
        }

        public void ClearTree(SkillTreeConfig treeConfig)
        {
            var removedNodes = tree.RemoveTree(treeConfig).ToList();
            skillCost.RemoveTree(treeConfig);
            learn.RemoveTree(tree);

            nodes.ClearNodes(removedNodes);
            links.RemoveLinks(removedNodes);
        }
    }
}
