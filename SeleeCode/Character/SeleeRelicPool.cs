using BaseLib.Abstracts;
using Selee.SeleeCode.Extensions;
using Godot;

namespace Selee.SeleeCode.Character;

public class SeleeRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Selee.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}