

using Business_Layer.SearchTries;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    public ProductTrie ProductTrie { get; }
    public string ConnectionString { get; }

    public SearchController(ProductTrie trie, string connectionString)
    {
        ProductTrie = trie;
        ConnectionString = connectionString;
    }

    [HttpGet("{word}")]
    public async Task<IActionResult> GetWordsStartWith(string word)
    {
        return Ok(await ProductTrie.GetWordsStartWith(word));
    }


}
