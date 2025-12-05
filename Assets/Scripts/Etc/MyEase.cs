using UnityEngine;

public static class MyEase
{
    public static float EaseOutQuad(float t)
    {
        return 1 - (1 - t) * (1 - t);

    }
    
    public static float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }

    public static float EaseOutBack(float t)
    {

        float c1 = 1.70158f;
        float c2 = c1 + 1;
        return 1 + c2 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);

    }
}
