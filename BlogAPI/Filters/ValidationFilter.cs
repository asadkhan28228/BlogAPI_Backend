using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BlogAPI.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            foreach (var actionArgument in context.ActionArguments.Values)
            {
                if (actionArgument == null)
                {
                    continue;
                }

                var argumentType = actionArgument.GetType();

                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

                if (context.HttpContext.RequestServices.GetService(validatorType)
                    is not IValidator validator)
                {
                    // Is DTO ka koi validator register nahi, skip kar do
                    continue;
                }

                var validationContext = new ValidationContext<object>(actionArgument);

                var validationResult = await validator.ValidateAsync(validationContext);

                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        context.ModelState.AddModelError(
                            error.PropertyName,
                            error.ErrorMessage);
                    }
                }
            }

            if (!context.ModelState.IsValid)
            {
                var problemDetailsFactory = context.HttpContext.RequestServices
                    .GetRequiredService<ProblemDetailsFactory>();

                var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                    context.HttpContext,
                    context.ModelState);

                context.Result = new BadRequestObjectResult(problemDetails);

                return;
            }

            await next();
        }
    }
}