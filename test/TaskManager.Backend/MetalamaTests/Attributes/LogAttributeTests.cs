using Microsoft.Extensions.Logging;
using Moq;

namespace Metalama.Attributes.Tests
{
    [TestFixture]
    public class LogAttributeTests
    {
        private Mock<ILogger<MyClass>> mockLogger;
        private StringWriter logOutput;
        private MyClass myClass;

        [SetUp]
        public void SetUp()
        {
            mockLogger = new Mock<ILogger<MyClass>>();
            logOutput = new StringWriter();
            mockLogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            mockLogger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()))
                .Callback<LogLevel, EventId, object, Exception, Delegate>((logLevel, eventId, state, exception, formatter) =>
                {
                    logOutput.WriteLine(state);
                });
            myClass = new MyClass(mockLogger.Object);
        }
        [TearDown]
        public void TearDown()
        {
            logOutput.Dispose();
        }

        [Test]
        public void Method_WithLogAttribute_ShouldLogEntryAndExit()
        {
            // Act
            myClass.VoidMethod(5);
            // Assert
            var logs = logOutput.ToString();
            Assert.IsTrue(logs.Contains("started"));
            Assert.IsTrue(logs.Contains("Oops"));
            Assert.IsTrue(logs.Contains("succeeded"));
        }
        [Test]
        public void Method_WithLogAttribute_ShouldLogResultEntryAndExit()
        {
            // Act
            myClass.ResultMethod(5);
            // Assert
            var logs = logOutput.ToString();
            Assert.IsTrue(logs.Contains("returned"));
            Assert.IsTrue(logs.Contains("5"));
        }
        [Test]
        public void Method_WithLogAttribute_ShouldRedactSensitiveParams()
        {
            // Act
            myClass.SensitiveMethod(50);
            // Assert
            var logs = logOutput.ToString();
            Assert.IsTrue(logs.Contains("redacted"));
        }
        [Test]
        public void Method_WithLogAttribute_ThrowsException_ShouldLogException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => myClass.ExceptionMethod(5));
            // Assert
            var logs = logOutput.ToString();
            Assert.IsTrue(logs.Contains("Oops"));
            Assert.IsTrue(logs.Contains("failed"));
        }
    }

    public class MyClass
    {
        private readonly ILogger<MyClass> logger;

        public MyClass(ILogger<MyClass> logger)
        {
            this.logger = logger;
        }

        [Log]
        public void VoidMethod(int p)
        {
            logger.LogInformation("Oops");
        }
        [Log]
        public int ResultMethod(int p)
        {
            logger.LogInformation("Oops");
            return p;
        }
        [Log]
        public int SensitiveMethod(int password)
        {
            logger.LogInformation("Oops");
            return password;
        }
        [Log]
        public void ExceptionMethod(int p)
        {
            logger.LogInformation("Oops");
            throw new Exception("Error in method");
        }
    }
}