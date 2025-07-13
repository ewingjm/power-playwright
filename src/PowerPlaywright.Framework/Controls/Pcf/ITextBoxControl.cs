namespace PowerPlaywright.Framework.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;

    /// <summary>
    /// An interface for the MscrmControls.FieldControls.TextBox control.
    /// </summary>
    [PcfControl("MscrmControls.FieldControls.TextBoxControl")]
    public interface ITextBoxControl : ISingleLineText, ISingleLineTextArea, IMultiLineText
    {
    }
}