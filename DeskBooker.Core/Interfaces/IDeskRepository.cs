using System;
using System.Collections.Generic;
using DeskBooker.Core.Domain;

namespace DeskBooker.Core.Interfaces;

public interface IDeskRepository
{
	IEnumerable<Desk> GetAvailableDesks(DateTime date);
}
