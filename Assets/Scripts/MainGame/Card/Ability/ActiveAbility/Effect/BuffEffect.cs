using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BuffEffect : BaseEffect
{
    public BuffEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(List<BaseComponent> targetCard)
    {
        List<CardData> cardDatas = CommonModule.CastList<BaseComponent, CardData>(targetCard);
        for (int i = 0, max = cardDatas.Count; i < max; i++)
        {
            cardDatas[i].AddStatus(param[0], param[1]);
        }
    }
}
