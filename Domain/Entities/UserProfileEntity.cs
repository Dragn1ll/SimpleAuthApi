using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models;

namespace Domain.Entities;

public class UserProfileEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }
    public UserEntity User { get; set; }
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    public DateOnly? DateOfBirth { get; set; }
    
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    [MaxLength(100)]
    public string? Country { get; set; }
    
    [MaxLength(100)]
    public string? City { get; set; }
    
    [MaxLength(500)]
    public string? Bio { get; set; }
    
    [MaxLength(255)]
    public string? AvatarUrl { get; set; }
}