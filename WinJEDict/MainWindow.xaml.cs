using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Data;
using System.Data.SQLite;
using System.Runtime.InteropServices;


namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern UInt32 GetClipboardSequenceNumber();
        private IntPtr windowHandle;
        private SQLiteConnection db_conn;
        private UInt32 ClipboardSequenceNumber;

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            //Get our handle, and register ourselves for WNDPROC and WM_CLIPBOARDCHANGED Events
            windowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource src = HwndSource.FromHwnd(windowHandle);
            src.AddHook(new HwndSourceHook(WndProc));
            AddClipboardFormatListener(windowHandle);
            //Connect to our database.
            db_conn = new SQLiteConnection();
            db_conn.ConnectionString = "Data Source=" + @".\Inputs\jmdict.sqlite";
            db_conn.Open();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            result_box.Text = "WinEngJa contains an adapted version of JMDict. Original JMDict is available from http://www.edrdg.org under a Creative Commons License.\n\nWinEngJa © 2011 John Rozewicki";
        }

        private void Clipboard_Changed()
        {
            if (Clipboard.ContainsText())
            {
                search_box.Text = Clipboard.GetText();
                Search_Database();
            }
        }

        private void Search_Database()
        {
            string query = search_box.Text.Trim().ToLower();
            
            switch (search_type.SelectedIndex)
            {
                case 0:
                    //starts with
                    if (!query[query.Length - 1].Equals("%"))
                    {
                        query = query + "%";
                    }
                    break;

                case 1:
                    //contains
                    if (!query[query.Length - 1].Equals("%"))
                    {
                        query = query + "%";
                    }
                    if (!query[0].Equals("%"))
                    {
                        query = "%" + query;
                    }
                    break;

                case 2:
                    //ends with
                    if (!query[0].Equals("%"))
                    {
                        query = "%" + query;
                    }
                    break;

                default:
                    break;
            }
            
            SQLiteCommand search_command = new SQLiteCommand();
            search_command.CommandText = String.Format("select * from jmdict where (words like '{0}') or (glosses like '{0}') or (readings like '{0}')", query);
            search_command.Connection = db_conn;
            SQLiteDataReader results_reader = search_command.ExecuteReader();
            result_box.Text = String.Format("Results for Term: '{0}':\n", query);
            while (results_reader.Read())
            {
                string words = results_reader["words"].ToString().Replace(";:;", ", ");
                string readings = results_reader["readings"].ToString().Replace(";:;", ", ");
                string glosses = results_reader["glosses"].ToString().Replace(";:;", ", ");
                string result = "     "+String.Format("{0} [{1}] {2}", words, readings, glosses).Trim()+"\n";
                result_box.AppendText(result);
            }
            results_reader.Dispose();
            search_command.Dispose();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //Check for WM_CLIPBOARDUPDATE messages
            if (msg == 797)
            {
                if (ClipboardSequenceNumber != GetClipboardSequenceNumber())
                    Clipboard_Changed();
                ClipboardSequenceNumber = GetClipboardSequenceNumber();
                handled = true;
                
            }

            return IntPtr.Zero;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            RemoveClipboardFormatListener(windowHandle);
            //db_conn.Close();
            //db_conn.Dispose();
            HwndSource src = HwndSource.FromHwnd(windowHandle);
            src.RemoveHook(new HwndSourceHook(this.WndProc));
        }

        private void search_box_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search_Database();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Search_Database();
        }

        private void result_box_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}
