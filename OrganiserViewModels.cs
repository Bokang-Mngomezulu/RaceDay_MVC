using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDay_MVC.Models
{
    public class EventFormViewModel
    {
        public int? EventId { get; set; }

        [Required(ErrorMessage = "Please enter the event name.")]
        [StringLength(150)]
        [Display(Name = "Event name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please choose the event date and time.")]
        [Display(Name = "Date and time")]
        public DateTime? EventDate { get; set; }

        [Required(ErrorMessage = "Please enter the location.")]
        [StringLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the distance, for example 10km.")]
        [StringLength(50)]
        public string Distance { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please choose a sport type.")]
        [Display(Name = "Sport type")]
        public string SportType { get; set; } = "Run"; 

        [Display(Name = "Extra information")]
        public string? Information { get; set; }

        [StringLength(255)]
        [Display(Name = "Weather note")]
        public string? LiveWeather { get; set; }

        [Display(Name = "Banner image")]
        public IFormFile? Banner { get; set; }

        public string? ExistingBannerUrl { get; set; }
    }

    public class CategoryFormViewModel
    {
        [Required(ErrorMessage = "Please enter a category name.")]
        [StringLength(50)]
        [Display(Name = "Category name")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Age group")]
        public string? AgeGroup { get; set; }

        [StringLength(50)]
        public string? Distance { get; set; }

        [Range(0, 100000, ErrorMessage = "The entry fee must be 0 or more.")]
        [Display(Name = "Entry fee (R)")]
        public decimal? EntryFee { get; set; }

        [Range(1, 100000, ErrorMessage = "The maximum entrants must be at least 1.")]
        [Display(Name = "Maximum entrants")]
        public int? MaxCapacity { get; set; }
    }

    public class ResultFormViewModel
    {
        public int EventId { get; set; }

        public int EnrolmentId { get; set; }

        [Required(ErrorMessage = "Please enter a finish time.")]
        [Range(0.01, 9999.99, ErrorMessage = "The finish time must be between 0.01 and 9999.99 minutes.")]
        public decimal? Time { get; set; }

        [Range(1, 100000, ErrorMessage = "The position must be 1 or more.")]
        public int? OverallPosition { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalEvents { get; set; }
        public int TotalEnrolments { get; set; }
        public List<OrganiserEventDto> Upcoming { get; set; } = new();
        public List<OrganiserEventDto> Past { get; set; } = new();
    }

    public class CategoriesViewModel
    {
        public EventDetailDto Event { get; set; } = null!;
        public CategoryFormViewModel Form { get; set; } = new();
    }

    public class EnrolmentsViewModel
    {
        public EventDetailDto Event { get; set; } = null!;
        public List<EventEnrolmentDto> Enrolments { get; set; } = new();
    }

    public class ResultsViewModel
    {
        public EventDetailDto Event { get; set; } = null!;
        public bool Concluded { get; set; }
        public List<EventEnrolmentDto> Pending { get; set; } = new();
        public List<EventResultDto> Published { get; set; } = new();
    }
}
