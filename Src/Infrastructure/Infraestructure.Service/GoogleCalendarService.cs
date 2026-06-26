using Domain.Entity;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Dtos.Request;
using Application.Application.Interfaces;
public class GoogleCalendarService : IGoogleCalendarService
{
    private readonly HttpClient _httpClient;

    public GoogleCalendarService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

     public async Task SyncInscriptionAsync(
        string accessToken,
        Inscription inscription,
        string className,
        List<ClassScheduleRequest> schedules)
    {
        foreach (var schedule in schedules)
        {
            await CreateRecurringEventAsync(
                accessToken,
                className,
                inscription.InscriptionDate,
                schedule
            );
        }
    }

    // creates a recurring event in Google Calendar based on the class schedule
    private async Task CreateRecurringEventAsync(
    string accessToken,
    string title,
    DateTime startDate,
    ClassScheduleRequest schedule)
    {
        var dayCode = ToSystemDay(schedule.Day);

        var firstDate = GetFirstOccurrence(startDate, ToSystemDay(schedule.Day));

        var startDateTime = firstDate.Date + schedule.StartTime;
        var endDateTime = firstDate.Date + schedule.EndTime;

        var body = new
        {
            summary = title,

            start = new
            {
                dateTime = startDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss"),
                timeZone = "America/Argentina/Buenos_Aires"
            },

            end = new
            {
                dateTime = endDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss"),
                timeZone = "America/Argentina/Buenos_Aires"
            },

            recurrence = new[]
            {
            $"RRULE:FREQ=WEEKLY;BYDAY={dayCode}"
        }
        };

        var json = JsonSerializer.Serialize(body);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://www.googleapis.com/calendar/v3/calendars/primary/events"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Google Calendar error: {response.StatusCode} - {content}");
        }
    }

    // calculates the first occurrence of the target day of week on or after the start date
    private DateTime GetFirstOccurrence(DateTime start, DayOfWeek targetDay)
    {
        var diff = ((int)targetDay - (int)start.DayOfWeek + 7) % 7;
        return start.AddDays(diff);
    }

    // converts the custom Day enum to the standard DayOfWeek enum used by Google Calendar
    private DayOfWeek ToSystemDay(Domain.Entity.Day day)
    {
        return day switch
        {
            Domain.Entity.Day.Lunes => DayOfWeek.Monday,
            Domain.Entity.Day.Martes => DayOfWeek.Tuesday,
            Domain.Entity.Day.Miercoles => DayOfWeek.Wednesday,
            Domain.Entity.Day.Jueves => DayOfWeek.Thursday,
            Domain.Entity.Day.Viernes => DayOfWeek.Friday,
            Domain.Entity.Day.Sabado => DayOfWeek.Saturday,
            Domain.Entity.Day.Domingo => DayOfWeek.Sunday,
            _ => throw new Exception("Invalid day")
        };
    }
     }