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
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

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
                        Color = t.Color,
                    }
                )
                .OrderByDescending(a => a.PublicationDate)
                .ToList();
            List<Post> Posts = new List<Post>();
            Post AuxiliarPost = new Post();
            AuxiliarPost.IdPost = -1;
            foreach(var postTag in query) {
                if(postTag.IdPost != AuxiliarPost.IdPost) {
                    Tag tag = new Tag(postTag.IdTag, postTag.Tag, postTag.Color);
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
                    Tag tag = new Tag(postTag.IdTag, postTag.Tag, postTag.Color);
                    Posts.LastOrDefault().Tags.Add(tag);
                }
            }
            ViewBag.Posts = Posts.Take(3);
            List<Tag> Tags = _context.Tags.ToList();
            ViewBag.Tags = Tags;
            return View(await _context.Posts.ToListAsync());
		}

        public IActionResult Sobre() {
            return View();
        }
        
        public IActionResult Login(string User, string Password) {
            if((User == "Marlon Braga") && (ComparaMD5(Password, "fdb05cf0bfab1b69c5622fcf29dff7c6"))) {
                //TODO: Mensagem de Bem-Vindo, Sr. Marlon.
                HttpContext.Session.SetString("user", "Marlon Braga");
                return RedirectToAction("Administration","Posts");
            } else {
                //TODO: Mensagem de senha incorreta
			}
            return View();
        }
        public IActionResult Logout() {
            HttpContext.Session.SetString("user", "");
            HttpContext.Session.Remove("user");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        private string RetornarMD5(string Senha) {
            using(MD5 md5Hash = MD5.Create()) {
                return RetonarHash(md5Hash, Senha);
            }
        }
        private bool ComparaMD5(string senhabanco, string Senha_MD5) {
            using(MD5 md5Hash = MD5.Create()) {
                var senha = RetornarMD5(senhabanco);
                if(VerificarHash(Senha_MD5, senha)) {
                    return true;
                } else {
                    return false;
                }
            }
        }
        private string RetonarHash(MD5 md5Hash, string input) {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for(int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
        private bool VerificarHash(string input, string hash) {
            StringComparer compara = StringComparer.OrdinalIgnoreCase;

            if(0 == compara.Compare(input, hash)) {
                return true;
            } else {
                return false;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() {
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
