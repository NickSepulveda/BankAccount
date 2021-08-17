using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

public class User
{
    [Key]
    public int UserId {get;set;}
    [Required]
    [MinLength(2, ErrorMessage="Password must be 2 characters or longer!")]
    public string FirstName {get;set;}
    [Required]
    [MinLength(2, ErrorMessage="Password must be 2 characters or longer!")]
    public string LastName {get;set;}
    [EmailAddress]
    [Required]
    public string Email {get;set;}
    [DataType(DataType.Password)]
    [Required]
    [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
    public string Password {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    // Will not be mapped to your users table!
    [NotMapped]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string Confirm {get;set;}
    public decimal Balance {get;set;}
    public List<Transaction> CreatedTransactions {get;set;}
}