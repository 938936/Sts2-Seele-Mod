using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Selee.SeleeCode.Patch;

namespace Selee.SeleeCode.Powers;

public class LianLiXingPower() : SeleePower, ISeleeHook
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("BaoFaBlock", 0m),
    ];

    public async Task AfterGongMingTrigger(Player owner, CardModel triggerCard)
    {
        if (owner.Creature == Owner)
        {
            Flash();
            decimal block = base.Amount;
            if (Owner.HasPower<XingHuanBaoFaPower>())
            {
                block += DynamicVars["BaoFaBlock"].BaseValue;
            }
            await CreatureCmd.GainBlock(Owner, block, ValueProp.Unpowered, null);
        }
    }
}
