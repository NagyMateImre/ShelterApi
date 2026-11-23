namespace ShelterApi.Models.DTO
{
    public class UpdateAnimaDTO
    {

        public string? Name { get; set; }

        public string? Species { get; set; }

        public int? Age { get; set; }

        public double? Weight { get; set; }

        public Guid? ShelterId { get; set; }

    }
}
