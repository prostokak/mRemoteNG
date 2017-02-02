﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using mRemoteNG.UI.Forms;
using mRemoteNG.UI.Window;
using WeifenLuo.WinFormsUI.Docking;

namespace mRemoteNG.Messages.MessagePrinters
{
    public class ErrorAndInfoWindowMessagePrinter : IMessagePrinter
    {
        private readonly ErrorAndInfoWindow _messageWindow;
        private Timer _ecTimer;

        public ErrorAndInfoWindowMessagePrinter(ErrorAndInfoWindow messageWindow)
        {
            if (messageWindow == null)
                throw new ArgumentNullException(nameof(messageWindow));

            _messageWindow = messageWindow;
            CreateTimer();
        }

        public void Print(IMessage message)
        {
            _ecTimer.Enabled = true;

            var lvItem = BuildListViewItem(message);
            AddToList(lvItem);
        }

        public void Print(IEnumerable<IMessage> messages)
        {
            foreach (var message in messages)
                Print(message);
        }

        private static ListViewItem BuildListViewItem(IMessage nMsg)
        {
            var lvItem = new ListViewItem
            {
                ImageIndex = Convert.ToInt32(nMsg.Class),
                Text = nMsg.Text.Replace(Environment.NewLine, "  "),
                Tag = nMsg
            };
            return lvItem;
        }

        private void CreateTimer()
        {
            _ecTimer = new Timer
            {
                Enabled = false,
                Interval = 300
            };
            _ecTimer.Tick += SwitchTimerTick;
        }

        private void SwitchTimerTick(object sender, EventArgs e)
        {
            SwitchToMessage();
            _ecTimer.Enabled = false;
        }

        private void SwitchToMessage()
        {
            _messageWindow.PreviousActiveForm = (DockContent)frmMain.Default.pnlDock.ActiveContent;
            ShowMcForm();
            _messageWindow.lvErrorCollector.Focus();
            _messageWindow.lvErrorCollector.SelectedItems.Clear();
            _messageWindow.lvErrorCollector.Items[0].Selected = true;
            _messageWindow.lvErrorCollector.FocusedItem = _messageWindow.lvErrorCollector.Items[0];
        }

        private void ShowMcForm()
        {
            if (frmMain.Default.pnlDock.InvokeRequired)
                frmMain.Default.pnlDock.Invoke((MethodInvoker)ShowMcForm);
            else
                _messageWindow.Show(frmMain.Default.pnlDock);
        }

        private void AddToList(ListViewItem lvItem)
        {
            if (_messageWindow.lvErrorCollector.InvokeRequired)
                _messageWindow.lvErrorCollector.Invoke((MethodInvoker)(() => AddToList(lvItem)));
            else
                _messageWindow.lvErrorCollector.Items.Insert(0, lvItem);
        }
    }
}