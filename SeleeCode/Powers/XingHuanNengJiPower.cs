using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Selee.SeleeCode.Powers;

public class XingHuanNengJiPower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Threshold", 100m),
    ];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && base.Amount >= DynamicVars["Threshold"].IntValue)
        {
            Flash();
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<XingHuanBaoFaPower>(new ThrowingPlayerChoiceContext(), base.Owner, XingHuanBaoFaPower.BaseDurationTurn, base.Owner, null);
        }
    }
}
