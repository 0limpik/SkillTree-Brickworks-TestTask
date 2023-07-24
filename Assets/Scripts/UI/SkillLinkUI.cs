using Assets.Scripts.Configuration;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    internal class SkillLinkUI : MaskableGraphic
    {
        private static readonly Vector2 uv0 = new(0, 0);
        private static readonly Vector2 uv1 = new(0, 1);
        private static readonly Vector2 uv2 = new(1, 1);
        private static readonly Vector2 uv3 = new(1, 0);

        [field: SerializeField] public SkillNodeUI Node { get; private set; }
        [field: SerializeField] public SkillNodeUI Necessary { get; private set; }

        [SerializeField] private float thickness = 100f;

        private Vector3 firstLastPostion;
        private Vector3 secondLastPostion;

        private readonly Vector3[] corners = new Vector3[4];

        public bool Is(SkillNodeUI first, SkillNodeUI second)
            => (Node == first && Necessary == second) || (Node == second && Necessary == first);

        public bool Is(SkillConfig config)
            => Node.Config == config || Necessary.Config == config;

        public void Set(SkillNodeUI node, SkillNodeUI necessary)
        {
            this.Node = node;
            this.Necessary = necessary;
            this.name = $"{this.Node.name} - {Necessary.name}";

            Redraw();
        }

        void Update()
        {
            if (Node != null && Node.RectTransform.position != firstLastPostion
                || Necessary != null && Necessary.RectTransform.position != secondLastPostion)
            {
                Redraw();
            }
        }

        private void Redraw()
        {
            this.SetVerticesDirty();
            firstLastPostion = Node.RectTransform.position;
            secondLastPostion = Necessary.RectTransform.position;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (Node == null || Necessary == null)
            {
                return;
            }

            Vector3 first = Node.RectTransform.anchoredPosition;
            Vector3 second = Necessary.RectTransform.anchoredPosition;

            var thicknessVector = (second - first).normalized * thickness;

            corners[0] = Quaternion.AngleAxis(+90f, Vector3.forward) * thicknessVector + first;
            corners[1] = Quaternion.AngleAxis(-90f, Vector3.forward) * thicknessVector + first;
            corners[2] = Quaternion.AngleAxis(+90f, Vector3.forward) * thicknessVector + second;
            corners[3] = Quaternion.AngleAxis(-90f, Vector3.forward) * thicknessVector + second;

            vh.AddVert(corners[0], color, uv0);
            vh.AddVert(corners[1], color, uv1);
            vh.AddVert(corners[2], color, uv3);
            vh.AddVert(corners[3], color, uv2);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(1, 2, 3);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[3]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[2], corners[0]);
        }
    }
}
