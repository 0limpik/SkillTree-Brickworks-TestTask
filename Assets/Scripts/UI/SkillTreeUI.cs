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
        [SerializeField] private SkillLinksSelector linksSelector;

        [SerializeField] private SkillNodesUI nodes;
        [SerializeField] private SkillLinksUI links;

        [Header("Setup")]
        [SerializeField, HideInInspector] public List<SkillTreeConfig> treeConfigs;

        private readonly SkillSelector skillSelector = new();
        private readonly SkillCostService skillCost = new();
        private readonly SkillLearnService skillLearn = new();

        public readonly SkillTreeContainer treeContainer = new();

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
            nodes.Construct(skillSelector, skillLearn);
            links.Consturct();
            learn.Construct(skillCost, skillSelector, skillLearn);
            linksSelector.Consturct(skillSelector, skillLearn);
        }

        void Awake() => Construct();

        void Start()
        {
            foreach (var config in treeConfigs)
            {
                CreateTree(config);
            }
        }

        void OnEnable()
        {
            learn.Subscribe();
            linksSelector.Subscribe();
            nodes.Subscribe();
        }

        void OnDisable()
        {
            learn.Unscribe();
            linksSelector.Unscribe();
            nodes.Unscribe();
        }

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
            var nodes = treeContainer.AddTree(treeConfig);
            skillCost.AddTree(treeConfig);
            learn.AddTree(treeContainer.Tree);

            foreach (var node in nodes)
            {
                var nodeUI = this.nodes.CreateNode(node);
                links.CreateLinks(node, nodeUI);
            }
        }

        public void ClearTree(SkillTreeConfig treeConfig)
        {
            var removedNodes = treeContainer.RemoveTree(treeConfig).ToList();
            skillCost.RemoveTree(treeConfig);
            learn.RemoveTree(treeContainer.Tree);

            foreach (var node in removedNodes)
            {
                links.RemoveLinks(node);
                nodes.RemoveNode(node);
            }
        }
    }
}
