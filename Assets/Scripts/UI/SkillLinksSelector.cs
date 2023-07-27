using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestTask.Configuration;
using TestTask.Services;
using UnityEngine;

namespace TestTask.UI
{
    internal class SkillLinksSelector : MonoBehaviour
    {
        [SerializeField] private float selectDelay = 1;

        private SkillSelector skillSelector;
        private SkillLearnService skillLearn;

        private readonly List<SkillLinkUI> lastSelected = new();
        private readonly Stack<SkillLinkUI> stackLinks = new();
        private readonly Stack<SkillLinkUI> selectedStackLinks = new();

        private Coroutine selectCoroutine;

        public void Construct(SkillSelector skillSelector, SkillLearnService skillLearn)
        {
            this.skillSelector = skillSelector;
            this.skillLearn = skillLearn;
        }

        public void Subscribe()
        {
            skillSelector.OnSelect += Select;
            skillLearn.OnLearn += Select;
            skillLearn.OnForget += Select;
        }

        public void Unscribe()
        {
            skillSelector.OnSelect -= Select;
            skillLearn.OnLearn -= Select;
            skillLearn.OnForget -= Select;
        }

        private void Select(SkillNodeUI _) => Select();
        private void Select(SkillConfig _) => Select();

        private void Select()
        {
            if (selectCoroutine != null)
            {
                StopCoroutine(selectCoroutine);
            }

            selectCoroutine = StartCoroutine(StartSelect(skillSelector.Selected));
        }

        private IEnumerator StartSelect(SkillNodeUI node)
        {
            SelectNonNecessaty(node);

            foreach (var item in stackLinks)
            {
                item.Deselect();
            }
            stackLinks.Clear();
            selectedStackLinks.Clear();

            if (!NeedDisplay(node))
            {
                yield break;
            }

            while (true)
            {
                if (stackLinks.Count == 0)
                {
                    foreach (var link in GetNotLearned(node))
                    {
                        stackLinks.Push(link);
                    }
                }

                var curentLink = stackLinks.Peek();
                curentLink.Select(curentLink.Node);
                if (NeedDisplay(curentLink.Necessary))
                {
                    if (selectedStackLinks.TryPeek(out var lastLink) && lastLink == curentLink)
                    {
                        curentLink.Deselect();
                        stackLinks.Pop();
                        selectedStackLinks.Pop();
                    }
                    else
                    {
                        selectedStackLinks.Push(curentLink);
                        foreach (var link in GetNotLearned(curentLink.Necessary))
                        {
                            stackLinks.Push(link);
                        }
                    }
                }
                else
                {
                    yield return new WaitForSeconds(selectDelay);
                    curentLink.Deselect();
                    stackLinks.Pop();
                }

                //prevent while stuck
#if UNITY_EDITOR
                yield return null;
#endif
            }
        }

        private void SelectNonNecessaty(SkillNodeUI node)
        {
            foreach (var link in lastSelected)
            {
                link.Deselect();
            }
            lastSelected.Clear();

            foreach (var link in GetAvailable(node))
            {
                link.Select(node);
                lastSelected.Add(link);
            }

            foreach (var link in GetLearned(node))
            {
                link.Select(node);
                lastSelected.Add(link);
            }
        }

        private bool NeedDisplay(SkillNodeUI node)
            => GetNotLearned(node).Any() && !GetLearned(node).Any();

        private IEnumerable<SkillLinkUI> GetLearned(SkillNodeUI node) => node.Links
            .Where(x => x.Node == node)
            .Where(x => x.Necessary.IsLearned);

        private IEnumerable<SkillLinkUI> GetNotLearned(SkillNodeUI node) => node.Links
            .Where(x => x.Node == node)
            .Where(x => !x.Necessary.IsLearned);

        private IEnumerable<SkillLinkUI> GetAvailable(SkillNodeUI node) => node.Links
            .Where(x => x.Necessary == node);
    }
}
