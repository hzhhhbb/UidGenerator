using Microsoft.AspNetCore.Mvc;

namespace Vincent.UidGenerator.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UidController : ControllerBase
{
    private readonly IUidGenerator _uidGenerator;

    public UidController(IUidGenerator uidGenerator)
    {
        _uidGenerator = uidGenerator;
    }
    
    [HttpGet]
    public  long Get()
    {
        return _uidGenerator.GetUid();
    }
}