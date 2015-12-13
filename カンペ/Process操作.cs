using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Test
{
    [TestClass]
    public class Process操作
    {
        Process _process;

        [TestInitialize]
        public void TestInitialize()
        {
            string path = Path.GetFullPath("../../../Target/bin/Debug/Target.exe");
            _process = Process.Start(path);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _process.Kill();
        }

        [TestMethod]
        public void タイトルを確認する()
        {
            while (string.IsNullOrEmpty(_process.MainWindowTitle)) 
            {
                Thread.Sleep(10);
                _process.Refresh();
            }
            Assert.AreEqual("テスト対象", _process.MainWindowTitle);
        }
    }
}