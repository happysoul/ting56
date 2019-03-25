/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2018/11/13
 * 时间: 17:45
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text;
using NSoup.Nodes;

namespace ting56
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void MainFormLoad(object sender, EventArgs e)
		{
			
		}
		void Label1Click(object sender, EventArgs e)
		{
	
		}
		
		void TextBox1TextChanged(object sender, EventArgs e)
		{
			
		}
		
		// 是否能点击下载
		Boolean clickAble = false;
		void Button1Click(object sender, EventArgs e)
		{
			//MessageBox.Show("随便提示", "错误提示", MessageBoxButtons.OK,   MessageBoxIcon.Warning);
			
//			http://www.ting56.com/mp3/4303.html  
			
			
			
			if(Regex.IsMatch(textBox1.Text,"http://www.ting56.com/mp3/([0-9,*]*)\\.html",RegexOptions.IgnoreCase)){
				clickAble = true;
				MessageBox.Show("可以下载了，去吧皮卡丘！");
			}else{
				MessageBox.Show("我只能识别 http://www.ting56.com/mp3/数字.html 这种格式");
			}
			
			
		}
		void Button2Click(object sender, EventArgs e)
		{
			if(!clickAble){
				MessageBox.Show("请先校验", "错误提示", MessageBoxButtons.OK,   MessageBoxIcon.Warning);
			}else{
				MessageBox.Show("点击确定以后 我就开始工作了 你就关心下载目录就好 软件会提示无响应");
				download2();
			}
			
		}
		void TextBox2TextChanged(object sender, EventArgs e)
		{
	
		}
		
		
		private void download2(){
		
			// 下载地址
			string url = textBox1.Text;
			
			string html = getHtml(url);
			NSoup.Nodes.Document d = NSoup.NSoupClient.Parse(html);
			//获取标题
			String title = d.GetElementsByClass("tit").First.GetElementsByTag("h1").Text;
			//如果没有标题就用毫秒数
            title = (title!=null&&!title.Equals(""))?title:DateTime.Now.ToUniversalTime().Ticks+"";
			NSoup.Nodes.Element el = d.GetElementById("vlink_1");    
            NSoup.Select.Elements es = el.GetElementsByTag("li");
            
            richTextBox1.Text="";
            
			foreach(var e in es){
            	
            	string subHtml = getHtml("http://www.ting56.com"+e.GetElementsByTag("a").Attr("href"));
            	Document d1 = NSoup.NSoupClient.Parse(subHtml);
            	
            	Match mc = Regex.Match(subHtml, "FonHen_JieMa\\('([0-9,*]*)'\\)");
            	//获取加密url
        		string miwen = mc.Groups[1].Value;
        		string[] tArr = Regex.Split(miwen,"\\*",RegexOptions.IgnoreCase);
            	
        		int n = tArr.Length;    
        		string s = "";
                for(int i=0;i<n;i++){    
        			if(!tArr[i].Equals("")){
        				s+= (char) int.Parse(tArr[i]);
        			}   
                }
        		
        		//下载地址
        		string downUrl = Regex.Split(s,"\\&",RegexOptions.IgnoreCase)[0];
        		//文件扩展名
                string downFileExt = Regex.Split(s,"\\&",RegexOptions.IgnoreCase)[2];
        		
                
                string path = textBox2.Text+"\\"+title+"\\";
                
                                
	            string fileName = e.Text();
	            string localFile = path+fileName+downFileExt;
//            	MessageBox.Show(localFile);
            	
            	
				richTextBox1.AppendText("开始下载 "+fileName+"\n\r"+downUrl+"\n\r");
            	
				//用线程执行下载
//				Thread oGetArgThread = new Thread(new ThreadStart(HttpDownload));
//	    		oGetArgThread.IsBackground = true;
//	    		oGetArgThread.Start(); 
    		
//				DownFile hd = new DownFile();
//				hd.downUrl=downUrl;
//				hd.localFile=localFile;
				
//				DownFile df = new DownFile(downUrl,localFile);
            	
//            	ThreadPool.QueueUserWorkItem(new WaitCallback(df.HttpDownload));
				
//				Thread oGetArgThread = new Thread(new ThreadStart(hd.HttpDownload));
//	    		oGetArgThread.IsBackground = true;
//	    		oGetArgThread.Start(); 
            	
            	HttpDownload(downUrl,localFile);
            	
            }
            
//			MessageBox.Show(title);
		}
		
		
		/// http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="path">文件存放地址，包含文件名</param>
        /// <returns></returns>
        public bool HttpDownload(string url, string path)
        {
            string tempPath = System.IO.Path.GetDirectoryName(path) + @"\temp";
            System.IO.Directory.CreateDirectory(tempPath);  //创建临时文件目录
            string tempFile = tempPath + @"\" + System.IO.Path.GetFileName(path) + ".temp"; //临时文件
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);    //存在则删除
            }
            // 如果已经下载则直接跳过
            if(System.IO.File.Exists(path)){
            	return true;
            }
            
            try
            {
                FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                //Stream stream = new FileStream(tempFile, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    //stream.Write(bArr, 0, size);
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                //stream.Close();
                fs.Close();
                responseStream.Close();
                System.IO.File.Move(tempFile, path);
                return true;
            }
            catch (Exception ex)
            {
            	Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
		
		private static string getHtml(string url){
			string pageHtml = "";
			try {
				System.Net.WebClient MyWebClient = new System.Net.WebClient();
		        MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
		        Byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据
		        pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句    
//		        pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句
		        Console.WriteLine(pageHtml);//在控制台输入获取的内容
		       
//		       	临时测试显示
//		        richTextBox1.Text=pageHtml;
		        
//		        MessageBox.Show(pageHtml);
		    }
		    catch(WebException webEx) {
		        Console.WriteLine(webEx.Message.ToString());
		    }
			return pageHtml;
		}
		
		
		
	}
	
	class DownFile{
    	private string url,path;
    	
    	public DownFile(string downUrl, string localFile){
    		url = downUrl;
			path=localFile;
    	}
    		
		/// http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="path">文件存放地址，包含文件名</param>
        /// <returns></returns>
        public bool HttpDownload()
        {
            string tempPath = System.IO.Path.GetDirectoryName(path) + @"\temp";
            System.IO.Directory.CreateDirectory(tempPath);  //创建临时文件目录
            string tempFile = tempPath + @"\" + System.IO.Path.GetFileName(path) + ".temp"; //临时文件
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);    //存在则删除
            }
            try
            {
                FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                //Stream stream = new FileStream(tempFile, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    //stream.Write(bArr, 0, size);
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                //stream.Close();
                fs.Close();
                responseStream.Close();
                System.IO.File.Move(tempFile, path);
                return true;
            }
            catch (Exception ex)
            {
            	Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
	
	}
}
