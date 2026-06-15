using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Selee.SeleeCode.Patch;

namespace Selee.SeleeCode.Powers;

public class BaiZhouDengYuHeiYePower() : SeleePower, ISeleeHook
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterDieJiaTrigger(Player owner, CardModel? triggerCard)
    {
        if (owner.Creature == Owner)
        {
            Flash();
            await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), base.Amount, owner);
        }
    }
}
