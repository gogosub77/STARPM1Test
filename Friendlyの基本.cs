﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
//★NugetでFriendly.Windowsを取得しておく必要がある
using Codeer.Friendly;
using Codeer.Friendly.Dynamic;//★拡張メソッドを使うために必要
using Codeer.Friendly.Windows;
using System.Threading;
using Ong.Friendly.FormsStandardControls;
using Codeer.Friendly.Windows.Grasp;

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
			//シリアライズ可能なものしか送れない
			var form = _app.Type<Application>().OpenForms[0];
			//var e = new MouseEventArgs(MouseButtons.Left, 1, 10, 20, 1);
			var e = _app.Type<MouseEventArgs>()(MouseButtons.Left, 1, 10, 20, 1);
			form.OnMouseDown(e);


			Assert.AreEqual("Left, click = 1, delta = 1, x = 10, y = 20",
				(string)form._textBoxMouseInfo.Text);
		}

        //⑤
        [TestMethod]
        public void EnumFunc呼び出し()
        {
			var form = _app.Type<Application>().OpenForms[0];
			var e = _app.Type("Target.MyEnum").B;
			string ret = form.EnumFunc(e);
			Assert.AreEqual("B", ret);
		}

        //⑥
        [TestMethod]
        public void DynamicAppTypeの書き方を4パターン写経()
        {
			var t1 = _app.Type<Application>();
			var t2 = _app.Type(typeof(Application));
			var t3 = _app.Type("Target MyEnum");
			var t4 = _app.Type().Target.MyEnum;
        }

        //➆
        [TestMethod]
        public void モーダルボタンを非同期でクリック()
        {
			var form = _app.Type<Application>().OpenForms[0];
			var async = new Async();

			form._buttonModal.PerformClick(async);
			//Button b;
			//b.PerformClick
			//画面が２つ立ち上がっているかチェックをする。
			var wcount = (int)_app.Type<Application>().OpenForms.Count;
			while (2 !=(int)_app.Type<Application>().OpenForms.Count)
			{
				Thread.Sleep(10);
			}

			var dig = _app.Type<Application>().OpenForms[1];
			dig.Close();

			async.WaitForCompletion();
		}

        //⑧
        [TestMethod]
        public void 最後の演習()
        {
            //日付を2013/11/12
            //ちなみに自分のプロセスのDateTimePickerに対してならこんな感じ
            //DateTimePicker _dateTimePicker = ...
            //_dateTimePicker.Value = new DateTime(2013, 11, 12);
            var form = _app.Type<Application>().OpenForms[0];
            var _dateTimePicker = form.
            _dateTimePicker.Value = new DateTime(2013, 11, 12);

            //トラックバーを7に設定
            //ちなみに自分のプロセスのTrackBarにならこんな感じ
            //TrackBar _trackBar = ...
            //_trackBar.Value = 7;
            var _trackBar = form._trackBar.Value = 7;

            //ボタンを押す
            //ちなみに自分のプロセスのButtonならこんな感じ
            //Button _buttonApply = ...
            //_buttonApply.PerformClick();
            var _buttonApply = form._buttonApply.PerformClick();

            //フォームの持っている_dataフィールドの情報を取得
            DateTime DateTime = form._data.DateTime;
            int TrackValue = form._data.TrackValue;

            Assert.AreEqual(new DateTime(2013, 11, 12), DateTime);
            Assert.AreEqual(7, TrackValue);

        }

        //⑨
        [TestMethod]
        public void 最後の演習_Byコントロールドライバ()
        {

            //日付を2013/11/12
            //ちなみに自分のプロセスのDateTimePickerに対してならこんな感じ
            //DateTimePicker _dateTimePicker = ...
            //_dateTimePicker.Value = new DateTime(2013, 11, 12);
            var form = _app.Type<Application>().OpenForms[0];
            var _dataTimePicker = new FormsDateTimePicker(form._dateTimePicker);
            var _trackBar = new FormsTrackBar(form._trackBar);
            var _buttonApply = new FormsButton(form._buttonApply);

            _dataTimePicker.EmulateSelectDay(new DateTime(2013, 11, 12));
            _trackBar.EmulateChangeValue(7);
            _buttonApply.EmulateClick();

            //フォームの持っている_dataフィールドの情報を取得
            DateTime DateTime = form._data.DateTime;
            int TrackValue = form._data.TrackValue;
            Assert.AreEqual(new DateTime(2013, 11, 12), DateTime);
            Assert.AreEqual(7, TrackValue);
        }

        //⑩
        [TestMethod]
        public void モーダルボタンを非同期でクリック_Byコントロールドライバ()
        {
            var form = _app.Type<Application>().OpenForms[0];
            var w = new WindowControl (form);

            var async = new Async();
            form._buttonModal.PerformClick(async);

            var dig = w.WaitForNextModal();
            dig.Dynamic().Close();

            async.WaitForCompletion();
        }
    }
}