using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Game string util.项目中用来存全局变量的
/// </summary>
public class GameStringUtil
{
    /// <summary>
    /// 
    /// </summary>
    public static Color Green_Color = new Color(75 / 255f, 230 / 255f, 10 / 255f);
    /// <summary>
    /// 
    /// </summary>
    public static Color Red_Color = new Color(255 / 255f, 0 / 255f, 0 / 255f);

    /****************************************************************************************************************************************************/
    /// <summary>
    /// 小数点截取
    /// </summary>
    /// <param name="f"></param>
    /// <param name="acc"></param>
    /// <returns></returns>
    public static float Round(float f, int acc)
    {
        float temp = f * Mathf.Pow(10, acc);
        return Mathf.Round(temp) / Mathf.Pow(10, acc);
    }
}
