using System.Collections.Generic;
using System.Linq;
using TestTask.Configuration;
using TestTask.Model;
using UnityEngine;

namespace TestTask.UI
{
    internal class SkillLinksUI : MonoBehaviour
    {
        [SerializeField] private Transform linksRoot;
        [SerializeField] private SkillLinkUI linkTemplate;

        private SkillViewFactory<SkillLinkUI> factory;

        private readonly Dictionary<ISkillNode, SkillNodeUI> nodes = new();

        public void Consturct()
        {
            factory = new(linkTemplate, linksRoot);
        }

        public void CreateLinks(ISkillNode node, SkillNodeUI nodeUI)
        {
            nodes.Add(node, nodeUI);
            foreach (var necessary in node.Necessary)
            {
                var necessaryNodeUI = nodes[necessary];

                var link = factory.Items.FirstOrDefault(x => x.Is(nodeUI, necessaryNodeUI));
                if (link == null)
                {
                    link = factory.Create();
                }

                nodeUI.AddLink(link);
                necessaryNodeUI.AddLink(link);
                link.Set(nodeUI, necessaryNodeUI);
            }
        }

        public void RemoveLinks(ISkillNode node)
        {
            foreach (var link in factory.Items)
            {
                if (link.Is(node.Config))
                {
                    continue;
                }

                link.Node.RemoveLink(link);
                link.Necessary.RemoveLink(link);
            }

            factory.Remove(x => x.Is(node.Config));
            nodes.Remove(node);
        }

        public IEnumerable<SkillLinkUI> GetLinks(SkillConfig config)
            => factory.Items.Where(x => x.Is(config));
    }
}
