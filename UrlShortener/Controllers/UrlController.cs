using Microsoft.AspNetCore.Mvc;
using UrlShortener.Db;
using UrlShortener.Models;
using UrlShortener.Util;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {
        //Hamit Yıldırım
        //18.09.2023
        //23:00
        //this project prepared for the interview of the company named "XXX"
        
        public UrlController()
        {
        }

        [HttpPost("shorten")]
        public ActionResult<string> ShortenUrl([FromBody] UrlModel model)
        {
            var originalUrlValidationErrorMessage = ShortUrlValidator.IsOriginalURLValid(model.LongUrl);
            if(originalUrlValidationErrorMessage != string.Empty)
            {                 
                return BadRequest(originalUrlValidationErrorMessage);   
            }

            UrlShortener.Util.UrlShortener urlShortener = new UrlShortener.Util.UrlShortener();
            string shortUrlCode = urlShortener.ShortenUrl(model.LongUrl);

            if (!string.IsNullOrEmpty(model.CustomUrl))
            {
                var customUrlValidationErrorMessage = ShortUrlValidator.IsCustomURLValid(model.LongUrl, model.CustomUrl);
                if (customUrlValidationErrorMessage != string.Empty)
                {
                    return BadRequest(customUrlValidationErrorMessage);
                }

                shortUrlCode = model.CustomUrl;
            }

            if (!ShortUrlValidator.IsUrlAliasUnique(model.LongUrl, shortUrlCode))
            {
                return Conflict("The collision part was implemented just for it's existence.");
            }

            var shortenedUrl = urlShortener.CombineUrl(model.LongUrl, shortUrlCode);
            MemoryDb.Urls.Add(shortenedUrl, model.LongUrl);

            return Ok(shortenedUrl);
        }

        [HttpGet("{shortUrl}")]
        public ActionResult RedirectUrl(string shortenedUrl)
        {
            if (MemoryDb.Urls.TryGetValue(shortenedUrl, out var longUrl))
            {
                return Ok(longUrl);

                // CORS policy error
                //return Redirect(longUrl);
            }

            return NotFound();
        }

    }
}