﻿using System;

namespace EventSourcing.Samples.Core.Commands
{
    public class DepostiFundsCommand : Command
    {
        public decimal Amount { get; }

        public DepostiFundsCommand(Guid correlationId, Guid accountId, decimal amount)
            : base(correlationId, accountId)
        {
            Amount = amount;
        }
    }
}