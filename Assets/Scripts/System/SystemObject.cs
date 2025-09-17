using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SystemObject : MonoBehaviour
{
    /// <summary>
    /// ‰Šú‰»
    /// </summary>
    public async virtual UniTask Initialize()
    {
        // ‰Šú‰»ˆ—
        await UniTask.CompletedTask;
    }
}
