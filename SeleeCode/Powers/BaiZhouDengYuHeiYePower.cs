using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
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
            HookPlayerChoiceContext hookPlayerChoiceContext = new HookPlayerChoiceContext(owner, LocalContext.NetId??0, GameActionType.Combat);
            await CardPileCmd.Draw(hookPlayerChoiceContext, base.Amount, owner);
        }
    }
}
