using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using marlonbraga.dev.Models;
using marlonbraga.dev.Models.Context;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace marlonbraga.dev {
	public class PostsController : Controller
    {
        private readonly Context _context;
		private readonly IWebHostEnvironment _hostEnvironment;

		public PostsController(Context context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
			this._hostEnvironment = hostEnvironment;
		}

        private bool CheckUser() {
            return HttpContext.Session.GetString("user") == "Marlon Braga";
        }

        // GET: Posts
        public async Task<IActionResult> Index() {
            ViewBag.Posts = GetPostsWithTags();
            return View(await _context.Posts.ToListAsync());
        }
        public async Task<IActionResult> Administration() {
            ViewBag.Posts = GetPostsWithTags();
            return View(await _context.Posts.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = GetPostsWithTags().Find(post => post.IdPost==id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
			if(!CheckUser()) {
                return RedirectToAction("Login", "Home");
            }
            List<Tag> Tags = _context.Tags.ToList();
            ViewBag.Tags = Tags;
            var m = new Post() {
                Tags = ViewBag.Tags
            };
            return View(m);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if(!CheckUser()) {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                //Save image to wwwroot/img/headers
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(post.ImageFile.FileName);
                string extension = Path.GetExtension(post.ImageFile.FileName);
                post.TumbNail = fileName = fileName + DateTime.Now.ToString("yyMMddssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/img/headers/", fileName);
				using(var fileStream =  new FileStream (path,FileMode.Create))
                {
                    await post.ImageFile.CopyToAsync(fileStream);
				}

                //Insert Record
                _context.Add(post);

                //Get last Post Id
                int value = int.Parse(_context.Posts
                            .OrderByDescending(p => p.IdPost)
                            .Select(r => r.IdPost)
                            .First().ToString());

                //"1,2,3,"   |   ""   |   "10,"
                string[] tagsId = post.TagsId.Split(',');
				if(tagsId.Count() > 1) {
                    tagsId = tagsId.Take(tagsId.Count() - 1).ToArray();
				}
                List<PostTag> PostTags = new List<PostTag>();
                foreach(var tag in tagsId) {
                    PostTag postTag = new PostTag() {
                        IdPost = post.IdPost,
                        IdTag = Int16.Parse(tag),
                        Post = post,
                        Tag = null
                    };
                    _context.Add(postTag);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if(!CheckUser()) {
                return RedirectToAction("Login", "Home");
            }
            List<Tag> Tags = _context.Tags.ToList();
            ViewBag.Tags = Tags;
            var m = new Post() {
                Tags = ViewBag.Tags
            };

            if (id == null)
            {
                return NotFound();
            }

            //var post = await _context.Posts.FindAsync(id);
            var post = GetPostsWithTags().Find(post => post.IdPost == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if(!CheckUser()) {
                return RedirectToAction("Login", "Home");
            }
            if (id != post.IdPost)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Delete old PostTags
                    List<PostTag> query = _context.PostTags
                    .Where(pt => pt.IdPost == id)
                    .ToList();

					foreach(PostTag item in query) {
                        _context.Remove(item);
                    }

                    //"Add PostTags"
                    string[] tagsId = post.TagsId.Split(',');
                    if(tagsId.Count() > 1) {
                        tagsId = tagsId.Take(tagsId.Count() - 1).ToArray();
                    }
                    List<PostTag> PostTags = new List<PostTag>();
                    foreach(var tag in tagsId) {
                        PostTag postTag = new PostTag() {
                            IdPost = post.IdPost,
                            IdTag = Int16.Parse(tag),
                            Post = post,
                            Tag = null
                        };
                        _context.Add(postTag);
                    }
                    Post p = _context.Posts.Find(id);
                    if(post.ImageFile != null) {
                        //Delete image from wwwroot/img/headers
                        if(p.TumbNail != null) {
                            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "img/headers", p.TumbNail);
                            if(System.IO.File.Exists(imagePath)) {
                                System.IO.File.Delete(imagePath);
                            }
                        }

                        //Save image to wwwroot/img/headers
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(post.ImageFile.FileName);
                        string extension = Path.GetExtension(post.ImageFile.FileName);
                        post.TumbNail = fileName = fileName + DateTime.Now.ToString("yyMMddssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/img/headers/", fileName);
                        using(var fileStream = new FileStream(path, FileMode.Create)) {
                            await post.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.IdPost))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if(!CheckUser()) {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.IdPost == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if(!CheckUser()) {
                return RedirectToAction("Login", "Home");
            }
            var post = await _context.Posts.FindAsync(id);

            //Delete image from wwwroot/img/headers
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "img/headers", post.TumbNail);
			if(System.IO.File.Exists(imagePath)) {
                System.IO.File.Delete(imagePath);
			}

            //Delete the record
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.IdPost == id);
        }

        private List<Post> GetPostsWithTags() {
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
            return Posts;
		}
    }
}
