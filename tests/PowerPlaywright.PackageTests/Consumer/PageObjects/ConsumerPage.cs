namespace Consumer.PageObjects;

using Microsoft.Playwright;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Controls;
using PowerPlaywright.Framework.Controls.External;
using PowerPlaywright.Framework.Controls.External.Attributes;
using PowerPlaywright.Framework.Pages;
using PowerPlaywright.Pages;

public class ConsumerPage : CustomPage
{
    public ConsumerPage(IPage page, IControlFactory controls)
        : base(page, controls)
    {
    }

    public IConsumerControl CustomControl => this.GetControl<IConsumerControl>();
}

[ExternalControl]
public interface IConsumerControl : IExternalControl
{
    string Read();
}

[ExternalControlStrategy(1)]
public class ConsumerControl : Control, IConsumerControl
{
    public ConsumerControl(IAppPage page)
        : base(page)
    {
    }

    public string Read() => "consumer control";

    protected override ILocator GetRoot(ILocator context) => context;
}
