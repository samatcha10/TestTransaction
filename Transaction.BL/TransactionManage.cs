using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction.DAL.EF;
using Transaction.Model;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Transaction.BL
{
    public class TransactionManage
    {
        private readonly RRContext _context;
        private readonly ILogger _logger; 

        public TransactionManage(RRContext Context, ILogger Logger)
        {
            _context = Context;
            _logger = Logger;  
        }

        public async Task<List<TransactionModel>> GetAllTransactionListFromCurrency(string Currency)
        {
            try
            {
                var _query = from tr in _context.TestTable
                             where tr.CurrencyCode == Currency
                             select new TransactionModel
                             {
                                 id = tr.TransactionId,
                                 payment = tr.Amount.ToString() + " " + tr.CurrencyCode,
                                 Status = tr.Status
                             };
                 
                return await Task.Run(() => _query.ToList());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex;
            }
        }

        public async Task<List<TransactionModel>> GetAllTransactionListFromDate(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var _query = from tr in _context.TestTable
                             where tr.TransactionDate >= StartDate && tr.TransactionDate <= EndDate
                             select new TransactionModel
                             {
                                 id = tr.TransactionId,
                                 payment = tr.Amount.ToString() + " " + tr.CurrencyCode,
                                 Status = tr.Status
                             };

                return await Task.Run(() => _query.ToList());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex;
            }
        }

        public async Task<List<TransactionModel>> GetAllTransactionListFromStatus(string Status)
        {
            try
            {
                var _query = from tr in _context.TestTable
                             where tr.FileStatus == Status
                             select new TransactionModel
                             {
                                 id = tr.TransactionId,
                                 payment = tr.Amount.ToString() + " " + tr.CurrencyCode,
                                 Status = tr.Status
                             };

                return await Task.Run(() => _query.ToList());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex;
            }
        }

        public async Task<string> UpdateFile(IFormFile Model)
        {

            try
            {
                if (Model != null)
                {
                    string[] strName = Model.FileName.Split('.');
                    string type = strName[strName.Length - 1];

                    if (type.ToUpper() == "CSV")
                    {
                        await ProcessCSV(Model);
                        return "Success";
                    }
                    else if (type.ToUpper() == "XML")
                    {
                        ProcessXML(Model);
                        return "Success";
                    }
                    else
                        return "Unknown format";

                }

                return "Unknown format";
            }
            catch (Exception ex)
            { 
                _logger.LogError(ex.Message, ex);
                throw ex;
            } 
            
        }

        public async Task ProcessCSV(IFormFile file)
        {
            List<TransactionInsertModel> dataList = new List<TransactionInsertModel>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                { 
                    string text = reader.ReadLine();
                    string[] textData = text.Split(",");

                    TransactionInsertModel _data = new TransactionInsertModel();
                    _data.TransactionID = textData[0];
                    _data.Amount = decimal.Parse(textData[1]);
                    _data.CurrencyCode = textData[2]; 
                    _data.TransactionDate = DateTime.Parse(textData[3], new CultureInfo("en-GB")); 
                    _data.FileStatus = textData[4];

                    dataList.Add(_data);
                }
                 
            } 
             
            using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var data in dataList)
                    {
                        TestTable _testTable = new TestTable();
                        _testTable.TransactionId = data.TransactionID;
                        _testTable.Amount = data.Amount;
                        _testTable.CurrencyCode = data.CurrencyCode;
                        _testTable.TransactionDate = data.TransactionDate;
                        _testTable.FileType = "CSV";
                        _testTable.FileStatus = data.FileStatus;
                        
                        switch(_testTable.FileStatus)
                        {
                            case "Approved":
                                _testTable.Status = "A"; break;
                            case "Failed":
                                _testTable.Status = "R"; break;
                            case "Finished":
                                _testTable.Status = "D"; break; 
                        }

                        _context.TestTable.Add(_testTable);
                    }

                    _context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    _logger.LogError(ex.Message, ex);
                    throw ex;
                }
            }
        }

        public void ProcessXML(IFormFile file)
        {
            List<TransactionInsertModel> dataList = new List<TransactionInsertModel>();

            //XmlDocument doc = new XmlDocument();
            //doc.Load(file.OpenReadStream());

            //TransactionsXMLModel _tr = JsonConvert.DeserializeObject<TransactionsXMLModel>(doc.OuterXml);


            XmlDocument doc = new XmlDocument();
            doc.Load(file.OpenReadStream());

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                TransactionInsertModel _data = new TransactionInsertModel();
                _data.TransactionID = node.Attributes["id"].Value;

                var childList = node.ChildNodes;
                foreach (XmlNode child in childList)
                {
                    if (child.Name == "TransactionDate")
                        _data.TransactionDate = DateTime.Parse(child.InnerText, new CultureInfo("en-GB"));
                    if (child.Name == "Status")
                        _data.FileStatus = child.InnerText;

                    if (child.Name == "PaymentDetails")
                    {
                        var subChildList = child.ChildNodes;
                        foreach (XmlNode subChild in subChildList)
                        {
                            if (subChild.Name == "Amount")
                                _data.Amount = decimal.Parse(subChild.InnerText);
                            if (subChild.Name == "CurrencyCode")
                                _data.CurrencyCode = subChild.InnerText;
                        }
                    }
                }

                dataList.Add(_data);
            }

            try
            {
                foreach (var data in dataList)
                {
                    TestTable _testTable = new TestTable();
                    _testTable.TransactionId = data.TransactionID;
                    _testTable.Amount = data.Amount;
                    _testTable.CurrencyCode = data.CurrencyCode;
                    _testTable.TransactionDate = data.TransactionDate;
                    _testTable.FileType = "XML";
                    _testTable.FileStatus = data.FileStatus;

                    switch (_testTable.FileStatus)
                    {
                        case "Approved":
                            _testTable.Status = "A"; break;
                        case "Rejected":
                            _testTable.Status = "R"; break;
                        case "Done":
                            _testTable.Status = "D"; break;
                    }

                    _context.TestTable.Add(_testTable);
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
                throw ex;
            }


        }
    }
}
