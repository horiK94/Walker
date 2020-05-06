using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAppearanceParameter : MonoBehaviour
{
    [System.Serializable]
    public class EnemyParam
    {
        [SerializeField]
        private int searcher = 0;
        public int Searcher { get { return searcher; } }

        [SerializeField]
        private int pursuer = 0;
        public int Pursuer { get { return pursuer; } }
    }

    [SerializeField]
    EnemyParam[] enemyParams = null;

    public EnemyParam GetParameter(int _floor)
    {
        if (_floor > enemyParams.Length)
        {
            return enemyParams[enemyParams.Length - 1];
        }
        else if(_floor <= 0)
        {
            return null;
        }
        return enemyParams[_floor - 1];
    }
}
