using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmptyWeb.Models;

namespace EmptyWeb
{
	public class BlogEntriesStorage : IStorage
	{
		//public IEnumerable<BlogEntry> BlogEntries { get; set; }
		public List<BlogEntry> BlogEntries { get; set; }

		public BlogEntriesStorage()
		{
			BlogEntries = new List<BlogEntry>();
		}

		public void Load(BlogEntry blog)
		{
			BlogEntries.Add(blog);
		}

		public void Save()
		{

		}

		public void Delete()
		{
			if (File.Exists($"{BlogEntries[0].FileName}.txt"))
				File.Delete($"{BlogEntries[0].FileName}.txt");
			if (File.Exists($"{BlogEntries[0].FileName}.jpg"))
				File.Delete($"{BlogEntries[0].FileName}.jpg");
			BlogEntries.Remove(BlogEntries[0]);
		}
	}
}
