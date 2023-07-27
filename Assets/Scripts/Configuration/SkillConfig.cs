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
        [SerializeField] private string _name;

        public string Name => _name;

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

            foreach (var (path, config) in emptyNameCfgs)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                config._name = name;
                EditorUtility.SetDirty(config);
                Debug.Log($"{path} {nameof(config.Name)} has been set to: {name}");
            }
        }
#endif
    }
}

