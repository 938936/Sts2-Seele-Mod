using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Powers;

public class XueDingEDeXiErPower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Threshold", 0m),
        new DynamicVar("CardsLeft", 0m),
    ];

    public override int DisplayAmount => base.DynamicVars["CardsLeft"].IntValue;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        DynamicVars["Threshold"].BaseValue = base.Amount;
        DynamicVars["CardsLeft"].BaseValue = base.Amount;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<LiangZiDieJiaPower>(),
    ];

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner && result.UnblockedDamage > 0)
        {
            Flash();
            int threshold = DynamicVars["Threshold"].IntValue;
            int newAmount = DynamicVars["CardsLeft"].IntValue - result.UnblockedDamage;
            int grantedCount = 0;

            while (newAmount <= 0)
            {
                grantedCount++;
                newAmount += threshold;
            }

            if (grantedCount > 0)
            {
                await PowerCmd.Apply<LiangZiDieJiaPower>(choiceContext, base.Owner, grantedCount, base.Owner, null);
            }

            DynamicVars["CardsLeft"].BaseValue = newAmount;
            InvokeDisplayAmountChanged();
        }
    }
}
