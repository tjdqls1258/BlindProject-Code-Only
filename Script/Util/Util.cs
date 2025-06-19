using UnityEngine;

public static class Util
{

    public static bool IsEmptyOrNull(this string value)
    {
        return value == null || value == "";
    }
}
