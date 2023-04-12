using RunClubSite.Data.Enum;
using RunClubSite.Models;

namespace RunClubSite.ViewModels
{
	public class CreateRaceViewModel
	{

		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public Address Address { get; set; }
		public IFormFile Image { get; set; }
		public RaceCategory RaceCategory { get; set; }
        public string AppUserId { get; set; }
    }
}
