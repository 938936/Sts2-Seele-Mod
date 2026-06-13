using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Selee.SeleeCode.Powers;

public class FenYangFengLingDeYaoYuePower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2),
        new CardsVar(2),
    ];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is XingHuanBaoFaPower && power.Owner == base.Owner && amount > 0m)
        {
            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, base.Owner.Player!);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, base.Owner.Player!);
            await PowerCmd.Remove(this);
        }
    }
}
