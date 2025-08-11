using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudentInfoSys.WebApi.Filters
{
    public class TimeControlFilter : ActionFilterAttribute
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public TimeControlFilter(string startTime, string endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //var now = DateTime.Now.TimeOfDay; // Local, Turkey (UTC+03:00)
            var now = DateTime.UtcNow.TimeOfDay; // Global (UTC+00:00)

            var start = TimeSpan.Parse(StartTime);
            var end = TimeSpan.Parse(EndTime);

            bool isBlocked;
            if (start < end)
                isBlocked = now >= start && now <= end;
            else
                isBlocked = now >= start || now <= end;

            if (isBlocked)
            {
                context.Result = new ContentResult
                {
                    Content = $"Requests cannot be sent to this endpoint between {StartTime} and {EndTime}",
                    StatusCode = 403
                };
            }
            else
            {
                base.OnActionExecuting(context);
            }
        }
    }
}
