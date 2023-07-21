using System.Linq;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Configuration
{
    [CreateAssetMenu(menuName = "TestTask/SkillCfg")]
    public class SkillConfig : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }

        public override string ToString() => $"{Name}";

#if UNITY_EDITOR
        [MenuItem("TestTask/FillEmptyNames")]
        static void FillEmptyNames()
        {
            var emptyNameCfgs = AssetDatabase.FindAssets($"t:{typeof(SkillConfig).FullName}")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(path => (path, cfg: AssetDatabase.LoadAssetAtPath<SkillConfig>(path)))
                .Where(x => string.IsNullOrEmpty(x.cfg.Name))
                .ToArray();

            foreach (var (path, cfg) in emptyNameCfgs)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                cfg.Name = name;
                EditorUtility.SetDirty(cfg);
                Debug.Log($"{path} {nameof(cfg.Name)} has been set to: {name}");
            }
        }
#endif
    }
}

