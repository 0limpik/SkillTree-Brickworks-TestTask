using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Screen = UnityEngine.Device.Screen;

namespace TestTask.UI
{
    [ExecuteAlways]
    public class SafeArea : MonoBehaviour
    {
        [SerializeField] private CanvasScaler scaler;
        [SerializeField] private RectTransform safeArea;

        private Action activeShimChanged;
        private bool needSetSafeArea;

        void Start() => SetSafeArea();

#if UNITY_EDITOR
        private static readonly FieldInfo activeShimChangedProperty = typeof(Screen).Assembly
            .GetType("UnityEngine.ShimManager")
            .GetField("ActiveShimChanged", BindingFlags.Static | BindingFlags.NonPublic);

        void OnEnable()
        {
            if (activeShimChanged == null)
            {
                activeShimChanged = activeShimChangedProperty.GetValue(null) as Action;
                if (activeShimChanged == null)
                {
                    activeShimChanged += SetSafeArea;
                    activeShimChangedProperty.SetValue(null, activeShimChanged);
                    return;
                }
            }
            activeShimChanged += SetSafeArea;
        }

        void OnDisable() => activeShimChanged -= SetSafeArea;
#endif

        private async void SetSafeArea()
        {
            needSetSafeArea = true;
            await Task.Yield();
            if (this == null || !needSetSafeArea)
            {
                return;
            }
            needSetSafeArea = false;

            var size = new Vector2(Screen.width, Screen.height);
            safeArea.anchorMin = Screen.safeArea.min / size;
            safeArea.anchorMax = Screen.safeArea.max / size;
        }
    }

}