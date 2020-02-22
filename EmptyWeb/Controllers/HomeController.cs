using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmptyWeb.Models;
using Microsoft.AspNetCore.Http;

namespace EmptyWeb
{
	public class HomeController
	{
		private IStorage storage;
		public HomeController(IStorage storage)
		{
			this.storage = storage;
		}

		public async Task GetForm(HttpContext context)
		{
			await context.Response.WriteAsync(File.ReadAllText("Views\\index.html"));
		}

		public async Task AddEntry(HttpContext context)
		{
			string filePath = "Files";

			int fileCount = Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories).Length;
			fileCount++;

			foreach (var formFile in context.Request.Form.Files)
			{
				if (formFile.Length > 0)
				{
					string newFile = Path.Combine(filePath, fileCount + System.IO.Path.GetExtension(formFile.FileName));
					using (var inputStream = new FileStream(newFile, FileMode.Create))
					{
						// read file to stream
						await formFile.CopyToAsync(inputStream);
						// stream to byte array
						byte[] array = new byte[inputStream.Length];
						inputStream.Seek(0, SeekOrigin.Begin);
						inputStream.Read(array, 0, array.Length);
						// get file name
						string fName = formFile.FileName;
					}
				}
			}

			string name = context.Request.Form["name"];
			string text = context.Request.Form["text"];
			string txtFileName = Path.Combine(filePath, fileCount.ToString());
			File.AppendAllLines(txtFileName, new string[] {name, text});

			storage.Load(new BlogEntry { Name = name, Text = text, FileName = txtFileName });
			await context.Response.WriteAsync("New Entry was added");
		}

		public async Task GetBlogs(HttpContext context)
		{
			string filePath = "Files";

			string[] files = Directory.GetFiles(filePath, "*.txt", SearchOption.AllDirectories);
			var html = new StringBuilder();
			html.Append(@"<!DOCTYPE html><html><head><meta charset=""utf-8""/><title>Cписок постов</title></head><body>");

			foreach (var blog in (storage as BlogEntriesStorage).BlogEntries)
			{
				html.Append($@"<form>Наименование:<b>{blog.Name}</b><br/>
                                     Текст:<b>{blog.Text}</b><br/>
                                     Картинка:<img src=""{blog.FileName}""/><br/>
                                     </form>");
				html.Append(@"<form action=""/Home/ChangeBlog"" method=""get"" enctype=""multipart/form-data""><button type=""submit"">Меняем</button></form>");
				html.Append(@"<form action=""/Home/DeleteBlog"" method=""get"" enctype=""multipart/form-data""><button type=""submit"">Удоляем</button></form>");
				html.Append("\n");
			}
			html.Append(@"</body></html>");

			await context.Response.WriteAsync(html.ToString());
		}

		public async Task ChangeBlog(HttpContext context)
		{
			await context.Response.WriteAsync("lets change");
		}

		public async Task DeleteBlog(HttpContext context)
		{
			storage.Delete();
			await context.Response.WriteAsync("Deleted");
		}
	}
}
