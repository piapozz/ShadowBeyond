using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterCardFieldEffect : BaseEffect
{
    public EnterCardFieldEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(bool isOwn)
    {
        // カードを生成
        CardObject enterCard = UIManager.instance.GetNewCardObject(param[0]);
        if (isOwn)
        {
            // 自分の場に出す
        }
        else
        {
            // 相手の場に出す
        }
    }
}
