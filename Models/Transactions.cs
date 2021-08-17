using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Transaction
{
    [Key]
    public int TransactionId {get;set;}
    [Required]
    public Decimal Amount {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    // Navigation property for related User object
    [Required]
    public int UserId {get;set;}

    public User Creator {get;set;}
}