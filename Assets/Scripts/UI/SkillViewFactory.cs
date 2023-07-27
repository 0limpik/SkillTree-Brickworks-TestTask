using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TestTask.UI
{
    internal class SkillViewFactory<T> where T : MonoBehaviour
    {
        public IEnumerable<T> Items => items;

        private readonly T template;
        private readonly Transform root;
        private readonly HashSet<T> items = new();

        public SkillViewFactory(T template, Transform root)
        {
            this.template = template;
            this.root = root;

            items.AddRange(GetAllScript(this.root));
        }

        public T Create()
        {
            var item = Create(template, root);
            items.Add(item);
            return item;
        }

        public void Remove(T item)
        {
            GameObject.DestroyImmediate(item.gameObject);
            items.Remove(item);
        }

        public void Remove(Predicate<T> predicate)
        {
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    GameObject.DestroyImmediate(item.gameObject);
                }
            }
            items.RemoveWhere(predicate);
        }

        private static T Create(T template, Transform parent)
        {
#if UNITY_EDITOR
            return (T)UnityEditor.PrefabUtility.InstantiatePrefab(template, parent);
#else
            return GameObject.Instantiate(template, parent);
#endif
        }

        private static IEnumerable<T> GetAllScript(Transform root)
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
