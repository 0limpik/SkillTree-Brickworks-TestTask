using System.Collections.Generic;
using System.Linq;
using TestTask.Configuration;
using TestTask.Model;
using TestTask.Services;
using UnityEngine;

namespace TestTask.UI
{
    public class SkillTreeUI : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField, HideInInspector] public List<SkillTreeConfig> treeConfigs;

        [Header("Links")]
        [SerializeField] private SkinLearnUI learn;

        [SerializeField] private SkillNodesUI nodes;
        [SerializeField] private SkillLinksUI links;

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
            nodes.Subscribe();
            learn.Subscribe();
        }

        void OnDisable()
        {
            learn.Unscribe();
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
