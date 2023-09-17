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

        public UrlController(ILogger<UrlController> logger)
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
                // The 6 character restriction is not used for the CustomUrl but that is exist in normal shorened url code
                var customUrlValidationErrorMessage = ShortUrlValidator.IsCustomURLValid(model.LongUrl, model.CustomUrl);
                if (customUrlValidationErrorMessage != string.Empty)
                {
                    return BadRequest(customUrlValidationErrorMessage);
                }

                shortUrlCode = model.CustomUrl;
            }

            if (!ShortUrlValidator.IsUrlAliasUnique(model.LongUrl, shortUrlCode))
            {
                return Conflict("The collision part was not implemented! User ID information can be enhanced by adding Id, sessions or tokens etc.");
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
                return Redirect(longUrl);
            }

            return NotFound();
        }
    }
}