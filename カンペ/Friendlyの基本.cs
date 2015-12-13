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
using Ong.Friendly.FormsStandardControls;
using Codeer.Friendly.Windows.Grasp;

namespace Test
{
    [TestClass]
    public class Friendlyの基本
    {
        WindowsAppFriend _app;
        Process _process;

        [TestInitialize]
        public void TestInitialize()
        {
            string path = Path.GetFullPath("../../../Target/bin/Debug/Target.exe");
            _process = Process.Start(path);
            _app = new WindowsAppFriend(_process);
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
            string targetPath = _app.Type<Application>().ExecutablePath;
        }

        //②
        [TestMethod]
        public void 開いているフォームの数取得()
        {
            //自分のプロセスなら
            int myCount = Application.OpenForms.Count;

            //Target.exeで実行
            int targetCount = _app.Type<Application>().OpenForms.Count;
        }

        //③
        [TestMethod]
        public void 開いているフォームをアクティブにしてタイトル書き換え()
        {
            //自分のプロセスでやるなら
            //Form form = new Form();//ないからnewしたけど、対象プロセスではこれはやらない
            //form.Activate();
            //form.Text = "abc";

            //Target.exeで実行
            var form = _app.Type<Application>().OpenForms[0];
            form.Activate();
            form.Text = "abc";
        }

        //★ここで説明

        //④マウスダウン直呼び
        [TestMethod]
        public void マウスダウン直呼び()
        {
            var form = _app.Type<Application>().OpenForms[0];
            form.MainForm_MouseDown(form, _app.Type<MouseEventArgs>()(MouseButtons.Left, 1, 100, 100, 0));
        }

        //⑤
        [TestMethod]
        public void EnumFunc呼び出し()
        {
            var form = _app.Type<Application>().OpenForms[0];
            string ret = form.EnumFunc(_app.Type("Target.MyEnum").A);
            Assert.AreEqual("A", ret);
        }
        
        //⑥
        [TestMethod]
        public void DynamicAppTypeの書き方を4パターン写経()
        {
            //Application
            //これが一番書きやすい
            var application = _app.Type<Application>();
            //staticクラスにはGeneric使えない
            application = _app.Type(typeof(Application));
            //型を使えない場合
            application = _app.Type("System.Windows.Forms.Application");
            //好みで
            application = _app.Type().System.Windows.Forms.Application;
        }

        //➆
        [TestMethod]
        public void モーダルボタンを非同期でクリック()
        {
            var form = _app.Type<Application>().OpenForms[0];
            var async = new Async();
            form._buttonModal.PerformClick(async);
            while ((int)_app.Type<Application>().OpenForms.Count < 2)
            {
                Thread.Sleep(10);
            }
            _app.Type<Application>().OpenForms[1].Close();
            async.WaitForCompletion();
        }

        //⑧
        [TestMethod]
        public void 最後の演習() 
        {
            var form = _app.Type<Application>().OpenForms[0];
            //日付を2013/11/12
            //ちなみに自分のプロセスのDateTimePickerに対してならこんな感じ
            //var _dateTimePicker = new DateTimePicker();//ないからnewしたけど、対象プロセスではこれはやらない
            //_dateTimePicker.Value = new DateTime(2013, 11, 12);
            form._dateTimePicker.Value = new DateTime(2013, 11, 12);

            //トラックバーを7に設定
            //ちなみに自分のプロセスのTrackBarにならこんな感じ
            //var _trackBar = new TrackBar();//ないからnewしたけど、対象プロセスではこれはやらない
            //_trackBar.Value = 7;
            form._trackBar.Value = 7;

            //ボタンを押す
            //ちなみに自分のプロセスのButtonならこんな感じ
            //var _buttonApply = new Button();//ないからnewしたけど、対象プロセスではこれはやらない
            //_buttonApply.PerformClick();
            form._buttonApply.PerformClick();

            //フォームの持っている_dataフィールドの情報を取得
            DateTime dateTime = form._data.DateTime;
            int trackValue = form._data.TrackValue;
            Assert.AreEqual(new DateTime(2013, 11, 12), dateTime);
            Assert.AreEqual(7, trackValue);
        }

        //⑨
        [TestMethod]
        public void 最後の演習_Byコントロールドライバ()
        {
            //マッピング
            var form = _app.Type<Application>().OpenForms[0];
            var dateTimePicker = new FormsDateTimePicker(form._dateTimePicker);
            var trackBar = new FormsTrackBar(form._trackBar);
            var buttonApply = new FormsButton(form._buttonApply);

            //操作
            dateTimePicker.EmulateSelectDay(new DateTime(2013, 11, 12));
            trackBar.EmulateChangeValue(7);
            buttonApply.EmulateClick();

            //フォームの持っている_dataフィールドの情報を取得
            DateTime dateTime = form._data.DateTime;
            int trackValue = form._data.TrackValue;
            Assert.AreEqual(new DateTime(2013, 11, 12), dateTime);
            Assert.AreEqual(7, trackValue);
        }

        //何とコントロールドライバがない・・・
        //そんな時は、作ってね
        public class FormsTrackBar : IAppVarOwner
        {
            public AppVar AppVar { get; private set; }
            public FormsTrackBar(AppVar v)
            {
                AppVar = v;
            }
            public void EmulateChangeValue(int value)
            {
                this.Dynamic().Value = value;
            }
        }

        //⑩
        [TestMethod]
        public void モーダルボタンを非同期でクリック_Byコントロールドライバ()
        {
            var form = new WindowControl(_app.Type<Application>().OpenForms[0]);
            var buttonModal = new FormsButton(form.Dynamic()._buttonModal);

            var async = new Async();
            buttonModal.EmulateClick(async);
            var next = form.WaitForNextModal(async);
            next.Dynamic().Close();
            async.WaitForCompletion();
        }

    }
}