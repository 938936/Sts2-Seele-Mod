using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Selee.SeleeCode.Patch;

namespace Selee.SeleeCode.Powers;

public class JianBingDongNengPower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("DamageMultiplier", 2m),
    ];
    
    private class GongMingCardPlayData
    {
        public readonly HashSet<CardModel> GongMingTriggerCards = new ();
    }
    
    protected override object InitInternalData()
    {
        return new GongMingCardPlayData();
    }
    

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != base.Owner) return 1m;
        if (!props.IsPoweredAttack() || cardSource == null) return 1m;
        return DynamicVars["DamageMultiplier"].BaseValue;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        //先记录触发共鸣的卡，再卡牌结算后减少共鸣层数，以避免卡牌效果过程中新获得的简并动能层数会被自身触发而减少。
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner.Creature == Owner && Amount>0)
        {
            GetInternalData<GongMingCardPlayData>().GongMingTriggerCards.Add(cardPlay.Card);
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (GetInternalData<GongMingCardPlayData>().GongMingTriggerCards.Remove(cardPlay.Card))
        {
            Flash();
            await PowerCmd.Decrement(this);
            await SeleeHook.AfterGongMingTrigger(base.Owner.Player!, cardPlay.Card, choiceContext);
            if (base.Amount <= 0 && base.Owner.HasPower<XingHuanBaoFaPower>())
            {
                await PowerCmd.Remove<XingHuanBaoFaPower>(base.Owner);
            }
        }
    }
}
