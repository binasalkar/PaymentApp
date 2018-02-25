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
            if (ModelState.IsValid)
            {
                var paymentFilePath = _configuration["PaymentInfoPath"];
                if(string.IsNullOrEmpty(paymentFilePath))
                {
                    _logger.LogError($"Payment File Path not set. Transaction Details - Account Name: {model.AccountName}, Amount: {model.Amount} AUD,  Account Number: {model.BSBNumber}/{model.AccountNumber}, Reference: {model.Reference}");
                    ViewBag.Message = $"The trasaction initiated by {model.AccountName} has ended in errors.";
                    return View(model);

                }
                try
                {
                    if (!System.IO.Directory.Exists(Path.GetDirectoryName(paymentFilePath)))
                    {
                        _logger.LogError($"FilePath {paymentFilePath} not found. Transaction Details - Account Name: {model.AccountName}, Amount: {model.Amount} AUD,  Account Number: {model.BSBNumber}/{model.AccountNumber}, Reference: {model.Reference}");
                        ViewBag.Message = $"The trasaction initiated by {model.AccountName} has ended in errors.";
                        return View(model);
                    }
                    using (StreamWriter writer = new StreamWriter(paymentFilePath, true))
                    {
                        writer.WriteLine($"The below transaction was done successfully at {DateTime.Now}");
                        writer.WriteLine($"Account Name : {model.AccountName}");
                        writer.WriteLine($"Account Number : {model.AccountNumber}");
                        writer.WriteLine($"BSB Number : {model.BSBNumber}");
                        writer.WriteLine($"Amount : {model.Amount}$");
                        writer.WriteLine($"Reference : {model.Reference}");
                        writer.WriteLine($"Description : {model.Description}");
                        writer.WriteLine();
                    }


                    _logger.LogInformation($"The trasaction has been completed successfully. Transaction Details - Account Name: {model.AccountName}, Amount: {model.Amount} AUD,  Account Number: {model.BSBNumber}/{model.AccountNumber}, Reference: {model.Reference}");
                    ViewBag.Message = $"The trasaction initiated by  {model.AccountName} has been completed successfully.";
                    return View(new PaymentInfo());
                }
                
                catch (Exception ex)
                {
                    _logger.LogInformation($"The exception has occured while processing the trasaction. Transaction Details - Account Name: {model.AccountName}, Amount: {model.Amount} AUD,  Account Number: {model.BSBNumber}/{model.AccountNumber}, Reference: {model.Reference}. Exception Details - {ex.ToString()}");
                    ViewBag.Message = $"The transaction initiated by {model.AccountName} has ended in errors.";
                    return View(model);

                }
            }
            _logger.LogError($"ModelState is invalid. Transaction Details - Account Name: {model.AccountName}, Amount: {model.Amount} AUD,  Account Number: {model.BSBNumber}/{model.AccountNumber}, Reference: {model.Reference}");
            ViewBag.Message = $"The trasaction initiated by {model.AccountName} has ended in errors.";
            return View(model);

        }
    }
}