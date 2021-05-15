using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace FacebookFriendsBackup
{
	// Token: 0x02000004 RID: 4
	public partial class MainWindow : Window
	{
		// Token: 0x0600000D RID: 13 RVA: 0x0000284F File Offset: 0x00000A4F
		public MainWindow()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002860 File Offset: 0x00000A60
		private static List<string> ExtractFromBody(string body, string start, string end)
		{
			List<string> matched = new List<string>();
			bool exit = false;
			while (!exit)
			{
				int indexStart = body.IndexOf(start);
				bool flag = indexStart != -1;
				if (flag)
				{
					int indexEnd = indexStart + body.Substring(indexStart).IndexOf(end);
					matched.Add(body.Substring(indexStart + start.Length, indexEnd - indexStart - start.Length));
					body = body.Substring(indexEnd + end.Length);
				}
				else
				{
					exit = true;
				}
			}
			return matched;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000028EC File Offset: 0x00000AEC
		private void _dangnhap_Click(object sender, RoutedEventArgs e)
		{
			string token = this._cc.Text;
			try
			{
				token = "EAA" + MainWindow.ExtractFromBody(this._cc.Text, "EAA", "\",")[0];
			}
			catch
			{
				token = this._cc.Text;
			}
			RestClient client = new RestClient("https://graph.facebook.com/v3.0/me?access_token=" + token + "&limit=5000");
			client.Timeout = -1;
			RestRequest request = new RestRequest(Method.GET);
			IRestResponse response = client.Execute(request);
			JToken stuff = JToken.Parse(response.Content);
			bool flag = stuff["error"] != null;
			if (flag)
			{
				MessageBox.Show("Token đã die, vui lòng nhập lại!");
			}
			else
			{
				base.Hide();
				uiscan form = new uiscan(token, stuff["id"].ToString(), stuff["name"].ToString());
				form.Show();
			}
		}
	}
}
