using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
//★NugetでFriendly.Windowsを取得しておく必要がある
using Codeer.Friendly;
using Codeer.Friendly.Dynamic;//★拡張メソッドを使うために必要
using Codeer.Friendly.Windows;
using System.Threading;

namespace Test
{
    [TestClass]
    public class Friendlyの基本
    {
		Process _process;
		WindowsAppFriend _app;
		[TestInitialize]
		public void TestInitialize()
		{
			string path = Path.GetFullPath("../../../Target/bin/Debug/Target.exe");
			_process = Process.Start(path);
			_app = new WindowsAppFriend(_process);

			while (_process.MainWindowTitle.Length == 0)
			{
				Thread.Sleep(10);
				_process.Refresh();
			}
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_app.Dispose();
			_process.Kill();
		}

		//①
		[TestMethod]
        public void プロセスのパス取得()
        {
            //自分のプロセスなら
            string myPath = Application.ExecutablePath;

			//Target.exeで実行
			string targetPaht = _app.Type<Application>().ExecutablePath;
        }

        //②
        [TestMethod]
        public void 開いているフォームの数取得()
        {
            //自分のプロセスなら
            int myCount = Application.OpenForms.Count;

			//Target.exeで実行
			int targetCount = 0;
			targetCount = _app.Type<Application>().OpenForms.Count;
			Assert.AreEqual(1, targetCount);

        }

        //③
        [TestMethod]
        public void 開いているフォームをアクティブにしてタイトル書き換え()
        {
			//自分のプロセスでやるなら
			//Form form = Application.OpenForms[0];
			//form.Activate();
			//form.Text = "abc";
			var form = _app.Type<Application>().OpenForms[0];
			form.Activate();
			form.Text = "abc";

			Assert.AreEqual("abc", (string)form.Text);
			//Target.exeで実行
		}

		//★ここで説明

		//④マウスダウン直呼び
		[TestMethod]
        public void マウスダウン直呼び()
        {
        }

        //⑤
        [TestMethod]
        public void EnumFunc呼び出し()
        {
        }

        //⑥
        [TestMethod]
        public void DynamicAppTypeの書き方を4パターン写経()
        {
        }

        //➆
        [TestMethod]
        public void モーダルボタンを非同期でクリック()
        {
        }

        //⑧
        [TestMethod]
        public void 最後の演習()
        {
            //日付を2013/11/12
            //ちなみに自分のプロセスのDateTimePickerに対してならこんな感じ
            //DateTimePicker _dateTimePicker = ...
            //_dateTimePicker.Value = new DateTime(2013, 11, 12);

            //トラックバーを7に設定
            //ちなみに自分のプロセスのTrackBarにならこんな感じ
            //TrackBar _trackBar = ...
            //_trackBar.Value = 7;

            //ボタンを押す
            //ちなみに自分のプロセスのButtonならこんな感じ
            //Button _buttonApply = ...
            //_buttonApply.PerformClick();

            //フォームの持っている_dataフィールドの情報を取得
        }

        //⑨
        [TestMethod]
        public void 最後の演習_Byコントロールドライバ()
        {
        }

        //⑩
        [TestMethod]
        public void モーダルボタンを非同期でクリック_Byコントロールドライバ()
        {
        }
    }
}