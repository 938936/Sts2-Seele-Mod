using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Selee.SeleeCode.Patch;

namespace Selee.SeleeCode.Powers;

public class HengChangShiYuanPower() : SeleePower, ISeleeHook
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Threshold", 3m),
        new DynamicVar("GongMingLeft", 3m),
    ];

    public override int DisplayAmount => DynamicVars["GongMingLeft"].IntValue;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        DynamicVars["Threshold"].BaseValue = base.Amount;
        DynamicVars["GongMingLeft"].BaseValue = base.Amount;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this),
        HoverTipFactory.FromKeyword(SeleeCardKeyword.GongMing),
    ];

    public async Task AfterGongMingTrigger(Player owner, CardModel triggerCard)
    {
        if (owner.Creature == Owner && Owner.HasPower<XingHuanBaoFaPower>() && triggerCard.Type==CardType.Attack)
        {
            Flash();
            int newLeft = DynamicVars["GongMingLeft"].IntValue - 1;
            if (newLeft <= 0)
            {
                await PlayerCmd.GainEnergy(1, owner);
                newLeft += DynamicVars["Threshold"].IntValue;
            }
            DynamicVars["GongMingLeft"].BaseValue = newLeft;
            InvokeDisplayAmountChanged();
        }
    }
}
