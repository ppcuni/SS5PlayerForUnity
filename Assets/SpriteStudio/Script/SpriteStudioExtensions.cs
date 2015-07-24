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

    /// <summary>
    /// 親をたどって<see cref="Script_SpriteStudio_PartsRoot.BitStatus.REDECODE_INSTANCE"/>がtrueになっているかどうかをチェックする。
    /// 再帰的にたどることで孫以降の<see cref="Script_SpriteStudio_PartsInstance"/>のフレーム遅延問題を起きないようにする。
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public static bool CheckRedecodeInstanceInParent(this Script_SpriteStudio_PartsRoot root)
    {
        if (0 != (root.Status & Script_SpriteStudio_PartsRoot.BitStatus.REDECODE_INSTANCE))
            return true;

        var parentInstance = root.transform.parent.GetComponent<Script_SpriteStudio_PartsInstance>();
        if (parentInstance == null)
            return false;

        return parentInstance.ScriptRoot.CheckRedecodeInstanceInParent();
    }

    /// <summary>
    /// アニメーションの再生開始フレームかどうか。
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public static bool IsFirstPlay(this Script_SpriteStudio_PartsRoot root)
    {
        return root.FrameNoPrevious == root.FrameNoStart;
    }
}