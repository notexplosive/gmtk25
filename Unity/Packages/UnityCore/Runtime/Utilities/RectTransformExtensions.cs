using UnityEngine;

namespace SecretPlan.Core
{
    public static class RectTransformExtensions
    {
        public static Vector3 GetPointOnRectEdge(this RectTransform? rect, Vector2 dir)
        {
            if (rect == null)
            {
                return Vector3.zero;
            }

            if (dir != Vector2.zero)
            {
                dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            }

            dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
            return dir;
        }

        public static void StretchToFill(this RectTransform? rect)
        {
            if (rect == null)
            {
                return;
            }
            rect.anchoredPosition = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1,1);
            rect.anchorMin = new Vector2(0,0);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = Vector2.zero;
        }
    }
}
