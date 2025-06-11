namespace PowerPlaywright.TestApp.Model.Search
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Maps the standard attributes of the search response.
    /// </summary>
    public class Attributes
    {
        /// <summary>
        /// Gets or sets search ObjectTypeCode.
        /// </summary>
        [JsonProperty("@search.objecttypecode")]
        public int SearchObjectTypeCode { get; set; }

        /// <summary>
        /// Gets or sets search OwnerId.
        /// </summary>
        [JsonProperty("ownerid")]
        public string? OwnerId { get; set; }

        /// <summary>
        /// Gets or sets search OwnerIdName.
        /// </summary>
        [JsonProperty("owneridname")]
        public string? OwnerIdName { get; set; }

        /// <summary>
        /// Gets or sets search OwnerIdLogicalName.
        /// </summary>
        [JsonProperty("@search.ownerid.logicalname")]
        public string? OwnerIdLogicalName { get; set; }

        /// <summary>
        /// Gets or sets search OwningBusinessUnit.
        /// </summary>
        [JsonProperty("owningbusinessunit")]
        public string? OwningBusinessUnit { get; set; }

        /// <summary>
        /// Gets or sets search OwningBusinessUnitName.
        /// </summary>
        [JsonProperty("owningbusinessunitname")]
        public string? OwningBusinessUnitName { get; set; }

        /// <summary>
        /// Gets or sets search OwningBusinessUnitLogicalName.
        /// </summary>
        [JsonProperty("@search.owningbusinessunit.logicalname")]
        public string? OwningBusinessUnitLogicalName { get; set; }

        /// <summary>
        /// Gets or sets search CreatedOn.
        /// </summary>
        [JsonProperty("createdon")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets search ModifiedOn.
        /// </summary>
        [JsonProperty("modifiedon")]
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets search createdOnFormatted.
        /// </summary>
        [JsonProperty("createdon@OData.Community.Display.V1.FormattedValue")]
        public string? CreatedOnFormatted { get; set; }

        /// <summary>
        /// Gets or sets search ModifiedOnFormatted.
        /// </summary>
        [JsonProperty("modifiedon@OData.Community.Display.V1.FormattedValue")]
        public string? ModifiedOnFormatted { get; set; }
    }
}