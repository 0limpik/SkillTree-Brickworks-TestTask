using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SkillNodesUI : MonoBehaviour
    {
        [SerializeField] private Transform nodesRoot;
        [SerializeField] private SkillNodeUI nodeTemplate;

        private SkillViewFactory<SkillNodeUI> factory;
        private SkillSelector skillSelector;
        private RadialPlacer placer;

        internal void Construct(SkillSelector skillSelector)
        {
            this.skillSelector = skillSelector;

            factory = new(nodeTemplate, nodesRoot);
            placer = new RadialPlacer((nodeTemplate.transform as RectTransform).rect.width);
        }

        internal Dictionary<ISkillNode, SkillNodeUI> CreateNodes(IEnumerable<ISkillNode> nodes)
        {
            var result = new Dictionary<ISkillNode, SkillNodeUI>();

            foreach (var node in nodes)
            {
                var pos = placer.StepNext();
                var nodeView = factory.Items.FirstOrDefault(x => x.Config == node.Config);
                if (nodeView == null)
                {
                    nodeView = factory.Create();
                    nodeView.RectTransform.anchoredPosition += pos;
                }

                nodeView.Set(node.Config);
                skillSelector.Register(nodeView);
                result.Add(node, nodeView);
            }

            return result;
        }

        public void ClearNodes(IEnumerable<ISkillNode> nodes)
        {
            foreach (var node in nodes.SkipLast(1))
            {
                placer.StepBack();

                var nodeUI = factory.Items.First(x => x.Config == node.Config);
                skillSelector.Unregister(nodeUI);
                factory.Remove(nodeUI);
            }

            {
                var node = nodes.Last();
                placer.StepBack();

                var nodeUI = factory.Items.First(x => x.Config == node.Config);
                skillSelector.Unregister(nodeUI);
                factory.Remove(nodeUI);
            }
        }

        public class RadialPlacer
        {
            private readonly float objectWidth;
            private float degreeValue;

            private int CurrentCircle => (int)(degreeValue / 360f);

            public RadialPlacer(float objectWidth)
            {
                this.objectWidth = objectWidth;
            }

            public Vector2 StepNext()
            {
                if (CurrentCircle == 0)
                {
                    degreeValue = 360f;
                    return Vector2.zero;
                }

                return Step();
            }

            public Vector2 StepBack()
            {
                if(CurrentCircle == 0)
                {
                    degreeValue = 0;
                    return Vector2.zero;
                }

                return Step(true);
            }

            private Vector2 Step(bool isBack = false)
            {
                var radius = CurrentCircle * objectWidth + objectWidth * 0.5f;
                var cos = Mathf.Tan(objectWidth / radius) * Mathf.Rad2Deg;
                var step = degreeValue % 360f / cos;
                degreeValue = CurrentCircle * 360 + (step + (isBack ? -1 : 1)) * cos;
                var quat = Quaternion.AngleAxis(cos * step, Vector3.back);
                return quat * Vector2.up * radius;
            }
        }
    }
}
