using System.Collections.Generic;
using System.Linq;
using TestTask.Configuration;
using TestTask.Model;
using TestTask.UI;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    internal class Tests
    {
        [MenuItem("TestTask/Tests/FillTree")]
        static void FillTree()
        {
            var configs = Selection.objects.Cast<SkillTreeConfig>();

            if (configs == null)
            {
                return;
            }

            var skillTreeContainer = new SkillTreeContainer();

            foreach (var config in configs)
            {
                skillTreeContainer.AddTree(config);
            }

            var skillTree = skillTreeContainer.Tree;

            Debug.Log(string.Join("\n", skillTree.Nodes.Select(x => DebutSkill(x))));

            foreach (var root in skillTree.GetRoots())
            {
                Debug.Log($"Available graph:\n{DebugSkill(root, true)}");
            }

            foreach (var root in skillTree.GetLeaves())
            {
                Debug.Log($"Nesessary graph:\n{DebugSkill(root, false)}");
            }

            static string DebutSkill(ISkillNode node)
                => $"{node}{DebugSkills(node.Necessary, "Necessary")}{DebugSkills(node.Available, "Available")}";

            static string DebugSkills(IEnumerable<ISkillNode> nodes, string name)
                => nodes.Any() ? $"\n    {name}\n        {string.Join($"\n{new string(' ', 8)}", nodes)}" : "";

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

        [MenuItem("TestTask/Tests/Placer")]
        static void TestPlacer()
        {
            var placer = new SkillNodesUI.RadialPlacer(100);
            for (int i = 0; i < 5; i++)
            {
                Debug.Log($"{placer.StepNext()}");
            }
        }
    }
}
