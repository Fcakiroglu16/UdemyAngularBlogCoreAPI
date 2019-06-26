using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UdemyAngularBlogCore.API.Models;
using UdemyAngularBlogCore.API.Responses;

namespace UdemyAngularBlogCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly UdemyAngularBlogDBContext _context;

        //api/articles
        public ArticlesController(UdemyAngularBlogDBContext context)
        {
            _context = context;
        }

        // GET: api/Articles/1/5
        [HttpGet]
        public IActionResult GetArticle()
        {
            var articles = _context.Article.Include(a => a.Category).Include(b => b.Comment).OrderByDescending(x => x.PublishDate).ToList().Select(y => new ArticleResponse()
            {
                Id = y.Id,
                Title = y.Title,
                Picture = y.Picture,
                Category = new CategoryResponse() { Id = y.Category.Id, Name = y.Category.Name },
                CommentCount = y.Comment.Count,

                ViewCount = y.ViewCount,
                PublishDate = y.PublishDate
            });
            return Ok(articles);
        }

        [HttpGet("{page}/{pageSize}")]
        public IActionResult GetArticle(int page = 1, int pageSize = 5)
        {
            System.Threading.Thread.Sleep(3000);

            try
            {
                IQueryable<Article> query;

                query = _context.Article.Include(x => x.Category).Include(y => y.Comment).OrderByDescending(z => z.PublishDate);

                int totalCount = query.Count();

                // 5*(1-1) => 0
                //5*(2-1)=>5
                var articlesResponse = query.Skip((pageSize * (page - 1))).Take(5).ToList().Select(x => new ArticleResponse()
                {
                    Id = x.Id,
                    Title = x.Title,
                    ContentMain = x.ContentMain,
                    ContentSummary = x.ContentSummary,
                    Picture = x.Picture,
                    ViewCount = x.ViewCount,
                    CommentCount = x.Comment.Count,
                    Category = new CategoryResponse() { Id = x.Category.Id, Name = x.Category.Name }
                });

                var result = new
                {
                    TotalCount = totalCount,
                    Articles = articlesResponse
                };
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //localhost/api/articles/GetArticlesWithCategory/2/1/5
        [HttpGet]
        [Route("GetArticlesWithCategory/{categoryId}/{page}/{pageSize}")]
        public IActionResult GetArticlesWithCategory(int categoryId, int page = 1, int pageSize = 5)
        {
            IQueryable<Article> query = _context.Article.Include(x => x.Category).Include(y => y.Comment).Where(z => z.CategoryId == categoryId).OrderByDescending(x => x.PublishDate);

            var queryResult = ArticlesPagination(query, page, pageSize);

            var result = new
            {
                TotalCount = queryResult.Item2,
                Articles = queryResult.Item1
            };
            return Ok(result);
        }

        [HttpGet]
        [Route("SearchArticles/{searchText}/{page}/{pageSize}")]
        public IActionResult SearchArticles(string searchText, int page = 1, int pageSize = 5)
        {
            IQueryable<Article> query;

            query = _context.Article.Include(x => x.Category).Include(y => y.Comment).Where(z => z.Title.Contains(searchText)).OrderByDescending(f => f.PublishDate);

            var resultQuery = ArticlesPagination(query, page, pageSize);

            var result = new
            {
                Articles = resultQuery.Item1,
                TotalCount = resultQuery.Item2
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("GetArticlesByMostView")]
        public IActionResult GetArticlesByMostView()
        {
            System.Threading.Thread.Sleep(2000);
            var articles = _context.Article.OrderByDescending(x => x.ViewCount).Take(5).Select(x => new ArticleResponse()
            {
                Title = x.Title,
                Id = x.Id
            });

            return Ok(articles);
        }

        [HttpGet]
        [Route("GetArticlesArchive")]
        public IActionResult GetArticlesArchive()
        {
            System.Threading.Thread.Sleep(1000);
            var query = _context.Article.GroupBy(x => new { x.PublishDate.Year, x.PublishDate.Month }).Select(y =>
                 new
                 {
                     year = y.Key.Year,
                     month = y.Key.Month,
                     count = y.Count(),
                     monthName = new DateTime(y.Key.Year, y.Key.Month, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("tr"))
                 });

            return Ok(query);
        }

        [HttpGet]
        [Route("GetArticleArchiveList/{year}/{month}/{page}/{pageSize}")]
        public IActionResult GetArticleArchiveList(int year, int month, int page, int pageSize)
        {
            System.Threading.Thread.Sleep(1700);

            IQueryable<Article> query;
            query = _context.Article.Include(x => x.Category).Include(y => y.Comment).Where(z => z.PublishDate.Year == year && z.PublishDate.Month == month).OrderByDescending(f => f.PublishDate);

            var resultQuery = ArticlesPagination(query, page, pageSize);

            var result = new
            {
                Articles = resultQuery.Item1,
                TotalCount = resultQuery.Item2
            };

            return Ok(result);
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public IActionResult GetArticle(int id)
        {
            System.Threading.Thread.Sleep(2000);

            var article = _context.Article.Include(x => x.Category).Include(y => y.Comment).FirstOrDefault(z => z.Id == id);

            if (article == null)
            {
                return NotFound();
            }
            ArticleResponse articleResponse = new ArticleResponse()
            {
                Id = article.Id,
                Title = article.Title,
                ContentMain = article.ContentMain,
                ContentSummary = article.ContentSummary,
                Picture = article.Picture,
                PublishDate = article.PublishDate,
                ViewCount = article.ViewCount,
                Category = new CategoryResponse() { Id = article.Category.Id, Name = article.Category.Name },
                CommentCount = article.Comment.Count
            };

            return Ok(articleResponse);
        }

        // PUT: api/Articles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, Article article)
        {
            Article firstArticle = _context.Article.Find(id);

            firstArticle.Title = article.Title;
            firstArticle.ContentSummary = article.ContentSummary;
            firstArticle.ContentMain = article.ContentMain;
            firstArticle.CategoryId = article.Category.Id;
            firstArticle.Picture = article.Picture;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Articles
        [HttpPost]
        public async Task<IActionResult> PostArticle(Article article)
        {
            if (article.Category != null)
            {
                article.CategoryId = article.Category.Id;
            }
            article.Category = null;
            article.ViewCount = 0;
            article.PublishDate = DateTime.Now;

            _context.Article.Add(article);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            return Ok();
            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Article.Remove(article);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }

        [Route("ArticleViewCountUp/{id}")]
        [HttpGet()]
        public IActionResult ArticleViewCountUp(int id)
        {
            Article article = _context.Article.Find(id);
            article.ViewCount += 1;
            _context.SaveChanges();
            return Ok();
        }

        public System.Tuple<IEnumerable<ArticleResponse>, int> ArticlesPagination(IQueryable<Article> query, int page, int pageSize)
        {
            System.Threading.Thread.Sleep(1500);

            int totalCount = query.Count();

            var articlesResponse = query.Skip((pageSize * (page - 1))).Take(pageSize).ToList().Select(x => new ArticleResponse()
            {
                Id = x.Id,
                Title = x.Title,
                ContentMain = x.ContentMain,
                ContentSummary = x.ContentSummary,
                Picture = x.Picture,
                ViewCount = x.ViewCount,
                CommentCount = x.Comment.Count,
                Category = new CategoryResponse() { Id = x.Category.Id, Name = x.Category.Name }
            });

            return new System.Tuple<IEnumerable<ArticleResponse>, int>(articlesResponse, totalCount);
        }

        [HttpPost]
        [Route("SaveArticlePicture")]
        public async Task<IActionResult> SaveArticlePicture(IFormFile picture)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/articlePictures", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await picture.CopyToAsync(stream);
            };
            var result = new
            {
                path = "https://" + Request.Host + "/articlePictures/" + fileName
            };

            return Ok(result);
        }
    }
}