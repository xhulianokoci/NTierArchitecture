﻿namespace Shared.DTO;

public class ResetPasswordDTO
{
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}
