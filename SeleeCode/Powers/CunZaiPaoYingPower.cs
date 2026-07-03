using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Selee.SeleeCode.Patch;

namespace Selee.SeleeCode.Powers;

public class CunZaiPaoYingPower() : SeleePower, ISeleeHook
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterDieJiaTrigger(Player owner, CardModel? triggerCard,PlayerChoiceContext? choiceContext = null)
    {
        if (owner.Creature == Owner)
        {
            var hittableEnemies = base.CombatState!.HittableEnemies;
            if (hittableEnemies.Count > 0)
            {
                Creature target = base.Owner.Player!.RunState.Rng.CombatTargets.NextItem(hittableEnemies)!;
                if (target!=null)
                {
                    VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_attack_blunt");
                    await CreatureCmd.Damage(choiceContext ?? new ThrowingPlayerChoiceContext(), target, base.Amount,
                        ValueProp.Unpowered, base.Owner, null, null);
                }
            }
        }
    }
}
