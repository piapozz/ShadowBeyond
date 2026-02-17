using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

// 【ファンファーレ】『覇道の金龍』1枚と『覇道の銀龍』1枚を自分の場に出す。
// 【超進化時】自分の場の『覇道の金龍』すべては【疾走】を持つ。自分の場の『覇道の銀龍』すべては【バリア】を持つ。
public class CardAbility_001418 : BaseCardAbility
{
    private const int Supreme_Golden_Dragon_ID = 001421;
    private const int Supreme_Silver_Dragon_ID = 001422;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { Supreme_Golden_Dragon_ID, 1 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
        enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { Supreme_Silver_Dragon_ID, 1 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
    }

    public override void SuperEvolve(bool isOwn, List<BaseComponent> selected = null)
    {
        List<CardData> ownFieldCards = BattleManager.instance.field.GetCards((card) => { return card.id == Supreme_Golden_Dragon_ID || card.id == Supreme_Silver_Dragon_ID; }, isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        foreach (CardData card in ownFieldCards)
        {
            if (card.id == Supreme_Golden_Dragon_ID)
            {
                GiveKeywordAbilityEffect giveKeywordAbilityEffect = new GiveKeywordAbilityEffect(new List<int> { (int)GameEnum.KeywordAbility.Storm });
                giveKeywordAbilityEffect.ExecuteEffect(card);
            }
            else if (card.id == Supreme_Silver_Dragon_ID)
            {
                //AddKeywordEffect addKeywordEffect = new AddKeywordEffect(new List<int> { (int)GameEnum.KeywordAbility.Barrier });
                //addKeywordEffect.ExecuteEffect(card);
            }
        }
    }
}
