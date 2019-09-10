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
            var result = await _workflowHost.StartWorkflow(MultipleWorkflow.Run);

            Console.WriteLine(result);

            return Ok();
        }
    }


    class MultipleWorkflow : IWorkflow
    {
        public const string Run = "Run";

        public string Id { get; } = Run;

        public int Version { get; } = 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder
                .StartWith<SayHello>()
                .Then<DetermineSomething>()
                .When(a => 1)
                .Do(a => a.StartWith<PrintMessage>()
                    .Input(a => a.Message, d => "dd 1")
                )
                .When(a => 2)
                .Do(a => a.StartWith<PrintMessage>()
                    .Input(a => a.Message, d => "dd 2")
                )
                .Then(context =>
                {
                    Console.WriteLine("finished");
                    return ExecutionResult.Next();
                });
        }
    }

    internal class PrintMessage : StepBody
    {
        public string Message { get; set; }
        
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine(Message);
            return ExecutionResult.Next();
        }
    }

    internal class DetermineSomething : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            return ExecutionResult.Outcome(1);
        }
    }

    internal class SayHello : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Hello");
            return ExecutionResult.Next();
        }
    }
}