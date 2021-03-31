using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.it_API.Controllers
{
    [Route("api/[controller]")]
    public class QuestsController : ControllerBase
    {
        public QuestsController()
        {

        }

        public object Get()
        {
            return new { QuestId = 5, Name = "BlaBla" };
        }
    }
}
