using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using marlonbraga.dev.Models;
using marlonbraga.dev.Models.Context;
using Microsoft.EntityFrameworkCore.Internal;

namespace marlonbraga.dev
{
    public class PostsController : Controller
    {
        private readonly Context _context;

        public PostsController(Context context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var query = _context.PostTags
                .Join(
                    _context.Posts,
                    pt => pt.IdPost,
                    p => p.IdPost,   
                    (pt,p) => new {
                        IdPost = p.IdPost,
                        PublicationDate = p.PublicationDate,
                        Title = p.Title,
                        TumbNail = p.TumbNail,
                        Description = p.Description,
                        IdTag = pt.IdTag
                    }
                )
				.Join(
					_context.Tags,
					join => join.IdTag,
					t => t.IdTag,
					(join, t) => new  {
                        IdPost = join.IdPost,
                        PublicationDate = join.PublicationDate,
                        Title = join.Title,
                        TumbNail = join.TumbNail,
                        Description = join.Description,
                        IdTag = join.IdTag,
                        Tag = t.Name,
                    }
				)
				.ToList();
			List<Post> Posts = new List<Post>();
            Post AuxiliarPost = new Post(-1);
            foreach(var postTag in query) {
                if(postTag.IdPost != AuxiliarPost.IdPost)
                {
                    Tag tag = new Tag(postTag.IdTag, postTag.Tag);
                    AuxiliarPost = new Post(postTag.IdPost);
                    AuxiliarPost.Tags.Add(tag);
                    AuxiliarPost.Description = postTag.Description;
                    AuxiliarPost.Title = postTag.Title;
                    AuxiliarPost.PublicationDate = postTag.PublicationDate;
                    AuxiliarPost.TumbNail = postTag.TumbNail;
                    Posts.Add(AuxiliarPost);
                }
                else
                {
                    Tag tag = new Tag(postTag.IdTag, postTag.Tag);
                    Posts.LastOrDefault().Tags.Add(tag);
                }
            }
            ViewBag.Posts = Posts;
            return View(await _context.Posts.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var post = await _context.Posts
				.FirstOrDefaultAsync(m => m.IdPost == id);

			List<PostTag> postTags = _context.PostTags
                .Where(i => i.IdPost == id).ToList();
            List<Tag> Tags = new List<Tag>();
            foreach(var postTag in postTags) {
                Tag tag = _context.Tags.Where(i => i.IdTag == postTag.IdTag).FirstOrDefault();
                Tags.Add(tag);
            }
            ViewBag.Tags = Tags;

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            //List<PostTag> postTags = _context.PostTags.OrderBy(i => i.IdPost).ToList();
            //postTags.Insert(0, new PostTag() {
            //    IdPost = 0,
            //    Title = "Selecione um título"
            //});
            ////var postTags = _context.PostTags.Where(i => i.IdPost)

            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPost,PublicationDate,Title,TumbNail,Description,Content")] Post post)
        {
            if (ModelState.IsValid)
            {
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
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.IdPost == id);
        }
    }
}
