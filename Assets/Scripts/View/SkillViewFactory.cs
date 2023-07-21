using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.View
{
    internal class SkillViewFactory
    {
        public IEnumerable<SkillNodeUI> Nodes => nodes;
        public IEnumerable<SkillLinkUI> Links => links;

        private SkillNodeUI nodeTemplate;
        private Transform nodesRoot;
        private HashSet<SkillNodeUI> nodes = new();

        private SkillLinkUI linkTemplate;
        private Transform linksRoot;
        private HashSet<SkillLinkUI> links = new();

        public void Set(SkillNodeUI template, Transform root)
        {
            nodesRoot = root;
            nodeTemplate = template;

            nodes.AddRange(GetAllScripts<SkillNodeUI>(root));
        }

        public void Set(SkillLinkUI template, Transform root)
        {
            linksRoot = root;
            linkTemplate = template;

            links.AddRange(GetAllScripts<SkillLinkUI>(root));
        }

        public SkillNodeUI CreateNode()
        {
            var node = Create(nodeTemplate, nodesRoot);
            nodes.Add(node);
            return node;
        }

        public void ClearAllNodes()
        {
            foreach (var node in nodes)
            {
                GameObject.DestroyImmediate(node.gameObject);
            }
            nodes.Clear();
        }

        public void ClearAllLinks()
        {
            foreach (var link in links)
            {
                GameObject.DestroyImmediate(link.gameObject);
            }
            links.Clear();
        }

        public SkillLinkUI CreateLink()
        {
            var link = Create(linkTemplate, linksRoot);
            links.Add(link);
            return link;
        }

        private static T Create<T>(T template, Transform parent) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            return (T)UnityEditor.PrefabUtility.InstantiatePrefab(template, parent);
#else
            return GameObject.Instantiate(template, parent);
#endif
        }

        private static IEnumerable<T> GetAllScripts<T>(Transform root) where T : MonoBehaviour
        {
            foreach (Transform transform in root)
            {
                if (transform.TryGetComponent<T>(out var script))
                {
                    yield return script;
                }
            }
        }
    }
}
