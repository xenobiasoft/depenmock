﻿using System;

namespace DeskBooker.Core.Domain;

public record DeskBookingBase
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Email { get; set; }
	public DateTime Date { get; set; }
}