﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.View
{
    [RequireComponent(typeof(CanvasRenderer))]
    internal class SkillLinkUI : MaskableGraphic
    {
        private static readonly Vector2 uv0 = new(0, 0);
        private static readonly Vector2 uv1 = new(0, 1);
        private static readonly Vector2 uv2 = new(1, 1);
        private static readonly Vector2 uv3 = new(1, 0);

        [field: SerializeField] public RectTransform First { get; private set; }
        [field: SerializeField] public RectTransform Second { get; private set; }

        [SerializeField] private float thickness = 100f;

        private Vector3 firstLastPostion;
        private Vector3 secondLastPostion;

        private Vector3[] corners = new Vector3[4];

        public bool Is(RectTransform first, RectTransform second)
            => (First == first && Second == second) || (First == second && Second == first);

        public void Set(RectTransform first, RectTransform second)
        {
            First = first;
            Second = second;
            Redraw();
            this.name = $"{First.name} - {Second.name}";
        }

        void Update()
        {
            if (First != null && First.position != firstLastPostion
             || Second != null && Second.position != secondLastPostion)
            {
                Redraw();
            }
        }

        private void Redraw()
        {
            this.SetVerticesDirty();
            firstLastPostion = First.position;
            secondLastPostion = Second.position;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (First == null || Second == null)
            {
                return;
            }

            Vector3 first = First.anchoredPosition;
            Vector3 second = Second.anchoredPosition;

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