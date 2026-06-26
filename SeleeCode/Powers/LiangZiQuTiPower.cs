using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Selee.SeleeCode.Patch;

namespace Selee.SeleeCode.Powers;

public class LiangZiQuTiPower() : SeleePower, ISeleeHook
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterDieJiaTrigger(Player owner, CardModel? triggerCard,PlayerChoiceContext? choiceContext = null)
    {
        if (owner.Creature == Owner)
        {
            Flash();
            await CreatureCmd.GainBlock(base.Owner, Amount, ValueProp.Unpowered, null);
        }

    }
}
