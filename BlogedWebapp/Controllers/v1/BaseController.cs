using BlogedWebapp.Data;
using Microsoft.AspNetCore.Mvc;

namespace BlogedWebapp.Controllers.v1
{
    /// <summary>
    ///  Base controller for other controllers
    /// </summary>
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class BaseController : ControllerBase
    {
        protected IUnitOfWork unitOfWork;
        
        public BaseController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
    }
}
