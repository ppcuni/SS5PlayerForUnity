/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

/// <summary>
/// SpriteStudio用の拡張メソッド。
/// </summary>
public static class SpriteStudioExtensions
{
    /// <summary>
    /// 表示優先順位を取得する。
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public static float GetDisplayOrder(this Script_SpriteStudio_PartsRoot root)
    {
        Matrix4x4 MatrixWorld = root.transform.localToWorldMatrix;
        Vector3 OriginWorld = MatrixWorld.MultiplyPoint3x4(Vector3.zero);

        return OriginWorld.z;
    }
}