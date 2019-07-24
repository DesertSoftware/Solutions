//
//  Copyright 2015, 2019 Desert Software Solutions Inc.
//    MessageDialog.cs:
//      https://github.com/DesertSoftware/Solutions
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DesertSoftware.Solutions.WinForms
{
    /// <summary>
    /// Provides a basic Message Dialog box implementation
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class MessageDialog : Form
    {
        private Panel headerPanel = new Panel();
        private Label messageLabel = new Label();

        private Panel footerPanel = new Panel();
        private FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
        private List<Button> buttonList = new List<Button>();

        //private Panel _plIcon = new Panel();
        //private PictureBox _picIcon = new PictureBox();
        //private static Timer _timer;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool MessageBeep(uint type);

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDialog"/> class.
        /// </summary>
        public MessageDialog() {
            InitializeComponent();

            this.Width = 400;
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
 
            this.messageLabel = new Label();
            this.messageLabel.ForeColor = SystemColors.ControlText;
            this.messageLabel.Font = new System.Drawing.Font("Segoe UI", 10);
            this.messageLabel.Dock = DockStyle.Fill;

            this.buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            this.buttonPanel.Dock = DockStyle.Fill;

            this.headerPanel = new Panel();
            this.headerPanel.Dock = DockStyle.Fill;
            this.headerPanel.Padding = new Padding(20);
            this.headerPanel.Controls.Add(this.messageLabel);

            this.footerPanel.Dock = DockStyle.Bottom;
            this.footerPanel.Padding = new Padding(8);
            this.footerPanel.Height = 48;
            this.footerPanel.Controls.Add(this.buttonPanel);

            //_picIcon.Width = 32;
            //_picIcon.Height = 32;
            //_picIcon.Location = new Point(30, 50);

            //_plIcon.Dock = DockStyle.Left;
            //_plIcon.Padding = new Padding(8);
            //_plIcon.Width = 50;
            //_plIcon.Controls.Add(_picIcon);

            this.Controls.Add(this.headerPanel);
            //this.Controls.Add(_plIcon);
            this.Controls.Add(this.footerPanel);

            this.Shown += this.MessageDialog_Shown;
        }

        /// <summary>
        /// Displays a message dialog with the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        static public DialogResult Show(string text) {
            using (var dlg = new MessageDialog()) {
                dlg.messageLabel.Text = text;
                dlg.InitButtons(MessageBoxButtons.OK);
                return dlg.ShowDialog();
            }
        }

        /// <summary>
        /// Displays a message dialog in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        static public DialogResult Show(System.Windows.Forms.IWin32Window owner, string text) {
            using (var dlg = new MessageDialog()) {
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.messageLabel.Text = text;
                dlg.InitButtons(MessageBoxButtons.OK);
                return dlg.ShowDialog(owner);
            }
        }

        /// <summary>
        /// Displays a message dialog with the specified text and caption.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        /// <returns></returns>
        static public DialogResult Show(string text, string caption) {
            using (var dlg = new MessageDialog()) {
                dlg.Text = caption;
                dlg.messageLabel.Text = text;
                dlg.InitButtons(MessageBoxButtons.OK);
                return dlg.ShowDialog();
            }
        }

        /// <summary>
        /// Displays a message dialog in front of the specified object and with the specified text and caption.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        /// <returns></returns>
        static public DialogResult Show(System.Windows.Forms.IWin32Window owner, string text, string caption) {
            using (var dlg = new MessageDialog()) {
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.Text = caption;
                dlg.messageLabel.Text = text;
                dlg.InitButtons(MessageBoxButtons.OK);
                return dlg.ShowDialog(owner);
            }
        }

        /// <summary>
        /// Displays a message dialog with the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns></returns>
        static public DialogResult Show(string text, string caption, MessageBoxButtons buttons) {
            using (var dlg = new MessageDialog()) {
                dlg.Text = caption;
                dlg.messageLabel.Text = text;
                dlg.InitButtons(buttons);
                return dlg.ShowDialog();
            }
        }

        /// <summary>
        /// Displays a message dialog.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns></returns>
        static public DialogResult Show(System.Windows.Forms.IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            using (var dlg = new MessageDialog()) {
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.Text = caption;
                dlg.messageLabel.Text = text;
                dlg.InitButtons(buttons);
                return dlg.ShowDialog(owner);
            }
        }

        private void InitButtons(MessageBoxButtons buttons) {
            switch (buttons) {
                case MessageBoxButtons.AbortRetryIgnore: {
                        Button abortButton = InitButton("Abort", DialogResult.Abort);

                        this.CancelButton = abortButton;
                        this.AcceptButton = abortButton;
                        this.buttonList.Add(InitButton("Ignore", DialogResult.Ignore));  // right
                        this.buttonList.Add(InitButton("Retry", DialogResult.Retry));   // middle
                        this.buttonList.Add(abortButton);           // left
                    }
                    break;

                case MessageBoxButtons.OK: {
                        Button okButton = InitButton("OK", DialogResult.OK);

                        this.CancelButton = okButton;
                        this.AcceptButton = okButton;
                        this.buttonList.Add(okButton);
                    }
                    break;

                case MessageBoxButtons.OKCancel: {
                        Button okButton = InitButton("OK", DialogResult.OK);
                        Button cancelButton = InitButton("Cancel", DialogResult.Cancel);

                        this.CancelButton = cancelButton;
                        this.AcceptButton = okButton;
                        this.buttonList.Add(cancelButton);
                        this.buttonList.Add(okButton);
                    }
                    break;

                case MessageBoxButtons.RetryCancel: {
                        Button cancelButton = InitButton("Cancel", DialogResult.Cancel);
                        Button retryButton = InitButton("Retry", DialogResult.Retry);

                        this.CancelButton = cancelButton;
                        this.AcceptButton = retryButton;
                        this.buttonList.Add(cancelButton);
                        this.buttonList.Add(retryButton);
                    }
                    break;

                case MessageBoxButtons.YesNo: {
                        Button noButton = InitButton("No", DialogResult.No);
                        Button yesButton = InitButton("Yes", DialogResult.Yes);

                        this.AcceptButton = yesButton;
                        this.CancelButton = noButton;
                        this.buttonList.Add(noButton);
                        this.buttonList.Add(yesButton);
                    }
                    break;

                case MessageBoxButtons.YesNoCancel: {
                        Button yesButton = InitButton("Yes", DialogResult.Yes);
                        Button cancelButton = InitButton("Cancel", DialogResult.Cancel);

                        this.AcceptButton = yesButton;
                        this.CancelButton = cancelButton;
                        this.buttonList.Add(cancelButton);
                        this.buttonList.Add(InitButton("No", DialogResult.No));
                        this.buttonList.Add(yesButton);
                    }
                    break;

            }

            this.buttonPanel.Controls.AddRange(this.buttonList.ToArray());
        }

        private Button InitButton(string text, DialogResult result) {
            Button btn = new Button();

            btn.Text = text;
            btn.DialogResult = result;
            btn.Click += ButtonClick;

            return btn;
        }

        private void ButtonClick(object sender, EventArgs e) {
            this.DialogResult = ((Button)sender).DialogResult;
            this.Close();
        }

        private void MessageDialog_Shown(object sender, EventArgs e) {
            ((Button)this.AcceptButton).Select();
        }
    }
}
