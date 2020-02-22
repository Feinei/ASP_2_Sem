using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmptyWeb
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient<IMessageSender, EmailMessageSender>();
			services.AddTransient<IStorage, BlogEntriesStorage>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStorage storage)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", new HomeController(storage).GetForm);
				endpoints.MapPost("/Home/AddEntry", new HomeController(storage).AddEntry);
				endpoints.MapGet("/Home/GetBlogs", new HomeController(storage).GetBlogs);
				endpoints.MapGet("/Home/ChangeBlog", new HomeController(storage).ChangeBlog);
				endpoints.MapGet("/Home/DeleteBlog", new HomeController(storage).DeleteBlog);
			});

			LoadFiles(storage);
		}

		private void LoadFiles(IStorage storage)
		{
			string filePath = "Files";
			string[] files = Directory.GetFiles(filePath, "*.txt", SearchOption.AllDirectories);
			storage = new BlogEntriesStorage();
			foreach (var file in files)
			{
				var fileName = file.Remove(0, filePath.Length + 1);
				fileName = fileName.Remove(fileName.Length - 4, 4);
				var image = Directory.GetFiles(filePath, $"{fileName}.jpg", SearchOption.AllDirectories).FirstOrDefault();

				string name;
				string text;
				using (StreamReader reader = new StreamReader(file))
				{
					name = reader.ReadLine();
					text = reader.ReadLine();
				}

				storage.Load(new Models.BlogEntry { Name = name, Text = text, FileName = image });
			}
		}
	}
}
