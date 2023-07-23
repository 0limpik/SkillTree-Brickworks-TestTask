using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
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
                var nodeView = factory.Items.FirstOrDefault(x => x.Config == node.Config);
                if (nodeView == null)
                {
                    nodeView = factory.Create();
                    nodeView.RectTransform.anchoredPosition += placer.StepNext();
                }

                nodeView.Set(node.Config);
                skillSelector.Register(nodeView);
                result.Add(node, nodeView);
            }

            return result;
        }

        public void ClearNodes(IEnumerable<ISkillNode> nodes)
        {
            foreach (var node in nodes)
            {
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
                    degreeValue += 360f;
                    return Vector2.zero;
                }

                var (result, cos) = Step();
                degreeValue += cos;
                return result;
            }

            public Vector2 StepBack()
            {
                if(CurrentCircle == 0)
                {
                    degreeValue = 0;
                    return Vector2.zero;
                }

                var (result, cos) = Step();
                degreeValue -= cos;
                return result;
            }

            private (Vector2 result, float cos) Step()
            {
                var radius = CurrentCircle * objectWidth + objectWidth * 0.5f;
                var cos = Mathf.Tan(objectWidth / radius) * Mathf.Rad2Deg;
                var step = degreeValue % 360f / cos;
                var quat = Quaternion.AngleAxis(cos * step, Vector3.back);
                var result = quat * Vector2.up * radius;
                return (result, cos);
            }
        }
    }
}
