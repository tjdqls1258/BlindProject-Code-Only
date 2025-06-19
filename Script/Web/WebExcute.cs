using UnityEngine;

public class WebRequset
{
    public virtual string API_URL()
    {
        return "";
    }
}

public class WebResponse
{
    public int error = 0;

    public virtual bool IsDone()
    { return error == 0; }
}

public static class WebExcute
{
    
}
