using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject searcherPrefab = null;

    [SerializeField]
    private GameObject pursuerPrefab = null;

    /// <summary>
    /// 探索者の生成
    /// </summary>
    /// <param name="_createPoint"></param>
    public GameObject CreateSearcher(Vector3 _createPoint)
    {
        return Instantiate(searcherPrefab, _createPoint, Quaternion.identity);
    }

    /// <summary>
    /// 追跡者の生成
    /// </summary>
    /// <param name="_createPoint"></param>
    public GameObject CreatePursuer(Vector3 _createPoint)
    {
        return Instantiate(pursuerPrefab, _createPoint, Quaternion.identity);
    }
}
