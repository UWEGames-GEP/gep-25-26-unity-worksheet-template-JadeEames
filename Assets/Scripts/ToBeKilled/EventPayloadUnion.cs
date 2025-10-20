using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public enum DataType
{
    INT, FLOAT, DOUBLE, STRING, BOOLEAN, CHAR, SHORT, VECT2
}
/*
public interface 

public class EventPayloadUnion
{
    private List<Dictionary<DataType, TypeSafeUnion<T>>> payload = new List<Dictionary<DataType, TypeSafeUnion<T>>>()
   
}

[StructLayout(LayoutKind.Explicit)]
public class UnionNode
{
    [FieldOffset(0)] public int _int;
    [FieldOffset(0)] public float _float;
    [FieldOffset(0)] public double _double;
    [FieldOffset(0)] public string _string;
    [FieldOffset(0)] public bool _bool;
    [FieldOffset(0)] public char _char;
    [FieldOffset(0)] public short _short;
    [FieldOffset(0)] public Vector2 _vector2;
}


public class TypeSafeUnion<T> : UnionNode
{
    private static readonly Dictionary<Type, Action<UnionNode, object>> _lookup_table =
    new()
    {
        { typeof(int),      (UnionNode u, object v) => u._int = (int)v },
        { typeof(float),    (UnionNode u, object v) => u._float = (float)v },
        { typeof(double),   (UnionNode u, object v) => u._double = (double)v },
        { typeof(string),   (UnionNode u, object v) => u._string = (string)v },
        { typeof(bool),     (UnionNode u, object v) => u._bool = (bool)v },
        { typeof(char),     (UnionNode u, object v) => u._char = (char)v },
        { typeof(short),    (UnionNode u, object v) => u._short = (short)v },
        { typeof(Vector2),  (UnionNode u, object v) => u._vector2 = (Vector2)v },
    };

    private Type type;
    private UnionNode union;

    public TypeSafeUnion(T data)
    {
        union = default;
        type = typeof(T);

        if (!_lookup_table.TryGetValue(type, out var table))
        {
            throw new NotSupportedException($"Type {type} not supported in TypeSafeUnion");
        }

        table(union, data!);
    }

    public Type valueType => type;
    public UnionNode raw => union;
}
*/
