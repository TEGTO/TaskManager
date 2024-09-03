namespace Metalama.Guards.Tests
{
    [TestFixture]
    public class LoggingRecursionGuardTests
    {
        [SetUp]
        public void SetUp()
        {
            LoggingRecursionGuard.IsLogging = false;
        }

        [Test]
        public void Begin_WhenNotAlreadyLogging_CanLogIsTrue()
        {
            // Act
            using (var cookie = LoggingRecursionGuard.Begin())
            {
                // Assert
                Assert.IsTrue(cookie.CanLog);
                Assert.IsTrue(LoggingRecursionGuard.IsLogging);
            }

            Assert.IsFalse(LoggingRecursionGuard.IsLogging);
        }

        [Test]
        public void Begin_WhenAlreadyLogging_CanLogIsFalse()
        {
            // Arrange
            LoggingRecursionGuard.IsLogging = true;

            // Act
            using (var cookie = LoggingRecursionGuard.Begin())
            {
                // Assert
                Assert.IsFalse(cookie.CanLog);
                Assert.IsTrue(LoggingRecursionGuard.IsLogging);
            }

            Assert.IsTrue(LoggingRecursionGuard.IsLogging);
        }
        [Test]
        public void Dispose_WhenCanLogIsTrue_IsLoggingIsSetToFalse()
        {
            // Act
            using (var cookie = LoggingRecursionGuard.Begin())
            {
                // Assert
                Assert.IsTrue(LoggingRecursionGuard.IsLogging);
            }

            Assert.IsFalse(LoggingRecursionGuard.IsLogging);
        }
        [Test]
        public void Dispose_WhenCanLogIsFalse_IsLoggingRemainsTrue()
        {
            // Arrange
            LoggingRecursionGuard.IsLogging = true;

            // Act
            using (var cookie = LoggingRecursionGuard.Begin())
            {
                // Assert
                Assert.IsFalse(cookie.CanLog);
                Assert.IsTrue(LoggingRecursionGuard.IsLogging);
            }

            Assert.IsTrue(LoggingRecursionGuard.IsLogging);
        }
        [Test]
        public async Task Begin_ThreadSafety_IsLoggingIsHandledCorrectlyAcrossThreads()
        {
            // Arrange
            LoggingRecursionGuard.IsLogging = false;

            // Act
            await Task.WhenAll(
                Task.Run(() =>
                {
                    using (var cookie = LoggingRecursionGuard.Begin())
                    {
                        Assert.IsTrue(cookie.CanLog);
                        Assert.IsTrue(LoggingRecursionGuard.IsLogging);
                    }
                }),
                Task.Run(() =>
                {
                    using (var cookie = LoggingRecursionGuard.Begin())
                    {
                        Assert.IsTrue(cookie.CanLog);
                        Assert.IsTrue(LoggingRecursionGuard.IsLogging);
                    }
                })
            );
            // Assert
            Assert.IsFalse(LoggingRecursionGuard.IsLogging);
        }
    }
}