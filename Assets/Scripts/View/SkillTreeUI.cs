using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configuration;
using Assets.Scripts.Model;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.View
{
    internal class SkillTreeUI : MonoBehaviour
    {
        [SerializeField] private Transform nodesRoot;
        [SerializeField] private Transform linksRoot;
        [SerializeField] private SkinLearnUI learnUI;

        [SerializeField] private SkillTreeConfig treeConfig;
        [SerializeField] private SkillNodeUI nodeTemplate;
        [SerializeField] private SkillLinkUI linkTemplate;

        private SkillViewFactory skillFactory = new ();
        private SkillSelectorView skillSelector = new ();

        void Awake()
        {
            CreateTree();
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

            skillFactory.Set(linkTemplate, linksRoot);
            skillFactory.Set(nodeTemplate, nodesRoot);

            skillSelector.OnSelect += SelectNode;

            var nodes = CreateNodes(tree.Nodes);
            CreateLinks(nodes);
        }

        [ContextMenu(nameof(ClearTree))]
        public void ClearTree()
        {
            skillSelector.OnSelect -= SelectNode;

            foreach (var node in skillFactory.Nodes)
            {
                skillSelector.Unregister(node);
            }

            skillFactory.ClearAllNodes();
            skillFactory.ClearAllLinks();
        }

        private void SelectNode(SkillNodeUI node) => learnUI.Select(node.Config);

        private Dictionary<ISkillNode, SkillNodeUI> CreateNodes(IEnumerable<ISkillNode> nodes)
        {
            var placer = new RadialPlacer((nodeTemplate.transform as RectTransform).rect.width);

            if(skillFactory.Nodes.Count() > 0)
            {
                placer.Step();
            }

            var result = new Dictionary<ISkillNode, SkillNodeUI>();

            foreach (var node in nodes)
            {
                var nodeView = skillFactory.Nodes.FirstOrDefault(x => x.Config == node.Config);
                if (nodeView == null)
                {
                    nodeView = skillFactory.CreateNode();
                    nodeView.RectTransform.anchoredPosition += placer.Step();
                }

                nodeView.Set(node.Config);
                skillSelector.Register(nodeView);
                result.Add(node, nodeView);
            }

            return result;
        }

        private void CreateLinks(Dictionary<ISkillNode, SkillNodeUI> skills)
        {
            foreach (var node in skills.Keys)
            {
                var skill = skills[node];
                foreach (var avalible in node.Available)
                {
                    var avalibleSkill = skills[avalible];
                    if (skillFactory.Links.Any(x => x.Is(skill.RectTransform, avalibleSkill.RectTransform)))
                    {
                        continue;
                    }

                    var link = skillFactory.CreateLink();
                    link.Set(skill.RectTransform, avalibleSkill.RectTransform);
                }
            }
        }

        private struct RadialPlacer
        {
            private readonly float objectWidth;
            private float degreeValue;

            public RadialPlacer(float objectWidth)
            {
                this.objectWidth = objectWidth;
                degreeValue = 0;
            }

            public Vector2 Step()
            {
                var radiusPoint = (int)(degreeValue / 360f);

                if (radiusPoint == 0)
                {
                    degreeValue += 360f;
                    return Vector2.zero;
                }

                var radius = radiusPoint * objectWidth + objectWidth * 0.5f;
                var cos = Mathf.Cos(objectWidth / radius) * Mathf.Rad2Deg;
                var remainder = degreeValue % 360f;
                var steps = remainder / cos;
                //Debug.Log($"cos:{cos} objectWidth:{objectWidth} raidus:{radius} value:{degreeValue} steps{steps}");
                var quat = Quaternion.AngleAxis(cos * steps, Vector3.back);
                var result = quat * Vector2.up * radius;
                degreeValue += cos;
                return result;
            }
        }
    }
}
