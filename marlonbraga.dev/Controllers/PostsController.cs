using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using marlonbraga.dev.Models;
using marlonbraga.dev.Models.Context;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;

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

        // GET: Posts
        public async Task<IActionResult> Index()
        {
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
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPost,PublicationDate,Title,ImageFile,Description,Content")] Post post)
        {
            if (ModelState.IsValid)
            {
                //Save image to wwwroot/img/headers
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(post.ImageFile.FileName);
                string extension = Path.GetExtension(post.ImageFile.FileName);
                post.TumbNail = fileName = fileName + DateTime.Now.ToString("yymmddssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/img/headers/", fileName);
				using(var fileStream =  new FileStream (path,FileMode.Create))
                {
                    await post.ImageFile.CopyToAsync(fileStream);
				}

                //Insert Record
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("IdPost,PublicationDate,Title,TumbNail,Description,Content")] Post post)
        {
            if (id != post.IdPost)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            return Posts;
		}
    }
}
