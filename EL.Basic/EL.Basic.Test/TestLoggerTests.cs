using EL.Sqlite;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EL.Basic.Test
{
    [TestFixture]
    public class TestLoggerTests
    {
        [Test]
        public void LogTest()
        {
            SqliteComponent component = new SqliteComponent();
            List<string> cmd = new List<string>();
            cmd.Add("insert into WinXinMsg (Id,Msg,Nickname,Time) values (123,'test','lee','123456')");
            //SqliteComponent.Instance.BulkInsert(cmd);

            //InternetExplorer IE = new InternetExplorer();
            //IE.Visible = true;
            //object nil = new object();
            //IE.Navigate("www.bing.com", ref nil, ref nil, ref nil, ref nil);
        }
        [Test]
        public void IsTraceEnabled_False_TraceLoggingIsDisabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsTraceEnabled = false;

            // Act
            mock.Trace(string.Empty);
            mock.Trace(String.Empty, new Exception());
            mock.Trace("{0}", 1);
            mock.Trace("{0}", new Exception(), 1);

            // Assert
            mock.DidNotReceive().PublicTrace(Arg.Any<string>());
        }

        [Test]
        public void IsTraceEnabled_True_TraceLoggingIsEnabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsTraceEnabled = true;

            // Act
            mock.Trace(String.Empty);
            mock.Trace(String.Empty, new Exception());
            mock.Trace("{0}", 1);
            mock.Trace("{0}", new Exception(), 1);

            // Assert
            mock.Received(4).PublicTrace(Arg.Any<string>());
        }

        [Test]
        public void IsDebugEnabled_False_DebugLoggingIsDisabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsDebugEnabled = false;

            // Act
            mock.Debug(String.Empty);
            mock.Debug(String.Empty, new Exception());
            mock.Debug("{0}", 1);
            mock.Debug("{0}", new Exception(), 1);

            // Assert
            mock.DidNotReceive().PublicDebug(Arg.Any<string>());
        }

        [Test]
        public void IsDebugEnabled_True_DebugLoggingIsEnabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsDebugEnabled = true;

            // Act
            mock.Debug(String.Empty);
            mock.Debug(String.Empty, new Exception());
            mock.Debug("{0}", 1);
            mock.Debug("{0}", new Exception(), 1);

            // Assert
            mock.Received(4).PublicDebug(Arg.Any<string>());
        }

        [Test]
        public void IsInfoEnabled_False_InfoLoggingIsDisabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsInfoEnabled = false;

            // Act
            mock.Info(String.Empty);
            mock.Info(String.Empty, new Exception());
            mock.Info("{0}", 1);
            mock.Info("{0}", new Exception(), 1);

            // Assert
            mock.DidNotReceive().PublicInfo(Arg.Any<string>());
        }

        [Test]
        public void IsInfoEnabled_True_InfoLoggingIsEnabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsInfoEnabled = true;

            // Act
            mock.Info(String.Empty);
            mock.Info(String.Empty, new Exception());
            mock.Info("{0}", 1);
            mock.Info("{0}", new Exception(), 1);

            // Assert
            mock.Received(4).PublicInfo(Arg.Any<string>());
        }

        [Test]
        public void IsWarnEnabled_False_WarnLoggingIsDisabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsWarnEnabled = false;

            // Act
            mock.Warn(String.Empty);
            mock.Warn(String.Empty, new Exception());
            mock.Warn("{0}", 1);
            mock.Warn("{0}", new Exception(), 1);

            // Assert
            mock.DidNotReceive().PublicWarn(Arg.Any<string>());
        }

        [Test]
        public void IsWarnEnabled_True_WarnLoggingIsEnabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsWarnEnabled = true;

            // Act
            mock.Warn(String.Empty);
            mock.Warn(String.Empty, new Exception());
            mock.Warn("{0}", 1);
            mock.Warn("{0}", new Exception(), 1);

            // Assert
            mock.Received(4).PublicWarn(Arg.Any<string>());
        }

        [Test]
        public void IsErrorEnabled_False_ErrorLoggingIsDisabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsErrorEnabled = false;

            // Act
            mock.Error(String.Empty);
            mock.Error(String.Empty, new Exception());
            mock.Error("{0}", 1);
            mock.Error("{0}", new Exception(), 1);

            // Assert
            mock.DidNotReceive().PublicError(Arg.Any<string>());
        }

        [Test]
        public void IsErrorEnabled_True_ErrorLoggingIsEnabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsErrorEnabled = true;

            // Act
            mock.Error(String.Empty);
            mock.Error(String.Empty, new Exception());
            mock.Error("{0}", 1);
            mock.Error("{0}", new Exception(), 1);

            // Assert
            mock.Received(4).PublicError(Arg.Any<string>());
        }

        [Test]
        public void IsFatalEnabled_False_FatalLoggingIsDisabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsFatalEnabled = false;

            // Act
            mock.Fatal(String.Empty);
            mock.Fatal(String.Empty, new Exception());
            mock.Fatal("{0}", 1);
            mock.Fatal("{0}", new Exception(), 1);

            // Assert  «∑Ò÷¥––
            mock.DidNotReceive().PublicFatal(Arg.Any<string>());
        }

        [Test]
        public void IsFatalEnabled_True_FatalLoggingIsEnabled()
        {
            // Arrange
            var mock = Substitute.ForPartsOf<TestLogger>();
            mock.IsFatalEnabled = true;

            // Act
            mock.Fatal(String.Empty);
            mock.Fatal(String.Empty, new Exception());
            mock.Fatal("{0}", 1);
            mock.Fatal("{0}", new Exception(), 1);

            // Assert
            mock.Received(4).PublicFatal(Arg.Any<string>());
        }
    }
    public class TestLogger : LoggerBase
    {
        protected override void GatedTrace(string message)
        {
            PublicTrace(message);
        }

        protected override void GatedDebug(string message)
        {
            PublicDebug(message);
        }

        protected override void GatedInfo(string message)
        {
            PublicInfo(message);
        }

        protected override void GatedWarn(string message)
        {
            PublicWarn(message);
        }

        protected override void GatedError(string message)
        {
            PublicError(message);
        }

        protected override void GatedFatal(string message)
        {
            PublicFatal(message);
        }

        public virtual void PublicTrace(string message)
        {
        }

        public virtual void PublicDebug(string message)
        {
        }

        public virtual void PublicInfo(string message)
        {
        }

        public virtual void PublicWarn(string message)
        {
        }

        public virtual void PublicError(string message)
        {
        }

        public virtual void PublicFatal(string message)
        {
        }
    }
}