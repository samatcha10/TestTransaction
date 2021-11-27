using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Transaction.Model
{
    public class TransactionModel
    {
        public string id { get; set; }
        public string payment { get; set; }
        public string Status { get; set; }
    }

    public class TransactionInsertModel
    {
        public string TransactionID { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public string FileType { get; set; }
        public string FileStatus { get; set; }
    }

    public class TransactionDateParamModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class TransactionCurrencyParamModel
    {
        public string CurrencyCode { get; set; }
    }

    public class TransactionStatusParamModel
    {
        public string Status { get; set; }
    }



    [JsonObject(MemberSerialization.OptIn)] 
    public class TransactionsXMLModel
    {
        [JsonProperty]
        public TransactionsXML Transactions { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class TransactionsXML
    {
        [JsonProperty]
        public List<TransactionID> Transaction { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class TransactionID
    {
        //[JsonProperty]
        //public int id { get; set; }

        [JsonProperty]
        public string TransactionDate { get; set; }

        [JsonProperty]
        public string Status { get; set; }

        [JsonProperty]
        public List<PaymentDetails> PaymentDetails { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class PaymentDetails
    {
        [JsonProperty]
        public decimal Amount { get; set; }

        [JsonProperty]
        public string CurrencyCode { get; set; }
    }
}
