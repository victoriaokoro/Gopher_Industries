﻿using System.ComponentModel.DataAnnotations;

namespace foodremedy.api.Models.Requests;

public record CreateTag(
    [Required(AllowEmptyStrings = false)] string Name
);