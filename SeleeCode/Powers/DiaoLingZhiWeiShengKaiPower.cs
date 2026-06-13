using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;

namespace Selee.SeleeCode.Powers;

public class DiaoLingZhiWeiShengKaiPower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (!Owner.IsDead)
        {
            Flash();
            await CreatureCmd.Heal(base.Owner, base.Amount);
        }
    }
}
