using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDataManager : SingletonMonoBehaviour<ScoreDataManager>
{
    /// <summary>
    /// 階数
    /// </summary>
    public int Floor { get; private set; } = 0;

    /// <summary>
    /// リセット
    /// </summary>
    public void Reset()
    {
        Floor = 0;
    }

    /// <summary>
    /// 上の階に移動
    /// </summary>
    public void GoUpstairs()
    {
        Floor++;
    }
}
