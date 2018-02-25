using PaymentApp.Controllers;
using System;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentApp.Models.Payments;
using System.IO;

namespace PaymentApp.Tests
{
    public class PaymentsControllerTests
    {
        [Fact]
        public void Create_PaymentFolderNotSet_ErrorMessageSet()
        {
            //Arrange
            var confMock = Mock.Of<IConfiguration>();
            var loggerMock = Mock.Of<ILogger<PaymentsController>>();
            PaymentsController sut = new PaymentsController(loggerMock,confMock);

            //Act
            PaymentInfo model = new PaymentInfo();
            model.AccountName = null;
            sut.Create(model);

            //Assert
            Assert.EndsWith("ended in errors.", sut.ViewBag.Message);
        }

        [Fact]
        public void Create_PaymentFolderNotFound_ErrorMessageSet()
        {
            //Arrange
            var confMock = new Mock<IConfiguration>();
            confMock.Setup(x => x["PaymentInfoPath"]).Returns(@"C:\Project\Logs.txt");
            
            var loggerMock = Mock.Of<ILogger<PaymentsController>>();
            PaymentsController sut = new PaymentsController(loggerMock, confMock.Object);

            //Act
            PaymentInfo model = new PaymentInfo();
            model.AccountName = null;
            sut.Create(model);

            //Assert
            Assert.EndsWith("ended in errors.", sut.ViewBag.Message);
        }

        [Fact]
        public void Create_PaymentFolderNotAccessible_ErrorMessageSet()
        {
            //Arrange
            var confMock = new Mock<IConfiguration>();
            confMock.Setup(x => x["PaymentInfoPath"]).Returns(@"C:\Logs.txt");

            var loggerMock = Mock.Of<ILogger<PaymentsController>>();
            PaymentsController sut = new PaymentsController(loggerMock, confMock.Object);

            //Act
            PaymentInfo model = new PaymentInfo();
            model.AccountName = null;
            sut.Create(model);

            //Assert
            Assert.EndsWith("ended in errors.", sut.ViewBag.Message);
        }

        [Fact]
        public void Create_ValidPaymentFolder_SuccessMessageSet()
        {
            //Arrange
            var confMock = new Mock<IConfiguration>();
            confMock.Setup(x => x["PaymentInfoPath"]).Returns(@"E:\Logs.txt");

            var loggerMock = Mock.Of<ILogger<PaymentsController>>();
            PaymentsController sut = new PaymentsController(loggerMock, confMock.Object);

            //Act
            PaymentInfo model = new PaymentInfo();
            sut.Create(model);

            //Assert
            Assert.Contains("has been completed successfully.", sut.ViewBag.Message);
        }

        [Fact]
        public void Create_ValidPaymentFolder_PaymentDetailsUpdated()
        {
            //Arrange
            var confMock = new Mock<IConfiguration>();
            confMock.Setup(x => x["PaymentInfoPath"]).Returns(@"E:\Logs.txt");
            using (var writer = new StreamWriter(@"E:\Logs.txt"))
            {
                writer.Flush();
            }

            var loggerMock = Mock.Of<ILogger<PaymentsController>>();
            PaymentsController sut = new PaymentsController(loggerMock, confMock.Object);

            //Act
            PaymentInfo model = new PaymentInfo()
            {
                AccountName = "ABC XYZ",
                AccountNumber = "123456789",
                BSBNumber = "987654",
                Reference = 367,
                Amount = 3400,
                Description = "Credit Card Payment"
            };
            sut.Create(model);

            //Assert
            Assert.Contains("has been completed successfully.", sut.ViewBag.Message);
            using (var reader = new StreamReader(@"E:\Logs.txt"))
            {
                var fileData = reader.ReadToEnd();
                Assert.Contains($"The below transaction was done successfully at {DateTime.Now.Date.ToString("M/d/yyyy")}", fileData);
                Assert.Contains("Account Name : ABC XYZ", fileData);
                Assert.Contains("Account Number : 123456789", fileData);
                Assert.Contains("BSB Number : 987654", fileData); 
                Assert.Contains("Amount : 3400$", fileData);
                Assert.Contains("Reference : 367", fileData);
                Assert.Contains("Description : Credit Card Payment", fileData);
            }
        }

