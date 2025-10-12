using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

public class UserEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MinLength(8)]
    [MaxLength(50)]
    public string Email { get; set; }
    
    [Required]
    [MinLength(8)]
    [MaxLength(50)]
    public string Password { get; set; }
    
    [Required]
    [MinLength(4)]
    [MaxLength(50)]
    public string Username { get; set; }
    
    [Required]
    public DateOnly CreatedDate { get; set; }
    
    [Required]
    public DateOnly UpdatedDate { get; set; }
    
    [Required]
    public int GenderId { get; set; }
    public GenderEntity Gender { get; set; }

    [Required]
    public int RoleId { get; set; }
    public RoleEntity Role { get; set; }
    
    [Required]
    public bool IsActive { get; set; }
    
    public DateTime? LastLoginDate { get; set; }

    public UserProfileEntity UserProfile { get; set; }
}