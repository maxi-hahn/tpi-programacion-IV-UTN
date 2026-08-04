using Application.Dtos.Request;
using Application.Dtos.Responses;
using Domain.Entity;

namespace Application.Mapper
{
    public static class ClassMapper
    {
        public static ClassResponse ToClassResponse(this Class gymClass, Guid? userId = null)
        {
            var enrolledUsers = gymClass.Schedules?
                .SelectMany(s => s.Inscriptions)
                .Count(i => i.IsActive) ?? 0;

            var totalMaxUsers = (gymClass.Schedules?.Count ?? 0) * gymClass.Max_Users;
            var availableSpots = Math.Max(0, totalMaxUsers - enrolledUsers);
            var isFull = (gymClass.Schedules?.Count ?? 0) > 0 && availableSpots == 0;

            return new ClassResponse
            {
                Id = gymClass.Id,
                Name = gymClass.Name,
                Max_Users = gymClass.Max_Users,
                EnrolledUsers = enrolledUsers,
                AvailableSpots = availableSpots,
                IsFull = isFull,

                Schedules = gymClass.Schedules?
                    .Select(s => s.ToScheduleResponse(gymClass.Max_Users, userId))
                    .ToList() ?? new()
            };
        }
        public static ClassDetailResponse ToClassDetailResponse(this Class gymClass, int currentInscriptions, List<ClientInfoResponse> clients)
        {
            return new ClassDetailResponse
            {
                Id = gymClass.Id,
                Name = gymClass.Name,
                Max_Users = gymClass.Max_Users,
                CurrentInscriptions = currentInscriptions,
                Schedules = gymClass.Schedules
                .Select(s => s.ToScheduleResponse(gymClass.Max_Users))
                .ToList(),
                Clients = clients
            };
        }

        public static Class ToClass(this CreateClassRequest request)
        {
            return new Class
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Max_Users = request.Max_Users
            };
        }
    }
}
