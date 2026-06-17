using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Selee.SeleeCode.Cards;

public class LiangHaoZuoXi() : SeleeCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<YuHePower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var yuHe = base.Owner.Creature.GetPower<YuHePower>();
        if (yuHe != null && yuHe.Amount > 0)
        {
            int healAmount = (int)Math.Floor(yuHe.Amount * 0.7m);
            if (healAmount > 0)
            {
                await CreatureCmd.Heal(base.Owner.Creature, healAmount);
            }
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
