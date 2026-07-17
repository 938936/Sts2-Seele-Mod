using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class GanLinHuiXiang() : SeleeCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.GongMing];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5m, ValueProp.Move),
    ];

    private bool HasGongMing =>
        base.IsMutable && base.Owner != null && base.Owner.Creature.HasPower<JianBingDongNengPower>();

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && base.Owner.Creature.HasPower<JianBingDongNengPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this,cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
    
    protected override CardLocation GetResultLocationForCardPlay()
    {
        CardLocation resultLocationForCardPlay = base.GetResultLocationForCardPlay();
        if (resultLocationForCardPlay.pileType == PileType.Discard  && HasGongMing)
        {
            resultLocationForCardPlay.pileType = PileType.Hand;
        }
        return resultLocationForCardPlay;
    }
    
    // protected override (PileType, CardPilePosition) GetResultPileTypeAndPositionForCardPlay()
    // {
    //     var (pileType, position) = base.GetResultPileTypeAndPositionForCardPlay();
    //     if (pileType == PileType.Discard && HasGongMing)
    //     {
    //         return (PileType.Hand, position);
    //     }
    //     return (pileType, position);
    // }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
