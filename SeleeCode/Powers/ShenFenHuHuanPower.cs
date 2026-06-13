using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Powers;

public class ShenFenHuHuanPower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("BlockPercent",50m)
    ];

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side==Owner.Side && participants.Contains(base.Owner))
        {
            Flash();
            if (Applier != null)
            {
                await CreatureCmd.GainBlock(base.Applier, base.Owner.Block*DynamicVars["BlockPercent"].BaseValue/100m
                    , ValueProp.Unpowered, null);
            }
            await PowerCmd.Remove(this);
        }
    }
}
