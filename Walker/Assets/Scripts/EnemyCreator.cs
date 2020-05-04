using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject searcherPrefab = null;

    public void CreateSearcher(Transform _playerTrans, Vector3 _createPoint)
    {
        GameObject searcher = Instantiate(searcherPrefab, _createPoint, Quaternion.identity);
        searcher.GetComponent<Searcher>().SetPlayerTransform(_playerTrans);
    }
}
