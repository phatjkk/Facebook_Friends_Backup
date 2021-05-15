using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using RestSharp;
using Spire.Xls;

namespace FacebookFriendsBackup
{
	// Token: 0x02000002 RID: 2
	public partial class uiscan : Window
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public uiscan(string token, string id, string name)
		{
			this.tk = token;
			this.idman = id;
			this.nameman = name;
			this.InitializeComponent();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020A8 File Offset: 0x000002A8
		private void _scan_Click(object sender, RoutedEventArgs e)
		{
			this.checkoke = 1;
			this._list.Items.Clear();
			RestClient client = new RestClient("https://graph.facebook.com/v3.0/me/friends?access_token=" + this.tk + "&limit=5000&fields=id,name");
			client.Timeout = -1;
			RestRequest request = new RestRequest(Method.GET);
			IRestResponse response = client.Execute(request);
			string data = response.Content;
			JToken stuff = JToken.Parse(data);
			this.savejson = stuff;
			int count = Regex.Matches(data, "\"id\"").Count;
			this.max = count;
			for (int i = 0; i < count; i++)
			{
				this._list.Items.Add(new uiscan.MyItem
				{
					Stt = i,
					Link = "https://facebook.com/" + stuff["data"][i]["id"],
					Name = stuff["data"][i]["name"].ToString()
				});
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021D0 File Offset: 0x000003D0
		private void _scan_Loaded(object sender, RoutedEventArgs e)
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri("https://graph.facebook.com/" + this.idman + "/picture?height=500");
			bitmapImage.EndInit();
			this._id.Text = this.idman;
			this._name.Text = this.nameman;
			this._image.Source = bitmapImage;
			base.Title = "Facebook Friends BackUp - " + this.nameman + " - " + this.idman;
			this.checkoke = 1;
			this._list.Items.Clear();
			this._process.Content = "Đang loading vui lòng chờ...";
			RestClient client = new RestClient("https://graph.facebook.com/v3.0/me/friends?access_token=" + this.tk + "&limit=5000&fields=id,name");
			client.Timeout = -1;
			RestRequest request = new RestRequest(Method.GET);
			IRestResponse response = client.Execute(request);
			string data = response.Content;
			JToken stuff = JToken.Parse(data);
			this.savejson = stuff;
			int count = Regex.Matches(data, "\"id\"").Count;
			this.max = count;
			for (int i = 0; i < count; i++)
			{
				this._list.Items.Add(new uiscan.MyItem
				{
					Stt = i,
					Link = "https://facebook.com/" + stuff["data"][i]["id"],
					Name = stuff["data"][i]["name"].ToString()
				});
			}
			this._process.Content = "Đã xong! Đã load xong " + this.max + " người bạn.";
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000023BC File Offset: 0x000005BC
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

		// Token: 0x06000005 RID: 5 RVA: 0x00002448 File Offset: 0x00000648
		private void _savee_Click(object sender, RoutedEventArgs e)
		{
			bool flag = this.checkoke == 1;
			if (flag)
			{
				this._process.Content = "Đang lưu danh sách, vui lòng chờ...";
				Workbook workbook = new Workbook();
				workbook.LoadFromFile("Mau.xlsx");
				Worksheet sheet = workbook.Worksheets[0];
				sheet.Name = this.nameprofile + "_" + DateTime.UtcNow.ToString("MM-dd-yyyy");
				sheet.Range["B1"].Text = this.nameman;
				sheet.Range["B2"].Text = this.idman;
				sheet.Range["C2"].Text = DateTime.UtcNow.ToString("MM-dd-yyyy");
				for (int i = 0; i < this.max; i++)
				{
					sheet.Range["A" + (i + 7).ToString()].Text = i.ToString();
					sheet.Range["B" + (i + 7).ToString()].Text = this.savejson["data"][i]["name"].ToString();
					sheet.Range["C" + (i + 7).ToString()].Text = "https://facebook.com/" + this.savejson["data"][i]["id"].ToString();
				}
				workbook.SaveToFile(this.nameman + "_" + DateTime.UtcNow.ToString("MM-dd-yyyy") + ".xls", ExcelVersion.Version97to2003);
				Process.Start(this.nameman + "_" + DateTime.UtcNow.ToString("MM-dd-yyyy") + ".xls");
				this._process.Content = "Đang khởi động excel...";
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002690 File Offset: 0x00000890
		private void Label_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start("https://www.facebook.com/phatjk");
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000269E File Offset: 0x0000089E
		private void Window_Closed(object sender, EventArgs e)
		{
			Environment.Exit(1);
		}

		// Token: 0x04000001 RID: 1
		private string nameprofile = "";

		// Token: 0x04000002 RID: 2
		private string idprofile = "";

		// Token: 0x04000003 RID: 3
		private int max = 0;

		// Token: 0x04000004 RID: 4
		public string[] listname;

		// Token: 0x04000005 RID: 5
		public string[] listurl;

		// Token: 0x04000006 RID: 6
		private int checkoke = 0;

		// Token: 0x04000007 RID: 7
		private JToken savejson;

		// Token: 0x04000008 RID: 8
		private string tk;

		// Token: 0x04000009 RID: 9
		private string idman;

		// Token: 0x0400000A RID: 10
		private string nameman;

		// Token: 0x02000007 RID: 7
		public class MyItem
		{
			// Token: 0x17000004 RID: 4
			// (get) Token: 0x06000019 RID: 25 RVA: 0x00002B2A File Offset: 0x00000D2A
			// (set) Token: 0x0600001A RID: 26 RVA: 0x00002B32 File Offset: 0x00000D32
			public string Link { get; set; }

			// Token: 0x17000005 RID: 5
			// (get) Token: 0x0600001B RID: 27 RVA: 0x00002B3B File Offset: 0x00000D3B
			// (set) Token: 0x0600001C RID: 28 RVA: 0x00002B43 File Offset: 0x00000D43
			public string Name { get; set; }

			// Token: 0x17000006 RID: 6
			// (get) Token: 0x0600001D RID: 29 RVA: 0x00002B4C File Offset: 0x00000D4C
			// (set) Token: 0x0600001E RID: 30 RVA: 0x00002B54 File Offset: 0x00000D54
			public int Stt { get; set; }
		}
	}
}
