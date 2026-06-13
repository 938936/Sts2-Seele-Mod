using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Selee.SeleeCode.Powers;

public class XingHuanYingYuPower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is XingHuanBaoFaPower && power.Owner == base.Owner && amount > 0m)
        {
            if (power.Amount == amount)
            {
                Flash();
                await PowerCmd.Apply<XingHuanNengJiPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
            }
        }
    }
}
