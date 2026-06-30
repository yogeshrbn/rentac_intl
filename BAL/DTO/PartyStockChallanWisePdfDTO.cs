using System.Collections.Generic;
using Newtonsoft.Json;

namespace BAL.DTO
{
    /// <summary>
    /// Request body for party stock register (challan-wise) PDF export.
    /// Rows mirror the challan-wise grid built on the Party Stock Register screen.
    /// </summary>
    public class PartyStockChallanWisePdfRequest
    {
        public int LedgerId { get; set; }
        public int LedgerSiteId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public bool Print { get; set; }
        public bool Pdf { get; set; }

        /// <summary>Movement lines grouped by item (same shape as Angular challanWiseRows).</summary>
        public List<PartyStockChallanWisePdfRow> Rows { get; set; }
    }

    /// <summary>One printed line: one date/challan bucket per item.</summary>
    public class PartyStockChallanWisePdfRow
    {
        [JsonProperty("item")]
        public string Item { get; set; }

        /// <summary>Opening balance; usually only the first line per item has a value.</summary>
        [JsonProperty("openingBalance")]
        public double? OpeningBalance { get; set; }

        [JsonProperty("displayDate")]
        public string DisplayDate { get; set; }

        [JsonProperty("challanNo")]
        public string ChallanNo { get; set; }

        [JsonProperty("issue")]
        public double Issue { get; set; }

        [JsonProperty("receive")]
        public double Receive { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }

        /// <summary>Closing balance from stock; usually only the last line per item has a value.</summary>
        [JsonProperty("closingBalance")]
        public double? ClosingBalance { get; set; }
    }
}
