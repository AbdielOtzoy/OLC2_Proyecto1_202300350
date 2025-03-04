using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using analyzer;
using Antlr4.Runtime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Antlr4.Runtime.Misc;

namespace api.Controllers
{
    [Route("compile")]
    [ApiController] // Agrega esta anotación para mejores validaciones automáticas
    public class CompileController : ControllerBase
    {
        private readonly ILogger<CompileController> _logger;

        public CompileController(ILogger<CompileController> logger)
        {
            _logger = logger;
        }

        // GET /compile
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Compile API is running");
        }

        // GET /compile/error
        [HttpGet("error")]
        public IActionResult Error()
        {
            return Problem("An error occurred");
        }

        public class CompileRequest
        {
            [Required]
            public required string Code { get; set; }
        }

        // POST /compile
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CompileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Compiling code: {0}", request.Code);

            var inputStream = new AntlrInputStream(request.Code);
            var lexer = new LanguageLexer(inputStream);

            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new DescriptiveErrorListener());

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new LanguageParser(tokenStream);

            parser.RemoveErrorListeners();
            parser.AddErrorListener(new SyntaxErrorListener());

            try
            {
                var tree = parser.program();

                var visitor = new CompilerVisitor();
                visitor.Visit(tree);

                return Ok(new { result = visitor.output});
            }
            catch (ParseCanceledException e)
            {
                Console.WriteLine("mensaje:" + e.Message);
                return BadRequest( new { message = e.Message });
            }
            catch (RecognitionException e)
            {
                Console.WriteLine("mensaje:" + e.Message);
                return BadRequest( new { message = e.Message });
            }
            catch (SemanticError e)
            {
                Console.WriteLine("mensaje:" + e.Message);
                return BadRequest( new { message = e.Message });
            }

        }
    }
}
