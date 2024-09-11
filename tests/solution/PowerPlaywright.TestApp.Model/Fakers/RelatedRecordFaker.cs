namespace PowerPlaywright.TestApp.Model.Fakers
{
    using Bogus;

    /// <summary>
    /// A faker for the <see cref="pp_RelatedRecord"/> class.
    /// </summary>
    public class RelatedRecordFaker : Faker<pp_RelatedRecord>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedRecordFaker"/> class.
        /// </summary>
        public RelatedRecordFaker()
        {
            this.RuleFor(r => r.pp_RelatedRecordId, f => Guid.NewGuid());
            this.RuleFor(r => r.pp_Name, f => f.Random.AlphaNumeric(100));
        }
    }
}
