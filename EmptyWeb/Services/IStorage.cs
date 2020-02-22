using EmptyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmptyWeb
{
	public interface IStorage
	{
		void Load(BlogEntry blog);
		void Save();
		void Delete();
	}
}