        [Fact]
        public void Create_ValidPaymentDetails_PaymentDetailsAppended()
        {
            //Arrange
            var confMock = new Mock<IConfiguration>();
            confMock.Setup(x => x["PaymentInfoPath"]).Returns(@"E:\Logs.txt");
            using (var writer = new StreamWriter(@"E:\Logs.txt"))
            {
                writer.Flush();
            }

            var loggerMock = Mock.Of<ILogger<PaymentsController>>();
            PaymentsController sut = new PaymentsController(loggerMock, confMock.Object);

            //Act
            PaymentInfo model1 = new PaymentInfo()
            {
                AccountName = "ABC XYZ",
                AccountNumber = "123456789",
                BSBNumber = "987654",
                Reference = 367,
                Amount = 3400
            };
            sut.Create(model1);

            PaymentInfo model2 = new PaymentInfo()
            {
                AccountName = "New Person",
                AccountNumber = "3432432423",
                BSBNumber = "456-656",
                Reference = 2322222,
                Amount = 670
            };
            sut.Create(model2);

            //Assert
            Assert.Contains("has been completed successfully.", sut.ViewBag.Message);
            using (var reader = new StreamReader(@"E:\Logs.txt"))
            {
                var fileData = reader.ReadToEnd();
                Assert.Contains($"The below transaction was done successfully at {DateTime.Now.Date.ToString("M/d/yyyy")}", fileData);

                //file contains first set of payment data
                Assert.Contains("Account Name : ABC XYZ", fileData);
                Assert.Contains("Account Number : 123456789", fileData);
                Assert.Contains("BSB Number : 987654", fileData);
                Assert.Contains("Amount : 3400$", fileData);
                Assert.Contains("Reference : 367", fileData);

                //same file contains second set of payment data
                Assert.Contains("Account Name : New Person", fileData);
                Assert.Contains("Account Number : 3432432423", fileData);
                Assert.Contains("BSB Number : 456-656", fileData);
                Assert.Contains("Amount : 670", fileData);
                Assert.Contains("Reference : 2322222", fileData);
            }
        }

        [Fact]
        public void Create_DecimalAmount_PaymentDetailsUpdated()
        {
            //Arrange
            var confMock = new Mock<IConfiguration>();
            confMock.Setup(x => x["PaymentInfoPath"]).Returns(@"E:\Logs.txt");
            using (var writer = new StreamWriter(@"E:\Logs.txt"))
            {
                writer.Flush();
            }

            var loggerMock = Mock.Of<ILogger<PaymentsController>>();
            PaymentsController sut = new PaymentsController(loggerMock, confMock.Object);

            //Act
            PaymentInfo model1 = new PaymentInfo()
            {
                AccountName = "ABC XYZ",
                AccountNumber = "123456789",
                BSBNumber = "987654",
                Reference = 367,
                Amount = 3400.55
            };
            sut.Create(model1);


            //Assert
            Assert.Contains("has been completed successfully.", sut.ViewBag.Message);
            using (var reader = new StreamReader(@"E:\Logs.txt"))
            {
                var fileData = reader.ReadToEnd();
                Assert.Contains($"The below transaction was done successfully at {DateTime.Now.Date.ToString("M/d/yyyy")}", fileData);
                Assert.Contains("Account Name : ABC XYZ", fileData);
                Assert.Contains("Account Number : 123456789", fileData);
                Assert.Contains("BSB Number : 987654", fileData);
                Assert.Contains("Amount : 3400.55$", fileData);
                Assert.Contains("Reference : 367", fileData);

            }
        }
    }
}
