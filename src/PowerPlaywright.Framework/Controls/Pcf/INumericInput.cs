namespace PowerPlaywright.Framework.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;

    /// <summary>
    /// An interface for the PowerApps.CoreControls.NumericInput control.
    /// </summary>
    [PcfControl("PowerApps.CoreControls.NumericInput")]
    public interface INumericInput : ICurrencyControl, IDecimalNumberControl, IFloatingPointNumberControl, IWholeNumberControl
    {
    }
}