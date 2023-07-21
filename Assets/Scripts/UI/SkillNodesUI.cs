using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.View;
using Assets.Scripts.View.Factoryes;
using UnityEngine;

namespace Assets.Scripts.UI
{
    internal class SkillNodesUI : MonoBehaviour
    {
        [SerializeField] private Transform nodesRoot;
        [SerializeField] private SkillNodeUI nodeTemplate;

        private SkillViewFactory<SkillNodeUI> factory;
        private SkillSelectorView skillSelector;

        void Awake()
        {
            factory = new(nodeTemplate, nodesRoot);
        }

        public void Construct(SkillSelectorView skillSelector)
        {
            this.skillSelector = skillSelector;
        }

        public Dictionary<ISkillNode, SkillNodeUI> CreateNodes(IEnumerable<ISkillNode> nodes)
        {
            var placer = new RadialPlacer((nodeTemplate.transform as RectTransform).rect.width);

            if (factory.Items.Count() > 0)
            {
                placer.Step();
            }

            var result = new Dictionary<ISkillNode, SkillNodeUI>();

            foreach (var node in nodes)
            {
                var nodeView = factory.Items.FirstOrDefault(x => x.Config == node.Config);
                if (nodeView == null)
                {
                    nodeView = factory.Create();
                    nodeView.RectTransform.anchoredPosition += placer.Step();
                }

                nodeView.Set(node.Config);
                skillSelector.Register(nodeView);
                result.Add(node, nodeView);
            }

            return result;
        }

        public void Clear()
        {
            foreach (var node in factory.Items)
            {
                skillSelector.Unregister(node);
            }

            factory.Clear();
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
