using System;
using Assets.Scripts.Configuration;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    internal class SkillLinkUI : MaskableGraphic
    {
        [SerializeField] private SkillLinkUIConfig config;

        [field: SerializeField] public SkillNodeUI Node { get; private set; }
        [field: SerializeField] public SkillNodeUI Necessary { get; private set; }

        private readonly Vector3[] corners = new Vector3[4];

        private Vector3 firstLastPostion;
        private Vector3 secondLastPostion;
        private Color selectionColor;

        public bool Is(SkillNodeUI first, SkillNodeUI second)
            => (Node == first && Necessary == second) || (Node == second && Necessary == first);

        public bool Is(SkillConfig config) => Node.Config == config || Necessary.Config == config;

        public void Set(SkillNodeUI node, SkillNodeUI necessary)
        {
            this.Node = node;
            this.Necessary = necessary;
            this.name = $"{this.Node.name} - {Necessary.name}";

            NodeUpdate();
        }

        public void NodeUpdate()
        {
            color = Necessary.IsLearned ? config.LearnedColor : config.NotLearnedColor;
            Redraw();
        }

        public bool Select(SkillNodeUI node)
        {
            var select = node == Node && !Necessary.IsLearned;
            selectionColor = select ? config.SelectionNecessaryColor : config.SelectionColor;
            Redraw();
            return !select;
        }

        public void Deselect()
        {
            selectionColor = Color.clear;
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

            if (!Application.isPlaying)
            {
                selectionColor = config.SelectionColor;
            }

            if (Node == null || Necessary == null)
            {
                return;
            }

            var node = Node.RectTransform.anchoredPosition;
            var necessary = Necessary.RectTransform.anchoredPosition;
            Span<Vector3> pos = stackalloc Vector3[] { node, node, necessary, necessary };

            Span<Quaternion> ang = stackalloc Quaternion[] {
                Quaternion.AngleAxis(+90, Vector3.forward),
                Quaternion.AngleAxis(-90, Vector3.forward),
                Quaternion.AngleAxis(-90, Vector3.forward),
                Quaternion.AngleAxis(+90, Vector3.forward)
            };

            Span<Vector2> uv = stackalloc Vector2[] { new(0, 0), new(1, 0), new(1, 1), new(0, 1) };
            Span<Vector2> uvMod = stackalloc Vector2[] { new(+.25f, 0), new(-.25f, 0), new(-.25f, 0), new(+.25f, 0) };

            var delta = (pos[2] - pos[0]).normalized;

            //main
            var thickness = delta * config.Thickness;
            for (int i = 0; i < 4; i++)
                corners[i] = ang[i] * thickness + pos[i];
            for (int i = 0; i < 4; i++)
                vh.AddVert(corners[i], color, uv[i] + uvMod[i]);
            vh.AddTriangle(1, 2, 3);
            vh.AddTriangle(0, 1, 3);

            //selection
            thickness = delta * (config.Thickness + config.SelectionThickness * .5f);
            for (int i = 0; i < 4; i++)
                vh.AddVert(ang[i] * thickness + pos[i], selectionColor, uv[i] + uvMod[i] * .5f);
            AddTriangles(0);

            //selectionTransparent
            thickness = delta * (config.Thickness + config.SelectionThickness);
            for (int i = 0; i < 4; i++)
                vh.AddVert(ang[i] * thickness + pos[i], Color.clear, uv[i]);
            AddTriangles(4);

            void AddTriangles(int start)
            {
                vh.AddTriangle(start + 0, start + 3, start + 4);
                vh.AddTriangle(start + 3, start + 4, start + 7);
                vh.AddTriangle(start + 1, start + 2, start + 6);
                vh.AddTriangle(start + 1, start + 5, start + 6);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[3], corners[0]);
        }
    }
}
