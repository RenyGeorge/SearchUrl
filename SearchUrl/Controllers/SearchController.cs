using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SearchUrl.Models;
using SearchUrl.Services;

namespace SearchUrl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly SearchEngines _searchEngines;
        public SearchController(IOptions<SearchEngines> searchEngine)
        {
            _searchEngines = searchEngine.Value;
        }
        // GET api/get
        [HttpGet("[action]")]
        public ActionResult<SearchEngines> Get()
        {
            
            return new JsonResult(_searchEngines);
        }

        // POST api/search
        [HttpPost("[action]")]
        public ActionResult SearchResult([FromForm] string searchEngine,[FromForm] string searchString, [FromForm] string searchUrl)
        {
            if (!string.IsNullOrEmpty(searchEngine) && _searchEngines != null && _searchEngines.searchConfigs.Count() > 0)
            {
                var selectedEngine = _searchEngines.searchConfigs.Where(t => t.name == searchEngine).FirstOrDefault();
                SearchService searchService = new SearchService();
                return new JsonResult(searchService.ProcessSearchResult(selectedEngine, searchString,searchUrl));
            }
            return new JsonResult("0 Results Found");
        }
        
    }
}
