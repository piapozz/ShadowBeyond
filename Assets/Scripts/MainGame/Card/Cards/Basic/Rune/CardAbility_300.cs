using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_300 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        // 【ファンファーレ】【モード】1つを選んでその能力が働く。
        //（1）自分の手札すべては2回スペルブーストする。
        //（2）【土の秘術_1】これは + 2 / +2して【守護】を持つ。
        var targetPlayer = GetPlayer(isOwn);
        SpellBoostEffect spellBoostEffect = new SpellBoostEffect(new List<int>{2});
        spellBoostEffect.ExecuteEffect(targetPlayer.hand);
    }
}
