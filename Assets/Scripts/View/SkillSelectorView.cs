using System;

namespace Assets.Scripts.View
{
    internal class SkillSelectorView
    {
        public event Action<SkillNodeUI> OnSelect;
        public SkillNodeUI Selected { get; private set; }

        public void Register(SkillNodeUI skillNodeUI) => skillNodeUI.OnNodeSelect += SelectNode;
        public void Unregister(SkillNodeUI skillNodeUI) => skillNodeUI.OnNodeSelect -= SelectNode;

        private void SelectNode(SkillNodeUI node)
        {
            Selected?.Deselect();
            Selected = node;
            OnSelect?.Invoke(node);
            Selected.Select();
        }
    }
}
