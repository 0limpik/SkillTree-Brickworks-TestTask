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

        [SerializeField] private SkillNodeUI node;
        [SerializeField] private SkillNodeUI necessary;

        public SkillNodeUI Node => node;
        public SkillNodeUI Necessary => necessary;

        private readonly Vector3[] corners = new Vector3[4];

        private Vector3 nodeLastPostion;
        private Vector3 necessaryLastPostion;
        private Color selectionColor;

        public bool Is(SkillNodeUI first, SkillNodeUI second)
            => (node == first && necessary == second) || (node == second && necessary == first);

        public bool Is(SkillConfig config) => node.Config == config || necessary.Config == config;

        public void Set(SkillNodeUI node, SkillNodeUI necessary)
        {
            this.node = node;
            this.necessary = necessary;
            this.name = $"{node.name} - {necessary.name}";

            NodeUpdate();
        }

        public void NodeUpdate()
        {
            color = necessary.IsLearned ? config.Learned : config.Forgeted;
            this.SetVerticesDirty();
        }

        public void Select(SkillNodeUI node)
        {
            selectionColor = this.node == node && !necessary.IsLearned
                ? config.SelectedNecessary
                : config.Selected;
            this.SetVerticesDirty();
        }

        public void Deselect()
        {
            selectionColor = Color.clear;
            this.SetVerticesDirty();
        }

        void Update()
        {
            if (node == null || necessary == null)
            {
                return;
            }

            if (node.RectTransform.position != nodeLastPostion
                || necessary.RectTransform.position != necessaryLastPostion)
            {
                nodeLastPostion = node.RectTransform.position;
                necessaryLastPostion = necessary.RectTransform.position;
                this.SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (!Application.isPlaying)
            {
                selectionColor = config.Selected;
            }

            if (node == null || necessary == null)
            {
                return;
            }

            Span<Quaternion> ang = stackalloc Quaternion[] {
                Quaternion.AngleAxis(+90, Vector3.forward),
                Quaternion.AngleAxis(-90, Vector3.forward),
                Quaternion.AngleAxis(-90, Vector3.forward),
                Quaternion.AngleAxis(+90, Vector3.forward)
            };
            var nodePos         = Node.RectTransform.anchoredPosition;
            var necessaryPos    = Necessary.RectTransform.anchoredPosition;
            Span<Vector3> pos   = stackalloc Vector3[] { nodePos,       nodePos,        necessaryPos,   necessaryPos };
            Span<Vector2> uv    = stackalloc Vector2[] { new(0, 0),     new(1, 0),      new(1, 1),      new(0, 1) };
            Span<Vector2> uvMod = stackalloc Vector2[] { new(+.25f, 0), new(-.25f, 0),  new(-.25f, 0),  new(+.25f, 0) };

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
            thickness = delta * (config.Thickness + config.SelectedThickness * .5f);

            for (int i = 0; i < 4; i++)
                vh.AddVert(ang[i] * thickness + pos[i], selectionColor, uv[i] + uvMod[i] * .5f);

            AddTriangles(0);

            //selectionTransparent
            thickness = delta * (config.Thickness + config.SelectedThickness);

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
