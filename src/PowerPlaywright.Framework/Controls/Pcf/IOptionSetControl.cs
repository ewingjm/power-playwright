namespace PowerPlaywright.Framework.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;

    /// <summary>
    /// An interface for the PowerApps.CoreControls.OptionSetControl control.
    /// </summary>
    [PcfControl("PowerApps.CoreControls.OptionSetControl")]
    public interface IOptionSetControl : IChoice, IYesNo, IWholeNumberTimezone
    {
    }
}