﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Index(nameof(Order), nameof(QuestionId), IsUnique = true)]
public class Option
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Range(0, 255)]
    [Column(TypeName = "tinyint")]
    public int Order { get; set; }

    [StringLength(750)] public string? Content { get; set; }

    public bool IsCustom { get; set; }

    public bool IsAnswer { get; set; }

    [StringLength(750)] public string? QuestionExplain { get; set; }

    public int QuestionId { get; set; }

    public virtual Question Question { get; set; } = null!;
}