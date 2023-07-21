using Assets.Scripts.Configuration;
using Assets.Scripts.Model;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    internal class Tests
    {
        [MenuItem("TestTask/FillTree")]
        static void FillTree()
        {
            var config = Selection.activeObject as SkillTreeConfig;

            if (config == null)
            {
                return;
            }

            var skillTree = new SkillTree();
            skillTree.AddTree(config);

            foreach (var root in skillTree.GetRoots())
            {
                Debug.Log($"Available graph:\n{DebugSkill(root, true)}");
            }

            foreach (var root in skillTree.GetLeaves())
            {
                Debug.Log($"Nesessary graph:\n{DebugSkill(root, false)}");
            }

            static string DebugSkill(ISkillNode node, bool availableOrNesessary, int level = 1)
            {
                var text = $"{node}";

                foreach (var available in availableOrNesessary ? node.Available : node.Necessary)
                {
                    text += $"\n{new string(' ', level * 4)}{DebugSkill(available, availableOrNesessary, level + 1)}";
                }

                return text;
            }
        }
    }
}
