using System;

namespace TestTask.UI
{
    internal class SkillSelector
    {
        public event Action<SkillNodeUI> OnSelect;
        public SkillNodeUI Selected { get; private set; }

        public void Register(SkillNodeUI node) => node.OnNodeSelect += SelectNode;
        public void Unregister(SkillNodeUI node) => node.OnNodeSelect -= SelectNode;

        private void SelectNode(SkillNodeUI node)
        {
            if (Selected != null)
            {
                Selected.Deselect();
            }
            Selected = node;
            OnSelect?.Invoke(node);
            Selected.Select();
        }
    }
}
