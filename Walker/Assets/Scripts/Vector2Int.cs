using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2Int
{
    /// <summary>
    /// 中心点
    /// </summary>
    public static readonly Vector2Int Zero = new Vector2Int(0, 0);
    /// <summary>
    /// 上
    /// </summary>
    public static readonly Vector2Int Up = new Vector2Int(0, 1);
    /// <summary>
    /// 下
    /// </summary>
    public static readonly Vector2Int Down = new Vector2Int(0, -1);
    /// <summary>
    /// 右
    /// </summary>
    public static readonly Vector2Int Right = new Vector2Int(1, 0);
    /// <summary>
    /// 左
    /// </summary>
    public static readonly Vector2Int Left = new Vector2Int(-1, 0);

    public int x { get; }
    public int y { get; }

    public Vector2Int(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public static Vector2Int operator+(Vector2Int _x, Vector2Int _y)
    {
        return new Vector2Int(_x.x + _y.x, _x.y + _y.y);
    }

    public static Vector2Int operator -(Vector2Int _x, Vector2Int _y)
    {
        return new Vector2Int(_x.x - _y.x, _x.y - _y.y);
    }

    public static Vector2Int operator *(int _x, Vector2Int _y)
    {
        return new Vector2Int(_x * _y.x, _x * _y.y);
    }

    public static Vector2Int operator *(Vector2Int _x, int _y)
    {
        return new Vector2Int(_x.x * _y, _x.y * _y);
    }

    public static Vector2 operator /(Vector2Int _x, int _y)
    {
        return new Vector2(_x.x / (float)_y, _x.y / (float)_y);
    }
}
