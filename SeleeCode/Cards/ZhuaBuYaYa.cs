using System.Linq;
using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Selee.SeleeCode.Cards;

public class ZhuaBuYaYa() : SeleeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal =>
        base.CombatState != null && base.CombatState.HittableEnemies.Any(e => e.Monster?.IntendsToAttack ?? false);

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<YuHePower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<YuHePower>(5),
        new DynamicVar("StrengthLoss", 4m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        if (cardPlay.Target.Monster?.IntendsToAttack == true)
        { 
            await PowerCmd.Apply<YuHePower>(choiceContext, base.Owner.Creature, DynamicVars["YuHePower"].BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<ZhuaBuYaYaPower>(choiceContext, cardPlay.Target, DynamicVars["StrengthLoss"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["YuHePower"].UpgradeValueBy(2m);
        DynamicVars["StrengthLoss"].UpgradeValueBy(2m);
    }
}
