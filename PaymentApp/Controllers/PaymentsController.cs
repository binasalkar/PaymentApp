using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentApp.Models.Payments;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace PaymentApp.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _configuration;
        private const string filePath = @"C:\Log.txt";

        public PaymentsController(ILogger<PaymentsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(PaymentInfo model)
        {
            if(ModelState.IsValid)
            {
                var paymentFilePath = _configuration["PaymentInfoPath"];
                if(!System.IO.Directory.Exists(Path.GetDirectoryName(paymentFilePath)))
                {
                    _logger.LogError($"FilePath {paymentFilePath} not found. The payment of {model.Amount} AUD from {model.AccountName} with AccountNumber {model.BSBNumber}/{model.AccountNumber} and Reference - {model.Reference} has ended in errors");
                    ViewBag.Message = $"The payment of {model.Amount} AUD from {model.AccountName} has ended in errors";
                    return View(model);
                }
                using (StreamWriter writer = new StreamWriter(paymentFilePath, true))
                {
                    writer.WriteLine($"The below Payment is received successfully at {DateTime.Now}");
                    writer.WriteLine($"Account Name : {model.AccountName}");
                    writer.WriteLine($"Account Number : {model.AccountNumber}");
                    writer.WriteLine($"BSB Number : {model.BSBNumber}");
                    writer.WriteLine($"Amount : {model.Amount}$");
                    writer.WriteLine($"Reference : {model.Reference}$");
                    writer.WriteLine();
                }

            }
            _logger.LogInformation($"The payment of {model.Amount} AUD has been received successfully from {model.AccountName} with AccountNumber {model.BSBNumber}/{model.AccountNumber} and Reference - {model.Reference}");
            ViewBag.Message = $"The payment of {model.Amount} AUD has been received successfully from {model.AccountName} ";
            return View(new PaymentInfo());
        }
    }
}