using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Selee.SeleeCode.Cards;

public class KaiQiMenFei() : SeleeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJia];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<WeakPower>(1),
        new DynamicVar("DieJiaVulnerablePower", 2m),
        new DynamicVar("DieJiaWeakPower", 2m),
    ];

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && base.Owner.Creature.HasPower<LiangZiDieJiaPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(base.CombatState);
        var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();

        if (dieJiaPower != null)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, base.CombatState.HittableEnemies, DynamicVars["DieJiaVulnerablePower"].BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<WeakPower>(choiceContext, base.CombatState.HittableEnemies, DynamicVars["DieJiaWeakPower"].BaseValue, base.Owner.Creature, this);
            await SeleeHook.AfterDieJiaTrigger(Owner, this, choiceContext);
        }
        else
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, base.CombatState.HittableEnemies, DynamicVars["WeakPower"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DieJiaVulnerablePower"].UpgradeValueBy(1m);
        DynamicVars["DieJiaWeakPower"].UpgradeValueBy(1m);
    }
}
