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

        public readonly SkillTreeContainer treeContainer = new();
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
            linksSelector.Consturct(links, skillSelector);
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
        }

        void OnDisable()
        {
            learn.Unscribe();
            linksSelector.Unscribe();
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
                links.CreateLink(node, nodeUI);
            }
        }

        public void ClearTree(SkillTreeConfig treeConfig)
        {
            var removedNodes = treeContainer.RemoveTree(treeConfig).ToList();
            skillCost.RemoveTree(treeConfig);
            learn.RemoveTree(treeContainer.Tree);

            foreach(var node in removedNodes)
            {
                links.RemoveLink(node);
                nodes.RemoveNode(node);
            }
        }
    }
}
