﻿namespace PowerPlaywright.Framework.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;

    /// <summary>
    /// An interface for the PowerApps.CoreControls.ActionInput control.
    /// </summary>
    [PcfControl("PowerApps.CoreControls.TextInput")]
    public interface ITextInput : ISingleLineTextArea, ISingleLineText, IMultiLineText
    {
    }
}