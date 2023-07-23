using Assets.Scripts.Configuration;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;

namespace Assets.Scripts.UI
{
    [CustomEditor(typeof(SkillTreeUI))]
    internal class SkillTreeUIEditor : Editor
    {
        private ObjectField treeConfigProperty;
        private Button createButton;
        private Button clearButton;

        private new SkillTreeUI target => base.target as SkillTreeUI;
        private SkillTreeConfig treeConfig => treeConfigProperty.value as SkillTreeConfig;

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            var defaultIMGUI = new IMGUIContainer(OnInspectorGUI);
            container.Add(defaultIMGUI);

            var buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            buttons.style.marginTop = 10f;
            container.Add(buttons);

            treeConfigProperty = new ObjectField("Tree");
            treeConfigProperty.style.flexGrow = 1;
            treeConfigProperty.objectType = typeof(SkillTreeConfig);
            treeConfigProperty.RegisterValueChangedCallback(_ => UpdateButtons());

            createButton = new Button(CreateTree);
            createButton.text = "Create";
            createButton.style.marginLeft = 10f;

            clearButton = new Button(ClearTree);
            clearButton.style.marginRight = 0;
            clearButton.text = "Clear";

            buttons.Add(treeConfigProperty);
            buttons.Add(createButton);
            buttons.Add(clearButton);

            UpdateButtons();

            return container;
        }

        private void CreateTree()
        {
            var missing = target.treeContainer.CanAddTree(treeConfig).ToList();

            foreach (var config in missing)
            {
                var available = treeConfig.GetAvailable(config).Select(x => x.Config);
                Debug.LogError($"{config} missing in tree requered by nodes: {string.Join(", ", available)}", config);
            }

            if (missing.Any())
            {
                return;
            }

            target.CreateTree(treeConfig);
            target.treeConfigs.Add(treeConfig);
            UpdateButtons();
        }

        private void ClearTree()
        {
            var dependents = target.treeContainer.CanRemoveTree(treeConfig).ToList();

            foreach (var (config, dependent) in dependents)
            {
                Debug.LogError($"{config} dependent from tree nodes: {string.Join(", ", dependent)}", config);
            }

            if (dependents.Any())
            {
                return;
            }

            target.ClearTree(treeConfig);
            target.treeConfigs.Remove(treeConfig);
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            var enabled = treeConfig != null;

            createButton.SetEnabled(enabled && !target.treeConfigs.Contains(treeConfig));
            clearButton.SetEnabled(enabled && target.treeConfigs.Contains(treeConfig));
        }
    }
}
