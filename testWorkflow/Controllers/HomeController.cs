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
            var result = await _workflowHost.StartWorkflow(PassDataWorkflow.Add,
                new MyDataClass {Input1 = 1, Input2 = 2});

            Console.WriteLine(result);

            return Ok();
        }
    }


    class PassDataWorkflow : IWorkflow<MyDataClass>
    {
        public const string Add = "Add";

        public string Id { get; } = Add;

        public int Version { get; } = 1;

        public void Build(IWorkflowBuilder<MyDataClass> builder)
        {
            builder
                .StartWith<AddNumber>()
                .Input(a => a.Input1, d => d.Input1)
                .Input(a => a.Input2, d => d.Input2)
                .Output(d => d.Output, a => a.Outpet)
                .Then<CustomMessage>()
                .Input(a => a.Message, d => $"Ans is {d.Output}")
                .Then(context =>
                {
                    Console.WriteLine("finished");
                    return ExecutionResult.Next();
                });
        }
    }

    internal class CustomMessage : StepBody
    {
        public string Message { get; set; }
        
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine(Message);
            return ExecutionResult.Next();
        }
    }

    internal class MyDataClass
    {
        public int Input1 { get; set; }
        public int Input2 { get; set; }
        public int Output { get; set; }
    }

    public class AddNumber : StepBody
    {
        public int Input1 { get; set; }

        public int Input2 { get; set; }

        public int Outpet { get; set; }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Outpet = Input1 + Input2;
            return ExecutionResult.Next();
        }
    }
}