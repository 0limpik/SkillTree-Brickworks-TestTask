using System.Linq;
using Assets.Scripts.Configuration;
using Assets.Scripts.Model;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SkillNodesUI : MonoBehaviour
    {
        [SerializeField] private Transform nodesRoot;
        [SerializeField] private SkillNodeUI nodeTemplate;

        private SkillSelector skillSelector;
        private SkillLearnService skillLearn;

        private SkillViewFactory<SkillNodeUI> factory;
        private RadialPlacer placer;

        internal void Construct(SkillSelector skillSelector, SkillLearnService skillLearn)
        {
            this.skillSelector = skillSelector;
            this.skillLearn = skillLearn;

            factory = new(nodeTemplate, nodesRoot);
            placer = new RadialPlacer((nodeTemplate.transform as RectTransform).rect.width);
        }

        public void Subscribe()
        {
            skillLearn.OnLearn += Learn;
            skillLearn.OnForget += Forget;
        }

        public void Unscribe()
        {
            skillLearn.OnLearn -= Learn;
            skillLearn.OnForget -= Forget;
        }

        internal SkillNodeUI CreateNode(ISkillNode node)
        {
            var pos = placer.StepNext();
            var nodeUI = factory.Items.FirstOrDefault(x => x.Config == node.Config);
            if (nodeUI == null)
            {
                nodeUI = factory.Create();
                nodeUI.RectTransform.anchoredPosition += pos;
            }

            nodeUI.Set(node.Config);
            if (skillLearn.IsLearn(node))
            {
                nodeUI.Learn();
            }
            skillSelector.Register(nodeUI);

            return nodeUI;
        }

        public void RemoveNode(ISkillNode node)
        {
            placer.StepBack();

            var nodeUI = factory.Items.First(x => x.Config == node.Config);
            skillSelector.Unregister(nodeUI);
            factory.Remove(nodeUI);
        }

        private void Learn(SkillConfig config) => factory.Items.First(x => x.Config == config).Learn();
        private void Forget(SkillConfig config) => factory.Items.First(x => x.Config == config).Forget();

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
                if (CurrentCircle == 0)
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
