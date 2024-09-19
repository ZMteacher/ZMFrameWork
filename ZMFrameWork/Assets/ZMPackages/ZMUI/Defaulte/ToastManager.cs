using UnityEngine;
/// <summary>
/// 由于Toast 属于层级最高，且经常出现，故不适合做成Window窗口，独立起来使用更加方便
/// </summary>
public class ToastManager : MonoBehaviour
{
    private static Toast mToast;
    public static void ShowToast(string key)
    {
        if (mToast == null)
            mToast = UIModule.Instance.ResourcesLoadObj("Toast", null).GetComponent<Toast>();
        mToast.ShowToast(key);
    }
 
    public static void ShowToast(int key)
    {
        //201 在这个提示在特殊场景下会多次出现，影响体验。但不影响后续工作流程，故过滤。
        //200表示无错误
        if (mToast == null)
            mToast = UIModule.Instance.ResourcesLoadObj("Toast", null).GetComponent<Toast>();
        mToast.ShowToast(key);
    }
 
}
