using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using marlonbraga.dev.Models;
using marlonbraga.dev.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace marlonbraga.dev.Controllers {
	public class HomeController:Controller {
		private readonly Context _context;
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger, Context context) {
			_logger = logger;
			_context = context;
		}

		public async Task<IActionResult> IndexAsync() {
			return View(await _context.Posts.ToListAsync());
		}

		public IActionResult Privacy() {
			return View();
		}

		public IActionResult Sobre() {
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() {
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
