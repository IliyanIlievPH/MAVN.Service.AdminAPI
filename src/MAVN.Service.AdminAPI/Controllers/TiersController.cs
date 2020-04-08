namespace MAVN.Service.AdminAPI.Controllers
{
    /*
     
     Not required atm
    
    [ApiController]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class TiersController : ControllerBase
    {
        private readonly ITiersClient _tiersClient;
        private readonly IMapper _mapper;

        public TiersController(ITiersClient tiersClient, IMapper mapper)
        {
            _tiersClient = tiersClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all reward tiers with number of customers.
        /// </summary>
        /// <returns>
        /// A collection of reward tiers.
        /// </returns>
        /// <response code="200">A collection of reward tiers.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<TierModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<TierModel>> GetAllTiersAsync()
        {
            var items = await _tiersClient.Reports.GetNumberOfCustomersPerTierAsync();

            return _mapper.Map<List<TierModel>>(items.OrderBy(o => o.Threshold));
        }
    }
    
    */
}
