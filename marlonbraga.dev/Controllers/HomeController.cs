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
            var query = _context.PostTags
                .Join(
                    _context.Posts,
                    pt => pt.IdPost,
                    p => p.IdPost,
                    (pt, p) => new {
                        IdPost = p.IdPost,
                        PublicationDate = p.PublicationDate,
                        Title = p.Title,
                        TumbNail = p.TumbNail,
                        Description = p.Description,
                        Content = p.Content,
                        IdTag = pt.IdTag
                    }
                )
                .Join(
                    _context.Tags,
                    join => join.IdTag,
                    t => t.IdTag,
                    (join, t) => new {
                        IdPost = join.IdPost,
                        PublicationDate = join.PublicationDate,
                        Title = join.Title,
                        TumbNail = join.TumbNail,
                        Description = join.Description,
                        Content = join.Content,
                        IdTag = join.IdTag,
                        Tag = t.Name,
                    }
                )
                .OrderByDescending(a => a.PublicationDate)
                .ToList();
            List<Post> Posts = new List<Post>();
            Post AuxiliarPost = new Post();
            AuxiliarPost.IdPost = -1;
            foreach(var postTag in query) {
                if(postTag.IdPost != AuxiliarPost.IdPost) {
                    Tag tag = new Tag(postTag.IdTag, postTag.Tag);
                    AuxiliarPost = new Post();
                    AuxiliarPost.IdPost = postTag.IdPost;
                    AuxiliarPost.Tags = new List<Tag>();
                    AuxiliarPost.Tags.Add(tag);
                    AuxiliarPost.Description = postTag.Description;
                    AuxiliarPost.Content = postTag.Content;
                    AuxiliarPost.Title = postTag.Title;
                    AuxiliarPost.PublicationDate = postTag.PublicationDate;
                    AuxiliarPost.TumbNail = postTag.TumbNail;
                    Posts.Add(AuxiliarPost);
                } else {
                    Tag tag = new Tag(postTag.IdTag, postTag.Tag);
                    Posts.LastOrDefault().Tags.Add(tag);
                }
            }
            ViewBag.Posts = Posts.Take(3);
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
