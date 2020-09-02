using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marlonbraga.dev.Models.Context {
	public class Context : DbContext {
		public Context(DbContextOptions<Context> options) : base(options){}
	}
}
