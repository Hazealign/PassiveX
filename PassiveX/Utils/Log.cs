﻿using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using PassiveX.Forms;
using PassiveX.Utils;
using System.Windows.Forms;

namespace PassiveX
{
    class Log
    {
        private static readonly Regex escape = new Regex(@"\{(.+?)\}");
        internal static MainForm Form { get; set; }

        private static void Write(Color color, object format, params object[] args)
        {
            var formatted = format ?? "(null)";
            try
            {
                formatted = string.Format(formatted.ToString(), args);
            }
            catch (FormatException) { }

            var datetime = DateTime.Now.ToString("HH:mm:ss");
            var message = $"[{datetime}] {formatted}{Environment.NewLine}";

            Form.Invoke(() =>
            {
                var item = new ListViewItem(new[] { datetime, formatted.ToString() });
                item.ForeColor = color;
                Form.listView.Items.Insert(0, item);
                Form.listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            });
        }

        internal static void S(object format, params object[] args)
        {
            Write(Color.Green, format, args);
        }

        internal static void I(object format, params object[] args)
        {
            Write(Color.Black, format, args);
        }

        internal static void W(object format, params object[] args)
        {
            Write(Color.Yellow, format, args);
        }

        internal static void E(object format, params object[] args)
        {
            Write(Color.Red, format, args);
        }

        internal static void Ex(Exception ex, object format, params object[] args)
        {
#if DEBUG
            var message = ex.Message;
#else
            var message = ex.Message;
#endif
            message = Escape(message);
            E($"{format}: {message}", args);
        }

        internal static void D(object format, params object[] args)
        {
#if DEBUG
            Write(Color.Gray, format, args);
#endif
        }

        internal static void B(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();

            for (int i = 0; i < buffer.Length; i++)
            {
                if (i != 0)
                {
                    if (i % 16 == 0)
                    {
                        sb.AppendLine();
                    }
                    else if (i % 8 == 0)
                    {
                        sb.Append(' ', 2);
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }

                sb.Append(buffer[i].ToString("X2"));
            }

            D(sb.ToString());
        }

        private static string Escape(string line)
        {
            return escape.Replace(line, "{{$1}}");
        }
    }
}