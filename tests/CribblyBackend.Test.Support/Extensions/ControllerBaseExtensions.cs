using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CribblyBackend.Test.Support.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static void AddTestUser(this ControllerBase controller, string authId, string email = "")
        {
            var user = TestData.NewIdentityUser(authId, email);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        public static void AddHeader(this ControllerBase controller, string header, string value)
        {
            var context = controller.ControllerContext.HttpContext;
            if (controller.ControllerContext.HttpContext == null)
            {
                controller.ControllerContext.HttpContext = new DefaultHttpContext();
            }
            controller.ControllerContext.HttpContext.Request.Headers.Add(header, value);
        }
    }
}