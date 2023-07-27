using System.Linq;
using TestTask.Configuration;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TestTask.UI
{
    [CustomEditor(typeof(SkillTreeUI))]
    internal class SkillTreeUIEditor : Editor
    {
        private new SkillTreeUI target => base.target as SkillTreeUI;
        private SkillTreeConfig selectedTreeConfig => treeConfigProperty.value as SkillTreeConfig;

        private ObjectField treeConfigProperty;
        private Button createButton;
        private Button clearButton;

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            var defaultIMGUI = new IMGUIContainer(OnInspectorGUI);
            container.Add(defaultIMGUI);

            var treeConfigs = serializedObject.FindProperty(nameof(SkillTreeUI.treeConfigs));
            var treeConfigsProperty = new IMGUIContainer(() =>
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(treeConfigs, true);
                GUI.enabled = true;
            });
            container.Add(treeConfigsProperty);

            var buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            buttons.style.marginTop = 3f;
            container.Add(buttons);

            treeConfigProperty = new ObjectField("Tree");
            treeConfigProperty.style.flexGrow = 1;
            treeConfigProperty.style.marginLeft = 0;
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
            var missing = target.treeContainer.Tree.CanAddTree(selectedTreeConfig).ToList();

            foreach (var config in missing)
            {
                var available = selectedTreeConfig.GetAvailable(config).Select(x => x.Config);
                Debug.LogError($"{config} missing in tree requered by nodes: {string.Join(", ", available)}", config);
            }

            if (missing.Any())
            {
                return;
            }

            target.CreateTree(selectedTreeConfig);
            target.treeConfigs.Add(selectedTreeConfig);
            UpdateButtons();
        }

        private void ClearTree()
        {
            var dependents = target.treeContainer.Tree.CanRemoveTree(selectedTreeConfig).ToList();

            foreach (var (config, dependent) in dependents)
            {
                Debug.LogError($"{config} dependent from tree nodes: {string.Join(", ", dependent)}", config);
            }

            if (dependents.Any())
            {
                return;
            }

            target.ClearTree(selectedTreeConfig);
            target.treeConfigs.Remove(selectedTreeConfig);
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            var enabled = selectedTreeConfig != null;

            createButton.SetEnabled(enabled && !target.treeConfigs.Contains(selectedTreeConfig));
            clearButton.SetEnabled(enabled && target.treeConfigs.Contains(selectedTreeConfig));
        }
    }
}
