namespace PowerPlaywright.TestApp.Model.Fakers
{
    using Bogus;
    using Microsoft.Xrm.Sdk;
    using PowerPlaywright.TestApp.Model.Extensions;

    /// <summary>
    /// A faker for the <see cref="pp_Record"/> class.
    /// </summary>
    public class RecordFaker : Faker<pp_Record>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordFaker"/> class.
        /// </summary>
        public RecordFaker()
        {
            this.Locale = "en_GB";
            this.RuleFor(r => r.pp_choice, f => f.PickRandom<pp_record_pp_choice>());
            this.RuleFor(r => r.pp_choices, f => f.Random.EnumValuesRange<pp_record_pp_choices>(1));
            this.RuleFor(r => r.pp_currency, f => new Money(f.Random.Decimal()));
            this.RuleFor(r => r.pp_dateandtimedateandtime, f => DateTime.UtcNow);
            this.RuleFor(r => r.pp_dateandtimedateonly, f => DateTime.UtcNow.Date);
            this.RuleFor(r => r.pp_decimal, f => f.Random.Decimal());
            this.RuleFor(r => r.pp_float, f => f.Random.Float());
            this.RuleFor(r => r.pp_multiplelinesoftexttext, f => f.Lorem.Lines());
            this.RuleFor(r => r.pp_RecordId, f => Guid.NewGuid());
            this.RuleFor(r => r.pp_singlelineoftexttext, f => f.Lorem.Random.AlphaNumeric(100));
            this.RuleFor(r => r.pp_singlelineoftextemail, f => f.Internet.Email());
            this.RuleFor(r => r.pp_singlelineoftextphonenumber, f => f.Phone.PhoneNumber());
            this.RuleFor(r => r.pp_singlelineoftexttextarea, f => f.Lorem.Random.AlphaNumeric(100));
            this.RuleFor(r => r.pp_singlelineoftexttickersymbol, f => "MSFT");
            this.RuleFor(r => r.pp_singlelineoftexturl, f => f.Internet.Url());
            this.RuleFor(r => r.pp_wholenumberduration, f => f.Random.Int(0));
            this.RuleFor(r => r.pp_wholenumberlanguagecode, f => 1033);
            this.RuleFor(r => r.pp_wholenumbernone, f => f.Random.Int());
            this.RuleFor(r => r.pp_wholenumbertimezone, f => 0);
            this.RuleFor(r => r.pp_yesno, f => f.Random.Bool());
        }
    }
}