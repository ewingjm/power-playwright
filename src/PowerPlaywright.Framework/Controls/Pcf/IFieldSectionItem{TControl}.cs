namespace PowerPlaywright.Framework.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;

    /// <summary>
    /// An interface for the MscrmControls.Containers.FieldSectionItem control with a known child control type.
    /// </summary>
    [PcfControl("MscrmControls.Containers.FieldSectionItem")]
    public interface IFieldSectionItem<out TControl> : IField, IField<TControl>
        where TControl : IPcfControl
    {
    }
}
