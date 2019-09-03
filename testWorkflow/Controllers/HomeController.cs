using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using testWorkflow.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace testWorkflow.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWorkflowHost _workflowHost;

        public HomeController(IWorkflowHost workflowHost)
        {
            _workflowHost = workflowHost;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Workflow()
        {
            var result = await _workflowHost.StartWorkflow(HelloWorldWorkflow.Helloworld);
            Console.WriteLine(result);

            return Ok();
        }
    }

    class HelloWorld : StepBody
    {
        private readonly ILogger<HelloWorld> _logger;

        public HelloWorld(ILogger<HelloWorld> logger)
        {
            _logger = logger;
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Hello World");
            _logger.LogInformation("Hello World");
            return ExecutionResult.Next();
        }
    }

    class GoodbyeWorld : StepBody
    {
        private readonly ILogger<GoodbyeWorld> _logger;

        public GoodbyeWorld(ILogger<GoodbyeWorld> logger)
        {
            _logger = logger;
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Goodby");
            _logger.LogInformation("Goodby");
            return ExecutionResult.Next();
        }
    }

    class HelloWorldWorkflow : IWorkflow
    {
        public const string Helloworld = "HelloWorld";

        public string Id { get; } = Helloworld;

        public int Version { get; } = 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
//            builder.StartWith<HelloWorld>()
//                .Then<GoodbyeWorld>();

            builder.StartWith(context =>
            {
                Console.WriteLine($"Hello World - {context.Step.Id}");
                return ExecutionResult.Next();
            }).Then(context =>
            {
                Console.WriteLine($"Goodbye World - {context.Step.Id}");
                return ExecutionResult.Next();
            });
        }
    }
}